using Builder.Core.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Builder.Presentation.Converter
{
    public class NullableInvertedBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                Logger.Warning("InverterBooleanToVisibilityConverter tried to convert from null, setting to visible [param: {0}]", parameter);
                return Visibility.Collapsed;
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
