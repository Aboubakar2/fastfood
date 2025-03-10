using fastfood.Data;
using fastfood.ViewModels;



namespace fastfood.Pages;

public partial class ProviderPage : ContentPage
{
    private readonly ProvidersViewModel _providersViewModel;

    public ProviderPage(ProvidersViewModel providersViewModel)
    {
        InitializeComponent();
        _providersViewModel = providersViewModel;
        BindingContext = _providersViewModel;

        // S'abonner à l'événement ScrollToFormRequested
        _providersViewModel.ScrollToFormRequested += OnScrollToFormRequested;

        // S'abonner à l'événement de demande de suppression
        _providersViewModel.DeleteProviderRequested += OnDeleteProviderRequested;
        InitializeViewModelAsync();

    }

    private async void InitializeViewModelAsync() =>
      await _providersViewModel.InitializeAsync();


    // Méthode qui sera appelée lorsque l'événement est déclenché
    private async void OnScrollToFormRequested(object sender, EventArgs e)
    {
        // Fais défiler la page vers le formulaire
        await MainScrollview.ScrollToAsync(FormStackLayout, ScrollToPosition.Start, true);
    }

    // Pense à te désabonner de l'événement dans OnDisappearing pour éviter des fuites de mémoire



    private async void OnDeleteProviderRequested(object sender, Provider provider)
    {
        bool confirm = await DisplayAlert(
            "Confirmation",
            $"Êtes-vous sûr de vouloir supprimer le fournisseur {provider.Name} ?",
            "Oui",
            "Non"
        );

        if (confirm)
        {
            // Si l'utilisateur confirme, procéder à la suppression
            await _providersViewModel.PerformDeleteProviderAsync(provider);
        }
    }

    protected override void OnDisappearing()
    {
        var viewModel = (ProvidersViewModel)BindingContext;
        viewModel.ScrollToFormRequested -= OnScrollToFormRequested;
        viewModel.DeleteProviderRequested -= OnDeleteProviderRequested;
        base.OnDisappearing();
    }
}
