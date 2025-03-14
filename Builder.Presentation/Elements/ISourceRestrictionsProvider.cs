using System.Collections.Generic;

namespace Builder.Presentation.Elements
{
    public interface ISourceRestrictionsProvider
    {
        IEnumerable<string> GetRestrictedSources();

        IEnumerable<string> GetUndefinedRestrictedSources();

        IEnumerable<string> GetRestrictedElements();
    }
}
