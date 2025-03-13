using Builder.Core.Events;
using Builder.Data.Rules;
using Builder.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
