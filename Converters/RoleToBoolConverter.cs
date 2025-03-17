using System;
using System.Globalization;
using Microsoft.Maui.Controls;
namespace fastfood.Converters
{
    public class RoleToBoolConverter : IValueConverter
    {
        //Converte a string in boolean
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string role && role == "Administrateur")
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isAdmin && isAdmin)
            {
                return "Administrateur";
            }
            return "Caissier";
        }
    }
}
