using Builder.Data;

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
