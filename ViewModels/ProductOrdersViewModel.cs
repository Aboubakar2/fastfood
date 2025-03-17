using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fastfood.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace fastfood.ViewModels
{
    public partial class ProductOrderViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        // Propriétés observables
        [ObservableProperty]
        private ObservableCollection<Provider> _providers = new();

        [ObservableProperty]
        private ProductOrder _selectedProductOrder = new();

        [ObservableProperty]
        private Provider _selectedProvider;

        [ObservableProperty]
        private ObservableCollection<ProductOrder> _productOrders = new();

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private bool _isLoading;

        // Options pour les listes déroulantes
        [ObservableProperty]
        private List<string> _stateOptions = new() { "Livrée", "En cours", "Annulé" };

        [ObservableProperty]
        private List<string> _unitOfMeasureOptions = new()
        {
            "Kg", "Litres", "Sac(25 KG)", "Sac(50 KG)", "Sac(100 KG)", "Tia", "Paquet", "Carton"
        };


        [ObservableProperty]
        private string _selectedState;

        [ObservableProperty]
        private string _selectedUnitOfMeasure;

        private bool _isInitialized;


        //commands

        public ICommand SaveProductOrderCommand { get; }
        public ICommand EditProductOrderCommand { get; }
        public ICommand DeleteProductOrderCommand { get; }
        public ICommand ScrollToFormRequestedCommand { get; }


        // Événements
        public event EventHandler ScrollToFormRequested;
        public event EventHandler<ProductOrder> DeleteProductOrderRequested;

        // Constructor
        public ProductOrderViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            SaveProductOrderCommand = new RelayCommand(async () => await SaveProductOrderAsync());           
            EditProductOrderCommand = new RelayCommand<ProductOrder>(EditProductOrder);
            DeleteProductOrderCommand = new RelayCommand<ProductOrder>(DeleteProductOrder);
            ScrollToFormRequestedCommand = new RelayCommand(OnScrollToFormRequested);

            SelectedProductOrder.OrderDate = DateTime.Now;
        }

        

        // Initialize the providers list and other data
        public async ValueTask InitializeAsync()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;
            IsLoading = true;

            // Charger les fournisseurs depuis la base de données
            var dbProviders = await _databaseService.GetAllProvidersAsync();  
            Providers = new ObservableCollection<Provider>(dbProviders);

            // Vérifier si la liste est remplie
            if (Providers.Any())
            {
                Console.WriteLine("Providers loaded successfully.");
            }
            else
            {
                Console.WriteLine("No providers found.");
            }



            var dbProductsOrders = await _databaseService.GetAllProductOrdersAsync();
           

            foreach (var order in dbProductsOrders)
            {   order.Provider = Providers.FirstOrDefault(p => p.Id == order.ProviderId);
                ProductOrders.Add(order);
            }

            IsLoading = false;
        }

        // Calculate the  Amount whenever the Quantity or the  UnitPrice are modified
        private void CalculateAmount()
        {
            if (SelectedProductOrder != null)
            {
                SelectedProductOrder.Amount = SelectedProductOrder.UnitPrice * SelectedProductOrder.Quantity;
            }
        }

        private async Task<bool> ValidateForm()
        {
            if(SelectedProvider == null && string.IsNullOrWhiteSpace(SelectedProductOrder.ProductName) && (SelectedProductOrder.Quantity == 0) && (SelectedProductOrder.UnitPrice == 0) && string.IsNullOrWhiteSpace(SelectedProductOrder.State) && string.IsNullOrWhiteSpace(SelectedProductOrder.UnitOfMeasure))
            {
                await Shell.Current.DisplayAlert("Erreur de validation", "Les champs ne peuvent etre vides", "Ok");
                return false;
            }

            if (SelectedProvider == null)
            {
                await Shell.Current.DisplayAlert("Erreur de validation", "Veuillez sélectionner un fournisseur.", "Ok");
                return false;
            }

            if (string.IsNullOrWhiteSpace(SelectedProductOrder.ProductName))
            {
                await Shell.Current.DisplayAlert("Erreur de validation", "Le nom du produit est obligatoire.", "Ok");
                return false;
            }

            if (SelectedProductOrder.Quantity <= 0)
            {
                await Shell.Current.DisplayAlert("Erreur de validation", "La quantité doit être supérieure à zéro.", "Ok");
                return false;
            }

            if (SelectedProductOrder.UnitPrice <= 0)
            {
                await Shell.Current.DisplayAlert("Erreur de validation", "Le prix unitaire doit être supérieur à zéro.", "Ok");
                return false;
            }

            if (string.IsNullOrWhiteSpace(SelectedProductOrder.State))
            {
                await Shell.Current.DisplayAlert("Erreur de validation", "Le statut de la commande est obligatoire.", "Ok");
                return false;
            }

            if (string.IsNullOrWhiteSpace(SelectedProductOrder.UnitOfMeasure))
            {
                await Shell.Current.DisplayAlert("Erreur de validation", "L'unité de mesure est obligatoire.", "Ok");
                return false;
            }

            return true; // Tous les champs sont valides
        }
        private async Task SaveProductOrderAsync()
        {
            // Valider le formulaire
            if (!await ValidateForm())
            {
                return; // Ne pas enregistrer si le formulaire n'est pas valide
            }

            CalculateAmount();

            if (SelectedProvider != null)
            {
                SelectedProductOrder.ProviderId = SelectedProvider.Id;
                SelectedProductOrder.Provider = SelectedProvider;
            }

            if (IsEditMode)
            {
                // Mettre à jour la commande existante
                await _databaseService.UpdateProductOrderAsync(SelectedProductOrder);

                var existingOrder = ProductOrders.FirstOrDefault(o => o.Id == SelectedProductOrder.Id);
                if (existingOrder != null)
                {
                    // Mettre à jour les propriétés de la commande existante    
                    existingOrder.ProviderId = SelectedProductOrder.ProviderId;
                    existingOrder.Provider = SelectedProductOrder.Provider;
                    existingOrder.ProductName = SelectedProductOrder.ProductName;
                    existingOrder.Quantity = SelectedProductOrder.Quantity;
                    existingOrder.OrderDate = SelectedProductOrder.OrderDate;
                    existingOrder.UnitPrice = SelectedProductOrder.UnitPrice;
                    existingOrder.UnitOfMeasure = SelectedProductOrder.UnitOfMeasure;
                    existingOrder.Amount = SelectedProductOrder.Amount;
                    existingOrder.State = SelectedProductOrder.State;

                    // Ici, on notifie la CollectionView qu'un élément a été modifié
                    int index = ProductOrders.IndexOf(existingOrder);  // Trouver l'index
                    if (index >= 0)
                    {
                        ProductOrders[index] = existingOrder;  // Remplacer l'élément modifié
                    }
                    OnPropertyChanged(nameof(ProductOrders));
                }
            }
            else
            {
                // Ajouter une nouvelle commande
                await _databaseService.AddProductOrderAsync(SelectedProductOrder);
                ProductOrders.Add(SelectedProductOrder);
            }

            // Réinitialiser le formulaire
            SelectedProductOrder = new ProductOrder();
            SelectedProvider = null;
            IsEditMode = false;
        }

        /* private async Task SaveProductOrderAsync()
         {   
             //calculate the amount 
             CalculateAmount();

             if (SelectedProvider != null)
             {
                 SelectedProductOrder.ProviderId = SelectedProvider.Id;
                 SelectedProductOrder.Provider = SelectedProvider;
             }

             if (IsEditMode)
             {
                 // Mettre à jour la commande existante
                 await _databaseService.UpdateProductOrderAsync(SelectedProductOrder);

                 var existingOrder = ProductOrders.FirstOrDefault(o => o.Id == SelectedProductOrder.Id);
                 if (existingOrder != null)
                 {
                     existingOrder = SelectedProductOrder;
                 }
             }
             else
             {
                 // Ajouter une nouvelle commande
                 await _databaseService.AddProductOrderAsync(SelectedProductOrder);
                 ProductOrders.Add(SelectedProductOrder);
             }

             // Réinitialiser le formulaire
             SelectedProductOrder = new ProductOrder();
             SelectedProvider = null;
             IsEditMode = false;
         }*/


        private void OnScrollToFormRequested()
        {
            // Déclenchement de l'événement ScrollToFormRequested
            ScrollToFormRequested?.Invoke(this, EventArgs.Empty);
        }



        // Method to select a provider for editing
        private void EditProductOrder(ProductOrder productOrder)
        {
            SelectedProductOrder = productOrder;
            SelectedProvider = Providers.FirstOrDefault(p => p.Id == productOrder.ProviderId);
            OnPropertyChanged(nameof(ProductOrders));
            IsEditMode = true;

            // Notifier la vue de faire défiler vers le formulaire
            ScrollToFormRequested?.Invoke(this, EventArgs.Empty);

            // Forcer la notification de changement
            OnPropertyChanged(nameof(SelectedProductOrder));
            OnPropertyChanged(nameof(SelectedProvider));
        }


        // Event for confirmation
        public event EventHandler<ProductOrder> DeleteProducterOrderRequested;
        private async void DeleteProductOrder(ProductOrder productOrder)
        {
            // Lancer l'événement pour demander confirmation dans la vue
            DeleteProducterOrderRequested?.Invoke(this, productOrder);
        }
        public async Task PerformDeleteProductOrderAsync(ProductOrder productOrder)
        {
            if (productOrder == null || !ProductOrders.Contains(productOrder)) return;

            // Supprimer le provider de la base de données
            await _databaseService.DeleteProductOrderAsync(productOrder);

            // Retirer le provider de la collection ObservableCollection
            ProductOrders.Remove(productOrder);
        }

    } 
}
