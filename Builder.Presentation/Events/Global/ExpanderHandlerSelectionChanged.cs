using Builder.Core.Events;
using Builder.Data;
using Builder.Data.Rules;

namespace Builder.Presentation.Events.Global
{
    public class ExpanderHandlerSelectionChanged : EventBase
    {
        public ElementBase SelectedElement { get; }

        public SelectRule SelectionRule { get; }

        public ExpanderHandlerSelectionChanged(ElementBase selectedElement, SelectRule selectionRule)
        {
            SelectedElement = selectedElement;
            SelectionRule = selectionRule;
        }
    }
}
