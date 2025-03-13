using Builder.Data.Elements;
using System.Xml;

namespace Builder.Data.ElementParsers
{
    public sealed class ArchetypeParser : ElementParser
    {
        public override string ParserType => "Archetype";

        public override ElementBase ParseElement(XmlNode elementNode)
        {
            return base.ParseElement(elementNode).Construct<Archetype>();
        }
    }
}
