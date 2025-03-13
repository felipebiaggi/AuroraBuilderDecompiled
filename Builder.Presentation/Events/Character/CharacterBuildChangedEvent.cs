using Builder.Core.Events;
using Builder.Presentation.Models;

namespace Builder.Presentation.Events.Character
{
    public class CharacterBuildChangedEvent : EventBase
    {
        public Character Character { get; }

        public CharacterBuildChangedEvent(Character character)
        {
            Character = character;
        }
    }
}
