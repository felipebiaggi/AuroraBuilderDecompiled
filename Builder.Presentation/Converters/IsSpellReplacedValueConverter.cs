using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Builder.Presentation.Converters
{
    public class IsSpellReplacedValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            if (string.IsNullOrWhiteSpace(value.ToString()) || value.ToString().Equals("0"))
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
