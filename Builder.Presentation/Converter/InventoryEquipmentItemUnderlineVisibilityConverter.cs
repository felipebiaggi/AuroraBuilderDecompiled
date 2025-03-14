using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Builder.Presentation.Converter
{
    public class InventoryEquipmentItemUnderlineVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is RefactoredEquipmentItem refactoredEquipmentItem) || refactoredEquipmentItem.Item.HideFromInventory || (refactoredEquipmentItem.IsAdorned && refactoredEquipmentItem.AdornerItem.HideFromInventory))
            {
                return Visibility.Collapsed;
            }
            if (refactoredEquipmentItem.IsActivated && !string.IsNullOrWhiteSpace(refactoredEquipmentItem.EquippedLocation))
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
