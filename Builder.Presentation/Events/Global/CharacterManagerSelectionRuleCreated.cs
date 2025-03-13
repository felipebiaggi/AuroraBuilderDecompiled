using Builder.Core.Events;
using Builder.Data.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Events.Global
{
    public class CharacterManagerSelectionRuleCreated : EventBase
    {
        public SelectRule SelectionRule { get; }

        public CharacterManagerSelectionRuleCreated(SelectRule selectionRule)
        {
            SelectionRule = selectionRule;
        }
    }
}
