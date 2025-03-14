using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Elements;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Builder.Presentation.Services.Data
{
    public class SpellScrollContentGenerator
    {
        public List<ElementBase> Generate(IEnumerable<ElementBase> content, MagicItemElement template)
        {
            List<ElementBase> list = new List<ElementBase>();
            if (content == null || template == null)
            {
                return list;
            }
            foreach (Spell item in content.Where((ElementBase x) => x.Type.Equals("Spell")).Cast<Spell>().ToList())
            {
                if (!item.Id.Contains("_SPELL_"))
                {
                    Logger.Warning(item.Name + " doesn't contain _SPELL_ in the id (" + item.Id + ")");
                    continue;
                }
                string text = Regex.Split(item.Id, "_SPELL_").LastOrDefault() ?? "";
                string input = "ID_INTERNAL_MAGIC_ITEM_SPELL_SCROLL_" + text;
                if (!ElementsHelper.ValidateID(input) && Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                string id = ElementsHelper.SanitizeID(input);
                MagicItemElement magicItemElement = new MagicItemElement
                {
                    ElementHeader = new ElementHeader("Spell Scroll, " + item.Name, template.Type ?? "", item.Source, id)
                };
                magicItemElement.CalculableWeight = template.CalculableWeight;
                magicItemElement.Category = "Spell Scrolls";
                magicItemElement.ItemType = template.ItemType;
                magicItemElement.IsStackable = true;
                magicItemElement.Description = template.Description;
                foreach (ElementSetters.Setter elementSetter in template.ElementSetters)
                {
                    if (elementSetter.Name == "keywords" || elementSetter.Name == "rarity" || string.IsNullOrWhiteSpace(elementSetter.Value))
                    {
                        continue;
                    }
                    ElementSetters.Setter setter = new ElementSetters.Setter(elementSetter.Name, elementSetter.Value);
                    foreach (KeyValuePair<string, string> additionalAttribute in elementSetter.AdditionalAttributes)
                    {
                        setter.AdditionalAttributes.Add(additionalAttribute.Key, additionalAttribute.Value);
                    }
                    if (setter.Name == "cost")
                    {
                        switch (item.Level)
                        {
                            case 0:
                                setter.Value = "0";
                                break;
                            case 2:
                            case 3:
                                setter.Value = "0";
                                break;
                            case 4:
                            case 5:
                                setter.Value = "0";
                                break;
                            case 6:
                            case 7:
                            case 8:
                                setter.Value = "0";
                                break;
                            case 9:
                                setter.Value = "0";
                                break;
                        }
                    }
                    magicItemElement.ElementSetters.Add(setter);
                }
                if (item.HasSupports)
                {
                    foreach (string support in item.Supports)
                    {
                        magicItemElement.Keywords.Add(support);
                    }
                }
                magicItemElement.Keywords.AddRange(item.Keywords);
                int num = 0;
                int num2 = 0;
                string text2 = "Common";
                switch (item.Level)
                {
                    case 0:
                    case 1:
                        text2 = "Common";
                        num = 13;
                        num2 = 5;
                        break;
                    case 2:
                        text2 = "Uncommon";
                        num = 13;
                        num2 = 5;
                        break;
                    case 3:
                        text2 = "Uncommon";
                        num = 15;
                        num2 = 7;
                        break;
                    case 4:
                        text2 = "Rare";
                        num = 15;
                        num2 = 7;
                        break;
                    case 5:
                        text2 = "Rare";
                        num = 17;
                        num2 = 9;
                        break;
                    case 6:
                        text2 = "Very Rare";
                        num = 17;
                        num2 = 9;
                        break;
                    case 7:
                        text2 = "Very Rare";
                        num = 18;
                        num2 = 10;
                        break;
                    case 8:
                        text2 = "Very Rare";
                        num = 18;
                        num2 = 10;
                        break;
                    case 9:
                        text2 = "Legendary";
                        num = 19;
                        num2 = 11;
                        break;
                }
                magicItemElement.ElementSetters.Add(new ElementSetters.Setter("rarity", text2));
                magicItemElement.Rarity = text2;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<p>A <em>spell scroll</em> bears the words of a single spell, written in a mystical cipher. If the spell is on your class’s spell list, you can use an action to read the scroll and cast its spell without having to provide any of the spell’s components. Otherwise, the scroll is unintelligible.</p>");
                stringBuilder.Append("<p class=\"indent\">If the spell is on your class’s spell list but of a higher level than you can normally cast, you must make an ability check using your spellcasting ability to determine whether you cast it successfully. The DC equals 10 + the spell’s level. On a failed check, the spell disappears from the scroll with no other effect.</p>");
                stringBuilder.Append("<p class=\"indent\">Once the spell is cast, the words on the scroll fade, and the scroll itself crumbles to dust.</p>");
                stringBuilder.Append($"<p class=\"indent\">The level of the spell on the scroll determines the spell’s saving throw DC ({num}) and attack bonus (+{num2}), as well as the scroll’s rarity ({text2}).</p>");
                stringBuilder.Append("<div class=\"reference\">");
                stringBuilder.Append("<div element=\"" + item.Id + "\" />");
                stringBuilder.Append("</div>");
                magicItemElement.Description = stringBuilder.ToString();
                list.Add(magicItemElement);
            }
            return list;
        }
    }
}
