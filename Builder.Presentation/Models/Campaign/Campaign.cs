using System.Collections.Generic;

namespace Builder.Presentation.Models.Campaign
{
    public class Campaign
    {
        public string Name { get; set; }

        public List<string> RestrictedSources { get; set; }

        public List<string> Restricted { get; set; }

        public Campaign(string name)
        {
            Name = name;
            Restricted = new List<string>();
            RestrictedSources = new List<string>();
        }
    }
}
