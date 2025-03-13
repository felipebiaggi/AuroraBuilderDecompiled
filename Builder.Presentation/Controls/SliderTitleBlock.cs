using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

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
