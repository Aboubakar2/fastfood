using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;


using fastfood.Data;
using fastfood.Models;
using MenuItem = fastfood.Data.MenuItem;
using fastfood.Utils;
using System.Diagnostics;




namespace fastfood.ViewModels;
public partial class HomeViewModel : ObservableObject, IRecipient<MenuItemChangedMessage>
{
    // declaring our varaiables for ViewModels Needed
    private readonly DatabaseService _databaseService;
    private readonly OrdersViewModel _ordersViewModel;

    //declaring our receipt service 
    private readonly ReceiptService _receiptService;
    
    [ObservableProperty]
    private MenuCategoryModel[] _categories = [];

    [ObservableProperty]
    private MenuCategoryModel? _selectedCategory = null ;

    public ObservableCollection<CartModel> CartItems { get; set; } = new();


    [ObservableProperty]
    private MenuItem[] _menuItems = [];

    [ObservableProperty]
    private bool _isLoading;


   

    [ObservableProperty,NotifyPropertyChangedFor(nameof(TaxAmount))]
    [NotifyPropertyChangedFor(nameof(Total))]
    private decimal _subTotal;

    [ObservableProperty, NotifyPropertyChangedFor(nameof(TaxAmount))]
    [NotifyPropertyChangedFor(nameof(Total))]
    private decimal _taxPercentage;


    public decimal TaxAmount => (SubTotal * TaxPercentage) / 100;
    public decimal Total => SubTotal + TaxAmount;






    // Constructor 
    public HomeViewModel(DatabaseService databaseService, OrdersViewModel ordersViewModel, ReceiptService receiptService)
    {
        _databaseService = databaseService;
        _ordersViewModel = ordersViewModel;
        _receiptService = receiptService;
        CartItems.CollectionChanged += CartItems_CollectionChanged;
        WeakReferenceMessenger.Default.Register<MenuItemChangedMessage>(this);

    }

