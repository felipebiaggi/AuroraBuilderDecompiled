using Builder.Data;

namespace Builder.Presentation.Events.Character
{
    public class CharacterManagerElementUnregistered : ElementEventBase
    {
        public CharacterManagerElementUnregistered(ElementBase element)
            : base(element)
        {
        }
    }
}
