using Builder.Core.Events;
using Builder.Data.Rules;

namespace Builder.Presentation.Services
{
    public class ListSelectionRuleUnregisteredEvent : EventBase
    {
        public SelectRule Rule { get; }

        public ListSelectionRuleUnregisteredEvent(SelectRule rule)
        {
            Rule = rule;
        }
    }
}
