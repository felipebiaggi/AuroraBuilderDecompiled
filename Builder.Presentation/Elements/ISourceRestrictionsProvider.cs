using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Elements
{
    public interface ISourceRestrictionsProvider
    {
        IEnumerable<string> GetRestrictedSources();

        IEnumerable<string> GetUndefinedRestrictedSources();

        IEnumerable<string> GetRestrictedElements();
    }
}
