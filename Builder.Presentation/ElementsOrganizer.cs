using Builder.Core.Logging;
using Builder.Data.Elements;
using Builder.Data.Rules;
using Builder.Data;
using Builder.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation
{
    public class ElementsOrganizer
    {
        public IEnumerable<ElementBase> Elements { get; }

        public ElementsOrganizer(IEnumerable<ElementBase> elements)
        {
            Elements = new List<ElementBase>(elements);
        }

        public bool ContainsType(string type)
        {
            return Elements.Any((ElementBase x) => x.Type == type);
        }

        public IEnumerable<ElementBase> GetTypes(string type, bool includeDuplicates = true)
        {
            if (includeDuplicates)
            {
                return Elements.Where((ElementBase element) => element.Type == type);
            }
            return from element in Elements
                   where element.Type == type
                   select element into x
                   group x by x.Id into x
                   select x.First();
        }

        public IEnumerable<T> GetTypes<T>(string type, bool includeDuplicates = true) where T : ElementBase
        {
            if (includeDuplicates)
            {
                return Elements.Where((ElementBase element) => element.Type == type).Cast<T>();
            }
            return (from element in Elements
                    where element.Type == type
                    select element into x
                    group x by x.Id into x
                    select x.First()).Cast<T>();
        }

        public IEnumerable<Language> GetLanguages(bool includeDuplicates = true)
        {
            return GetTypes<Language>("Language", includeDuplicates);
        }

        public IEnumerable<Proficiency> GetProficiencies(bool includeDuplicates = true)
        {
            return GetTypes<Proficiency>("Proficiency", includeDuplicates);
        }

        public IEnumerable<Proficiency> GetWeaponProficiencies(bool includeDuplicates = true, bool trimName = false)
        {
            return from x in GetTypes<Proficiency>("Proficiency", includeDuplicates)
                   where x.Name.StartsWith("Weapon")
                   select x;
        }

        public IEnumerable<Proficiency> GetArmorProficiencies(bool includeDuplicates = true)
        {
            return from x in GetTypes<Proficiency>("Proficiency", includeDuplicates)
                   where x.Name.StartsWith("Armor")
                   select x;
        }

        public IEnumerable<Proficiency> GetToolProficiencies(bool includeDuplicates = true)
        {
            return from x in GetTypes<Proficiency>("Proficiency", includeDuplicates)
                   where x.Name.StartsWith("Tool")
                   select x;
        }

        public IEnumerable<Race> GetRaces(bool includeDuplicates = true)
        {
            return GetTypes<Race>("Race", includeDuplicates);
        }

        public IEnumerable<Class> GetClasses(bool includeDuplicates = true)
        {
            return GetTypes<Class>("Class", includeDuplicates);
        }

        public IEnumerable<ElementBase> GetBackgrounds(bool includeDuplicates = true)
        {
            return GetTypes<ElementBase>("Background", includeDuplicates);
        }

        public IEnumerable<Spell> GetSpells(bool includeDuplicates = true)
        {
            return GetTypes<Spell>("Spell", includeDuplicates);
        }

        public IEnumerable<ElementBase> GetInternalSupports()
        {
            return GetTypes<ElementBase>("Support");
        }

        public IEnumerable<ElementBase> GetSortedFeatures(List<ElementBase> children)
        {
            List<ElementBase> list = (from x in Elements
                                      where x.Type == "Vision" || x.Type == "Race" || x.Type == "Sub Race" || x.Type == "Race Variant" || x.Type == "Racial Trait" || x.Type == "Language Feature" || x.Type == "Class" || x.Type == "Class Feature" || x.Type == "Archetype" || x.Type == "Archetype Feature" || x.Type == "Feat" || x.Type == "Feat Feature"
                                      where !x.Name.StartsWith("Ability Score Increase")
                                      where !x.Name.StartsWith("Ability Score Improvement")
                                      select x).ToList();
            Logger.Info("====================features post sorting====================");
            List<ElementBase> list2 = new List<ElementBase>();
            foreach (ElementBase item in list)
            {
                if (!list2.Contains(item))
                {
                    list2.Add(item);
                    Logger.Info($"{item}");
                }
                if (!item.ContainsSelectRules)
                {
                    continue;
                }
                foreach (SelectRule rule in item.GetSelectRules())
                {
                    for (int i = 1; i <= rule.Attributes.Number; i++)
                    {
                        if (!SelectionRuleExpanderHandler.Current.HasExpander(rule.UniqueIdentifier, i))
                        {
                            continue;
                        }
                        ElementBase registeredElement = SelectionRuleExpanderHandler.Current.GetRegisteredElement(rule, i) as ElementBase;
                        if (!list.Contains(registeredElement))
                        {
                            continue;
                        }
                        ElementBase elementBase = list.First((ElementBase x) => x.Id == registeredElement.Id);
                        if (list2.Contains(elementBase))
                        {
                            continue;
                        }
                        ElementBase elementBase2 = CharacterManager.Current.GetElements().Single((ElementBase x) => x.Id == rule.ElementHeader.Id);
                        list2.Add(elementBase);
                        if ((!rule.ElementHeader.Name.StartsWith("Ability Score Increase") && rule.ElementHeader.Type != "Race") || (!rule.ElementHeader.Id.StartsWith("ID_CLASS_FEATURE_FEAT_") && rule.ElementHeader.Type != "Class Feature"))
                        {
                            if (elementBase2.SheetDescription.DisplayOnSheet)
                            {
                                children.Add(elementBase);
                                Logger.Info($"\t{elementBase}");
                            }
                            else
                            {
                                Logger.Info($"\t{elementBase}");
                            }
                        }
                        else
                        {
                            Logger.Info($"{elementBase}");
                        }
                    }
                }
            }
            return list2;
        }

        public IEnumerable<ElementBase> GetSortedFeaturesExcludingRacialTraits(List<ElementBase> children)
        {
            List<ElementBase> list = (from x in Elements
                                      where x.Type == "Vision" || x.Type == "Language Feature" || x.Type == "Class" || x.Type == "Class Feature" || x.Type == "Archetype" || x.Type == "Archetype Feature" || x.Type == "Feat" || x.Type == "Feat Feature"
                                      where !x.Name.StartsWith("Ability Score Increase")
                                      where !x.Name.StartsWith("Ability Score Improvement")
                                      select x).ToList();
            Logger.Info("====================features post sorting====================");
            List<ElementBase> list2 = new List<ElementBase>();
            foreach (ElementBase item in list)
            {
                if (!list2.Contains(item))
                {
                    list2.Add(item);
                    Logger.Info($"{item}");
                }
                if (!item.ContainsSelectRules)
                {
                    continue;
                }
                foreach (SelectRule rule in item.GetSelectRules())
                {
                    for (int i = 1; i <= rule.Attributes.Number; i++)
                    {
                        if (!SelectionRuleExpanderHandler.Current.HasExpander(rule.UniqueIdentifier, i))
                        {
                            continue;
                        }
                        ElementBase registeredElement = SelectionRuleExpanderHandler.Current.GetRegisteredElement(rule, i) as ElementBase;
                        if (!list.Contains(registeredElement))
                        {
                            continue;
                        }
                        ElementBase elementBase = list.First((ElementBase x) => x.Id == registeredElement.Id);
                        if (list2.Contains(elementBase))
                        {
                            continue;
                        }
                        ElementBase elementBase2 = CharacterManager.Current.GetElements().Single((ElementBase x) => x.Id == rule.ElementHeader.Id);
                        list2.Add(elementBase);
                        if ((!rule.ElementHeader.Name.StartsWith("Ability Score Increase") && rule.ElementHeader.Type != "Race") || (!rule.ElementHeader.Id.StartsWith("ID_CLASS_FEATURE_FEAT_") && rule.ElementHeader.Type != "Class Feature"))
                        {
                            if (elementBase2.SheetDescription.DisplayOnSheet)
                            {
                                children.Add(elementBase);
                                Logger.Info($"\t{elementBase}");
                            }
                            else
                            {
                                Logger.Info($"\t{elementBase}");
                            }
                        }
                        else
                        {
                            Logger.Info($"{elementBase}");
                        }
                    }
                }
            }
            return list2;
        }

        public IEnumerable<ElementBase> GetSortedRacialTraits(List<ElementBase> children)
        {
            List<ElementBase> list = (from x in Elements
                                      where x.Type == "Race" || x.Type == "Sub Race" || x.Type == "Race Variant" || x.Type == "Racial Trait"
                                      where !x.Name.StartsWith("Ability Score Increase")
                                      where !x.Name.StartsWith("Ability Score Improvement")
                                      select x).ToList();
            Logger.Info("====================features post sorting====================");
            List<ElementBase> list2 = new List<ElementBase>();
            foreach (ElementBase item in list)
            {
                if (!list2.Contains(item))
                {
                    list2.Add(item);
                    Logger.Info($"{item}");
                }
                if (!item.ContainsSelectRules)
                {
                    continue;
                }
                foreach (SelectRule rule in item.GetSelectRules())
                {
                    for (int i = 1; i <= rule.Attributes.Number; i++)
                    {
                        if (!SelectionRuleExpanderHandler.Current.HasExpander(rule.UniqueIdentifier, i))
                        {
                            continue;
                        }
                        ElementBase registeredElement = SelectionRuleExpanderHandler.Current.GetRegisteredElement(rule, i) as ElementBase;
                        if (!list.Contains(registeredElement))
                        {
                            continue;
                        }
                        ElementBase elementBase = list.First((ElementBase x) => x.Id == registeredElement.Id);
                        if (list2.Contains(elementBase))
                        {
                            continue;
                        }
                        ElementBase elementBase2 = CharacterManager.Current.GetElements().Single((ElementBase x) => x.Id == rule.ElementHeader.Id);
                        list2.Add(elementBase);
                        if ((!rule.ElementHeader.Name.StartsWith("Ability Score Increase") && rule.ElementHeader.Type != "Race") || (!rule.ElementHeader.Id.StartsWith("ID_CLASS_FEATURE_FEAT_") && rule.ElementHeader.Type != "Class Feature"))
                        {
                            if (elementBase2.SheetDescription.DisplayOnSheet)
                            {
                                children.Add(elementBase);
                                Logger.Info($"\t{elementBase}");
                            }
                            else
                            {
                                Logger.Info($"\t{elementBase}");
                            }
                        }
                        else
                        {
                            Logger.Info($"{elementBase}");
                        }
                    }
                }
            }
            return list2;
        }
    }
}
