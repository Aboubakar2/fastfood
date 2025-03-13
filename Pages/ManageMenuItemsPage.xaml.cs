using fastfood.ViewModels;
using System.Threading.Tasks;

namespace fastfood.Pages;

public partial class ManageMenuItemsPage : ContentPage
{
    private readonly ManageMenuViewModel _manageMenuItemViewModel;

    public ManageMenuItemsPage(ManageMenuViewModel manageMenuViewModel)
	{
		InitializeComponent();
        _manageMenuItemViewModel = manageMenuViewModel;
        BindingContext = _manageMenuItemViewModel;
        InitializeAsync();
	}



    private async void InitializeAsync()
    {
        await _manageMenuItemViewModel.InitializeAsync();
    }
    private async void CategoriesListControl_OnCategorySelected(Models.MenuCategoryModel category)
    {
        await _manageMenuItemViewModel.SelectCategoryCommand.ExecuteAsync(category.Id);
    }
    private async void MenuItemsListControl_OnSelectItem(Data.MenuItem menuItem)
    {
        await _manageMenuItemViewModel.EditMenuItemCommand.ExecuteAsync(menuItem);
    }


    private void SaveMenuItemFormControl_OnCancel()
    {
        _manageMenuItemViewModel.CancelCommand.Execute(null);
    }

    private async  void SaveMenuItemFormControl_OnSaveItem(Models.MenuItemModel menuItemModel)
    {
       await _manageMenuItemViewModel.SaveMenuItemCommand.ExecuteAsync(menuItemModel);
    }
}