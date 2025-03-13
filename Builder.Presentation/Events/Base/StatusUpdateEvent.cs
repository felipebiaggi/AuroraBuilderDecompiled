using Builder.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
