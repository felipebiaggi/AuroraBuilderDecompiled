using System.Linq;

namespace Builder.Data.Rules
{
    public abstract class RuleBase
    {
        public string RuleName { get; }

        public ElementHeader ElementHeader { get; set; }

        public ElementSetters Setter { get; set; }

        protected RuleBase(string ruleName, ElementHeader elementHeader)
        {
            RuleName = ruleName;
            ElementHeader = elementHeader;
            Setter = new ElementSetters();
        }

        public bool ContainsSetter()
        {
            return Setter?.Any() ?? false;
        }

        public override string ToString()
        {
            return RuleName + " (" + ElementHeader.Name + ")";
        }
    }
}
