using System.Windows;
using System.Windows.Controls;

namespace Builder.Presentation.Controls
{
    public class SliderTitleBlock : ContentControl
    {
        static SliderTitleBlock()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SliderTitleBlock), new FrameworkPropertyMetadata(typeof(SliderTitleBlock)));
        }
    }
}
