using Builder.Core.Events;
using Builder.Data;
using System;

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
