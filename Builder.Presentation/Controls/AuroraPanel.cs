using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

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
