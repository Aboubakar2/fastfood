using CommunityToolkit.Mvvm.ComponentModel;
using fastfood.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fastfood.Models
{
    public  partial class OrderModel : ObservableObject 
    {
        public long Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int TotalItemsCount { get; set; }
        public decimal TotalAmountPaid { get; set; }
        public string PaymentMode { get; set; }

        public OrderItem[] Items { get; set; }

        [ObservableProperty]
        private bool _isSelected;
    }
}
