using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Data.Rules;
using Builder.Presentation.Services.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Builder.Presentation.Services
{
    public class ProgressionManager
    {
        private readonly ExpressionInterpreter _interpreter;

        public ElementBaseCollection Elements { get; } = new ElementBaseCollection();

        public ObservableCollection<SelectRule> SelectionRules { get; } = new ObservableCollection<SelectRule>();

        public ObservableCollection<SpellcastingInformation> SpellcastingInformations { get; } = new ObservableCollection<SpellcastingInformation>();

        public int ProgressionLevel { get; set; }

        public event EventHandler<SelectRule> SelectionRuleCreated;

        public event EventHandler<SelectRule> SelectionRuleRemoved;

        public event EventHandler<SpellcastingInformation> SpellcastingInformationCreated;

        public event EventHandler<SpellcastingInformation> SpellcastingInformationRemoved;

        public ProgressionManager()
        {
            _interpreter = new ExpressionInterpreter();
        }

        public ElementBaseCollection GetElements()
        {
            ElementBaseCollection elementBaseCollection = new ElementBaseCollection();
            foreach (ElementBase element in Elements)
            {
                elementBaseCollection.AddRange(GetChildElements(element));
            }
            return elementBaseCollection;
        }

        private static IEnumerable<ElementBase> GetChildElements(ElementBase element)
        {
            ElementBaseCollection elementBaseCollection = new ElementBaseCollection();
            elementBaseCollection.Add(element);
            foreach (ElementBase ruleElement in element.RuleElements)
            {
                elementBaseCollection.Add(ruleElement);
                foreach (ElementBase ruleElement2 in ruleElement.RuleElements)
                {
                    elementBaseCollection.AddRange(GetChildElements(ruleElement2));
                }
            }
            return elementBaseCollection;
        }

        public IEnumerable<StatisticRule> GetStatisticRules(bool applyLevelRequirement = true)
        {
            List<StatisticRule> list = (from x in (from e in GetElements()
                                                   where e.ContainsStatisticRules
                                                   select e).SelectMany((ElementBase e) => e.GetStatisticRules())
                                        where !x.Attributes.Inline
                                        select x.Copy()).ToList();
            if (applyLevelRequirement)
            {
                list = list.Where((StatisticRule x) => x.Attributes.Level <= ProgressionLevel).ToList();
            }
            return list;
        }

        public IEnumerable<StatisticRule> GetStatisticRulesAtLevel(int level)
        {
            List<StatisticRule> source = (from x in (from e in GetElements()
                                                     where e.ContainsStatisticRules
                                                     select e).SelectMany((ElementBase e) => e.GetStatisticRules())
                                          where !x.Attributes.Inline
                                          select x.Copy()).ToList();
            _ = level;
            _ = ProgressionLevel;
            return source.Where((StatisticRule x) => x.Attributes.Level <= level).ToList();
        }

        public IEnumerable<StatisticRule> GetInlineStatisticRules(bool applyLevelRequirement = true)
        {
            List<StatisticRule> list = (from x in (from e in GetElements()
                                                   where e.ContainsStatisticRules
                                                   select e).SelectMany((ElementBase e) => e.GetStatisticRules())
                                        where x.Attributes.Inline
                                        select x.Copy()).ToList();
            if (applyLevelRequirement)
            {
                list = list.Where((StatisticRule x) => x.Attributes.Level <= ProgressionLevel).ToList();
            }
            return list;
        }

        public void Process(ElementBase element)
        {
            ProcessElement(element, ProgressionLevel);
        }

        public void Clean(ElementBase element)
        {
            CleanElement(element);
        }

        public void ProcessExistingElements()
        {
            foreach (ElementBase element in Elements)
            {
                ProcessElement(element, ProgressionLevel);
            }
        }

        private void ProcessElement(ElementBase element, int currentLevel)
        {
            foreach (ElementBase ruleElement in element.RuleElements)
            {
                ProcessElement(ruleElement, currentLevel);
            }
            ProcessSelectionRules(element, currentLevel);
            ProcessGrantRules(element, currentLevel);
            if (element.HasSpellcastingInformation && !SpellcastingInformations.Any((SpellcastingInformation x) => x.UniqueIdentifier.Equals(element.SpellcastingInformation.UniqueIdentifier)))
            {
                SpellcastingInformations.Add(element.SpellcastingInformation);
                OnSpellcastingSectionCreated(element.SpellcastingInformation);
            }
            foreach (ElementBase ruleElement2 in element.RuleElements)
            {
                ProcessElement(ruleElement2, currentLevel);
            }
        }

        private bool ProcessSelectionRules(ElementBase element, int currentLevel)
        {
            bool result = false;
            foreach (SelectRule selectRule in element.GetSelectRules())
            {
                if (!selectRule.Attributes.MeetsLevelRequirement(currentLevel))
                {
                    if (SelectionRules.Contains(selectRule))
                    {
                        SelectionRules.Remove(selectRule);
                        OnSelectionRuleRemoved(selectRule);
                    }
                    continue;
                }
                if (!string.IsNullOrWhiteSpace(selectRule.Attributes.Requirements))
                {
                    ElementBaseCollection elements = CharacterManager.Current.GetElements();
                    if (!_interpreter.EvaluateRuleRequirementsExpression(selectRule.Attributes.Requirements, elements.Select((ElementBase x) => x.Id)))
                    {
                        if (SelectionRules.Contains(selectRule))
                        {
                            SelectionRules.Remove(selectRule);
                            OnSelectionRuleRemoved(selectRule);
                        }
                        continue;
                    }
                }
                if (!SelectionRules.Contains(selectRule))
                {
                    result = true;
                    SelectionRules.Add(selectRule);
                    OnSelectionRuleCreated(selectRule);
                }
            }
            return result;
        }

        private bool ProcessGrantRules(ElementBase element, int currentLevel)
        {
            bool result = false;
            element.Name.Contains("Black Dragon Mask");
            foreach (ElementBase item in element.RuleElements.ToList())
            {
                if (!item.Aquisition.WasGranted)
                {
                    continue;
                }
                GrantRule grantRule = item.Aquisition.GrantRule;
                if (!grantRule.Attributes.MeetsLevelRequirement(currentLevel))
                {
                    CleanSelectionRules(item);
                    CleanGrantRules(item);
                    Logger.Info("\tungranting: {0} after losing level requirements", item);
                    element.RuleElements.Remove(item);
                    continue;
                }
                if (grantRule.HasRequirements())
                {
                    ElementBaseCollection elements = CharacterManager.Current.GetElements();
                    bool flag = _interpreter.EvaluateRuleRequirementsExpression(grantRule.Attributes.Requirements, elements.Select((ElementBase x) => x.Id));
                    elements = elements.WithoutRuleParent(grantRule);
                    if (!_interpreter.EvaluateRuleRequirementsExpression(grantRule.Attributes.Requirements, elements.Select((ElementBase x) => x.Id)))
                    {
                        CleanSelectionRules(item);
                        CleanGrantRules(item);
                        Logger.Info("\tungranting: {0} after losing requirements", item);
                        element.RuleElements.Remove(item);
                    }
                }
                if (item.HasRequirements)
                {
                    ElementBaseCollection elements2 = CharacterManager.Current.GetElements();
                    if (!_interpreter.EvaluateElementRequirementsExpression(item.Requirements, elements2.Select((ElementBase x) => x.Id)))
                    {
                        Logger.Warning("\tungranting: {0} after losing element requirements", item);
                        CleanElement(item);
                        element.RuleElements.Remove(item);
                    }
                }
            }
            foreach (GrantRule rule in element.GetGrantRules())
            {
                if (!rule.Attributes.MeetsLevelRequirement(currentLevel))
                {
                    continue;
                }
                if (rule.HasRequirements())
                {
                    ElementBaseCollection elements3 = CharacterManager.Current.GetElements();
                    bool flag2 = _interpreter.EvaluateRuleRequirementsExpression(rule.Attributes.Requirements, elements3.Select((ElementBase x) => x.Id));
                    elements3 = elements3.WithoutRuleParent(rule);
                    bool num = flag2;
                    flag2 = _interpreter.EvaluateRuleRequirementsExpression(rule.Attributes.Requirements, elements3.Select((ElementBase x) => x.Id));
                    if (num != flag2)
                    {
                        Logger.Warning($"without parent rule the before/after requirements don't match with {rule} on {rule.ElementHeader}");
                    }
                    if (!flag2)
                    {
                        continue;
                    }
                }
                ElementBase elementBase = DataManager.Current.ElementsCollection.FirstOrDefault((ElementBase x) => x.Id.Equals(rule.Attributes.Name));
                if (elementBase == null)
                {
                    string message = $"Unable to find {rule.Attributes.Name} of type {rule.Attributes.Type} that was set as grant in {element}.";
                    Logger.Warning(message);
                    MessageDialogService.Show(message);
                    continue;
                }
                if (elementBase.HasRequirements)
                {
                    List<string> elementsIDs = (from x in CharacterManager.Current.GetElements()
                                                select x.Id).ToList();
                    if (!_interpreter.EvaluateElementRequirementsExpression(elementBase.Requirements, elementsIDs))
                    {
                        if (!elementBase.Id.Equals("ID_RACIAL_TRAIT_DWARVEN_TOOL_PROFICIENCY"))
                        {
                            Logger.Warning("\tnot granting: {0} due to not meeting element requirements", elementBase);
                        }
                        continue;
                    }
                }
                elementBase.RuleElements.Any();
                if (element.RuleElements.ContainsRuleElement(elementBase, rule))
                {
                    Logger.Debug("\tgrant exists accoding to new contains rule element check: {0}", elementBase);
                    continue;
                }
                element.RuleElements.Add(elementBase);
                elementBase.Aquisition.GrantedBy(rule);
                result = true;
                Logger.Debug("\tgranted: {0}", elementBase);
                ProcessElement(elementBase, currentLevel);
            }
            return result;
        }

        private void CleanElement(ElementBase element)
        {
            CleanSelectionRules(element);
            CleanSpellcastingInformation(element);
            CleanGrantRules(element);
            element.Aquisition.Clear();
        }

        private void CleanSelectionRules(ElementBase element)
        {
            if (element.ContainsSelectRules)
            {
                if (element.Type.Equals("Item") || element.Type.Equals("Weapon") || element.Type.Equals("Armor") || element.Type.Equals("Magic Item"))
                {
                    foreach (SelectRule rule2 in element.GetSelectRules())
                    {
                        SelectRule item = SelectionRules.FirstOrDefault((SelectRule x) => x.UniqueIdentifier == rule2.UniqueIdentifier);
                        if (!SelectionRules.Remove(item))
                        {
                            CharacterManager.Current.SelectionRules.Remove(rule2);
                        }
                        OnSelectionRuleRemoved(rule2);
                    }
                }
                else
                {
                    foreach (SelectRule rule3 in SelectionRules.Where((SelectRule rule) => rule.ElementHeader.Id == element.Id).ToList())
                    {
                        SelectRule selectRule = SelectionRules.FirstOrDefault((SelectRule x) => x.UniqueIdentifier == rule3.UniqueIdentifier);
                        if (rule3 != selectRule && Debugger.IsAttached)
                        {
                            Debugger.Break();
                        }
                        if (!SelectionRules.Remove(rule3))
                        {
                            if (Debugger.IsAttached)
                            {
                                Debugger.Break();
                            }
                            CharacterManager.Current.SelectionRules.Remove(rule3);
                        }
                        OnSelectionRuleRemoved(rule3);
                    }
                }
            }
            if (element.SelectionRuleListItems.Any())
            {
                element.SelectionRuleListItems.Clear();
            }
            foreach (ElementBase ruleElement in element.RuleElements)
            {
                CleanSelectionRules(ruleElement);
            }
        }

        private void CleanGrantRules(ElementBase element)
        {
            foreach (ElementBase ruleElement in element.RuleElements)
            {
                if (ruleElement.HasSpellcastingInformation)
                {
                    OnSpellcastingSectionRemoved(ruleElement.SpellcastingInformation);
                }
                foreach (ElementBase ruleElement2 in ruleElement.RuleElements)
                {
                    CleanGrantRules(ruleElement2);
                    ruleElement2.Aquisition.Clear();
                    Logger.Info("\tungranting: {0}", ruleElement2);
                }
                ruleElement.RuleElements.Clear();
                Logger.Info("\tungranting: {0}", ruleElement);
            }
            element.RuleElements.Clear();
        }

        private void CleanSpellcastingInformation(ElementBase element)
        {
            if (element.HasSpellcastingInformation && SpellcastingInformations.Any((SpellcastingInformation x) => x.UniqueIdentifier.Equals(element.SpellcastingInformation.UniqueIdentifier)) && SpellcastingInformations.Remove(element.SpellcastingInformation))
            {
                OnSpellcastingSectionRemoved(element.SpellcastingInformation);
            }
            foreach (ElementBase ruleElement in element.RuleElements)
            {
                CleanSpellcastingInformation(ruleElement);
            }
        }

        protected virtual void OnSelectionRuleCreated(SelectRule e)
        {
            this.SelectionRuleCreated?.Invoke(this, e);
        }

        protected virtual void OnSelectionRuleRemoved(SelectRule e)
        {
            this.SelectionRuleRemoved?.Invoke(this, e);
        }

        protected virtual void OnSpellcastingSectionCreated(SpellcastingInformation e)
        {
            this.SpellcastingInformationCreated?.Invoke(this, e);
        }

        protected virtual void OnSpellcastingSectionRemoved(SpellcastingInformation e)
        {
            this.SpellcastingInformationRemoved?.Invoke(this, e);
        }
    }
}
