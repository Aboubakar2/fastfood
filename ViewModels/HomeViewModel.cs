using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;


using fastfood.Data;
using fastfood.Models;
using MenuItem = fastfood.Data.MenuItem;
using Microsoft.Maui.Controls;



namespace fastfood.ViewModels;
public partial class HomeViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly OrdersViewModel _ordersViewModel;
    
    [ObservableProperty]
    private MenuCategoryModel[] _categories = [];

    [ObservableProperty]
    private MenuCategoryModel? _selectedCategory = null ;


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



    public ObservableCollection<CartModel> CartItems { get; set; } = new();


    [ObservableProperty]
    private MenuItem[] _menuItems = [];



    public HomeViewModel(DatabaseService databaseService, OrdersViewModel ordersViewModel)
    {
        _databaseService = databaseService;
        _ordersViewModel = ordersViewModel;
        CartItems.CollectionChanged += CartItems_CollectionChanged;
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
        var result = await Shell.Current.DisplayPromptAsync("Tax Percentage ", "Enter the applicable tax percentage ", placeholder: "10", initialValue: TaxPercentage.ToString());
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
        await _ordersViewModel.PlaceOrderAsync([..CartItems], isPaidOnline);
        {
            CartItems.Clear();
        }
        IsLoading = false;

    }
}
