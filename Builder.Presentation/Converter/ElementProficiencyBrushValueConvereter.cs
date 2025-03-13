using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Presentation;

namespace Builder.Presentation.Converter
{
    public class ElementProficiencyBrushValueConvereter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Item item && item.AttemptGetSetterValue("proficiency", out var setter))
            {
                if ((from x in CharacterManager.Current.GetElements()
                     where x.Type.Equals("Proficiency")
                     select x.Id).Contains(setter.Value))
                {
                    return Application.Current.Resources["SuccessColorBrush"] as SolidColorBrush;
                }
                return Application.Current.Resources["DangerColorBrush"] as SolidColorBrush;
            }
            return Application.Current.Resources["SuccessColorBrush"] as SolidColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
