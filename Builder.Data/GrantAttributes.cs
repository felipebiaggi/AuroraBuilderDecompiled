using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Data.Rules.Attributes
{
    public sealed class GrantAttributes
    {
        public string Type { get; set; }

        [Obsolete("use ID property")]
        public string Name { get; set; }

        public int RequiredLevel { get; set; }

        public string Requirements { get; set; }

        public string Id => Name;

        public GrantAttributes()
        {
            RequiredLevel = 1;
        }

        public bool HasRequirements()
        {
            return !string.IsNullOrWhiteSpace(Requirements);
        }

        public bool MeetsLevelRequirements(int level)
        {
            return RequiredLevel <= level;
        }
    }
}
