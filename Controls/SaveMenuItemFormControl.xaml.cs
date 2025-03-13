using CommunityToolkit.Mvvm.Input;
using fastfood.Models;
using System.Threading.Tasks;

namespace fastfood.Controls
{

    public partial class SaveMenuItemFormControl : ContentView
    {

        private const string defaultIcon = "image_placeholder.png";
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
        private void ToogleCategorySelection(MenuCategoryModel category) =>
            category.IsSelected = !category.IsSelected;
        
        public string ExistingIcon { get; set; }

        private static void OnItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue is MenuItemModel menuItemModel)
            {
                if (bindable is SaveMenuItemFormControl thisControl)
                {
                    if (menuItemModel.Id > 0)
                    {                        
                        thisControl.SetIconImage(false, menuItemModel.Icon, thisControl);
                        thisControl.ExistingIcon = menuItemModel.Icon; 
                    }
                    else
                    {                        
                        thisControl.SetIconImage(true ,null, thisControl);
                    }
                }
            }

        }


        public event Action? OnCancel;
        [RelayCommand]
        public void Cancel() => OnCancel?.Invoke();

        private async void PickImageButton_Clicked(object sender, EventArgs e) 
        {
            var fileResult = await MediaPicker.PickPhotoAsync();
            if(fileResult != null)
            {
                //upload/Save the image on the disk

                var imageStream = await fileResult.OpenReadAsync();

                var localPath = Path.Combine(FileSystem.AppDataDirectory, fileResult.FileName);

                using var fs   = new FileStream(localPath, FileMode.Create, FileAccess.Write);
                
                await imageStream. CopyToAsync(fs);

                //update the imageIcon on the UI

                SetIconImage(false, localPath);
                Item.Icon = localPath;



            }
            else
            {
                //User cancelled from Image picker Dialod
                
                if(ExistingIcon != null)
                {
                    SetIconImage(isDefault: false, ExistingIcon);
                }
                else
                {
                    SetIconImage(isDefault: true);
                }

            }
        } 

        public void SetIconImage(bool isDefault, string? iconSource=null, SaveMenuItemFormControl? control=null)
        {
            int size = 100;

            if (isDefault)
            {
                iconSource = defaultIcon;
                size = 40;
            }
            control = control ?? this;
                        
            control.ItemIcon.Source = iconSource;
            control.ItemIcon.HeightRequest = control.ItemIcon.WidthRequest = size;
            

        }



        public event Action<MenuItemModel>? OnSaveItem;

        [RelayCommand]
        private async Task SaveMenuItem()
        {
            // validation
            if(string.IsNullOrWhiteSpace(Item.Name) || string.IsNullOrWhiteSpace(Item.Description))
            {
               await  ErrorAlertAsync("Le nom et la description son requis");
                return;
            }
            
            if(Item.SelectedCtegories.Length == 0)
            {
                await ErrorAlertAsync("Selectionner au moins une categorie!");
                return;
            }

            if(Item.Icon == defaultIcon)
            {
                await ErrorAlertAsync("Une image est pour ce plat");
            }

            OnSaveItem?.Invoke(Item);

            static async Task ErrorAlertAsync(string message) => await Shell.Current.DisplayAlert("Erreur de Validation", message, "Ok");

        }

    }
}