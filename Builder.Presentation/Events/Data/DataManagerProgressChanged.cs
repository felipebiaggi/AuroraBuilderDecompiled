using Builder.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Events.Data
{
    public class DataManagerProgressChanged : EventBase
    {
        public string ProgressMessage { get; set; }

        public int ProgressPercentage { get; set; }

        public bool InProgress { get; set; }

        public DataManagerProgressChanged(string progressMessage, int progressPercentage, bool inProgress)
        {
            InProgress = inProgress;
            ProgressMessage = progressMessage;
            ProgressPercentage = progressPercentage;
        }
    }
}
