using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;


using fastfood.Data;
using fastfood.Models;
using MenuItem = fastfood.Data.MenuItem;
using Microsoft.Maui.Controls;



namespace fastfood.ViewModels;
public partial class HomeViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;



    [ObservableProperty]
    private MenuCategoryModel[] _categories = [];

    [ObservableProperty]
    private MenuCategoryModel? _selectedCategory = null ;


    [ObservableProperty]
    private bool _isLoading;


    [ObservableProperty]
    private MenuItem[] _menuItems = [];



    public HomeViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    private bool _isInitialized;

    public async ValueTask InitializeAsync()
    {
        if (_isInitialized)
            return;

        _isInitialized = true;

        IsLoading = true;

        Categories = (await _databaseService.GetMenuCategoriesAsync())
                                            .Select(MenuCategoryModel.FromEntity)
                                            .ToArray();

        Categories[0].IsSelected = true;

        SelectedCategory = Categories[0];

        //await Task.Delay(3000);

        MenuItems = await _databaseService.GetMenuItemsByCategoryAsync(SelectedCategory.Id);

        IsLoading = false;


    }


    [RelayCommand]
    private async void SelectCategory(int categoryId)
    {
        if (SelectedCategory.Id == categoryId)
            return;
        IsLoading = true;

        var existingSleectedCategory = Categories.First(c => c.IsSelected);
        existingSleectedCategory.IsSelected = false;

        var newlySelectedCategory = Categories.First(c => c.Id == categoryId);
        newlySelectedCategory.IsSelected = true;

        SelectedCategory = newlySelectedCategory;

        MenuItems = await _databaseService.GetMenuItemsByCategoryAsync(SelectedCategory.Id);

        IsLoading = false;

        
    }



}
