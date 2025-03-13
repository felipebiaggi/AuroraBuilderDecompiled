using Builder.Data.Elements;
using System.Xml;

namespace Builder.Data.ElementParsers
{
    public sealed class FeatFeatureParser : ElementParser
    {
        public override string ParserType => "Feat Feature";

        public override ElementBase ParseElement(XmlNode elementNode)
        {
            return base.ParseElement(elementNode).Construct<FeatFeature>();
        }
    }
}
