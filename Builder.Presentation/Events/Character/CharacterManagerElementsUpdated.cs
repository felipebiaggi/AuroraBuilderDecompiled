using Builder.Data;

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
