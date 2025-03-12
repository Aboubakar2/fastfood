using CommunityToolkit.Mvvm.Input;
using fastfood.Models;

namespace fastfood.Controls
{



    public partial class SaveMenuItemFormControl : ContentView
    {
        public SaveMenuItemFormControl()
        {
            InitializeComponent();
        }


        public static readonly BindableProperty ItemProperty = BindableProperty.Create(nameof(Item), typeof(MenuItemModel), typeof(SaveMenuItemFormControl), new MenuItemModel(), propertyChanged: OnItemChanged);

        public MenuItemModel Item
        {
            get => (MenuItemModel)GetValue(ItemProperty);
            set => SetValue(ItemProperty, value);
        }

        [RelayCommand]
        private void ToogleCategorySelection(MenuCategoryModel category)
        {
            category.IsSelected = !category.IsSelected;
        }

        private static void OnItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            //if (newValue is MenuItemModel menuItemModel)
            //{
            //    if(bindable is SaveMenuItemFormControl thisControl)
            //    {
            //        if(menuItemModel.Id > 0)
            //        {
            //            thisControl.ItemIcon = menuItemModel.Icon;
            //            thisControl.Item.HeightRequest = thisControl.WidthRequest = 100;

            //        }
            //        else
            //        {
            //            thisControl.ItemIcon = "add.png";
            //            thisControl.Item.HeightRequest = thisControl.WidthRequest = 36;
            //        }
            //    }
            //}

        }

        public event Action? OnCancel;
        public void Cancel() => OnCancel?.Invoke();



    }
}