using Builder.Data;
using Builder.Data.Rules;

namespace Builder.Presentation.Events
{
    public class ExpanderSelectionChangedEventArgs
    {
        private readonly SelectRule _selectionRule;

        private readonly ElementBase _selectedElement;

        public SelectRule SelectionRule => _selectionRule;

        public ElementBase SelectedElement => _selectedElement;

        public ExpanderSelectionChangedEventArgs(SelectRule selectionRule, ElementBase selectedElement)
        {
            _selectionRule = selectionRule;
            _selectedElement = selectedElement;
        }
    }
}
