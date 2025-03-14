using Builder.Core.Logging;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Builder.Presentation.Converters
{
    public class InverterBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                Logger.Warning("InverterBooleanToVisibilityConverter tried to convert from null, setting to visible [param: {0}]", parameter);
                return Visibility.Visible;
            }
            if (System.Convert.ToBoolean(value))
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
