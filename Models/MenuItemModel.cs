using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace fastfood.Models
{
    public partial class MenuItemModel : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _icon;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private decimal _price;


        public ObservableCollection<MenuCategoryModel> Categories { get; set; } = []; 
    }
}
