using Builder.Data.Elements;
using System.Xml;


namespace Builder.Data.ElementParsers
{

    public sealed class ProficiencyParser : ElementParser
    {
        public override string ParserType => "Proficiency";

        public override ElementBase ParseElement(XmlNode elementNode)
        {
            return base.ParseElement(elementNode).Construct<Proficiency>();
        }
    }
}
