using Builder.Data.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Builder.Data.ElementParsers
{
    public class WeaponElementParser : ItemElementParser
    {
        public override string ParserType => "Weapon";

        public override ElementBase ParseElement(XmlNode elementNode)
        {
            WeaponElement weaponElement = base.ParseElement(elementNode).ConstructFrom<WeaponElement, Item>();
            weaponElement.Supports.Add(weaponElement.Name);
            weaponElement.Supports.Add(weaponElement.Name.ToLowerInvariant());
            return weaponElement;
        }
    }
}
