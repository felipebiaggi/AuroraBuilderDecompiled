using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Interfaces
{
    public interface ISupportExpanders
    {
        string Name { get; }

        IEnumerable<string> Listings { get; }

        ObservableCollection<ISelectionRuleExpander> Expanders { get; set; }

        void AddExpander(ISelectionRuleExpander expander);
    }

}
