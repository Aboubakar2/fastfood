using fastfood.Data;
using fastfood.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fastfood.Data
{
    public class DatabaseService : IAsyncDisposable
    {
        private readonly SQLiteAsyncConnection _connection;
        public DatabaseService()
        {
            // database path 
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "restpos.db3");


            _connection = new SQLiteAsyncConnection(dbPath,
                SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);
        }


        public async Task InitializeDatabaseAsync()
        {
            await _connection.CreateTableAsync<MenuCategory>();
            await _connection.CreateTableAsync<MenuItem>();
            await _connection.CreateTableAsync<MenuItemCategoryMapping>();
            await _connection.CreateTableAsync<Order>();
            await _connection.CreateTableAsync<OrderItem>();

            await SeedDataAsync();
            var menuItems = await GetMenuItemsByCategoryAsync(1);

        }


        private async Task SeedDataAsync()
        {
            var firstCategory = await _connection.Table<MenuCategory>().FirstOrDefaultAsync();

            if (firstCategory != null)
            {
                return;
            }

            var categories = SeedData.GetMenuCategories();
            var menuItems = SeedData.GetMenuItems();
            var mappings = SeedData.GetMenuItemCategoryMappings();


            await _connection.InsertAllAsync(categories);
            await _connection.InsertAllAsync(menuItems);
            await _connection.InsertAllAsync(mappings);


        }


        public async Task<MenuCategory[]> GetMenuCategoriesAsync() =>
            await _connection.Table<MenuCategory>().ToArrayAsync();


        public async Task<MenuItem[]> GetMenuItemsByCategoryAsync(int categoryId)
        {
            var query = @"
                        SELECT menu.* 
                        FROM MenuItem AS menu 
                        INNER JOIN MenuItemCategoryMapping AS mapping 
                            ON menu.Id = MenuItemId
                        WHERE mapping.MenuCategoryId = ?             
            ";

            var menuItems = await _connection.QueryAsync<MenuItem>(query, categoryId);

            return [.. menuItems];
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<string?> PlaceOrderAsync(OrderModel model)
        {
            var order = new Order
            {
                OrderDate = DateTime.Now,
                PaymentMode = model.PaymentMode,
                TotalAmountPaid = model.TotalAmountPaid,
                TotalItemsCount = model.TotalItemsCount,
            };

            if (await _connection.InsertAsync(order) > 0)
            {
                
                foreach (var item in model.Items)
                {                        
                    item.OrderId = order.Id;
                }
                if (await _connection.InsertAllAsync(model.Items) == 0)
                {                        
                    await _connection.DeleteAsync(order);
                    return "Commande vide";                        
                }                                                      
                
            }
            else
            {
                return "Error in  placing order";
            }
            model.Id = order.Id;
            return null;

        }


    }
}
