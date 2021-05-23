using System;
using System.Numerics;
using System.Globalization;
using System.Windows.Data;

namespace AppV3
{
    [ValueConversion(typeof(Vector2), typeof(string))]
    public class CoordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                Vector2 coord = (Vector2)value;
                string result = $"Coordinates: ({coord.X}, {coord.Y})\n";
                return result;
            }
            else
                return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return value;
        }
    }
}
