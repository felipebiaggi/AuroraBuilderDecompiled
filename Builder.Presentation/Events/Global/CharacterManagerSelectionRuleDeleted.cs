using Builder.Core.Events;
using Builder.Data.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Events.Global
{
    public class CharacterManagerSelectionRuleDeleted : EventBase
    {
        public SelectRule SelectionRule { get; }

        public CharacterManagerSelectionRuleDeleted(SelectRule selectionRule)
        {
            SelectionRule = selectionRule;
        }
    }
}
