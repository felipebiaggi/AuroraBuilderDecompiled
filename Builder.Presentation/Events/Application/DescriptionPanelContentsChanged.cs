using Builder.Core.Events;

namespace Builder.Presentation.Events.Application
{
    public class DescriptionPanelContentsChanged : EventBase
    {
        public bool IsResouceContent { get; }

        public DescriptionPanelContentsChanged(bool isResouceContent = false)
        {
            IsResouceContent = isResouceContent;
        }
    }
}
