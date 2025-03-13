using System;

namespace Builder.Data.Rules.Attributes
{
    public sealed class GrantAttributes
    {
        public string Type { get; set; }

        [Obsolete("use ID property")]
        public string Name { get; set; }

        public int RequiredLevel { get; set; }

        public string Requirements { get; set; }

#pragma warning disable CS0618 // Type or member is obsolete
        public string Id => Name;
#pragma warning restore CS0618 // Type or member is obsolete

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
