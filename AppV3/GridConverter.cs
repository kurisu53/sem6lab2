using System;
using System.Windows.Data;
using System.Globalization;
using ClassLibraryV3;

namespace AppV3
{
    [ValueConversion(typeof(V3DataOnGrid), typeof(string))]
    public class GridConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                V3DataOnGrid dataOnGrid = (V3DataOnGrid)value;
                string result = $"X axis has {dataOnGrid.XGrid.NodesCount} nodes,\nY axis has {dataOnGrid.YGrid.NodesCount} nodes\n";
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
