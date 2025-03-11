using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fastfood.Data;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace fastfood.ViewModels
{

    public partial class ProvidersViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private Provider _selectedProvider = new Provider();

        public ICommand SaveProviderCommand { get; }
        public ICommand EditProviderCommand { get; }
        public ICommand DeleteProviderCommand { get; }
        public ICommand ScrollToFormRequestedCommand { get; }


        public ObservableCollection<Provider> Providers { get; set; } = new ObservableCollection<Provider>();

        private bool _isInitialized;

        [ObservableProperty]
        private bool _isLoading;

        // Constructor
        public ProvidersViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            SaveProviderCommand = new RelayCommand(async () => await SaveProviderAsync());
            EditProviderCommand = new RelayCommand<Provider>(EditProvider);
            DeleteProviderCommand = new RelayCommand<Provider>(DeleteProvider);
            ScrollToFormRequestedCommand = new RelayCommand(OnScrollToFormRequested);
        }

        // Initialize Providers data
        public async ValueTask InitializeAsync()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;
            IsLoading = true;
            var dbProviders = await _databaseService.GetAllProvidersAsync();
            var providers = dbProviders.Select(p => new Provider
            {
                Id = p.Id,
                Name = p.Name,
                PhoneNumber = p.PhoneNumber,
            }).ToArray();

            foreach (var provider in providers)
            {
                Providers.Add(provider);
            }

            IsLoading = false;
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(SelectedProvider.Name))
            {
                Console.WriteLine("Le nom du fournisseur est obligatoire.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(SelectedProvider.PhoneNumber))
            {
                Console.WriteLine("Le numéro de téléphone est obligatoire.");
                return false;
            }

            return true; // Tous les champs sont valides
        }

        // Method to save a new provider or update an existing one

        private async Task SaveProviderAsync()
        {
            // Valider le formulaire
            if (!ValidateForm())
            {
                return; // Ne pas enregistrer si le formulaire n'est pas valide
            }

            if (IsEditMode)
            {
                // Mise à jour d'un fournisseur existant
                if (SelectedProvider != null)
                {
                    await _databaseService.UpdateProviderAsync(SelectedProvider);

                    // Trouver l'élément dans la collection et le mettre à jour
                    var providerToUpdate = Providers.FirstOrDefault(p => p.Id == SelectedProvider.Id);
                    if (providerToUpdate != null)
                    {
                        providerToUpdate.Name = SelectedProvider.Name;
                        providerToUpdate.PhoneNumber = SelectedProvider.PhoneNumber;
                    }
                }
            }
            else
            {
                // Ajouter un nouveau fournisseur
                if (SelectedProvider != null)
                {
                    await _databaseService.AddProviderAsync(SelectedProvider);
                    Providers.Add(SelectedProvider);  // Ajout direct à la liste
                }
            }

            // Reset après l'enregistrement
            IsEditMode = false;
            SelectedProvider = new Provider();  // Réinitialiser le formulaire
        }

        // Déclare un événement pour notifier la vue
        public event EventHandler ScrollToFormRequested;
        private void OnScrollToFormRequested()
        {
            // Déclenchement de l'événement ScrollToFormRequested
            ScrollToFormRequested?.Invoke(this, EventArgs.Empty);
        }

 

        // Method to select a provider for editing
        public void EditProvider(Provider provider)
        {
            SelectedProvider = provider;
            IsEditMode = true;  // Set edit mode to true
                                
            // Notifie la vue de faire défiler vers le formulaire
            ScrollToFormRequestedCommand.Execute(null);
        }


        // Event for confirmation
        public event EventHandler<Provider> DeleteProviderRequested;
        private async void DeleteProvider(Provider provider)
        {
            // Lancer l'événement pour demander confirmation dans la vue
             DeleteProviderRequested?.Invoke(this, provider);
        }
        public async Task PerformDeleteProviderAsync(Provider provider)
        {
            if (provider == null) return;

            // Supprimer le provider de la base de données
            await _databaseService.DeleteProviderAsync(provider);

            // Retirer le provider de la collection ObservableCollection
            Providers.Remove(provider);
        }
    }

}