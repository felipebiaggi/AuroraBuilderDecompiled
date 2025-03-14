namespace Builder.Presentation.ViewModels
{
    public sealed class NotificationElementDescriptionDisplayRequestEvent : HtmlDisplayRequestEvent
    {
        public NotificationElementDescriptionDisplayRequestEvent(string html, string stylesheet = null)
            : base(html, stylesheet)
        {
        }
    }

}
