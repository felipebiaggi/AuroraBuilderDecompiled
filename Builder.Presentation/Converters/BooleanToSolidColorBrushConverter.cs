﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Builder.Presentation.Converters
{
    public class BooleanToSolidColorBrushConverter : IValueConverter
    {
        public string TrueColor { get; set; }

        public string FalseColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (bool)value)
            {
                return new SolidColorBrush(Colors.DarkGreen);
            }
            return new SolidColorBrush(Colors.DarkRed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
