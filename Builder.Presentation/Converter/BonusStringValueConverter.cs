using Builder.Core.Logging;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Builder.Presentation.Converter
{
    public class BonusStringValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                Logger.Warning("BonusStringValueConverter tried to convert from null");
            }
            int num = System.Convert.ToInt32(value);
            if (num >= 0)
            {
                return $"+{num}";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
