using Builder.Core.Events;

namespace Builder.Presentation.ViewModels.Shell.Start
{
    public class IndexDownloadRequestEvent : EventBase
    {
        public string Url { get; }

        public IndexDownloadRequestEvent(string url)
        {
            Url = url;
        }
    }
}
