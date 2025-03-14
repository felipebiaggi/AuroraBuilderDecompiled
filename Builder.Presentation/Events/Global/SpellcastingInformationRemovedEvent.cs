using Builder.Core.Events;
using Builder.Data.Elements;

namespace Builder.Presentation.Events.Global
{
    public class SpellcastingInformationRemovedEvent : EventBase
    {
        public SpellcastingInformation Information { get; }

        public SpellcastingInformationRemovedEvent(SpellcastingInformation information)
        {
            Information = information;
        }
    }
}
