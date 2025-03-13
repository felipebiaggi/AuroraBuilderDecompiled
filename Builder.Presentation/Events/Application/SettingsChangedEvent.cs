using Builder.Core.Events;
using Builder.Presentation.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Events.Application
{
    internal class SettingsChangedEvent : EventBase
    {
        public Settings Settings => Settings.Default;
    }
}
