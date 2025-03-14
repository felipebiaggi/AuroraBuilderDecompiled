using Builder.Core.Events;

namespace Builder.Presentation.Events.Base
{
    public abstract class StatusUpdateEvent : EventBase
    {
        public string StatusMessage { get; set; }

        protected StatusUpdateEvent(string statusMessage)
        {
            StatusMessage = statusMessage;
        }
    }
}
