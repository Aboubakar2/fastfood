using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
  
 

namespace fastfood.Data
{
    class SeedData
    {

        public static List<MenuCategory> GetMenuCategories()
        {
            return new List<MenuCategory>
        {
            new MenuCategory { Id = 1, Name = "Entrées", Icon = "entree.png" },
            new MenuCategory { Id = 2, Name = "Plats principaux", Icon = "plat.png" },
            new MenuCategory { Id = 3, Name = "Desserts", Icon = "dessert.png" }
        };
        }

        public static List<MenuItem> GetMenuItems()
        {
            return new List<MenuItem>
        {
            // Entrées
            new MenuItem { Id = 1, Name = "Salade César", Icon = "salade_cesar.jpg", Description = "Salade avec poulet grillé et sauce César", Price = 8.5m },
            new MenuItem { Id = 2, Name = "Soupe de légumes", Icon = "soupe_legumes.jpg", Description = "Soupe chaude de légumes frais", Price = 6.0m },
            new MenuItem { Id = 3, Name = "Carpaccio de bœuf", Icon = "carpaccio.jpg", Description = "Tranches fines de bœuf cru avec roquette", Price = 9.5m },
            new MenuItem { Id = 4, Name = "Bruschetta", Icon = "bruschetta.jpg", Description = "Pain grillé avec tomates et basilic", Price = 5.5m },
            new MenuItem { Id = 5, Name = "Guacamole", Icon = "guacamole.jpg", Description = "Purée d'avocat avec épices et chips de maïs", Price = 7.0m },

            // Plats principaux
            new MenuItem { Id = 6, Name = "Burger classique", Icon = "burger.jpg", Description = "Burger avec viande de bœuf, fromage et légumes", Price = 12.0m },
            new MenuItem { Id = 7, Name = "Poulet rôti", Icon = "poulet_roti.jpg", Description = "Poulet rôti accompagné de légumes", Price = 15.5m },
            new MenuItem { Id = 8, Name = "Lasagnes végétariennes", Icon = "lasagnes.jpg", Description = "Lasagnes avec légumes et sauce béchamel", Price = 13.0m },
            new MenuItem { Id = 9, Name = "Pizza Margherita", Icon = "pizza_margherita.jpg", Description = "Pizza avec tomates, mozzarella et basilic", Price = 10.0m },
            new MenuItem { Id = 10, Name = "Paella", Icon = "paella.jpg", Description = "Riz cuit avec fruits de mer, poulet et légumes", Price = 18.0m },

            // Desserts
            new MenuItem { Id = 11, Name = "Tiramisu", Icon = "tiramisu.jpg", Description = "Dessert italien au café et mascarpone", Price = 6.5m },
            new MenuItem { Id = 12, Name = "Crème brûlée", Icon = "creme_brulee.jpg", Description = "Crème avec croûte caramélisée", Price = 7.5m },
            new MenuItem { Id = 13, Name = "Mousse au chocolat", Icon = "mousse_chocolat.jpg", Description = "Mousse légère au chocolat noir", Price = 5.5m },
            new MenuItem { Id = 14, Name = "Panna cotta", Icon = "panna_cotta.jpg", Description = "Crème cuite à la vanille", Price = 6.0m },
            new MenuItem { Id = 15, Name = "Cheesecake", Icon = "cheesecake.jpg", Description = "Gâteau au fromage sur fond de biscuits", Price = 8.0m }
        };
        }

        public static List<MenuItemCategoryMapping> GetMenuItemCategoryMappings()
        {
            return new List<MenuItemCategoryMapping>
        {
            new MenuItemCategoryMapping { MenuCategoryId = 1, MenuItemId = 1 },
            new MenuItemCategoryMapping { MenuCategoryId = 1, MenuItemId = 2 },
            new MenuItemCategoryMapping { MenuCategoryId = 1, MenuItemId = 3 },
            new MenuItemCategoryMapping { MenuCategoryId = 1, MenuItemId = 4 },
            new MenuItemCategoryMapping { MenuCategoryId = 1, MenuItemId = 5 },

            new MenuItemCategoryMapping { MenuCategoryId = 2, MenuItemId = 6 },
            new MenuItemCategoryMapping { MenuCategoryId = 2, MenuItemId = 7 },
            new MenuItemCategoryMapping { MenuCategoryId = 2, MenuItemId = 8 },
            new MenuItemCategoryMapping { MenuCategoryId = 2, MenuItemId = 9 },
            new MenuItemCategoryMapping { MenuCategoryId = 2, MenuItemId = 10 },

            new MenuItemCategoryMapping { MenuCategoryId = 3, MenuItemId = 11 },
            new MenuItemCategoryMapping { MenuCategoryId = 3, MenuItemId = 12 },
            new MenuItemCategoryMapping { MenuCategoryId = 3, MenuItemId = 13 },
            new MenuItemCategoryMapping { MenuCategoryId = 3, MenuItemId = 14 },
            new MenuItemCategoryMapping { MenuCategoryId = 3, MenuItemId = 15 }
        };
        }

        
       
    }
}
