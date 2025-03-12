using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fastfood.Data;
using fastfood.Models;
using Microsoft.Maui.Controls;


using MenuItem = fastfood.Data.MenuItem;

namespace fastfood.ViewModels
{
    public partial  class ManageMenuViewModel: ObservableObject 
    {
        private readonly DatabaseService _databaseService;

        public ManageMenuViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [ObservableProperty]
        private MenuCategoryModel[] _categories = [];


        [ObservableProperty]
        private MenuCategoryModel? _selectedCategory = null;


        [ObservableProperty]
        private MenuItem[] _menuItems = [];

        [ObservableProperty]
        private bool _isLoading;

        private bool _isInitialized;

        [ObservableProperty]
        private MenuItemModel _menuItem = new();

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

            SetEmptyCategoryToItem();
            IsLoading = false;


        }


        private void SetEmptyCategoryToItem()
        {
            MenuItem.Categories.Clear();

            foreach (var category in Categories)
            {
                var categoryOfitem = new MenuCategoryModel
                {
                    Icon = category.Icon,
                    Id = category.Id,
                    Name = category.Name,
                    IsSelected = category.IsSelected,
                };

                MenuItem.Categories.Add(categoryOfitem);

            }
        }

        [RelayCommand]
        private void Cancel()
        {
            MenuItem = new();
            SetEmptyCategoryToItem();
        }

        [RelayCommand]
        private async Task SelectCategory(int categoryId)
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

        [RelayCommand]

        private async Task EditMenuItemAsync(MenuItem menuItem)
        {
            //     await Shell.Current.DisplayAlert("Edit", "Edit Menu item", "Ok");

            var menuitemModel = new MenuItemModel
            {
              
                Description = menuItem.Description,
                Price = menuItem.Price,
                Icon = menuItem.Icon,
                Name = menuItem.Name,
            }; 

            var itemCategories = await _databaseService.GetCategoriesOfMenuItem(menuItem.Id);
            foreach (var category in itemCategories)
            {
                var categoryOfItem = new MenuCategoryModel
                {
                    Icon = category.Icon,
                    Id = category.Id,
                    Name = category.Name,
                };
                
                if (itemCategories.Any(c => c.Id == category.Id))
                {
                    categoryOfItem.IsSelected = true;
                }
                else
                {
                    categoryOfItem.IsSelected = false;
                }

                MenuItem = menuitemModel;
            } 
        }


    }
}
