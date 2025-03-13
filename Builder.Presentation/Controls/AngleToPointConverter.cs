using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Builder.Presentation.Controls
{
    internal class AngleToPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double num = (double)value;
            double num2 = 50.0;
            double num3 = num * Math.PI / 180.0;
            double x = Math.Sin(num3) * num2 + num2;
            double y = (0.0 - Math.Cos(num3)) * num2 + num2;
            return new Point(x, y);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
