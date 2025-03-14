using Builder.Core.Events;
using System;

namespace Builder.Presentation.Services
{
    [Obsolete("rename to generic navigation for main tab application")]
    public class SelectionRuleNavigationArgs : EventBase
    {
        public NavigationLocation Location { get; }

        public SelectionRuleNavigationArgs(NavigationLocation location)
        {
            Location = location;
        }
    }
}
