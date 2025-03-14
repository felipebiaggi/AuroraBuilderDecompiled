using Builder.Data;

namespace Builder.Presentation.ViewModels
{
    public sealed class CompendiumElementDescriptionDisplayRequestEvent : ElementDescriptionDisplayRequestEvent
    {
        public CompendiumElementDescriptionDisplayRequestEvent(ElementBase element, string stylesheet = null)
            : base(element, stylesheet)
        {
        }
    }
}
