using Builder.Data;

namespace Builder.Presentation.ViewModels
{
    public sealed class SpellcastingElementDescriptionDisplayRequestEvent : ElementDescriptionDisplayRequestEvent
    {
        public SpellcastingElementDescriptionDisplayRequestEvent(ElementBase element, string stylesheet = null)
            : base(element, stylesheet)
        {
        }
    }
}
