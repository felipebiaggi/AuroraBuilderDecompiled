using Builder.Presentation.Events.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Events.Shell
{
    public sealed class MainWindowStatusUpdateEvent : StatusUpdateEvent
    {
        public int ProgressPercentage { get; set; }

        public bool IsIndeterminateProgress { get; set; }

        public bool IsNormal { get; set; } = true;

        public bool IsSuccess { get; set; }

        public bool IsDanger { get; set; }

        public MainWindowStatusUpdateEvent(string statusMessage)
            : this(statusMessage, -1)
        {
        }

        public MainWindowStatusUpdateEvent(string statusMessage, int progressPercentage)
            : base(statusMessage)
        {
            ProgressPercentage = progressPercentage;
        }
    }
}
