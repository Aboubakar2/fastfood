using fastfood.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace fastfood.ViewModels
{
    public partial class UserViewModel: ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private bool _isEditMode;
        [ObservableProperty]
        private User _selectedUser = new User() ;
        [ObservableProperty]
        private bool _isLoading;

        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();
        private bool _isInitialized;

        //Options
        [ObservableProperty]
        private List<string> _rolesOptions = new() { "Administrateur", "Caissier" };

        // Commands
        public ICommand SaveUserCommand { get; }
        public ICommand EditUserCommand { get;  }

        public ICommand DeleteUserCommand { get;  }
        public ICommand ScrollToFormRequestedCommand { get; }

        //Events

        public event EventHandler<User> DeleteUserRequested;
        public event EventHandler ScrollToFormRequested;
        //Constructor
        public UserViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            SaveUserCommand = new RelayCommand(async () => SaveUserAsync());
            EditUserCommand = new RelayCommand<User>(EditUser);
            DeleteUserCommand = new RelayCommand<User>(DeleteUser);
            ScrollToFormRequestedCommand = new RelayCommand(OnScrollToFormRequested);
        }
        //Initialize users data
        public async ValueTask InitializeAsync()
        {
            if (_isInitialized)
            {
                return;
            }
            _isInitialized = true;
            IsLoading = true;
            var dbUsers = await _databaseService.GetUsersAsync();
            var users = dbUsers.Select(p => new User
            {
                Id = p.Id,
                UserName = p.UserName,
                Mail = p.Mail,
                Role = p.Role,
                Password = p.Password
            }).ToArray();


            foreach(var user in users)
            {
                Users.Add(user);
            }

            IsLoading = false;
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(SelectedUser.UserName))
            {
                Console.WriteLine("Le nom de l'utilisateur est obligatoire");
                return false;
            }
            if (string.IsNullOrWhiteSpace(SelectedUser.Mail))
            {
                Console.WriteLine("Le mail de l'utilisateur est obligatoire");
                return false;
            }
            if (string.IsNullOrWhiteSpace(SelectedUser.Role))
            {
                Console.WriteLine("Le rôle de l'utilisateur est obligatoire");
                return false;
            }
            if (string.IsNullOrWhiteSpace(SelectedUser.Password))
            {
                Console.WriteLine("Le mot de passe de l'utilisateur est obligatoire");
                return false;
            }
            return true; //All fields are ok
        }

        //Save or Update a new peovider or update 

        private async Task SaveUserAsync()
        {
            if (!ValidateForm())
            {
                return;
            }
            if (IsEditMode)
            {

                if(SelectedUser != null)
                {
                    await _databaseService.UpdateUserAsync(SelectedUser);

                    var userToUpdate = Users.FirstOrDefault(p => p.Id == SelectedUser.Id);
                    if(userToUpdate != null)
                    {
                        userToUpdate.UserName = SelectedUser.UserName;
                        userToUpdate.Mail = SelectedUser.Mail;
                        userToUpdate.Role = SelectedUser.Role;
                        userToUpdate.Password = SelectedUser.Password;
                    }

                }
            }
            else
            {
                //Add a user
                if(SelectedUser != null)
                {
                    await _databaseService.AddUserAsync(SelectedUser);
                    Users.Add(SelectedUser); //Ajout a la liste
                }
            }

            //Reseting after saving
            IsEditMode = false;
            SelectedUser = new User(); //reset the form
        }
        ////Declare a new event to notify the view
        //public event EventHandler ScrollToFormRequested;

        private void OnScrollToFormRequested()
        {
            //strating of the event ScrollToForkmRequested
            ScrollToFormRequested?.Invoke(this, EventArgs.Empty);
        }

        //Select a provider
        public void EditUser(User user)
        {
            SelectedUser = user;
            IsEditMode = true;
            // defile vers le formulaire
            ScrollToFormRequestedCommand.Execute(null);
        }
        private async void DeleteUser(User user)
        {
            //Confimation for the view
            DeleteUserRequested?.Invoke(this, user);
        }
        
        public async Task PerformDeleteUserAsync(User user)
        {
            if (user == null) return;

            //deleting the user
            await _databaseService.DeleteUserAsync(user);

            //remove from the observablecollection
            Users.Remove(user);
        }

        

    }
}
