using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Builder.Presentation.Converter
{
    public class SuccessColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Application.Current.Resources["DangerColorBrush"] as SolidColorBrush;
            }
            if ((bool)value)
            {
                return Application.Current.Resources["SuccessColorBrush"] as SolidColorBrush;
            }
            return Application.Current.Resources["DangerColorBrush"] as SolidColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
