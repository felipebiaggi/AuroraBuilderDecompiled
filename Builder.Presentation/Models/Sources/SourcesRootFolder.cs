using System.Collections;

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
