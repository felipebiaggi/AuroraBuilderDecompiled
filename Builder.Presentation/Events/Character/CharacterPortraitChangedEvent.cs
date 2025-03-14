using Builder.Core.Events;

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
