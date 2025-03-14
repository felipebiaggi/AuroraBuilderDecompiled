using System;
using System.Globalization;
using System.Windows.Data;

namespace Builder.Presentation.Converter
{
    public class AccentNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString().Replace("Application", "").Replace("Aurora", "")
                .Trim()
                .ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
