using Builder.Core.Events;
using Builder.Data.Rules;

namespace Builder.Presentation.Services
{
    public class NavigationServiceEvaluationEvent : EventBase
    {
        public int Remaining { get; }

        public int Count { get; }

        public SelectRule FirstRequiredSelectionRule { get; }

        public NavigationServiceEvaluationEvent(int remaining, int count, SelectRule firstRequiredSelectionRule)
        {
            Remaining = remaining;
            Count = count;
            FirstRequiredSelectionRule = firstRequiredSelectionRule;
        }
    }
}
