using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Data.Elements
{
    public class LevelElement : ElementBase
    {
        public override bool AllowMultipleElements => true;

        public int RequiredExperience { get; set; }

        public int Level { get; set; }
    }

}
