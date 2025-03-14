using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Builder.Presentation.Converters
{
    public class AvailablePointsValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (System.Convert.ToInt32(value.ToString()) <= 0)
            {
                return Application.Current.Resources["DangerColorBrush"] as SolidColorBrush;
            }
            return Application.Current.Resources["BlackBrush"] as SolidColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
