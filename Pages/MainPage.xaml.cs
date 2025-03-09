
using fastfood.Models;
using fastfood.ViewModels;
using System.Threading.Tasks;

using MenuItem = fastfood.Data.MenuItem;

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

        private async void CategoriesListControl_OnCategorySelected(Models.MenuCategoryModel category)
        {
            await _homeViewModel.SelectCategoryCommand.ExecuteAsync(category.Id);
        }

        private  void MenuItemsListControl_OnSelectItem(MenuItem menuItem)
        {
             _homeViewModel.AddToCartCommand.Execute(menuItem);
        }
    }
}
