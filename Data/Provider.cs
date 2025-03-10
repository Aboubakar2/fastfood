using SQLite;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;





namespace fastfood.Data
{
    

    public class Provider : INotifyPropertyChanged
    {
        // L'ID est une clé primaire auto-incrémentée par la base de données
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }  // L'Id sera généré automatiquement par la DB (auto-increment)

        private string name;
        private string phoneNumber;

        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PhoneNumber
        {
            get => phoneNumber;
            set
            {
                if (phoneNumber != value)
                {
                    phoneNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        // Implémentation de INotifyPropertyChanged pour notifier les changements dans l'UI
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
