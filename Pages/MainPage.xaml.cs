
using fastfood.ViewModels;

namespace fastfood.Pages
{
    public partial class MainPage : ContentPage
    {
        private readonly HomeViewModel _homeViewModel;
        public MainPage(HomeViewModel homeViewModel)
        {
            InitializeComponent();
            _homeViewModel = homeViewModel;
            BindingContext =  _homeViewModel;

            IniTiliaze();
        }

         private  async  void IniTiliaze()
        {
            await _homeViewModel.InitializeAsync();
        }
    }
}
