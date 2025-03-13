using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Builder.Presentation.Models;

namespace Builder.Presentation.Converter
{
    [Obsolete]
    public class IsExpertiseVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SkillItem skillItem && parameter is int proficiencyBonus)
            {
                return (!skillItem.IsExpertise(proficiencyBonus)) ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
