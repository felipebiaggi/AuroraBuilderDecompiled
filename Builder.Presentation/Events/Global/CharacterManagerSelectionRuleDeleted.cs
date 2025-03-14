using Builder.Core.Events;
using Builder.Data.Rules;

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
