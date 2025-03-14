using Builder.Core.Events;

namespace Builder.Presentation.Services.QuickBar.Commands
{
    public class BundleCommandEvent : EventBase
    {
        public string Command { get; }

        public BundleCommandEvent(string command)
        {
            Command = command;
        }
    }
}
