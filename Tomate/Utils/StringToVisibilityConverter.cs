using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Tomate.Models.Catalogo;

namespace Tomate.Utils
{
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class StringCollapse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace((string)value) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visiblity = (Visibility)value;
            return visiblity == Visibility.Visible;
        }
    }

    [ValueConversion(typeof(string), typeof(Visibility))]
    public class StringHidden : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace((string)value) ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visiblity = (Visibility)value;
            return visiblity == Visibility.Visible;
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolHidden : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visiblity = (Visibility)value;
            return visiblity == Visibility.Visible;
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolCollased : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visiblity = (Visibility)value;
            return visiblity == Visibility.Visible;
        }
    }


    [ValueConversion(typeof(string), typeof(int))]
    public class StringOpacity : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace((string)value) ? 1 : 0.7;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var opacity = (int)value;
            return opacity == 1;
        }
    }

    [ValueConversion(typeof(string), typeof(bool))]
    public class StringEnable : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace((string)value) ? false : true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value;
        }
    }

    [ValueConversion(typeof(string), typeof(bool))]
    public class StringEnableInvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace((string)value) ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value;
        }
    }

    

    [ValueConversion(typeof(string), typeof(Visibility))]
    public class StringCollapseGrid : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Producto[,] productos = (Producto[,])value;
            string[] args = $"{parameter}".Split('|');
            var producto = productos[int.Parse(args[1]), int.Parse(args[2])];
            if (producto == null)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visiblity = (Visibility)value;
            return visiblity == Visibility.Visible;
        }
    }

    [ValueConversion(typeof(object), typeof(object))]
    public class MatrixProducto : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            Producto[,] productos = (Producto[,])value;
            string[] args = $"{parameter}".Split('|');
            var producto = productos[int.Parse(args[1]), int.Parse(args[2])];
            if (producto == null)
            {
                return "";
            }
            if (args[0] == "Precio")
            {
                return producto.PrecioFormato;
            }
            else if (args[0] == "Nombre")
            {
                return producto.Nombre;
            }
            return producto;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

   

}
