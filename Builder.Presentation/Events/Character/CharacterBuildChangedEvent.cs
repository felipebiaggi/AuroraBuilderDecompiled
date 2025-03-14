using Builder.Core.Events;

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
