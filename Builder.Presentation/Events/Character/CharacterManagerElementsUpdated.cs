using Builder.Data;
using Builder.Presentation.Events;
using Builder.Presentation.Events.Character;

namespace Builder.Presentation.Events.Character
{
    public class CharacterManagerElementsUpdated : ElementEventBase
    {
        public CharacterManagerUpdateReason Reason { get; }

        public CharacterManagerElementsUpdated(ElementBase element, CharacterManagerUpdateReason reason)
            : base(element)
        {
            Reason = reason;
        }
    }
}
