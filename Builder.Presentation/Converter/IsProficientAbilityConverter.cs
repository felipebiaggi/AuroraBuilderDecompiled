using Builder.Presentation.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Builder.Presentation.Converter
{
    [Obsolete]
    public class IsProficientAbilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SkillItem skillItem)
            {
                return skillItem.IsProficient;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
