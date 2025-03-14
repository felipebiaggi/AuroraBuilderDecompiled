using System.Collections.Generic;
using System.Linq;
using Builder.Data;
using Builder.Data.Rules;
using Builder.Presentation;
using Builder.Presentation.Services;


namespace Builder.Presentation
{
    public class CharacterContentOrganizer
    {
        public CharacterManager Manager => CharacterManager.Current;

        public IEnumerable<ElementBase> GetFeatures(List<ElementBase> elements)
        {
            List<ElementBase> list = new List<ElementBase>();
            list.AddRange(elements.Where((ElementBase x) => x.Type == "Vision"));
            list.AddRange(elements.Where((ElementBase x) => x.Type == "Race"));
            list.AddRange(elements.Where((ElementBase x) => x.Type == "Race Variant"));
            list.AddRange(elements.Where((ElementBase x) => x.Type == "Sub Race"));
            list.AddRange(elements.Where((ElementBase x) => x.Type == "Racial Trait"));
            list.AddRange(elements.Where((ElementBase x) => x.Type == "Class"));
            list.AddRange(elements.Where((ElementBase x) => x.Type == "Class Feature"));
            list.AddRange(elements.Where((ElementBase x) => x.Type == "Archetype"));
            list.AddRange(elements.Where((ElementBase x) => x.Type == "Archetype Feature"));
            list.AddRange(elements.Where((ElementBase x) => x.Type == "Feat"));
            list.AddRange(elements.Where((ElementBase x) => x.Type == "Feat Feature"));
            return from x in list
                   where !x.Name.StartsWith("Ability Score Increase")
                   where !x.Name.StartsWith("Ability Score Improvement")
                   select x;
        }

        public IEnumerable<ElementContainer> GetContainers(List<ElementBase> elements)
        {
            List<ElementBase> list = GetFeatures(elements).ToList();
            List<ElementBase> list2 = new List<ElementBase>();
            List<ElementBase> list3 = new List<ElementBase>();
            foreach (ElementBase item3 in list)
            {
                if (!list2.Contains(item3))
                {
                    list2.Add(item3);
                }
                if (!item3.ContainsSelectRules)
                {
                    continue;
                }
                foreach (SelectRule rule in item3.GetSelectRules())
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
                        ElementBase item = list.First((ElementBase x) => x.Id == registeredElement.Id);
                        if (!list2.Contains(item))
                        {
                            ElementBase elementBase = CharacterManager.Current.GetElements().Single((ElementBase x) => x.Id == rule.ElementHeader.Id);
                            list2.Add(item);
                            if (((!rule.ElementHeader.Name.StartsWith("Ability Score Increase") && rule.ElementHeader.Type != "Race") || (!rule.ElementHeader.Id.StartsWith("ID_CLASS_FEATURE_FEAT_") && rule.ElementHeader.Type != "Class Feature")) && elementBase.SheetDescription.DisplayOnSheet)
                            {
                                list3.Add(item);
                            }
                        }
                    }
                }
            }
            List<ElementContainer> list4 = new List<ElementContainer>();
            foreach (ElementBase item4 in list2)
            {
                ElementContainer item2 = new ElementContainer(item4)
                {
                    IsNested = list3.Contains(item4)
                };
                list4.Add(item2);
            }
            return list4;
        }

        public void UpdateContainers(IEnumerable<ElementContainer> containers, IEnumerable<ElementContainer> existingContainers)
        {
            foreach (ElementContainer container in containers)
            {
                ElementContainer elementContainer = existingContainers.FirstOrDefault((ElementContainer x) => x.Element.Id == container.Element.Id);
                if (elementContainer != null)
                {
                    container.Name.Content = elementContainer.Name.Content;
                    container.Description.Content = elementContainer.Description.Content;
                    container.IsEnabled = elementContainer.IsEnabled;
                    container.IsNested = elementContainer.IsNested;
                }
            }
        }
    }
}
