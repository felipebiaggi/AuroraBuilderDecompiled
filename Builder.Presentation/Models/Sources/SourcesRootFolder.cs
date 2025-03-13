using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Models.Sources
{
    public class SourcesRootFolder
    {
        public string Name { get; set; }

        public IEnumerable Items { get; }

        public SourcesRootFolder(string name, IEnumerable items = null)
        {
            Name = name;
            Items = items;
        }

        public override string ToString()
        {
            return Name ?? "";
        }
    }
}
