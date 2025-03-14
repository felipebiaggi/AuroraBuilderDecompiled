using Builder.Core.Events;
using System.Collections.Generic;

namespace Builder.Presentation.Models.Campaign
{
    public class RestrictedSourcesLoadRequest : EventBase
    {
        public IEnumerable<string> SourceIds { get; }

        public RestrictedSourcesLoadRequest(IEnumerable<string> sourceIds)
        {
            SourceIds = sourceIds;
        }
    }
}
