using Builder.Data.Elements;
using Builder.Data.Extensions;
using System.Xml;

namespace Builder.Data.ElementParsers
{
    public class ClassElementParser : ElementParser
    {
        public override string ParserType => "Class";

        public override ElementBase ParseElement(XmlNode elementNode)
        {
            Class @class = base.ParseElement(elementNode).Construct<Class>();
            ValidateElementSetters(@class, "hd");
            @class.HitDice = @class.ElementSetters.GetSetter("hd").Value;
            if (@class.ElementSetters.ContainsSetter("short"))
            {
                @class.Short = @class.ElementSetters.GetSetter("short").Value;
            }
            XmlElement xmlElement = elementNode["multiclass"];
            if (xmlElement != null)
            {
                @class.MulticlassId = xmlElement.GetAttributeValue("id");
                @class.CanMulticlass = true;
            }
            return @class;
        }
    }
}
