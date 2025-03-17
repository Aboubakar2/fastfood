using fastfood.Data;
using fastfood.Models;
using Microsoft.UI.Xaml.Documents;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserDataTasks;

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
            await _connection.CreateTableAsync<ProductOrder>();
            await _connection.CreateTableAsync<Provider>();
            await _connection.CreateTableAsync<User>();

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
            var providers = SeedData.GetProviders();
            var users = SeedData.GetUsers();


            await _connection.InsertAllAsync(categories);
            await _connection.InsertAllAsync(menuItems);
            await _connection.InsertAllAsync(mappings);
            await _connection.InsertAllAsync(providers);
            await _connection.InsertAllAsync(users);


        }


        public async Task<MenuCategory[]> GetMenuCategoriesAsync() =>
            await _connection.Table<MenuCategory>().ToArrayAsync();

        public async Task<string?> SaveMenuItemAsync(MenuItemModel model)
        {
            if (model.Id == 0)
            {
                // Creatting a new menu item
                MenuItem menuItem = new()
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    Icon = model.Icon,
                    Price = model.Price,

                };

                if (await _connection.InsertAsync(menuItem) > 0)
                {
                    var categoryMapping = model.SelectedCtegories
                        .Select(c => new MenuItemCategoryMapping
                        {
                            Id = c.Id,
                            MenuItemId = menuItem.Id,
                            MenuCategoryId = c.Id
                        });


                    if (await _connection.InsertAllAsync(categoryMapping) > 0)
                    {
                        model.Id = menuItem.Id;
                        return null;
                    }
                    else
                    {
                        await _connection.DeleteAsync(menuItem);
                    }
                }
                return "Error au moment de l'enregistrement de l'item";
            }
            else
            {
                // updating a menu item 

                string errorMessage = null;

                await _connection.RunInTransactionAsync(db => {

                    var menuItem = db.Find<MenuItem>(model.Id);

                    menuItem.Id = model.Id;
                    menuItem.Name = model.Name;
                    menuItem.Description = model.Description;
                    menuItem.Icon = model.Icon;
                    menuItem.Price = model.Price;

                    if (db.Update(menuItem) == 00)
                    {
                        //this operation failed
                        // return error message
                        errorMessage = "Erreur au niveau de la modification de cet  item";
                        throw new Exception();
                    }

                    var deleteQuerry = @"
                        DELETE FROM MenuItemCategoryMapping
                        WHERE MenuItemId = ?";

                    db.Execute(deleteQuerry, menuItem.Id);

                    var categoryMapping = model.SelectedCtegories
                        .Select(c => new MenuItemCategoryMapping
                        {
                            Id = c.Id,
                            MenuItemId = menuItem.Id,
                            MenuCategoryId = c.Id
                        });


                    if (db.InsertAll(categoryMapping) == 0)
                    {   //this operation failed
                        // return error message
                        errorMessage = "Erreur au niveau de la modification de cet  item";
                        throw new Exception();
                    }

                });


                return errorMessage;

            }
        }
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

        public async Task<Order[]> GetOrdersAsync() =>
            await _connection.Table<Order>().ToArrayAsync();

        public async Task<long> GetLastOrderIdAsync()
        {
            var lastOrder = await _connection.Table<Order>().OrderByDescending(o => o.Id).FirstOrDefaultAsync();
            return lastOrder?.Id ?? 0;
        }

        public async Task<OrderItem[]> GetOrderItemsAsync(long orderId) =>       
            await _connection.Table<OrderItem>()
                .Where(oi => oi.OrderId == orderId)
                .ToArrayAsync();

        public async Task<MenuCategory[]> GetCategoriesOfMenuItem(int menuItemId)
        {
            var query = @"
                        SELECT cat.* 
                        FROM MenuCategory AS cat 
                        INNER JOIN MenuItemCategoryMapping AS mapping 
                            ON cat.Id = mapping.MenuCategoryId
                        WHERE mapping.MenuItemId = ?             
            ";
            var categories = await _connection.QueryAsync<MenuCategory>(query, menuItemId);
            return [.. categories];
        }

        
        
        // operations pour provider
        public async Task<List<Provider>> GetAllProvidersAsync()
        {
            return await _connection.Table<Provider>().ToListAsync();
        }


        public async Task<long> AddProviderAsync(Provider provider)
        {
            var result = await _connection.InsertAsync(provider);
            return result > 0 ? provider.Id : -1;
        }


        public async Task<bool> UpdateProviderAsync(Provider provider)
        {
            var result = await _connection.UpdateAsync(provider);
            return result > 0;
        }

        public async Task<bool> DeleteProviderAsync(Provider provider)
        {
            if (provider != null)
            {
                var result = await _connection.DeleteAsync(provider);
                return result > 0;  // Retourne vrai si la suppression a réussi
            }
            return false;
        }

        // Operation pour userExemption
        public async Task<List<User>> GetUsersAsync()
        {
            return await _connection.Table<User>().ToListAsync();
        }
        public async Task<long> AddUserAsync(User user)
        {
            var result = await _connection.InsertAsync(user);
            return result > 0 ? user.Id : -1;
        }
        public async Task<bool> UpdateUserAsync(User user)
        {
            var result = await _connection.UpdateAsync(user);
            return result > 0;
        }
        public async Task<bool> DeleteUserAsync(User user)
        {
            var result = await _connection.DeleteAsync(user);
            return result > 0;
        }


        // O[peration pour la loginPage

        public async Task<User> AuthenticationAsync(string usernameOrMail, string password)
        {
            var users = await GetUsersAsync();
            var user = users.FirstOrDefault(u =>
            (u.UserName == usernameOrMail || u.Mail == usernameOrMail) &&
            u.Password == password
            );
            return user;
        }





        //operations pour productOrder

        public async Task<List<ProductOrder>> GetAllProductOrdersAsync()
        {
            return await _connection.Table<ProductOrder>().ToListAsync();
        }

        public async Task<long> AddProductOrderAsync(ProductOrder productOrder)
        {
            var result = await _connection.InsertAsync(productOrder);
            return result > 0 ? productOrder.Id : -1;
        }


        public async Task<bool> UpdateProductOrderAsync(ProductOrder productOrder)
        {
            var result = await _connection.UpdateAsync(productOrder);
            return result > 0;
        }

        public async Task<bool> DeleteProductOrderAsync(ProductOrder productOrder)
        {
            if (productOrder != null)
            {
                var result = await _connection.DeleteAsync(productOrder);
                return result > 0;  // Retourne vrai si la suppression a réussi
            }
            return false;
        }



    }
}
