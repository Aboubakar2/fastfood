using SQLite;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;



   

namespace fastfood.Data
{
    public class ProductOrder : INotifyPropertyChanged
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        private long providerId;
        private string productName;
        private int quantity;
        private DateTime orderDate;
        private double unitprice;
        public double amount;  
        public string unitOfMeasure;
        private string state;

        public long ProviderId
        {
            get => providerId;
            set
            {
                if (providerId != value)
                {
                    providerId = value;
                    OnPropertyChanged();
                }
            }
        }

        [Ignore] // MAking sure that  SQLite can't  map this object
        public Provider Provider { get; set; }


        public int Quantity
        {
            get => quantity;
            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    OnPropertyChanged();
                }
            }
        } 
        public string ProductName
        {
            get => productName;
            set
            {
                if (productName != value)
                {
                    productName = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime OrderDate
        {
            get => orderDate;
            set
            {
                if (orderDate != value)
                {
                    orderDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public double UnitPrice
        {
            get => unitprice;
            set
            {
                if (unitprice != value)
                {
                    unitprice = value;
                    OnPropertyChanged();
                }
            }
        }
        public string UnitOfMeasure
        {
            get => unitOfMeasure;
            set
            {
                if (unitOfMeasure != value)
                {
                    unitOfMeasure = value;
                    OnPropertyChanged();
                }
            }
        }
        public double Amount
        {
            get => amount;
            set
            {
                if (amount != value)
                {
                    amount = value;
                    OnPropertyChanged();
                }
            }
        }

        public string State
        {
            get => state;
            set
            {
                if (state != value)
                {
                    state = value;
                    OnPropertyChanged();
                }
            }
        }

        // Implementing INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


