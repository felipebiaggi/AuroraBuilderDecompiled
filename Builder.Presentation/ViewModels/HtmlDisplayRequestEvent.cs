using Builder.Core.Events;

namespace Builder.Presentation.ViewModels
{
    public class HtmlDisplayRequestEvent : EventBase
    {
        public string Html { get; }

        public string Stylesheet { get; }

        public bool ContainsStylesheet => !string.IsNullOrWhiteSpace(Stylesheet);

        public HtmlDisplayRequestEvent(string html, string stylesheet = null)
        {
            Html = html;
            Stylesheet = stylesheet;
        }
    }
}
