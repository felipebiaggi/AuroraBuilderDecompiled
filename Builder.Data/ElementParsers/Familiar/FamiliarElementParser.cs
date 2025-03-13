using Builder.Data.Elements.Familiar;
using System.Xml;

namespace Builder.Data.ElementParsers.Familiar
{
    public sealed class FamiliarElementParser : ElementParser
    {
        public override string ParserType => "Familiar";

        public override ElementBase ParseElement(XmlNode elementNode)
        {
            return base.ParseElement(elementNode).Construct<FamiliarElement>();
        }
    }
}
