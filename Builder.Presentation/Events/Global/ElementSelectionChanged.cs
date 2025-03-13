using Builder.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Events.Global
{
    public class ElementSelectionChanged : ElementEventBase
    {
        public ElementSelectionChanged(ElementBase element)
            : base(element)
        {
        }
    }
}
