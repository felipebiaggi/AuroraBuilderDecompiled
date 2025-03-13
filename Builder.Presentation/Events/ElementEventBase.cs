using Builder.Core.Events;
using Builder.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Events
{
    public abstract class ElementEventBase : EventBase
    {
        public ElementBase Element { get; }

        protected ElementEventBase(ElementBase element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            Element = element;
        }
    }
}
