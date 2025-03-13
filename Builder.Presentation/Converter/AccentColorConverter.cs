using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MahApps.Metro;

namespace Builder.Presentation.Converter
{
    public class AccentColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Accent accent)
            {
                return accent.Resources["HighlightBrush"] as SolidColorBrush;
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
