using Builder.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Views
{
    public class QuickSearchBarEventArgs : EventBase
    {
        public bool IsSearch { get; set; }

        public string SearchCriteria { get; }

        public bool IsCommand { get; set; }

        public string Command { get; set; }

        public string[] CommandParameters { get; set; }

        public QuickSearchBarEventArgs(string searchCriteria)
        {
            SearchCriteria = searchCriteria;
        }
    }
}
