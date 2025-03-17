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
        [RelayCommand]
        private async void Logout()
        {
            Application.Current.MainPage = new LoginPage(new LoginViewModel(new DatabaseService()));
        }
    }
}
