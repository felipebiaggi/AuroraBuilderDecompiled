using Builder.Core.Events;
using Builder.Data.Rules;

namespace Builder.Presentation.Services
{
    public class ListSelectionRuleRegisteredEvent : EventBase
    {
        public SelectRule Rule { get; }

        public ListSelectionRuleRegisteredEvent(SelectRule rule)
        {
            Rule = rule;
        }
    }
}
