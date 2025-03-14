using System.Windows;
using System.Windows.Controls;

namespace Builder.Presentation.Extensions
{
    public class ComboBoxItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SelectedItemTemplate { get; set; }

        public DataTemplate ItemTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            bool flag = false;
            if (container is FrameworkElement frameworkElement)
            {
                DependencyObject templatedParent = frameworkElement.TemplatedParent;
                if (templatedParent != null && templatedParent is ComboBox)
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                return ItemTemplate;
            }
            return SelectedItemTemplate;
        }
    }
}
