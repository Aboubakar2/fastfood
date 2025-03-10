using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace fastfood.Converters
{
    public class EditModeButtonTextConverter : IValueConverter
    {
        // Cette méthode convertit la valeur booléenne IsEditMode en texte pour le bouton
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Modifier" : "Ajouter";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;  // Nous n'avons pas besoin de convertir en arrière
        }
    }

}