using Builder.Data.Elements;
using System.Xml;

namespace Builder.Data.ElementParsers
{
    public sealed class FeatParser : ElementParser
    {
        public override string ParserType => "Feat";

        public override ElementBase ParseElement(XmlNode elementNode)
        {
            return base.ParseElement(elementNode).Construct<Feat>();
        }
    }
}
