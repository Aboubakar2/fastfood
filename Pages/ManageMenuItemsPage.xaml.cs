using fastfood.ViewModels;

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

    private void MenuItemsListControl_OnSelectItem(MenuItem menuItem)
    {
        _manageMenuItemViewModel.EditMenuItemCommand.Execute(menuItem);
    }
}