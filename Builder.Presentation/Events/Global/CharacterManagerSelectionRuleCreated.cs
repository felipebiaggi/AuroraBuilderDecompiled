using Builder.Core.Events;
using Builder.Data.Rules;

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
