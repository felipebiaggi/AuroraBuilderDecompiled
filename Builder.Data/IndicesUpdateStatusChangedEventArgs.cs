using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Data.Files.Updater
{
    public class IndicesUpdateStatusChangedEventArgs
    {
        public string StatusMessage { get; set; }

        public InformationSection Info { get; set; }

        public int ProgressPercentage { get; set; }

        public IndicesUpdateStatusChangedEventArgs(string statusMessage, int progressPercentage, InformationSection information)
        {
            StatusMessage = statusMessage;
            ProgressPercentage = progressPercentage;
            Info = information;
        }
    }
}
