using System;
using System.Globalization;
using System.Windows.Data;

namespace Builder.Presentation.Controls
{
    internal class AngleToIsLargeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value > 180.0;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
