using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Data.Rules;
using Builder.Data.Rules.Parsers;
using Builder.Data.Strings;
using Builder.Presentation.Models;
using Builder.Presentation.Models.Collections;
using Builder.Presentation.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Builder.Presentation.Services.Calculator
{
    public class StatisticsHandler2
    {
        [Obsolete]
        public class StatisticNames
        {
            public string StrengthScore => "Strength:Score";

            public string DexterityScore => "Dexterity:Score";

            public string ConstitutionScore => "Constitution:Score";

            public string IntelligenceScore => "Intelligence:Score";

            public string WisdomScore => "Wisdom:Score";

            public string CharismaScore => "Charisma:Score";

            public string Strength => "Strength";

            public string Dexterity => "Dexterity";

            public string Constitution => "Constitution";

            public string Intelligence => "Intelligence";

            public string Wisdom => "Wisdom";

            public string Charisma => "Charisma";

            public string StrengthModifier => Strength + " Modifier";

            public string DexterityModifier => Dexterity + " Modifier";

            public string ConstitutionModifier => Constitution + " Modifier";

            public string IntelligenceModifier => Intelligence + " Modifier";

            public string WisdomModifier => Wisdom + " Modifier";

            public string CharismaModifier => Charisma + " Modifier";

            public string StrengthMaximum => Strength + " Maximum";

            public string DexterityMaximum => Dexterity + " Maximum";

            public string ConstitutionMaximum => Constitution + " Maximum";

            public string IntelligenceMaximum => Intelligence + " Maximum";

            public string WisdomMaximum => Wisdom + " Maximum";

            public string CharismaMaximum => Charisma + " Maximum";

            public string StrengthSavingThrowProficiency => Strength + " Saving Throw Proficiency";

            public string DexteritySavingThrowProficiency => Dexterity + " Saving Throw Proficiency";

            public string ConstitutionSavingThrowProficiency => Constitution + " Saving Throw Proficiency";

            public string IntelligenceSavingThrowProficiency => Intelligence + " Saving Throw Proficiency";

            public string WisdomSavingThrowProficiency => Wisdom + " Saving Throw Proficiency";

            public string CharismaSavingThrowProficiency => Charisma + " Saving Throw Proficiency";

            public string StrengthSavingThrowMisc => Strength + " Saving Throw Misc";

            public string DexteritySavingThrowMisc => Dexterity + " Saving Throw Misc";

            public string ConstitutionSavingThrowMisc => Constitution + " Saving Throw Misc";

            public string IntelligenceSavingThrowMisc => Intelligence + " Saving Throw Misc";

            public string WisdomSavingThrowMisc => Wisdom + " Saving Throw Misc";

            public string CharismaSavingThrowMisc => Charisma + " Saving Throw Misc";

            public string Acrobatics => "Acrobatics";

            public string AnimalHandling => "Animal Handling";

            public string Arcana => "Arcana";

            public string Athletics => "Athletics";

            public string Deception => "Deception";

            public string History => "History";

            public string Insight => "Insight";

            public string Intimidation => "Intimidation";

            public string Investigation => "Investigation";

            public string Medicine => "Medicine";

            public string Nature => "Nature";

            public string Perception => "Perception";

            public string Performance => "Performance";

            public string Persuasion => "Persuasion";

            public string Religion => "Religion";

            public string SleightOfHand => "Sleight of Hand";

            public string Stealth => "Stealth";

            public string Survival => "Survival";

            public string AcrobaticsProficiency => Acrobatics + " Proficiency";

            public string AnimalHandlingProficiency => AnimalHandling + " Proficiency";

            public string ArcanaProficiency => Arcana + " Proficiency";

            public string AthleticsProficiency => Athletics + " Proficiency";

            public string DeceptionProficiency => Deception + " Proficiency";

            public string HistoryProficiency => History + " Proficiency";

            public string InsightProficiency => Insight + " Proficiency";

            public string IntimidationProficiency => Intimidation + " Proficiency";

            public string InvestigationProficiency => Investigation + " Proficiency";

            public string MedicineProficiency => Medicine + " Proficiency";

            public string NatureProficiency => Nature + " Proficiency";

            public string PerceptionProficiency => Perception + " Proficiency";

            public string PerformanceProficiency => Performance + " Proficiency";

            public string PersuasionProficiency => Persuasion + " Proficiency";

            public string ReligionProficiency => Religion + " Proficiency";

            public string SleightOfHandProficiency => SleightOfHand + " Proficiency";

            public string StealthProficiency => Stealth + " Proficiency";

            public string SurvivalProficiency => Survival + " Proficiency";

            public string AcrobaticsMisc => Acrobatics + " Misc";

            public string AnimalHandlingMisc => AnimalHandling + " Misc";

            public string ArcanaMisc => Arcana + " Misc";

            public string AthleticsMisc => Athletics + " Misc";

            public string DeceptionMisc => Deception + " Misc";

            public string HistoryMisc => History + " Misc";

            public string InsightMisc => Insight + " Misc";

            public string IntimidationMisc => Intimidation + " Misc";

            public string InvestigationMisc => Investigation + " Misc";

            public string MedicineMisc => Medicine + " Misc";

            public string NatureMisc => Nature + " Misc";

            public string PerceptionMisc => Perception + " Misc";

            public string PerformanceMisc => Performance + " Misc";

            public string PersuasionMisc => Persuasion + " Misc";

            public string ReligionMisc => Religion + " Misc";

            public string SleightOfHandMisc => SleightOfHand + " Misc";

            public string StealthMisc => Stealth + " Misc";

            public string SurvivalMisc => Survival + " Misc";

            public string AcrobaticsPassive => Acrobatics + " Passive";

            public string AnimalHandlingPassive => AnimalHandling + " Passive";

            public string ArcanaPassive => Arcana + " Passive";

            public string AthleticsPassive => Athletics + " Passive";

            public string DeceptionPassive => Deception + " Passive";

            public string HistoryPassive => History + " Passive";

            public string InsightPassive => Insight + " Passive";

            public string IntimidationPassive => Intimidation + " Passive";

            public string InvestigationPassive => Investigation + " Passive";

            public string MedicinePassive => Medicine + " Passive";

            public string NaturePassive => Nature + " Passive";

            public string PerceptionPassive => Perception + " Passive";

            public string PerformancePassive => Performance + " Passive";

            public string PersuasionPassive => Persuasion + " Passive";

            public string ReligionPassive => Religion + " Passive";

            public string SleightOfHandPassive => SleightOfHand + " Passive";

            public string StealthPassive => Stealth + " Passive";

            public string SurvivalPassive => Survival + " Passive";

            public string HP => "HP";

            public string StartingHP => "Starting HP";

            public string Level => "Level";

            public string Proficiency => "Proficiency";

            public string ArmorClass => "AC";

            public string Speed => "Speed";

            public string Initiative => "Initiative";

            public string ProficiencyHalf => "Proficiency Half";
        }

        private ExpressionInterpreter _interpreter;

        private readonly CharacterManager _manager;

        public AuroraStatisticStrings Names { get; }

        public Dictionary<string, string> InlineValues { get; }

        public StatisticValuesGroupCollection StatisticValues { get; private set; }

        public StatisticsHandler2(CharacterManager manager)
        {
            _manager = manager;
            Names = new AuroraStatisticStrings();
            InlineValues = new Dictionary<string, string>();
        }

        public StatisticValuesGroupCollection CalculateValuesAtLevel(int level, ElementBaseCollection elements, StatisticValuesGroupCollection seed = null)
        {
            if (level <= 0)
            {
                return CalculateValues(elements, seed);
            }
            return CalculateValues(elements, seed, level, setProperties: false);
        }

        public StatisticValuesGroupCollection CalculateValues(ElementBaseCollection elements, StatisticValuesGroupCollection seed = null)
        {
            return CalculateValues(elements, seed, -1, setProperties: true);
        }

        public string ReplaceInline(string input)
        {
            string text = input;
            foreach (Match item in Regex.Matches(text, "\\$\\((.*?)\\)"))
            {
                string value = item.Value;
                string input2 = item.Value.Substring(2, item.Value.Length - 3);
                input2 = StatisticRuleParser.MapLegacyName(input2);
                string newValue = "==UNKNOWN_VALUE==";
                if (StatisticValues.ContainsGroup(input2))
                {
                    newValue = StatisticValues.GetValue(input2).ToString();
                }
                else if (InlineValues.ContainsKey(input2))
                {
                    newValue = InlineValues[input2];
                }
                else
                {
                    Logger.Warning("UNKNOWN REPLACE STRING: " + value);
                }
                text = text.Replace(value, newValue);
            }
            if (text.Contains("{{"))
            {
                text = ReplaceDoubleBrackets(text);
            }
            return text;
        }

        public StatisticValuesGroupCollection CreateSeed(int level, CharacterManager characterManager)
        {
            AuroraStatisticStrings auroraStatisticStrings = new AuroraStatisticStrings();
            StatisticValuesGroupCollection statisticValuesGroupCollection = new StatisticValuesGroupCollection();
            StatisticValuesGroup statisticValuesGroup = new StatisticValuesGroup(auroraStatisticStrings.Level);
            statisticValuesGroup.AddValue("Character", level);
            statisticValuesGroupCollection.AddGroup(statisticValuesGroup);
            statisticValuesGroupCollection.AddGroup(CreateHalf(statisticValuesGroup));
            statisticValuesGroupCollection.AddGroup(CreateHalfUp(statisticValuesGroup));
            foreach (ClassProgressionManager item in characterManager.ClassProgressionManagers.Where((ClassProgressionManager x) => x.ClassElement != null))
            {
                StatisticValuesGroup statisticValuesGroup2 = new StatisticValuesGroup(item.GetClassLevelStatisticsName());
                statisticValuesGroup2.AddValue(item.ClassElement.Name, item.ProgressionLevel);
                statisticValuesGroupCollection.AddGroup(statisticValuesGroup2);
                statisticValuesGroupCollection.AddGroup(CreateHalf(statisticValuesGroup2));
                statisticValuesGroupCollection.AddGroup(CreateHalfUp(statisticValuesGroup2));
                StatisticValuesGroup statisticValuesGroup3 = new StatisticValuesGroup("hp:" + item.ClassElement.Name.ToLowerInvariant());
                statisticValuesGroup3.AddValue(item.ClassElement.Name, item.GetHitPoints());
                statisticValuesGroupCollection.AddGroup(statisticValuesGroup3);
                statisticValuesGroupCollection.GetGroup(auroraStatisticStrings.HitPoints).AddValue(item.ClassElement.Name, statisticValuesGroup3.Sum());
            }
            if (characterManager.Status.HasCompanion)
            {
                Companion companion = characterManager.Character.Companion;
                StatisticValuesGroup statisticValuesGroup4 = new StatisticValuesGroup("companion:proficiency");
                statisticValuesGroup4.AddValue(companion.Element.Name, companion.Element.Proficiency);
                statisticValuesGroupCollection.AddGroup(statisticValuesGroup4);
            }
            foreach (SpellcastingInformation spellcastingInformation in characterManager.GetSpellcastingInformations())
            {
                StatisticValuesGroup group = new StatisticValuesGroup(spellcastingInformation.GetSpellcasterSpellAttackStatisticName());
                StatisticValuesGroup group2 = new StatisticValuesGroup(spellcastingInformation.GetSpellcasterSpellAttackStatisticName());
                statisticValuesGroupCollection.AddGroup(group);
                statisticValuesGroupCollection.AddGroup(group2);
            }
            statisticValuesGroupCollection.GetGroup("attunement:current").AddValue("Internal", characterManager.Character.Inventory.AttunedItemCount);
            return statisticValuesGroupCollection;
        }

        public string ReplaceDoubleBrackets(string input)
        {
            string text = input;
            foreach (Match item in Regex.Matches(text, "{{(.*?)}}"))
            {
                string value = item.Value;
                string input2 = item.Value.Substring(2, item.Value.Length - 4).Trim();
                input2 = StatisticRuleParser.MapLegacyName(input2);
                string newValue = "==UNKNOWN_VALUE==";
                if (StatisticValues.ContainsGroup(input2))
                {
                    newValue = StatisticValues.GetValue(input2).ToString();
                }
                else if (InlineValues.ContainsKey(input2))
                {
                    newValue = InlineValues[input2];
                }
                else
                {
                    Logger.Warning("UNKNOWN REPLACE STRING: " + value);
                }
                text = text.Replace(value, newValue);
            }
            return text;
        }

        private StatisticValuesGroupCollection CalculateValues(ElementBaseCollection elements, StatisticValuesGroupCollection seed = null, int atLevel = -1, bool setProperties = true)
        {
            if (_interpreter == null)
            {
                _interpreter = new ExpressionInterpreter();
            }
            List<StatisticRule> list = CharacterManager.Current.GetStatisticRules2().ToList();
            if (atLevel > 0)
            {
                list = CharacterManager.Current.GetStatisticRulesAtLevel(atLevel).ToList();
            }
            foreach (StatisticRule item in list.ToList())
            {
                if (item.Attributes.HasEquipmentConditions && !_interpreter.EvaluateEquippedExpression(item.Attributes.Equipped))
                {
                    list.Remove(item);
                }
            }
            foreach (StatisticRule item2 in list.ToList())
            {
                if (item2.Attributes.HasRequirementsConditions && !_interpreter.EvaluateElementRequirementsExpression(item2.Attributes.Requirements, elements.Select((ElementBase e) => e.Id).ToList()))
                {
                    list.Remove(item2);
                }
            }
            Queue<StatisticRule> queue = new Queue<StatisticRule>();
            Queue<StatisticRule> queue2 = new Queue<StatisticRule>();
            foreach (StatisticRule item3 in list)
            {
                if (item3.Attributes.IsNumberValue() && !item3.Attributes.HasBonus && !item3.Attributes.HasCap)
                {
                    queue.Enqueue(item3);
                }
                else
                {
                    queue2.Enqueue(item3);
                }
            }
            AuroraStatisticStrings strings = new AuroraStatisticStrings();
            StatisticValuesGroupCollection statisticValuesGroupCollection = seed ?? new StatisticValuesGroupCollection();
            foreach (KeyValuePair<string, int> value in new StatisticsCalculatedResult(initializeDefaults: true).GetValues())
            {
                StatisticValuesGroup group = statisticValuesGroupCollection.GetGroup(value.Key);
                if (value.Value != 0)
                {
                    group.AddValue("Initial", value.Value);
                }
            }
            while (queue.Any())
            {
                StatisticRule statisticRule = queue.Dequeue();
                statisticValuesGroupCollection.GetGroup(statisticRule.Attributes.Name).AddValue(statisticRule.Attributes.HasAlt ? statisticRule.Attributes.Alt : statisticRule.ElementHeader.Name, statisticRule.Attributes.GetValue());
            }
            if (Debugger.IsAttached)
            {
                if (queue2.Any((StatisticRule x) => x.Attributes.Name.Equals(strings.Strength, StringComparison.OrdinalIgnoreCase)))
                {
                    Debugger.Break();
                }
                if (queue2.Any((StatisticRule x) => x.Attributes.Name.Equals(strings.Dexterity, StringComparison.OrdinalIgnoreCase)))
                {
                    Debugger.Break();
                }
                if (queue2.Any((StatisticRule x) => x.Attributes.Name.Equals(strings.Constitution, StringComparison.OrdinalIgnoreCase)))
                {
                    Debugger.Break();
                }
                if (queue2.Any((StatisticRule x) => x.Attributes.Name.Equals(strings.Intelligence, StringComparison.OrdinalIgnoreCase)))
                {
                    Debugger.Break();
                }
                if (queue2.Any((StatisticRule x) => x.Attributes.Name.Equals(strings.Wisdom, StringComparison.OrdinalIgnoreCase)))
                {
                    Debugger.Break();
                }
                if (queue2.Any((StatisticRule x) => x.Attributes.Name.Equals(strings.Charisma, StringComparison.OrdinalIgnoreCase)))
                {
                    Debugger.Break();
                }
                if (queue2.Any((StatisticRule x) => x.Attributes.Name.Equals(strings.StrengthMaximum, StringComparison.OrdinalIgnoreCase)))
                {
                    Debugger.Break();
                }
                if (queue2.Any((StatisticRule x) => x.Attributes.Name.Equals(strings.DexterityMaximum, StringComparison.OrdinalIgnoreCase)))
                {
                    Debugger.Break();
                }
                if (queue2.Any((StatisticRule x) => x.Attributes.Name.Equals(strings.ConstitutionMaximum, StringComparison.OrdinalIgnoreCase)))
                {
                    Debugger.Break();
                }
                if (queue2.Any((StatisticRule x) => x.Attributes.Name.Equals(strings.IntelligenceMaximum, StringComparison.OrdinalIgnoreCase)))
                {
                    Debugger.Break();
                }
                if (queue2.Any((StatisticRule x) => x.Attributes.Name.Equals(strings.WisdomMaximum, StringComparison.OrdinalIgnoreCase)))
                {
                    Debugger.Break();
                }
                if (queue2.Any((StatisticRule x) => x.Attributes.Name.Equals(strings.CharismaMaximum, StringComparison.OrdinalIgnoreCase)))
                {
                    Debugger.Break();
                }
            }
            AbilitiesCollection abilities = _manager.Character.Abilities;
            abilities.Strength.AdditionalScore = statisticValuesGroupCollection.GetValue(strings.Strength);
            abilities.Dexterity.AdditionalScore = statisticValuesGroupCollection.GetValue(strings.Dexterity);
            abilities.Constitution.AdditionalScore = statisticValuesGroupCollection.GetValue(strings.Constitution);
            abilities.Intelligence.AdditionalScore = statisticValuesGroupCollection.GetValue(strings.Intelligence);
            abilities.Wisdom.AdditionalScore = statisticValuesGroupCollection.GetValue(strings.Wisdom);
            abilities.Charisma.AdditionalScore = statisticValuesGroupCollection.GetValue(strings.Charisma);
            AbilitiesCollection abilities2 = _manager.Character.Companion.Abilities;
            abilities2.Strength.AdditionalScore = statisticValuesGroupCollection.GetValue("companion:strength");
            abilities2.Dexterity.AdditionalScore = statisticValuesGroupCollection.GetValue("companion:dexterity");
            abilities2.Constitution.AdditionalScore = statisticValuesGroupCollection.GetValue("companion:constitution");
            abilities2.Intelligence.AdditionalScore = statisticValuesGroupCollection.GetValue("companion:intelligence");
            abilities2.Wisdom.AdditionalScore = statisticValuesGroupCollection.GetValue("companion:wisdom");
            abilities2.Charisma.AdditionalScore = statisticValuesGroupCollection.GetValue("companion:charisma");
            if (queue2.Any((StatisticRule x) => x.Attributes.Name.Contains(":score:set")))
            {
                List<StatisticRule> statisticRules = queue2.Where((StatisticRule x) => x.Attributes.Name.Contains(":score:set")).ToList();
                StatisticValuesGroupCollection statisticValuesGroupCollection2 = new StatisticValuesGroupCollection();
                CalculateGroups(statisticRules, statisticValuesGroupCollection2, forceUnhandledQueue: true);
                abilities.Strength.OverrideScore = statisticValuesGroupCollection2.GetValue(strings.StrengthSet);
                abilities.Dexterity.OverrideScore = statisticValuesGroupCollection2.GetValue(strings.DexteritySet);
                abilities.Constitution.OverrideScore = statisticValuesGroupCollection2.GetValue(strings.ConstitutionSet);
                abilities.Intelligence.OverrideScore = statisticValuesGroupCollection2.GetValue(strings.IntelligenceSet);
                abilities.Wisdom.OverrideScore = statisticValuesGroupCollection2.GetValue(strings.WisdomSet);
                abilities.Charisma.OverrideScore = statisticValuesGroupCollection2.GetValue(strings.CharismaSet);
                abilities2.Strength.OverrideScore = statisticValuesGroupCollection2.GetValue("companion:" + strings.StrengthSet);
                abilities2.Dexterity.OverrideScore = statisticValuesGroupCollection2.GetValue("companion:" + strings.DexteritySet);
                abilities2.Constitution.OverrideScore = statisticValuesGroupCollection2.GetValue("companion:" + strings.ConstitutionSet);
                abilities2.Intelligence.OverrideScore = statisticValuesGroupCollection2.GetValue("companion:" + strings.IntelligenceSet);
                abilities2.Wisdom.OverrideScore = statisticValuesGroupCollection2.GetValue("companion:" + strings.WisdomSet);
                abilities2.Charisma.OverrideScore = statisticValuesGroupCollection2.GetValue("companion:" + strings.CharismaSet);
            }
            else
            {
                abilities.Strength.OverrideScore = statisticValuesGroupCollection.GetValue(strings.StrengthSet);
                abilities.Dexterity.OverrideScore = statisticValuesGroupCollection.GetValue(strings.DexteritySet);
                abilities.Constitution.OverrideScore = statisticValuesGroupCollection.GetValue(strings.ConstitutionSet);
                abilities.Intelligence.OverrideScore = statisticValuesGroupCollection.GetValue(strings.IntelligenceSet);
                abilities.Wisdom.OverrideScore = statisticValuesGroupCollection.GetValue(strings.WisdomSet);
                abilities.Charisma.OverrideScore = statisticValuesGroupCollection.GetValue(strings.CharismaSet);
                abilities2.Strength.OverrideScore = statisticValuesGroupCollection.GetValue("companion:" + strings.StrengthSet);
                abilities2.Dexterity.OverrideScore = statisticValuesGroupCollection.GetValue("companion:" + strings.DexteritySet);
                abilities2.Constitution.OverrideScore = statisticValuesGroupCollection.GetValue("companion:" + strings.ConstitutionSet);
                abilities2.Intelligence.OverrideScore = statisticValuesGroupCollection.GetValue("companion:" + strings.IntelligenceSet);
                abilities2.Wisdom.OverrideScore = statisticValuesGroupCollection.GetValue("companion:" + strings.WisdomSet);
                abilities2.Charisma.OverrideScore = statisticValuesGroupCollection.GetValue("companion:" + strings.CharismaSet);
            }
            if (Settings.Default.UseDefaultAbilityScoreMaximum)
            {
                abilities.Strength.MaximumScore = statisticValuesGroupCollection.GetValue(strings.StrengthMaximum);
                abilities.Dexterity.MaximumScore = statisticValuesGroupCollection.GetValue(strings.DexterityMaximum);
                abilities.Constitution.MaximumScore = statisticValuesGroupCollection.GetValue(strings.ConstitutionMaximum);
                abilities.Intelligence.MaximumScore = statisticValuesGroupCollection.GetValue(strings.IntelligenceMaximum);
                abilities.Wisdom.MaximumScore = statisticValuesGroupCollection.GetValue(strings.WisdomMaximum);
                abilities.Charisma.MaximumScore = statisticValuesGroupCollection.GetValue(strings.CharismaMaximum);
            }
            else
            {
                abilities.Strength.MaximumScore = 100;
                abilities.Dexterity.MaximumScore = 100;
                abilities.Constitution.MaximumScore = 100;
                abilities.Intelligence.MaximumScore = 100;
                abilities.Wisdom.MaximumScore = 100;
                abilities.Charisma.MaximumScore = 100;
            }
            statisticValuesGroupCollection.GetGroup(strings.StrengthScore).AddValue("Strength", abilities.Strength.FinalScore);
            statisticValuesGroupCollection.GetGroup(strings.DexterityScore).AddValue("Dexterity", abilities.Dexterity.FinalScore);
            statisticValuesGroupCollection.GetGroup(strings.ConstitutionScore).AddValue("Constitution", abilities.Constitution.FinalScore);
            statisticValuesGroupCollection.GetGroup(strings.IntelligenceScore).AddValue("Intelligence", abilities.Intelligence.FinalScore);
            statisticValuesGroupCollection.GetGroup(strings.WisdomScore).AddValue("Wisdom", abilities.Wisdom.FinalScore);
            statisticValuesGroupCollection.GetGroup(strings.CharismaScore).AddValue("Charisma", abilities.Charisma.FinalScore);
            statisticValuesGroupCollection.GetGroup(strings.StrengthModifier).AddValue("Strength Modifier", abilities.Strength.Modifier);
            statisticValuesGroupCollection.GetGroup(strings.DexterityModifier).AddValue("Dexterity Modifier", abilities.Dexterity.Modifier);
            statisticValuesGroupCollection.GetGroup(strings.ConstitutionModifier).AddValue("Constitution Modifier", abilities.Constitution.Modifier);
            statisticValuesGroupCollection.GetGroup(strings.IntelligenceModifier).AddValue("Intelligence Modifier", abilities.Intelligence.Modifier);
            statisticValuesGroupCollection.GetGroup(strings.WisdomModifier).AddValue("Wisdom Modifier", abilities.Wisdom.Modifier);
            statisticValuesGroupCollection.GetGroup(strings.CharismaModifier).AddValue("Charisma Modifier", abilities.Charisma.Modifier);
            StatisticValuesGroup group2 = CreateHalf(statisticValuesGroupCollection.GetGroup(strings.StrengthModifier), "Strength Modifier");
            StatisticValuesGroup group3 = CreateHalf(statisticValuesGroupCollection.GetGroup(strings.DexterityModifier), "Dexterity Modifier");
            StatisticValuesGroup group4 = CreateHalf(statisticValuesGroupCollection.GetGroup(strings.ConstitutionModifier), "Constitution Modifier");
            StatisticValuesGroup group5 = CreateHalf(statisticValuesGroupCollection.GetGroup(strings.IntelligenceModifier), "Intelligence Modifier");
            StatisticValuesGroup group6 = CreateHalf(statisticValuesGroupCollection.GetGroup(strings.WisdomModifier), "Wisdom Modifier");
            StatisticValuesGroup group7 = CreateHalf(statisticValuesGroupCollection.GetGroup(strings.CharismaModifier), "Charisma Modifier");
            statisticValuesGroupCollection.AddGroup(group2);
            statisticValuesGroupCollection.AddGroup(group3);
            statisticValuesGroupCollection.AddGroup(group4);
            statisticValuesGroupCollection.AddGroup(group5);
            statisticValuesGroupCollection.AddGroup(group6);
            statisticValuesGroupCollection.AddGroup(group7);
            StatisticValuesGroup group8 = CreateHalfUp(statisticValuesGroupCollection.GetGroup(strings.StrengthModifier), "Strength Modifier");
            StatisticValuesGroup group9 = CreateHalfUp(statisticValuesGroupCollection.GetGroup(strings.DexterityModifier), "Dexterity Modifier");
            StatisticValuesGroup group10 = CreateHalfUp(statisticValuesGroupCollection.GetGroup(strings.ConstitutionModifier), "Constitution Modifier");
            StatisticValuesGroup group11 = CreateHalfUp(statisticValuesGroupCollection.GetGroup(strings.IntelligenceModifier), "Intelligence Modifier");
            StatisticValuesGroup group12 = CreateHalfUp(statisticValuesGroupCollection.GetGroup(strings.WisdomModifier), "Wisdom Modifier");
            StatisticValuesGroup group13 = CreateHalfUp(statisticValuesGroupCollection.GetGroup(strings.CharismaModifier), "Charisma Modifier");
            statisticValuesGroupCollection.AddGroup(group8);
            statisticValuesGroupCollection.AddGroup(group9);
            statisticValuesGroupCollection.AddGroup(group10);
            statisticValuesGroupCollection.AddGroup(group11);
            statisticValuesGroupCollection.AddGroup(group12);
            statisticValuesGroupCollection.AddGroup(group13);
            statisticValuesGroupCollection.GetGroup(strings.ProficiencyHalf).AddValue("INTERNAL", statisticValuesGroupCollection.GetValue(strings.Proficiency) / 2);
            statisticValuesGroupCollection.GetGroup(strings.ProficiencyHalfUp).AddValue("INTERNAL", statisticValuesGroupCollection.GetValue(strings.Proficiency) / 2 + statisticValuesGroupCollection.GetValue(strings.Proficiency) % 2);
            statisticValuesGroupCollection.GetGroup("companion:" + strings.StrengthScore).AddValue("Companion Strength", abilities2.Strength.FinalScore);
            statisticValuesGroupCollection.GetGroup("companion:" + strings.DexterityScore).AddValue("Companion Dexterity", abilities2.Dexterity.FinalScore);
            statisticValuesGroupCollection.GetGroup("companion:" + strings.ConstitutionScore).AddValue("Companion Constitution", abilities2.Constitution.FinalScore);
            statisticValuesGroupCollection.GetGroup("companion:" + strings.IntelligenceScore).AddValue("Companion Intelligence", abilities2.Intelligence.FinalScore);
            statisticValuesGroupCollection.GetGroup("companion:" + strings.WisdomScore).AddValue("Companion Wisdom", abilities2.Wisdom.FinalScore);
            statisticValuesGroupCollection.GetGroup("companion:" + strings.CharismaScore).AddValue("Companion Charisma", abilities2.Charisma.FinalScore);
            statisticValuesGroupCollection.GetGroup("companion:" + strings.StrengthModifier).AddValue("Companion Strength Modifier", abilities2.Strength.Modifier);
            statisticValuesGroupCollection.GetGroup("companion:" + strings.DexterityModifier).AddValue("Companion Dexterity Modifier", abilities2.Dexterity.Modifier);
            statisticValuesGroupCollection.GetGroup("companion:" + strings.ConstitutionModifier).AddValue("Companion Constitution Modifier", abilities2.Constitution.Modifier);
            statisticValuesGroupCollection.GetGroup("companion:" + strings.IntelligenceModifier).AddValue("Companion Intelligence Modifier", abilities2.Intelligence.Modifier);
            statisticValuesGroupCollection.GetGroup("companion:" + strings.WisdomModifier).AddValue("Companion Wisdom Modifier", abilities2.Wisdom.Modifier);
            statisticValuesGroupCollection.GetGroup("companion:" + strings.CharismaModifier).AddValue("Companion Charisma Modifier", abilities2.Charisma.Modifier);
            statisticValuesGroupCollection.GetGroup("companion:" + strings.ProficiencyHalf).AddValue("INTERNAL", statisticValuesGroupCollection.GetValue("companion:" + strings.Proficiency) / 2);
            statisticValuesGroupCollection.GetGroup("companion:" + strings.ProficiencyHalfUp).AddValue("INTERNAL", statisticValuesGroupCollection.GetValue("companion:" + strings.Proficiency) / 2 + statisticValuesGroupCollection.GetValue("companion:" + strings.Proficiency) % 2);
            List<StatisticRule> list2 = CalculateGroups(queue2, statisticValuesGroupCollection).ToList();
            if (list2.Any())
            {
                foreach (StatisticRule item4 in CalculateGroups(list2, statisticValuesGroupCollection, forceUnhandledQueue: true))
                {
                    Logger.Warning($"invalid rule that was not calculated: {item4}");
                    try
                    {
                        if (item4.Attributes.HasBonus)
                        {
                            Logger.Warning($"rule with bonus getting added without bonus type in mind! {item4}");
                        }
                        if (item4.Attributes.Value.StartsWith("+") || item4.Attributes.Value.StartsWith("-"))
                        {
                            Logger.Warning($"{item4} has a named value with + or - in front, todo: check and fix in proper method, not here!");
                            if (Debugger.IsAttached)
                            {
                                Debugger.Break();
                            }
                            continue;
                        }
                        StatisticValuesGroup statisticValuesGroup = new StatisticValuesGroup(item4.Attributes.Name);
                        if (statisticValuesGroupCollection.ContainsGroup(item4.Attributes.Name))
                        {
                            statisticValuesGroup = statisticValuesGroupCollection.GetGroup(item4.Attributes.Name);
                        }
                        if (HandleSingleRule(item4, statisticValuesGroup, statisticValuesGroupCollection))
                        {
                            statisticValuesGroupCollection.AddGroup(statisticValuesGroup);
                        }
                        else
                        {
                            Logger.Warning($"unable to add '{item4.Attributes.Name}' from todoRules ({item4})");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Warning($"unable to add '{item4.Attributes.Name}' from todoRules ({item4})");
                        Logger.Exception(ex, "CalculateValues");
                    }
                }
            }
            StatisticValuesGroup group14 = statisticValuesGroupCollection.GetGroup("ac");
            group14.Merge(statisticValuesGroupCollection.GetGroup("ac:calculation", createNonExisting: false));
            group14.Merge(statisticValuesGroupCollection.GetGroup("ac:shield", createNonExisting: false));
            group14.Merge(statisticValuesGroupCollection.GetGroup("ac:misc", createNonExisting: false));
            foreach (SpellcastingInformation spellcastingInformation in _manager.GetSpellcastingInformations())
            {
                StatisticValuesGroup group15 = statisticValuesGroupCollection.GetGroup(spellcastingInformation.GetSpellcasterSpellAttackStatisticName());
                StatisticValuesGroup group16 = statisticValuesGroupCollection.GetGroup(spellcastingInformation.GetSpellcasterSpellSaveStatisticName());
                group15.Merge(statisticValuesGroupCollection.GetGroup(spellcastingInformation.GetSpellAttackStatisticName(), createNonExisting: false));
                group16.Merge(statisticValuesGroupCollection.GetGroup(spellcastingInformation.GetSpellSaveStatisticName(), createNonExisting: false));
            }
            foreach (StatisticRule item5 in (from rule in _manager.GetStatisticRules()
                                             where rule.Attributes.Level <= _manager.Character.Level && rule.Attributes.Inline
                                             select rule).ToList())
            {
                SetInlineValue(item5);
            }
            if (setProperties)
            {
                StatisticValues = statisticValuesGroupCollection;
            }
            return statisticValuesGroupCollection;
        }

        private IEnumerable<StatisticRule> CalculateGroups(IEnumerable<StatisticRule> statisticRules, StatisticValuesGroupCollection groups, bool forceUnhandledQueue = false)
        {
            List<StatisticRule> list = new List<StatisticRule>();
            Queue<IGrouping<string, StatisticRule>> queue = new Queue<IGrouping<string, StatisticRule>>(from x in statisticRules
                                                                                                        group x by x.Attributes.Name);
            List<string> list2 = new List<string>();
            while (queue.Any())
            {
                IGrouping<string, StatisticRule> grouping = queue.Dequeue();
                List<string> pendingRules = queue.Select((IGrouping<string, StatisticRule> x) => x.Key).ToList();
                StatisticValuesGroup statisticValuesGroup = new StatisticValuesGroup(grouping.Key);
                if (grouping.Count() == 1)
                {
                    StatisticRule statisticRule = grouping.First();
                    if (HandleSingleRule(statisticRule, statisticValuesGroup, groups, pendingRules))
                    {
                        groups.AddGroup(statisticValuesGroup);
                    }
                    else
                    {
                        list.Add(statisticRule);
                    }
                    continue;
                }
                Queue<StatisticRule> queue2 = new Queue<StatisticRule>();
                foreach (StatisticRule item in grouping.Where((StatisticRule x) => !x.Attributes.HasBonus))
                {
                    if (!HandleSingleRule(item, statisticValuesGroup, groups, pendingRules))
                    {
                        queue2.Enqueue(item);
                    }
                }
                foreach (IGrouping<string, StatisticRule> item2 in from x in grouping
                                                                   where x.Attributes.HasBonus
                                                                   group x by x.Attributes.Bonus)
                {
                    if (item2.Count() == 1)
                    {
                        StatisticRule statisticRule2 = item2.First();
                        if (!HandleSingleRule(statisticRule2, statisticValuesGroup, groups, pendingRules))
                        {
                            queue2.Enqueue(statisticRule2);
                        }
                        continue;
                    }
                    int num = 0;
                    string text = "";
                    foreach (StatisticRule item3 in item2)
                    {
                        if (!TryGetValue(item3, out var value, groups, pendingRules))
                        {
                            queue2.Enqueue(item3);
                            continue;
                        }
                        if (item3.Attributes.HasCap)
                        {
                            if (!TryGetCappedValue(item3, out var capValue, groups, pendingRules))
                            {
                                queue2.Enqueue(item3);
                                continue;
                            }
                            value = Math.Min(value, capValue);
                        }
                        if (value > num)
                        {
                            num = value;
                            text = (item3.Attributes.HasAlt ? item3.Attributes.Alt : item3.ElementHeader.Name);
                        }
                        else if (value == num)
                        {
                            text = text + " | " + (item3.Attributes.HasAlt ? item3.Attributes.Alt : item3.ElementHeader.Name);
                        }
                    }
                    statisticValuesGroup.AddValue(text, num);
                }
                if (!queue2.Any())
                {
                    groups.AddGroup(statisticValuesGroup);
                }
                else if (list2.Contains(grouping.Key))
                {
                    if (forceUnhandledQueue)
                    {
                        groups.AddGroup(statisticValuesGroup);
                        list.AddRange(queue2);
                    }
                    else
                    {
                        list.AddRange(grouping);
                    }
                }
                else
                {
                    queue.Enqueue(grouping);
                    list2.Add(grouping.Key);
                }
            }
            return list;
        }

        private bool TryGetValue(StatisticRule rule, out int value, StatisticValuesGroupCollection groups, List<string> pendingRules = null)
        {
            if (rule.Attributes.IsNumberValue())
            {
                value = rule.Attributes.GetValue();
            }
            else
            {
                if (!groups.ContainsGroup(rule.Attributes.Value))
                {
                    value = 0;
                    return false;
                }
                _ = rule.Attributes.Merge;
                if (pendingRules != null && HasPendingStatistics(rule.Attributes.Value, pendingRules))
                {
                    value = 0;
                    return false;
                }
                if (rule.Attributes.Value.StartsWith("-"))
                {
                    value = -groups.GetGroup(rule.Attributes.Value).Sum();
                }
                else
                {
                    value = groups.GetGroup(rule.Attributes.Value).Sum();
                }
            }
            return true;
        }

        private bool TryGetCappedValue(StatisticRule rule, out int capValue, StatisticValuesGroupCollection groups, List<string> pendingRules = null)
        {
            if (rule.Attributes.IsNumberCap())
            {
                capValue = rule.Attributes.GetCapValue();
            }
            else
            {
                if (!groups.ContainsGroup(rule.Attributes.Cap))
                {
                    capValue = 0;
                    return false;
                }
                if (rule.Attributes.Merge && Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                if (pendingRules != null && HasPendingStatistics(rule.Attributes.Value, pendingRules))
                {
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }
                    Logger.Warning("TryGetCappedValue: there are still pending rules to be added to '" + rule.Attributes.Value + "' before this value should be recovered");
                }
                capValue = groups.GetGroup(rule.Attributes.Cap).Sum();
            }
            return true;
        }

        private bool HasPendingStatistics(string statisticName, IEnumerable<string> pendingStatisticRules)
        {
            return pendingStatisticRules.Any((string x) => x.Equals(statisticName, StringComparison.OrdinalIgnoreCase));
        }

        private bool HandleSingleRule(StatisticRule rule, StatisticValuesGroup statGroup, StatisticValuesGroupCollection groups, List<string> pendingRules = null)
        {
            _ = rule.Attributes.HasAlt;
            if (!TryGetValue(rule, out var value, groups, pendingRules))
            {
                return false;
            }
            if (rule.Attributes.HasCap)
            {
                if (!TryGetCappedValue(rule, out var capValue, groups, pendingRules))
                {
                    return false;
                }
                value = Math.Min(value, capValue);
            }
            if (pendingRules != null)
            {
                HasPendingStatistics(rule.Attributes.Value, pendingRules);
            }
            statGroup.AddValue(rule.Attributes.HasAlt ? rule.Attributes.Alt : rule.ElementHeader.Name, value);
            return true;
        }

        private StatisticValuesGroup CreateHalf(StatisticValuesGroup normalGroup, string sourceName = "Internal")
        {
            StatisticValuesGroup statisticValuesGroup = new StatisticValuesGroup(normalGroup.GroupName + ":half");
            statisticValuesGroup.AddValue(sourceName, normalGroup.Sum() / 2);
            return statisticValuesGroup;
        }

        private StatisticValuesGroup CreateHalfUp(StatisticValuesGroup normalGroup, string sourceName = "Internal")
        {
            StatisticValuesGroup statisticValuesGroup = new StatisticValuesGroup(normalGroup.GroupName + ":half:up");
            statisticValuesGroup.AddValue(sourceName, normalGroup.Sum() / 2 + normalGroup.Sum() % 2);
            return statisticValuesGroup;
        }

        private void SetInlineValue(StatisticRule rule)
        {
            if (InlineValues.ContainsKey(rule.Attributes.Name))
            {
                InlineValues[rule.Attributes.Name] = rule.Attributes.Value;
            }
            else
            {
                InlineValues.Add(rule.Attributes.Name, rule.Attributes.Value);
            }
        }

        [Obsolete]
        private Queue<IGrouping<string, StatisticRule>> CalculateGroupsPreRefactorHelpers(IEnumerable<StatisticRule> statisticRules, StatisticValuesGroupCollection groups)
        {
            Queue<IGrouping<string, StatisticRule>> queue = new Queue<IGrouping<string, StatisticRule>>(from x in statisticRules
                                                                                                        group x by x.Attributes.Name);
            Queue<IGrouping<string, StatisticRule>> queue2 = new Queue<IGrouping<string, StatisticRule>>();
            List<string> list = new List<string>();
            while (queue.Any())
            {
                IGrouping<string, StatisticRule> grouping = queue.Dequeue();
                StatisticValuesGroup statisticValuesGroup = new StatisticValuesGroup(grouping.Key);
                if (grouping.Count() == 1)
                {
                    StatisticRule statisticRule = grouping.First();
                    HandleSingleRule(statisticRule, statisticValuesGroup, groups);
                    if (statisticRule.Attributes.IsNumberValue())
                    {
                        statisticValuesGroup.AddValue(statisticRule.ElementHeader.Name, statisticRule.Attributes.GetValue());
                        groups.AddGroup(statisticValuesGroup);
                    }
                    else if (groups.ContainsGroup(statisticRule.Attributes.Value))
                    {
                        statisticValuesGroup.AddValue(statisticRule.ElementHeader.Name, groups.GetGroup(statisticRule.Attributes.Value).Sum());
                        groups.AddGroup(statisticValuesGroup);
                    }
                    else
                    {
                        queue2.Enqueue(grouping);
                    }
                    continue;
                }
                Queue<StatisticRule> queue3 = new Queue<StatisticRule>();
                foreach (StatisticRule item in grouping.Where((StatisticRule x) => !x.Attributes.HasBonus))
                {
                    if (item.Attributes.IsNumberValue())
                    {
                        statisticValuesGroup.AddValue(item.ElementHeader.Name, item.Attributes.GetValue());
                    }
                    else if (groups.ContainsGroup(item.Attributes.Value))
                    {
                        statisticValuesGroup.AddValue(item.ElementHeader.Name, groups.GetGroup(item.Attributes.Value).Sum());
                    }
                    else
                    {
                        queue3.Enqueue(item);
                    }
                }
                foreach (IGrouping<string, StatisticRule> item2 in from x in grouping
                                                                   where x.Attributes.HasBonus
                                                                   group x by x.Attributes.Bonus)
                {
                    if (item2.Count() == 1)
                    {
                        StatisticRule statisticRule2 = item2.First();
                        if (statisticRule2.Attributes.IsNumberValue())
                        {
                            int value = statisticRule2.Attributes.GetValue();
                            if (statisticRule2.Attributes.HasCap)
                            {
                                int val = (statisticRule2.Attributes.IsNumberCap() ? statisticRule2.Attributes.GetCapValue() : groups.GetGroup(statisticRule2.Attributes.Cap).Sum());
                                statisticValuesGroup.AddValue(statisticRule2.ElementHeader.Name, Math.Min(value, val));
                            }
                            else
                            {
                                statisticValuesGroup.AddValue(statisticRule2.ElementHeader.Name, value);
                            }
                        }
                        else if (groups.ContainsGroup(statisticRule2.Attributes.Value))
                        {
                            if (statisticRule2.Attributes.HasCap)
                            {
                                int val2 = groups.GetGroup(statisticRule2.Attributes.Value).Sum();
                                if (statisticRule2.Attributes.IsNumberCap())
                                {
                                    int capValue = statisticRule2.Attributes.GetCapValue();
                                    statisticValuesGroup.AddValue(statisticRule2.ElementHeader.Name, Math.Min(val2, capValue));
                                }
                                else if (groups.ContainsGroup(statisticRule2.Attributes.Cap))
                                {
                                    int val3 = groups.GetGroup(statisticRule2.Attributes.Cap).Sum();
                                    statisticValuesGroup.AddValue(statisticRule2.ElementHeader.Name, Math.Min(val2, val3));
                                }
                                else
                                {
                                    queue3.Enqueue(statisticRule2);
                                }
                            }
                            else
                            {
                                statisticValuesGroup.AddValue(statisticRule2.ElementHeader.Name, groups.GetGroup(statisticRule2.Attributes.Value).Sum());
                            }
                        }
                        else
                        {
                            queue3.Enqueue(statisticRule2);
                        }
                        continue;
                    }
                    int num = 0;
                    StatisticRule statisticRule3 = null;
                    foreach (StatisticRule item3 in item2)
                    {
                        if (item3.Attributes.IsNumberValue())
                        {
                            int num2 = item3.Attributes.GetValue();
                            if (item3.Attributes.HasCap)
                            {
                                int val4 = (item3.Attributes.IsNumberCap() ? item3.Attributes.GetCapValue() : groups.GetGroup(item3.Attributes.Cap).Sum());
                                num2 = Math.Min(num2, val4);
                            }
                            if (num2 > num)
                            {
                                num = num2;
                                statisticRule3 = item3;
                            }
                        }
                        else if (groups.ContainsGroup(item3.Attributes.Value))
                        {
                            int num3 = groups.GetGroup(item3.Attributes.Value).Sum();
                            if (item3.Attributes.HasCap)
                            {
                                if (item3.Attributes.IsNumberCap())
                                {
                                    int capValue2 = item3.Attributes.GetCapValue();
                                    num3 = Math.Min(num3, capValue2);
                                    if (num3 > num)
                                    {
                                        num = num3;
                                        statisticRule3 = item3;
                                    }
                                }
                                else if (groups.ContainsGroup(item3.Attributes.Cap))
                                {
                                    int val5 = groups.GetGroup(item3.Attributes.Cap).Sum();
                                    num3 = Math.Min(num3, val5);
                                    if (num3 > num)
                                    {
                                        num = num3;
                                        statisticRule3 = item3;
                                    }
                                }
                                else
                                {
                                    queue3.Enqueue(item3);
                                }
                            }
                            else if (num3 > num)
                            {
                                num = num3;
                                statisticRule3 = item3;
                            }
                        }
                        else
                        {
                            queue3.Enqueue(item3);
                        }
                    }
                    if (statisticRule3 != null)
                    {
                        statisticValuesGroup.AddValue(statisticRule3.ElementHeader.Name, num);
                    }
                    else
                    {
                        Logger.Warning($"bonusRule is null and the value is {num}");
                    }
                }
                if (queue3.Any())
                {
                    if (list.Contains(grouping.Key))
                    {
                        queue2.Enqueue(grouping);
                        Logger.Warning("adding (" + grouping.Key + ") to the remaining rule groups");
                        Logger.Warning($"\tcurrent statgroup: {statisticValuesGroup}");
                        Logger.Warning($"\tgroupQueue count: {queue3.Count}");
                    }
                    else
                    {
                        queue.Enqueue(grouping);
                        list.Add(grouping.Key);
                    }
                }
                else if (statisticValuesGroup.GetValues().Any())
                {
                    groups.AddGroup(statisticValuesGroup);
                }
            }
            return queue2;
        }
    }
}
