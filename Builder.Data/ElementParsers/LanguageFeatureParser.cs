using Builder.Data.Elements;
using System.Xml;

namespace Builder.Data.ElementParsers
{
    public sealed class LanguageFeatureParser : ElementParser
    {
        public override string ParserType => "Language Feature";

        public override ElementBase ParseElement(XmlNode elementNode)
        {
            return base.ParseElement(elementNode).Construct<LanguageFeature>();
        }
    }
}
