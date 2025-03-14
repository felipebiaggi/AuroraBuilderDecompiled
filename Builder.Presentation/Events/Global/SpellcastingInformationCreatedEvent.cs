using Builder.Core.Events;
using Builder.Data.Elements;

namespace Builder.Presentation.Events.Global
{
    public class SpellcastingInformationCreatedEvent : EventBase
    {
        public SpellcastingInformation Information { get; }

        public SpellcastingInformationCreatedEvent(SpellcastingInformation information)
        {
            Information = information;
        }
    }
}
