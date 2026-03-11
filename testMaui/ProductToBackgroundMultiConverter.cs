using System.Globalization;
using testMaui.Models;

namespace testMaui
{
    public class ProductToBackgroundMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] is Product current && values[1] is Product selected)
            {
                return current == selected ? Color.FromArgb("#E8F5E9") : Colors.White;
            }
            return Colors.White;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}