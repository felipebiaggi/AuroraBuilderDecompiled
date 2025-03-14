using Builder.Core.Events;
using Builder.Data;

namespace Builder.Presentation.Events
{
    public class ExpanderSelectionChanged : EventBase
    {
        public ElementBase Element { get; }

        public ExpanderSelectionChanged(ElementBase element)
        {
            Element = element;
        }
    }

    public class ExpanderSelectionChanged<TElement> : EventBase
    {
        public TElement Element { get; }

        public ExpanderSelectionChanged(TElement element)
        {
            Element = element;
        }
    }
}
