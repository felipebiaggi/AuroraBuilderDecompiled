using Builder.Core.Events;

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
