using Builder.Presentation.Events.Base;

namespace Builder.Presentation.Events.Developer
{
    public sealed class DeveloperWindowStatusUpdateEvent : StatusUpdateEvent
    {
        public DeveloperWindowStatusUpdateEvent(string statusMessage)
            : base(statusMessage)
        {
        }
    }
}
