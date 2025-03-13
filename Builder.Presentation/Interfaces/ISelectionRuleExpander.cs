using Builder.Data.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Interfaces
{
    public interface ISelectionRuleExpander
    {
        int Number { get; }

        SelectRule SelectionRule { get; }

        [Obsolete("get it from SelectionRule.UniqueIdentifier")]
        string UniqueIdentifier { get; }

        bool IsSelectionMade();

        void SetSelection(string elementId);

        void FocusExpander();
    }
}
