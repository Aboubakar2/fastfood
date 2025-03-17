using System;
using fastfood.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Windowing;

namespace fastfood.ViewModels
{
    public partial class LoginViewModel: ObservableObject
    {
        private readonly DatabaseService _databaseService;
        [ObservableProperty]
        private string _usernameOrMail;
        [ObservableProperty]
        private string _password;
        [ObservableProperty]
        private string _errorMessage;
        [ObservableProperty]
        private bool _hasError;
        [ObservableProperty]
        private bool _isBusy;
        public LoginViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            UsernameOrMail = string.Empty;
            Password = string.Empty;
            ErrorMessage = string.Empty;
            HasError = false;
            IsBusy = false;
        }
        [RelayCommand]
        private async Task LoginAsync()
        {
            HasError = false;
            ErrorMessage = string.Empty;
            if(string.IsNullOrWhiteSpace(UsernameOrMail) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Veuillez remplir tous les champs";
                HasError = true;
                return;
            }
            IsBusy = true;
            try
            {
                var user = await _databaseService.AuthenticationAsync(UsernameOrMail, Password);
                if (user != null)
                {
                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    ErrorMessage = "Nom d'utilisateur ou mot de pass incorrect";
                    HasError = true;
                }
            }

            catch(Exception ex)
            {
                ErrorMessage = "Une erreur est survenue. Veillez  réessayer";
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
