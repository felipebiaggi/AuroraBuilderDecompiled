using Builder.Core.Events;
using Builder.Presentation.Properties;

namespace Builder.Presentation.Events.Application
{
    internal class SettingsChangedEvent : EventBase
    {
        public Settings Settings => Settings.Default;
    }
}
