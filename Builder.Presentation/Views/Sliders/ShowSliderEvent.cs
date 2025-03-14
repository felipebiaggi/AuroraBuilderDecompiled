using Builder.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Views.Sliders
{
    public class ShowSliderEvent : EventBase
    {
        public Slider Slider { get; }

        public ShowSliderEvent(Slider slider)
        {
            Slider = slider;
        }
    }
}
