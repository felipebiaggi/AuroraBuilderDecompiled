using Builder.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
