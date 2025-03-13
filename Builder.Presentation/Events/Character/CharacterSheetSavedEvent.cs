using Builder.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Events.Character
{
    public class CharacterSheetSavedEvent : EventBase
    {
        public string File { get; }

        public CharacterSheetSavedEvent(string file)
        {
            File = file;
        }
    }
}
