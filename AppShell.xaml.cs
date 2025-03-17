using fastfood.Pages;
using fastfood.ViewModels;

namespace fastfood
{
    public partial class AppShell : Shell
    {

        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(ProductsPage), typeof(ProductsPage));
            Routing.RegisterRoute(nameof(ProviderPage), typeof(ProviderPage));
            Routing.RegisterRoute(nameof(ExemptionPage), typeof(ExemptionPage));
            Routing.RegisterRoute(nameof(ManageMenuItemsPage), typeof(ManageMenuItemsPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            BindingContext = new AppShellViewModel();
        }
    }
}
 