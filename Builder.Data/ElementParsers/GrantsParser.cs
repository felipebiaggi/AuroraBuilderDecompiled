using Builder.Data.Elements;
using System.Xml;

namespace Builder.Data.ElementParsers
{
    public sealed class GrantsParser : ElementParser
    {
        public override string ParserType => "Grants";

        public override ElementBase ParseElement(XmlNode elementNode)
        {
            return base.ParseElement(elementNode).Construct<Feat>();
        }
    }
}
