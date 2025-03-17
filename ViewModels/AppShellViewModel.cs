using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fastfood.Data;
using fastfood.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fastfood.ViewModels
{
    public partial class AppShellViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isAdmin;
        [RelayCommand]
        private async void Logout()
        {
            Preferences.Remove("UserRole");
            Application.Current.MainPage = new LoginPage(new LoginViewModel(new DatabaseService()));
        }
        private bool SetPermission()
        {
            var userRole = Preferences.Get("UserRole", string.Empty);
            if(userRole == "Administrateur")
            {
                return  _isAdmin = true;
            }
            else
            {
                return _isAdmin =  false;
            }
        }
        public AppShellViewModel()
        {
            SetPermission();
        }
    }
}
