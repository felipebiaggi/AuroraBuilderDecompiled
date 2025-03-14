using Builder.Core.Events;
using Builder.Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation
{
    public class CharacterStatusChangedEventArgs : EventBase
    {
        public Character Character { get; }

        public CharacterStatus Status { get; }

        public CharacterStatusChangedEventArgs(Character character, CharacterStatus status)
        {
            Character = character;
            Status = status;
        }
    }
}
