﻿using Builder.Data.Elements;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Builder.Presentation.Converters
{
    public class ItemCostValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }
            if (!(value is Item item))
            {
                return string.Empty;
            }
            return $"{item.Cost} {item.CurrencyAbbreviation.ToUpper()}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
