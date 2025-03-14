using Builder.Core.Events;

namespace Builder.Presentation.Events.Character
{
    public sealed class CharacterNameChangedEvent : EventBase
    {
        public string Name { get; }

        public CharacterNameChangedEvent(string name)
        {
            Name = name;
        }
    }
}
