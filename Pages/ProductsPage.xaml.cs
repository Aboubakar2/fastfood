using fastfood.Data;
using fastfood.ViewModels;

namespace fastfood.Pages;

public partial class ProductsPage : ContentPage
{
    private readonly ProductOrderViewModel _productOrderViewModel;

    public ProductsPage(ProductOrderViewModel productOrderViewModel)
	{
		InitializeComponent();
		_productOrderViewModel = productOrderViewModel;
        BindingContext = _productOrderViewModel;

        // S'abonner à l'événement ScrollToFormRequested
        _productOrderViewModel.ScrollToFormRequested += OnScrollToFormRequested;

        // S'abonner à l'événement de demande de suppression
        _productOrderViewModel.DeleteProducterOrderRequested += OnDeleteProductOrderRequested;
        InitializeViewModelAsync();
    }


    private async void InitializeViewModelAsync() =>
      await _productOrderViewModel.InitializeAsync();


    // Méthode qui sera appelée lorsque l'événement est déclenché
    private async void OnScrollToFormRequested(object sender, EventArgs e)
    {
        // Fais défiler la page vers le formulaire
        await MainScrollView.ScrollToAsync(FormGridLayout, ScrollToPosition.Start, true);
    }

    // Pense à te désabonner de l'événement dans OnDisappearing pour éviter des fuites de mémoire



    private async void OnDeleteProductOrderRequested(object sender, ProductOrder productOrder)
    {
        bool confirm = await DisplayAlert(
            "Confirmation",
            $"Êtes-vous sûr de vouloir supprimer la commande du produit  {productOrder.ProductName} ?",
            "Oui",
            "Non"
        );

        if (confirm)
        {
            // Si l'utilisateur confirme, procéder à la suppression
            await _productOrderViewModel.PerformDeleteProductOrderAsync(productOrder);
        }
    }
}