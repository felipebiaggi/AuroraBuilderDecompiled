using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Data.Elements
{
    public class Option : ElementBase
    {
        public override bool AllowMultipleElements => true;

        public bool IsInternal { get; set; }

        public bool IsDefault { get; set; }
    }
}
