using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Presentation;

namespace Builder.Presentation.Converter
{
    public class ElementProficiencyValueConvereter : IValueConverter
    {
        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Item item && item.AttemptGetSetterValue("proficiency", out var setter) && (from x in CharacterManager.Current.GetElements()
                                                                                                    where x.Type.Equals("Proficiency")
                                                                                                    select x.Id).Contains(setter.Value))
            {
                return Invert ? Visibility.Collapsed : Visibility.Visible;
            }
            return (!Invert) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
