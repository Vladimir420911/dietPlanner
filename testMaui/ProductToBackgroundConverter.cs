using System.Globalization;

namespace testMaui
{
    public class ProductToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currentProduct = value as Models.Product;
            var selectedProduct = parameter as Models.Product;
            return currentProduct != null && currentProduct == selectedProduct
                ? Color.FromArgb("#E8F5E9")
                : Colors.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}