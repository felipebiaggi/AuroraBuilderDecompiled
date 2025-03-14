using Builder.Core.Events;
using Builder.Presentation.Models;

namespace Builder.Presentation.Events.Character
{
    public class CharacterBuildChangedEvent : EventBase
    {
        public Models.Character Character { get; }

        public CharacterBuildChangedEvent(Models.Character character)
        {
            Character = character;
        }
    }
}
