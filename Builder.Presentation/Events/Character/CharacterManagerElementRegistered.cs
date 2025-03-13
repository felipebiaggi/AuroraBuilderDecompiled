using Builder.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Events.Character
{
    public class CharacterManagerElementRegistered : ElementEventBase
    {
        public CharacterManagerElementRegistered(ElementBase element)
            : base(element)
        {
        }
    }
}
