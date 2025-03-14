using Builder.Core.Events;

namespace Builder.Presentation.Events.Application
{
    public class ResourceDocumentDisplayRequestEvent : EventBase
    {
        public string ResourceFilename { get; }

        public ResourceDocumentDisplayRequestEvent(string resourceFilename)
        {
            ResourceFilename = resourceFilename;
        }
    }
}
