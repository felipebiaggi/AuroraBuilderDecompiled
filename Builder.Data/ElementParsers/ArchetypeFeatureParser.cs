using Builder.Data.Elements;
using System.Xml;

namespace Builder.Data.ElementParsers
{
    public sealed class ArchetypeFeatureParser : ElementParser
    {
        public override string ParserType => "Archetype Feature";

        public override ElementBase ParseElement(XmlNode elementNode)
        {
            return base.ParseElement(elementNode).Construct<ArchetypeFeature>();
        }
    }
}
