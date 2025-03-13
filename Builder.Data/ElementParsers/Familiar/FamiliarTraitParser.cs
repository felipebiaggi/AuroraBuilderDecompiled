using Builder.Data.Elements.Familiar;
using System.Xml;

namespace Builder.Data.ElementParsers.Familiar
{
    public sealed class FamiliarTraitParser : ElementParser
    {
        public override string ParserType => "Familiar Trait";

        public override ElementBase ParseElement(XmlNode elementNode)
        {
            return base.ParseElement(elementNode).Construct<FamiliarTraitElement>();
        }
    }
}
