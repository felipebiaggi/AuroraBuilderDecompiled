using MahApps.Metro;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Builder.Presentation.Converters
{
    public class InvertedThemeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AppTheme appTheme)
            {
                return appTheme.Resources["BlackBrush"] as SolidColorBrush;
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
