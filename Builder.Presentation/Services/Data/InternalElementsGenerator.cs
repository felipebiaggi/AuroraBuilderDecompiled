using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Data.Rules;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Builder.Presentation.Services.Data
{
    public class InternalElementsGenerator
    {
        public bool IncludeSpellsNotOnSpelllist { get; set; }

        private string GenerateInternalId(ElementBase element, string splitSection, string format)
        {
            string arg = Regex.Split(element.Id, splitSection).LastOrDefault() ?? "";
            return string.Format(format, arg).ToUpperInvariant();
        }

        public List<ElementBase> GenerateInternalFeats(IEnumerable<ElementBase> content)
        {
            List<ElementBase> list = new List<ElementBase>();
            if (content == null)
            {
                return list;
            }
            List<Feat> list2 = content.Where((ElementBase x) => x.Type.Equals("Feat")).Cast<Feat>().ToList();
            List<Source> source = content.Where((ElementBase x) => x.Type.Equals("Source")).Cast<Source>().ToList();
            foreach (Feat element in list2)
            {
                if (element.Source.Equals("Core") || element.Source.Equals("Internal"))
                {
                    continue;
                }
                if (!element.Id.Contains("_FEAT_"))
                {
                    Logger.Warning(element.Name + " doesn't contain _FEAT_ in the id (" + element.Id + ")");
                    continue;
                }
                string text = (source.FirstOrDefault((Source x) => x.Name.Equals(element.Source))?.Abbreviation ?? "").ToUpperInvariant();
                string id = GenerateInternalId(element, "_FEAT_", "ID_" + text + "_INTERNAL_ITEM_FEAT_PROXY_{0}");
                Item item = new Item
                {
                    ElementHeader = new ElementHeader("Additional " + element.Type + ", " + element.Name, "Item", element.Source, id)
                };
                item.Category = "Additional " + element.Type;
                item.Description = element.Description;
                item.Slot = "proxy";
                item.ElementSetters.Add(new ElementSetters.Setter("inventory-hidden", "true"));
                item.HideFromInventory = true;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<p><em>You can equip this item to “enable” it. It remains hidden from the inventory on your character sheet.</em></p>");
                stringBuilder.Append("<div class=\"reference\">");
                stringBuilder.Append("<div element=\"" + element.Id + "\" />");
                stringBuilder.Append("</div>");
                item.Description = stringBuilder.ToString();
                item.Keywords.AddRange(element.Keywords);
                GrantRule grantRule = new GrantRule(item.ElementHeader);
                grantRule.Attributes.Type = element.Type;
                grantRule.Attributes.Name = element.Id;
                item.Rules.Add(grantRule);
                item.IncludeInCompendium = false;
                list.Add(item);
            }
            return list;
        }

        public List<ElementBase> GenerateInternalLanguages(IEnumerable<ElementBase> content)
        {
            List<ElementBase> list = new List<ElementBase>();
            if (content == null)
            {
                return list;
            }
            List<Language> list2 = content.Where((ElementBase x) => x.Type.Equals("Language")).Cast<Language>().ToList();
            List<Source> source = content.Where((ElementBase x) => x.Type.Equals("Source")).Cast<Source>().ToList();
            foreach (Language element in list2)
            {
                if (element.Source.Equals("Core") || element.Source.Equals("Internal"))
                {
                    continue;
                }
                string text = "_LANGUAGE_";
                if (!element.Id.Contains(text))
                {
                    Logger.Warning(element.Name + " doesn't contain " + text + " in the id (" + element.Id + ")");
                    continue;
                }
                string text2 = (source.FirstOrDefault((Source x) => x.Name.Equals(element.Source))?.Abbreviation ?? "").ToUpperInvariant();
                string id = GenerateInternalId(element, text, "ID_" + text2 + "_INTERNAL_ITEM_LANGUAGE_PROXY" + text + "{0}");
                Item item = new Item
                {
                    ElementHeader = new ElementHeader("Additional " + element.Type + ", " + element.Name, "Item", element.Source, id)
                };
                item.Category = "Additional " + element.Type;
                item.Description = element.Description;
                item.Slot = "proxy";
                item.ElementSetters.Add(new ElementSetters.Setter("inventory-hidden", "true"));
                item.HideFromInventory = true;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<p><em>You can equip this item to “enable” it. It remains hidden from the inventory on your character sheet.</em></p>");
                stringBuilder.Append("<div class=\"reference\">");
                stringBuilder.Append("<div element=\"" + element.Id + "\" />");
                stringBuilder.Append("</div>");
                item.Description = stringBuilder.ToString();
                item.Keywords.AddRange(element.Keywords);
                GrantRule grantRule = new GrantRule(item.ElementHeader);
                grantRule.Attributes.Type = element.Type;
                grantRule.Attributes.Name = element.Id;
                item.Rules.Add(grantRule);
                item.IncludeInCompendium = false;
                list.Add(item);
            }
            return list;
        }

        public List<ElementBase> GenerateInternalProficiency(IEnumerable<ElementBase> content)
        {
            List<ElementBase> list = new List<ElementBase>();
            if (content == null)
            {
                return list;
            }
            List<Proficiency> list2 = content.Where((ElementBase x) => x.Type.Equals("Proficiency")).Cast<Proficiency>().ToList();
            List<Source> source = content.Where((ElementBase x) => x.Type.Equals("Source")).Cast<Source>().ToList();
            foreach (Proficiency element in list2)
            {
                if (element.Source.Equals("Core") || element.Source.Equals("Internal"))
                {
                    continue;
                }
                string text = "_PROFICIENCY_";
                if (!element.Id.Contains(text))
                {
                    Logger.Warning(element.Name + " doesn't contain " + text + " in the id (" + element.Id + ")");
                    continue;
                }
                string text2 = (source.FirstOrDefault((Source x) => x.Name.Equals(element.Source))?.Abbreviation ?? "").ToUpperInvariant();
                string id = GenerateInternalId(element, text, "ID_" + text2 + "_INTERNAL_ITEM_PROFICIENCY_PROXY" + text + "{0}");
                _ = "Skill Proficiency (" + element.Name + ")";
                string text3 = element.Name;
                if (element.HasSupports && element.Supports.Contains("Skill"))
                {
                    text3 = "Skill Proficiency (" + element.Name + ")";
                }
                Item item = new Item
                {
                    ElementHeader = new ElementHeader("Additional " + element.Type + ", " + text3, "Item", element.Source, id)
                };
                item.Category = "Additional " + element.Type;
                item.Slot = "proxy";
                item.ElementSetters.Add(new ElementSetters.Setter("inventory-hidden", "true"));
                item.HideFromInventory = true;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<p><em>You can equip this item to “enable” it. It remains hidden from the inventory on your character sheet.</em></p>");
                stringBuilder.Append("<div class=\"reference\">");
                stringBuilder.Append("<div element=\"" + element.Id + "\" />");
                stringBuilder.Append("</div>");
                item.Description = stringBuilder.ToString();
                item.ElementSetters.Add(new ElementSetters.Setter("inventory-hidden", "true"));
                item.Keywords.AddRange(element.Keywords);
                GrantRule grantRule = new GrantRule(item.ElementHeader);
                grantRule.Attributes.Type = element.Type;
                grantRule.Attributes.Name = element.Id;
                item.Rules.Add(grantRule);
                item.IncludeInCompendium = false;
                list.Add(item);
            }
            return list;
        }

        public List<ElementBase> GenerateInternalAsi(IEnumerable<ElementBase> content)
        {
            List<ElementBase> list = new List<ElementBase>();
            if (content == null)
            {
                return list;
            }
            List<ElementBase> list2 = content.Where((ElementBase x) => (x.Type.Equals("Ability Score Improvement") && x.Id.Equals("ID_INTERNAL_ASI_STRENGTH")) || x.Id.Equals("ID_INTERNAL_ASI_DEXTERITY") || x.Id.Equals("ID_INTERNAL_ASI_CONSTITUTION") || x.Id.Equals("ID_INTERNAL_ASI_INTELLIGENCE") || x.Id.Equals("ID_INTERNAL_ASI_WISDOM") || x.Id.Equals("ID_INTERNAL_ASI_CHARISMA")).ToList();
            List<Source> source = content.Where((ElementBase x) => x.Type.Equals("Source")).Cast<Source>().ToList();
            foreach (ElementBase element in list2)
            {
                if (element.Source.Equals("Core") || element.Source.Equals("Internal"))
                {
                    continue;
                }
                string text = "_ASI_";
                if (!element.Id.Contains(text))
                {
                    Logger.Warning(element.Name + " doesn't contain " + text + " in the id (" + element.Id + ")");
                    continue;
                }
                string text2 = (source.FirstOrDefault((Source x) => x.Name.Equals(element.Source))?.Abbreviation ?? "").ToUpperInvariant();
                string id = GenerateInternalId(element, text, "ID_" + text2 + "_INTERNAL_ITEM_PROXY" + text + "{0}");
                Item item = new Item
                {
                    ElementHeader = new ElementHeader("Additional " + element.Type + ", " + element.Name, "Item", element.Source, id)
                };
                item.Category = "Additional " + element.Type;
                item.Slot = "proxy";
                item.ElementSetters.Add(new ElementSetters.Setter("inventory-hidden", "true"));
                item.HideFromInventory = true;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<p><em>You can equip this item to “enable” it. It remains hidden from the inventory on your character sheet.</em></p>");
                stringBuilder.Append("<div class=\"reference\">");
                stringBuilder.Append("<div element=\"" + element.Id + "\" />");
                stringBuilder.Append("</div>");
                item.Description = stringBuilder.ToString();
                item.ElementSetters.Add(new ElementSetters.Setter("inventory-hidden", "true"));
                item.Keywords.AddRange(element.Keywords);
                GrantRule grantRule = new GrantRule(item.ElementHeader);
                grantRule.Attributes.Type = element.Type;
                grantRule.Attributes.Name = element.Id;
                item.Rules.Add(grantRule);
                item.IncludeInCompendium = false;
                list.Add(item);
            }
            return list;
        }

        public List<ElementBase> GenerateInternalSpells(IEnumerable<ElementBase> content)
        {
            List<ElementBase> list = new List<ElementBase>();
            if (content == null)
            {
                return list;
            }
            List<Spell> list2 = content.Where((ElementBase x) => x.Type.Equals("Spell")).Cast<Spell>().ToList();
            List<Source> source = content.Where((ElementBase x) => x.Type.Equals("Source")).Cast<Source>().ToList();
            List<string> list3 = (from x in (from x in content
                                             where x.HasSpellcastingInformation && !x.SpellcastingInformation.IsExtension && !x.SpellcastingInformation.ElementHeader.Id.Equals("ID_WOTC_UA20170313_CLASS_FEATURE_MYSTIC_PSIONICS")
                                             select x.SpellcastingInformation).ToList()
                                  select x.Name).Distinct().ToList();
            list3.Insert(0, "");
            foreach (Spell element in list2)
            {
                if (element.Source.Equals("Core") || element.Source.Equals("Internal"))
                {
                    continue;
                }
                string text = "_SPELL_";
                if (!element.Id.Contains(text))
                {
                    Logger.Warning(element.Name + " doesn't contain " + text + " in the id (" + element.Id + ")");
                    continue;
                }
                Source source2 = source.FirstOrDefault((Source x) => x.Name.Equals(element.Source));
                string text2 = (source2?.Abbreviation ?? "").ToUpperInvariant();
                if (string.IsNullOrWhiteSpace(text2) && source2 != null)
                {
                    text2 = source2.Name[0].ToString();
                }
                foreach (string item2 in list3)
                {
                    string id = GenerateInternalId(element, text, "ID_" + text2 + "_INTERNAL_ITEM_" + item2.Replace(" ", "_") + "_SPELL_PROXY" + text + "{0}");
                    string name = "Additional " + element.Type + ", " + element.Name;
                    if (!string.IsNullOrWhiteSpace(item2))
                    {
                        name = "Additional " + item2 + " " + element.Type + ", " + element.Name;
                    }
                    Item item = new Item
                    {
                        ElementHeader = new ElementHeader(name, "Item", element.Source, id)
                    };
                    item.Category = (string.IsNullOrWhiteSpace(item2) ? ("Additional " + element.Type) : ("Additional " + item2 + " " + element.Type));
                    item.Slot = "proxy";
                    item.ElementSetters.Add(new ElementSetters.Setter("inventory-hidden", "true"));
                    item.HideFromInventory = true;
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("<p><em>You can equip this item to “enable” it. It remains hidden from the inventory on your character sheet.</em></p>");
                    stringBuilder.Append("<div class=\"reference\">");
                    stringBuilder.Append("<div element=\"" + element.Id + "\" />");
                    stringBuilder.Append("</div>");
                    item.Description = stringBuilder.ToString();
                    item.ElementSetters.Add(new ElementSetters.Setter("inventory-hidden", "true"));
                    item.Keywords.AddRange(element.Keywords);
                    item.Keywords.Add(item2);
                    foreach (string support in element.Supports)
                    {
                        item.Keywords.Add(support);
                    }
                    item.SheetDescription.AlternateName = item.Name.Replace("Additional ", "");
                    GrantRule grantRule = new GrantRule(item.ElementHeader);
                    grantRule.Attributes.Type = element.Type;
                    grantRule.Attributes.Name = element.Id;
                    if (!string.IsNullOrWhiteSpace(item2))
                    {
                        grantRule.Setters.Add(new ElementSetters.Setter("spellcasting", item2));
                    }
                    item.Rules.Add(grantRule);
                    item.IncludeInCompendium = false;
                    list.Add(item);
                }
            }
            return list;
        }

        public List<ElementBase> GenerateInternalIgnore(IEnumerable<ElementBase> content)
        {
            List<ElementBase> list = new List<ElementBase>();
            if (content == null)
            {
                return list;
            }
            content.Where((ElementBase x) => x.Type.Equals("Racial Trait") && x.Type.Equals("Class Feature") && x.Type.Equals("Archetype Feature") && x.Type.Equals("Background Feature") && x.Type.Equals("Grants"));
            string text = "_IGNORE";
            foreach (ElementBase item in content.Where((ElementBase x) => x.Type.Equals("Racial Trait")))
            {
                if (!item.Source.Equals("Core") && !item.Source.Equals("Internal"))
                {
                    ElementBase elementBase = new ElementBase
                    {
                        ElementHeader = new ElementHeader("Ignore " + item.Name, "Ignore", "Internal", item.Id + text),
                        IncludeInCompendium = false
                    };
                    if (string.IsNullOrWhiteSpace(item.Requirements))
                    {
                        item.Requirements = "!" + elementBase.Id;
                    }
                    else
                    {
                        item.Requirements = "(" + item.Requirements + ")&&!" + elementBase.Id;
                    }
                    list.Add(elementBase);
                }
            }
            return list;
        }
    }
}
