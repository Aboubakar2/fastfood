 using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fastfood.Data;
using fastfood.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Services.Store;

namespace fastfood.ViewModels
{
    public partial class OrdersViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        public OrdersViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            var userRole = Preferences.Get("UserRole", "Caissier");
        }

        public async Task<bool> PlaceOrderAsync(CartModel[] cartItems,bool isPaidOnline)
        {
            var OrderItems = cartItems
            .Select(c => new OrderItem
            {
                Icon = c.Icon,
                ItemId = c.ItemId,
                Name = c.Name,
                Price = c.Price,
                Quantity = c.Quantity,
            }).ToArray();


            var orderModel = new OrderModel
            {
                OrderDate = DateTime.Now,
                PaymentMode = isPaidOnline ? "Online" : "Cash",
                TotalAmountPaid = cartItems.Sum(c => c.Amount),
                TotalItemsCount = cartItems.Length,
                Items = OrderItems,
            };

            var errormessage = await _databaseService.PlaceOrderAsync(orderModel);
            if (!string.IsNullOrEmpty(errormessage))
            {
                await Shell.Current.DisplayAlert("Error", errormessage, "Ok");
                return false;
            }
            else
            {
                Orders.Add(orderModel);
                try
                {
                    await Toast.Make("Commande enregistree avec Succes").Show();
                }
                catch (Exception ex)
                {
                    // Log l'erreur pour la déboguer
                    System.Diagnostics.Debug.WriteLine($"Toast Error: {ex.Message}");
                    await Shell.Current.DisplayAlert("Error", "Failed to display toast", "Ok");
                }

                return true;
            }
               
            
        }



        public ObservableCollection<OrderModel> Orders { get; set; } = [];

        private bool _isInitialized;


        [ObservableProperty]
        private bool _isLoading;

        public async ValueTask InitializeAsync()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;
            IsLoading = true;
            Orders.Clear();

            var dbOrders = await _databaseService.GetOrdersAsync();
            var orders = dbOrders.Select(o => new OrderModel
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                PaymentMode = o.PaymentMode,
                TotalAmountPaid = o.TotalAmountPaid,
                TotalItemsCount = o.TotalItemsCount,
            }).ToArray();

            foreach (var order in orders)
            {
                Orders.Add(order);
            }
            IsLoading = false;
        }

        [ObservableProperty]
        private OrderItem[] _orderItems = [];

        [RelayCommand]
        private async Task SelectOrderAsync(OrderModel? order)
        {
            var  preSelected = Orders.FirstOrDefault(o => o.IsSelected);
            if (preSelected != null)
            {
                preSelected.IsSelected = false;
                if(preSelected.Id == order?.Id)
                {
                    OrderItems = [];
                }
            }
            if (order == null || order.Id == 0)
            {
                OrderItems = [];
                return;
            }
            IsLoading = true;
            order.IsSelected = true;
            OrderItems = await _databaseService.GetOrderItemsAsync(order.Id);
            IsLoading = false;
        }

    }
}