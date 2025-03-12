using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Data.Rules
{
    public class SelectionRuleListItem
    {
        public int ID { get; set; }

        public string Text { get; set; }

        public SelectionRuleListItem(int iD, string text)
        {
            ID = iD;
            Text = text;
        }

        public override string ToString()
        {
            return $"({ID}) {Text}";
        }
    }
}
