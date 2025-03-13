using Builder.Core.Events;
using Builder.Data.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
