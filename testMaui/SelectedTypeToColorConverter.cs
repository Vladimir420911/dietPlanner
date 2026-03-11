using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testMaui
{
    public class SelectedTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string selected = value as string;
            string buttonType = parameter as string;
            return selected == buttonType ? Color.FromArgb("#4CAF50") : Color.FromArgb("#BDBDBD");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
