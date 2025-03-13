using System.Windows;
using System.Windows.Controls;
using Builder.Presentation.Extensions;

namespace Builder.Presentation.Extensions
{
    public class ComboBoxItemTemplateSelector2 : DataTemplateSelector
    {
        public DataTemplate SelectedTemplate { get; set; }

        public DataTemplate DropDownTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container.GetVisualParent<ComboBoxItem>() == null)
            {
                return SelectedTemplate;
            }
            return DropDownTemplate;
        }
    }
}
