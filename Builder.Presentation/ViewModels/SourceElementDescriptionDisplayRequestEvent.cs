using Builder.Data;

namespace Builder.Presentation.ViewModels
{
    public sealed class SourceElementDescriptionDisplayRequestEvent : ElementDescriptionDisplayRequestEvent
    {
        public SourceElementDescriptionDisplayRequestEvent(ElementBase element, string stylesheet = null)
            : base(element, stylesheet)
        {
        }
    }
}
