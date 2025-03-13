using Builder.Core.Events;
using Builder.Presentation.Models;

namespace Builder.Presentation.Events.Character
{
    public class CharacterSavedEvent : EventBase
    {
        public CharacterFile File { get; }

        public CharacterSavedEvent(CharacterFile file)
        {
            File = file;
        }
    }
}
