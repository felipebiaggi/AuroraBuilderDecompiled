using Builder.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Events.Character
{
    public class CharacterPortraitChangedEvent : EventBase
    {
        public string Filename { get; }

        public CharacterPortraitChangedEvent(string filename)
        {
            Filename = filename;
        }
    }
}
