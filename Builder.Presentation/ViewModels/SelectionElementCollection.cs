using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Builder.Presentation.ViewModels
{
    public class SelectionElementCollection : ObservableCollection<SelectionElement>
    {
        public void Initialize(IEnumerable<SelectionElement> selectionElements, bool recommendedFirst = false)
        {
            Clear();
            if (recommendedFirst)
            {
                foreach (SelectionElement item in selectionElements.Where((SelectionElement x) => x.IsRecommended))
                {
                    Add(item);
                }
                {
                    foreach (SelectionElement item2 in selectionElements.Where((SelectionElement x) => !x.IsRecommended))
                    {
                        Add(item2);
                    }
                    return;
                }
            }
            foreach (SelectionElement selectionElement in selectionElements)
            {
                Add(selectionElement);
            }
        }
    }
}
