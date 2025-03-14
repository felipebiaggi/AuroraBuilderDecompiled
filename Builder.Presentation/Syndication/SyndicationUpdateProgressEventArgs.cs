using System;

namespace Builder.Presentation.Syndication
{
    public class SyndicationUpdateProgressEventArgs : EventArgs
    {
        public int ProgressPercentage { get; set; }

        public SyndicationUpdateProgressEventArgs(int progressPercentage)
        {
            ProgressPercentage = progressPercentage;
        }
    }
}
