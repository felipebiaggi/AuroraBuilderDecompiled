using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Data.Services
{
    public class ContentFileReference
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public bool IsObsolete { get; set; }

        public bool IsIndex { get; set; }

        public override string ToString()
        {
            return Name + " [" + Url + "]";
        }
    }
}
