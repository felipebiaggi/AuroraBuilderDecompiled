using Builder.Presentation.Events.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
