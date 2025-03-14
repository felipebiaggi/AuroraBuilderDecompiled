using Builder.Core.Events;
using Builder.Data;

namespace Builder.Presentation.ViewModels
{
    public class ElementDescriptionDisplayRequestEvent : EventBase
    {
        public ElementBase Element { get; }

        public string Stylesheet { get; }

        public bool ContainsStylesheet => !string.IsNullOrWhiteSpace(Stylesheet);

        public bool IgnoreGeneratedDescription { get; set; }

        public ElementDescriptionDisplayRequestEvent(ElementBase element, string stylesheet = null)
        {
            Element = element;
            Stylesheet = stylesheet;
        }
    }
}
