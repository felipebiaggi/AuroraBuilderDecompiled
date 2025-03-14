using Builder.Data.Rules;
using System;

namespace Builder.Presentation.Services
{
    public class SelectionRuleEventArgs : EventArgs
    {
        public SelectRule SelectionRule { get; private set; }

        public SelectionRuleEventArgs(SelectRule selectionRule)
        {
            SelectionRule = selectionRule;
        }
    }
}
