using System;
using System.Globalization;
using System.Windows.Data;

namespace Builder.Presentation.Controls
{
    public class PortraitButtonInnerCircleValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return double.Parse(value.ToString()) - 2.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
