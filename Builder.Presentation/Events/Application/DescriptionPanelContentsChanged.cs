using Builder.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Events.Application
{
    public class DescriptionPanelContentsChanged : EventBase
    {
        public bool IsResouceContent { get; }

        public DescriptionPanelContentsChanged(bool isResouceContent = false)
        {
            IsResouceContent = isResouceContent;
        }
    }
}
