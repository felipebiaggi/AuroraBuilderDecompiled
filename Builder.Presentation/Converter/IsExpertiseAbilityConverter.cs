using Builder.Presentation.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Builder.Presentation.Converter
{
    [Obsolete]
    public class IsExpertiseAbilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SkillItem skillItem && parameter is int proficiencyBonus)
            {
                return skillItem.IsExpertise(proficiencyBonus);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
