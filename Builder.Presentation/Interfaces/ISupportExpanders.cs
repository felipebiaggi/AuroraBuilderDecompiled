using System.Collections.Generic;
using System.Collections.ObjectModel;

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
