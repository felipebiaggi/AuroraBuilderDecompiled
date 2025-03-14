using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Data.Rules;
using Builder.Data.Strings;
using Builder.Presentation.Models.Equipment;
using Builder.Presentation.ViewModels.Shell.Items;
using DynamicExpresso;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Builder.Presentation.Services
{
    public class ExpressionInterpreter
    {
        private readonly IExpressionConverter _expressionConverter;

        private const string BracketsExpression = "(\\[([^\\]])+])";

        private const string IdentifierExpression = "ID_([^+])\\w+";

        private readonly Interpreter _interpreter;

        private CharacterManager _manager;

        private SelectRule _currentRule;

        public ExpressionInterpreter()
        {
            _expressionConverter = new DynamicExpressionConverter();
            _manager = CharacterManager.Current;
            _interpreter = new Interpreter();
            _interpreter.EnableAssignment(AssignmentOperators.None);
            _interpreter.SetVariable("manager", _manager);
            _interpreter.SetVariable("evaluate", this);
        }

        public bool EvaluateElementRequirementsExpression(string expressionString, IEnumerable<string> elementsIDs)
        {
            expressionString = _expressionConverter.SanitizeExpression(expressionString);
            foreach (string item in from Match x in Regex.Matches(expressionString, "(\\[([^\\]])+])")
                                    select x.Value)
            {
                string[] array = item.Substring(1, item.Length - 2).Split(':');
                string text = array[0].ToLowerInvariant();
                string text2 = array[1].ToLowerInvariant();
                string text3 = item.Substring(1, item.Length - 2).Replace(":" + array.Last(), "");
                string text4 = array.Last();
                text = text3.ToLowerInvariant();
                text2 = text4.ToLowerInvariant();
                expressionString = expressionString.Replace(item, "evaluate.Require(\"" + text + "\", \"" + text2 + "\")");
            }
            foreach (string item2 in from Match x in Regex.Matches(expressionString, "ID_([^+])\\w+")
                                     select x.Value)
            {
                expressionString = expressionString.Replace(item2, "evaluate.Contains(ids, \"" + item2 + "\")");
            }
            _interpreter.SetVariable("ids", elementsIDs);
            return _interpreter.Eval<bool>(expressionString, new Parameter[0]);
        }

        public bool EvaluateRuleRequirementsExpression(string expressionString, IEnumerable<string> elementsIDs)
        {
            _interpreter.SetVariable("ids", elementsIDs);
            expressionString = _expressionConverter.SanitizeExpression(expressionString);
            expressionString = _expressionConverter.ConvertRequirementsExpression(expressionString);
            Logger.Debug("evaluating: " + expressionString);
            return _interpreter.Eval<bool>(expressionString, new Parameter[0]);
        }

        public bool EvaluateEquippedExpression(string expressionString)
        {
            Logger.Debug("generating expression from: " + expressionString);
            expressionString = _expressionConverter.SanitizeExpression(expressionString);
            if (!expressionString.Contains("[") && !expressionString.Contains("]"))
            {
                Logger.Warning("fix equipped expression: " + expressionString);
                expressionString = "[" + expressionString + "]";
            }
            foreach (string item in from Match x in Regex.Matches(expressionString, "(\\[([^\\]])+])")
                                    select x.Value)
            {
                string[] array = item.Substring(1, item.Length - 2).Split(':');
                string text = array[0].ToLowerInvariant();
                string text2 = array[1].ToLowerInvariant();
                expressionString = expressionString.Replace(item, "evaluate.Equipped(\"" + text + "\", \"" + text2 + "\")");
            }
            Logger.Debug("evaluating: " + expressionString);
            return _interpreter.Eval<bool>(expressionString, new Parameter[0]);
        }

        public IEnumerable<T> EvaluateSupportsExpression<T>(string expressionString, IEnumerable<T> elements, bool containsElementIDs = false)
        {
            if (string.IsNullOrWhiteSpace(expressionString))
            {
                return elements;
            }
            expressionString = _expressionConverter.SanitizeExpression(expressionString);
            expressionString = ((!containsElementIDs) ? _expressionConverter.ConvertSupportsExpression(expressionString) : _expressionConverter.ConvertSupportsExpression(expressionString, isRange: true));
            Expression<Func<T, bool>> predicate = _interpreter.ParseAsExpression<Func<T, bool>>(expressionString, new string[1] { "element" });
            return elements.AsQueryable().Where(predicate);
        }

        public void InitializeWithSelectionRule(SelectRule rule)
        {
            _currentRule = rule;
        }

        public bool Equipped(string key, string value)
        {
            return RefactoredEquipped(key, value);
        }

        public bool Require(string key, string value)
        {
            if (_manager == null)
            {
                _manager = CharacterManager.Current;
            }
            switch (key)
            {
                case "str":
                case "strength":
                    return _manager.Character.Abilities.Strength.FinalScore >= Convert.ToInt32(value);
                case "dex":
                case "dexterity":
                    return _manager.Character.Abilities.Dexterity.FinalScore >= Convert.ToInt32(value);
                case "con":
                case "constitution":
                    return _manager.Character.Abilities.Constitution.FinalScore >= Convert.ToInt32(value);
                case "int":
                case "intelligence":
                    return _manager.Character.Abilities.Intelligence.FinalScore >= Convert.ToInt32(value);
                case "wis":
                case "wisdom":
                    return _manager.Character.Abilities.Wisdom.FinalScore >= Convert.ToInt32(value);
                case "cha":
                case "charisma":
                    return _manager.Character.Abilities.Charisma.FinalScore >= Convert.ToInt32(value);
                case "character":
                    return _manager.Character.Level >= Convert.ToInt32(value);
                case "level":
                    if (_currentRule != null)
                    {
                        ClassProgressionManager classProgressionManager = _manager.ClassProgressionManagers.FirstOrDefault((ClassProgressionManager x) => x.SelectionRules.Contains(_currentRule));
                        Logger.Info($"comparing requirement [{key}:{value}] against {classProgressionManager}");
                        if (classProgressionManager != null)
                        {
                            return classProgressionManager.ProgressionLevel >= Convert.ToInt32(value);
                        }
                    }
                    return _manager.Character.Level >= Convert.ToInt32(value);
                case "type":
                    return _manager.GetElements().Any((ElementBase x) => x.Type.ToLowerInvariant().Equals(value));
                default:
                    foreach (ClassProgressionManager classProgressionManager2 in _manager.ClassProgressionManagers)
                    {
                        if (classProgressionManager2.ClassElement != null && key.Equals(classProgressionManager2.ClassElement.Name.ToLowerInvariant()))
                        {
                            return classProgressionManager2.ProgressionLevel >= Convert.ToInt32(value);
                        }
                    }
                    if (RequireStatisticsValue(key, value))
                    {
                        return true;
                    }
                    return false;
            }
        }

        public bool Contains(IEnumerable<string> elementIds, string id)
        {
            return elementIds.Contains(id);
        }

        public bool RefactoredEquipped(string key, string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            CharacterInventory inventory = _manager.Character.Inventory;
            if (key.Equals("armor"))
            {
                Item item = inventory.EquippedArmor?.Item;
                List<string> list = GetGrants().ToList();
                bool flag = list.Contains(InternalGrants.CountsAsEquippedLightArmor);
                bool flag2 = list.Contains(InternalGrants.CountsAsEquippedMediumArmor);
                bool flag3 = list.Contains(InternalGrants.CountsAsEquippedHeavyArmor);
                bool flag4 = list.Contains(InternalGrants.CountsAsEquippedArmor) || flag || flag2 || flag3;
                if ((string.IsNullOrWhiteSpace(value) || value.Equals("any")) && (item != null || flag4))
                {
                    return true;
                }
                if (value.Equals("none") && item == null && !flag4)
                {
                    return true;
                }
                if (value.Equals("none") && (item != null || flag4))
                {
                    return false;
                }
                if (item == null)
                {
                    if (value.Equals("light") && flag)
                    {
                        return true;
                    }
                    if (value.Equals("medium") && flag2)
                    {
                        return true;
                    }
                    if (value.Equals("heavy") && flag3)
                    {
                        return true;
                    }
                    return false;
                }
                if (value.Equals("light") && item.ElementSetters.GetSetter("armor").Value.ToLowerInvariant() == "light")
                {
                    return true;
                }
                if (value.Equals("medium") && item.ElementSetters.GetSetter("armor").Value.ToLowerInvariant() == "medium")
                {
                    return true;
                }
                if (value.Equals("heavy") && item.ElementSetters.GetSetter("armor").Value.ToLowerInvariant() == "heavy")
                {
                    return true;
                }
                if (value.Equals(item.Name.ToLowerInvariant()))
                {
                    return true;
                }
                if (value.Equals(item.Id.ToLowerInvariant()))
                {
                    return true;
                }
            }
            else
            {
                if (key.Equals("shield"))
                {
                    Item item2 = inventory.EquippedSecondary?.Item;
                    if (item2 != null && item2.ElementSetters.ContainsSetter("armor") && item2.ElementSetters.GetSetter("armor").Value.ToLowerInvariant().Equals("shield"))
                    {
                        if (string.IsNullOrWhiteSpace(value) || value.Equals("any"))
                        {
                            return true;
                        }
                        if (value.Equals("none"))
                        {
                            return false;
                        }
                        if (value.Equals(item2.Name.ToLowerInvariant()))
                        {
                            return true;
                        }
                        if (value.Equals(item2.Id.ToLowerInvariant()))
                        {
                            return true;
                        }
                    }
                    else if (value.Equals("none"))
                    {
                        return true;
                    }
                    return false;
                }
                if (key.Equals("main") || key.Equals("primary"))
                {
                    Item item3 = inventory.EquippedPrimary?.Item;
                    if (item3 != null && (string.IsNullOrWhiteSpace(value) || value.Equals("any")))
                    {
                        return true;
                    }
                    if (value.Equals("none") && item3 == null)
                    {
                        return true;
                    }
                    if (item3 == null)
                    {
                        return false;
                    }
                    if (value.Equals("weapon") && item3.ElementSetters.GetSetter("category").Value.ToLowerInvariant() == "weapon")
                    {
                        return true;
                    }
                    if (value.Equals("versatile") && inventory.IsEquippedVersatile())
                    {
                        return true;
                    }
                    if (value.Equals(item3.Name.ToLowerInvariant()))
                    {
                        return true;
                    }
                    if (value.Equals(item3.Id.ToLowerInvariant()))
                    {
                        return true;
                    }
                }
                else
                {
                    if (!key.Equals("offhand") && !key.Equals("secondary"))
                    {
                        if (key.Equals("attunement") || key.Equals("attuned"))
                        {
                            return inventory.Items.Any((RefactoredEquipmentItem x) => x.IsAttuned && (x.Item.Id.ToLowerInvariant().Equals(value) || (x.IsAdorned && x.AdornerItem.Id.ToLowerInvariant().Equals(value))));
                        }
                        if (key.Equals("item"))
                        {
                            return inventory.Items.Any((RefactoredEquipmentItem x) => x.IsEquipped && (x.Item.Id.ToLowerInvariant().Equals(value) || (x.IsAdorned && x.AdornerItem.Id.ToLowerInvariant().Equals(value))));
                        }
                        Logger.Warning("Unknown key in Equipped Expression [" + key + ":" + value + "]");
                        return false;
                    }
                    Item item4 = inventory.EquippedSecondary?.Item;
                    if (item4 != null && item4.ElementSetters.ContainsSetter("armor") && item4.ElementSetters.GetSetter("armor").Value.Equals("shield"))
                    {
                        return false;
                    }
                    if (item4 != null && (string.IsNullOrWhiteSpace(value) || value.Equals("any")))
                    {
                        return true;
                    }
                    if (value.Equals("none") && item4 == null)
                    {
                        return true;
                    }
                    if (item4 == null)
                    {
                        return false;
                    }
                    if (value.Equals("weapon") && item4.ElementSetters.GetSetter("category").Value.ToLowerInvariant() == "weapon")
                    {
                        return true;
                    }
                    if (value.Equals(item4.Name.ToLowerInvariant()))
                    {
                        return true;
                    }
                    if (value.Equals(item4.Id.ToLowerInvariant()))
                    {
                        return true;
                    }
                }
            }
            Logger.Info("returning final false on Equipped [" + key + ":" + value + "]");
            return false;
        }

        private IEnumerable<string> GetGrants()
        {
            return from x in _manager.GetElements()
                   where x.Type.Equals("Grants")
                   select x.Id;
        }

        public bool RequireStatisticsValue(string key, string value)
        {
            if (_manager == null)
            {
                _manager = CharacterManager.Current;
            }
            if (_manager.StatisticsCalculator.StatisticValues.ContainsGroup(key))
            {
                Logger.Warning("checking statistics expression key: [" + key + "]:[" + value + "]");
                return _manager.StatisticsCalculator.StatisticValues.GetValue(key) >= Convert.ToInt32(value);
            }
            Logger.Warning("unknown statistics expression key: [" + key + "]:[" + value + "]");
            return false;
        }

        [Obsolete("replace with DynamicExpressionConverter class")]
        private static string SanitizeExpressionString(string expression)
        {
            if (expression.Contains(","))
            {
                expression = expression.Replace(",", "&&");
            }
            if (expression.Contains("&amp;"))
            {
                expression = expression.Replace("&amp;", "&");
            }
            expression = expression.Replace("&", "&&");
            while (expression.Contains("&&&"))
            {
                expression = expression.Replace("&&&", "&&");
            }
            expression = expression.Replace("|", "||");
            while (expression.Contains("|||"))
            {
                expression = expression.Replace("|||", "||");
            }
            if (Debugger.IsAttached)
            {
                expression = expression.Replace("&&", " && ");
                expression = expression.Replace("||", " || ");
            }
            return expression;
        }
    }
}
