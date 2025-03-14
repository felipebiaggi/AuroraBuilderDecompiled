using Builder.Core.Events;

namespace Builder.Presentation.Events.Application
{
    public class AdditionalContentUpdatedEvent : EventBase
    {
        public string Message { get; }

        public bool IsUpdated { get; }

        public AdditionalContentUpdatedEvent(string message, bool isUpdated = true)
        {
            Message = message;
            IsUpdated = isUpdated;
        }
    }
}
