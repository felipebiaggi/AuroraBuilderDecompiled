using Builder.Presentation.Events.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Events.Shell
{
    public sealed class CharacterLoadingSliderStatusUpdateEvent : StatusUpdateEvent
    {
        public bool Success { get; }

        public CharacterLoadingSliderStatusUpdateEvent(string statusMessage, bool success = true)
            : base(statusMessage)
        {
            Success = success;
        }
    }
}
