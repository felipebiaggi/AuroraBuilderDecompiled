using Builder.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
