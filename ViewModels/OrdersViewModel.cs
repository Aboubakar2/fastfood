using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
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



        public ObservableCollection<Order> Orders { get; set; } = [];

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
            var orders = await _databaseService.GetOrdersAsync();
            IsLoading = false;
            foreach (var order in orders)
            {
                Orders.Add(order);
            }
            IsLoading = false;
        }

    }
}