    public HomeViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    private void CartItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RecalculateAmount();
    }

    private bool _isInitialized;

    public async ValueTask InitializeAsync()
    {
        if (_isInitialized)
            return;

        _isInitialized = true;

        IsLoading = true;

        Categories = (await _databaseService.GetMenuCategoriesAsync())
                                            .Select(MenuCategoryModel.FromEntity)
                                            .ToArray();

        Categories[0].IsSelected = true;

        SelectedCategory = Categories[0];

        //await Task.Delay(3000);

        MenuItems = await _databaseService.GetMenuItemsByCategoryAsync(SelectedCategory.Id);

        IsLoading = false;


    }


    [RelayCommand]
    private async Task SelectCategory(int categoryId)
    {
        if (SelectedCategory.Id == categoryId)
            return;
        IsLoading = true;

        var existingSleectedCategory = Categories.First(c => c.IsSelected);
        existingSleectedCategory.IsSelected = false;

        var newlySelectedCategory = Categories.First(c => c.Id == categoryId);
        newlySelectedCategory.IsSelected = true;

        SelectedCategory = newlySelectedCategory;

        MenuItems = await _databaseService.GetMenuItemsByCategoryAsync(SelectedCategory.Id);

        IsLoading = false;

        
    }
    [RelayCommand]
    private   void AddToCart(MenuItem menuItem)
    {
        var cartItem = CartItems.FirstOrDefault(c => c.ItemId == menuItem.Id);
        if(cartItem==null)
        {
            
            cartItem = new CartModel()
            {
                ItemId = menuItem.Id,
                Icon = menuItem.Icon,
                Name = menuItem.Name,
                Price = menuItem.Price,
                Quantity = 1
            };

            CartItems.Add(cartItem);
        }
        else
        {
            cartItem.Quantity++;
            RecalculateAmount();
        }
    }

    [RelayCommand] 
    private void IncreaseQuantity(CartModel cartItem)
    {
        cartItem.Quantity++;
        RecalculateAmount();
    }
    [RelayCommand]
    private void DecreaseQuantity(CartModel cartItem)
    {
        cartItem.Quantity--;
        if (cartItem.Quantity == 0)
            CartItems.Remove(cartItem);
        else
            RecalculateAmount();
    }

    [RelayCommand]
    private void RemoveFromCart(CartModel cartItem) => CartItems.Remove(cartItem);



    [RelayCommand]
    public async Task TaxPercentageClickAsync()
    {
        var result = await Shell.Current.DisplayPromptAsync("Tax Percentage ", "Entrer le pourcentage de la tax applicable ", placeholder: "10", initialValue: TaxPercentage.ToString());
            if(!string.IsNullOrEmpty(result))
            {
                if(!int.TryParse(result,out int enteredTaxpercentage))
                {
                    await Shell.Current.DisplayAlert("Invalid Value ", "Entered Tax Percentage is invalid ", "Ok");
                    return;
                }
                if(enteredTaxpercentage > 100)
                {
                    await Shell.Current.DisplayAlert("Invalid Value ", "Entered Tax Percentage is invalid ", "Ok");
                    return;
                }

            TaxPercentage = enteredTaxpercentage;
            }
    }

    [RelayCommand]
    private void ClearCart() => CartItems.Clear();
    private void RecalculateAmount()
    {
        SubTotal = CartItems.Sum(c => c.Amount);
    }


    [RelayCommand]
    private async Task PlaceOrderAsync(bool isPaidOnline)
    {
        IsLoading = true;

        var result = await _ordersViewModel.PlaceOrderAsync([..CartItems], isPaidOnline);
        if (result)
        {
            try
            {
                // Récupérer l'ID de la dernière commande (ou créer une logique pour obtenir l'ID)
                var orderId = await _databaseService.GetLastOrderIdAsync(); // Méthode à créer

                // Générer et imprimer le ticket
                var filePath = await _receiptService.GenerateReceiptAsync(
                    CartItems,
                    SubTotal,
                    TaxPercentage,
                    TaxAmount,
                    Total,
                    orderId.ToString(),
                    isPaidOnline ? "Online" : "Cash"
                );

                await _receiptService.PrintReceiptAsync(filePath);

                // Demander à l'utilisateur s'il souhaite imprimer
                bool shouldPrint = await Shell.Current.DisplayAlert(
                    "Impression",
                    "Voulez-vous imprimer le ticket?",
                    "Oui", "Non");

                if (shouldPrint)
                {
                    // Utiliser la méthode d'impression directe
                    using (Process process = new Process())
                    {
                        process.StartInfo = new ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true,
                            Verb = "print"
                        };
                        process.Start();
                    }
                }


                // free the Cart
                CartItems.Clear();
            }
                         
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erreur d'impression",
                    $"La commande a été enregistrée mais l'impression du ticket a échoué: {ex.Message}", "OK");
            }
        }

      
        IsLoading = false;

    }

    public void Receive(MenuItemChangedMessage message)
    {
        var model= message.Value;
        var menuItem = MenuItems.FirstOrDefault(m => m.Id == model.Id);


        if (menuItem != null)
        {
            //this menu item is on the screen

            // check if this menu item still have a mapping to select a cetegory

            if (!model.SelectedCtegories.Any(c => c.Id == SelectedCategory.Id))
            {
                //this item is no longer belongs to selected category
                //remove this from the UI 
                MenuItems = [.. MenuItems.Where(m => m.Id != model.Id)];
                return;
            }
            //update the details
            menuItem.Price = model.Price;
            menuItem.Name = model.Name;
            menuItem.Description = model.Description;
            menuItem.Icon = model.Icon;

            MenuItems = [.. MenuItems];
        }
        else if (model.SelectedCtegories.Any(c => c.Id == SelectedCategory.Id))
        {

            var newMwnuItem = new MenuItem
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Icon = model.Icon,
                Price = model.Price,
            };

            MenuItems = [.. MenuItems, newMwnuItem];
        }


        var cartItem = CartItems.FirstOrDefault(c => c.ItemId == model.Id);
        if( cartItem != null)
        {
            cartItem.Name = model.Name;
            cartItem.Price = model.Price;
            cartItem.Icon = model.Icon;

            var itemIndex  = CartItems.IndexOf(cartItem);
            // trigger collection chande event 
            CartItems[itemIndex] = cartItem;


        }


    }
}
