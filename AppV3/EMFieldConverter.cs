using System;
using System.Globalization;
using System.Windows.Data;

namespace AppV3
{
    [ValueConversion(typeof(double), typeof(string))]
    public class EMFieldConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                double field = (double)value;
                string result = $"Electromagnetic field: {field}\n";
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
