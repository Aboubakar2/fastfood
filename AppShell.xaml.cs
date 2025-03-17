using fastfood.Pages;
using fastfood.ViewModels;
using fastfood.Data;
using System.Diagnostics;

namespace fastfood
{
    public partial class AppShell : Shell
    {

        public AppShell()
        {
            InitializeComponent();
            BindingContext = new AppShellViewModel();
                                                                                                                                                                                                                                                                             

            var userRole = Preferences.Get("UserRole", string.Empty);
            if (userRole != "")
            {
                Debug.WriteLine(userRole);
            }
            else
            {
                Debug.WriteLine("UserRole not define");
            }

            if (userRole == "Caissier")
            {
                Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
                Routing.RegisterRoute(nameof(ProductsPage), typeof(ProductsPage));
                GoToAsync("//MainPage");
            }
            else if (userRole == "Administrateur")
            {
                Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
                Routing.RegisterRoute(nameof(ProductsPage), typeof(ProductsPage));
                Routing.RegisterRoute(nameof(ProviderPage), typeof(ProviderPage));
                Routing.RegisterRoute(nameof(ExemptionPage), typeof(ExemptionPage));
                Routing.RegisterRoute(nameof(ManageMenuItemsPage), typeof(ManageMenuItemsPage));
                Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
                GoToAsync("//MainPage");
            }
            else
            {
                Application.Current.MainPage = new LoginPage(new LoginViewModel(new DatabaseService()));
            }

        }
    }
}
 