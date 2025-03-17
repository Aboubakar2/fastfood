using fastfood.Data;
using fastfood.Pages;
using fastfood.ViewModels;

namespace fastfood
{
    public partial class App : Application
    {
        public App(DatabaseService databaseService)
        {
            InitializeComponent();
            var loginViewModel = new LoginViewModel(databaseService);
            MainPage = new LoginPage(loginViewModel);

            Task.Run(async () => await databaseService.InitializeDatabaseAsync())
                .GetAwaiter()
                .GetResult();

        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var   window =  base.CreateWindow(activationState);

            window.MinimumHeight = 600;
            window.MinimumWidth = 1200;
           
            return window;
        }
    }
}