using System.Windows;
using System.Windows.Controls;

namespace Builder.Presentation.Controls
{
    public class AuroraPanel : ContentControl
    {
        static AuroraPanel()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(AuroraPanel), new FrameworkPropertyMetadata(typeof(AuroraPanel)));
        }
    }
}
