using Aurora.Documents.ExportContent;
using Aurora.Documents.Sheets;
using Builder.Core.Events;
using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Data.Extensions;
using Builder.Data.Rules;
using Builder.Data.Strings;
using Builder.Presentation.Documents;
using Builder.Presentation.Extensions;
using Builder.Presentation.Models;
using Builder.Presentation.Models.CharacterSheet;
using Builder.Presentation.Models.Equipment;
using Builder.Presentation.Models.Helpers;
using Builder.Presentation.Models.Sheet;
using Builder.Presentation.Properties;
using Builder.Presentation.Services.Calculator;
using Builder.Presentation.Services.Data;
using Builder.Presentation.Telemetry;
using Builder.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Aurora.Documents.ExportContent;
using Aurora.Documents.Sheets;
using Builder.Core.Events;
using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Data.Extensions;
using Builder.Data.Rules;
using Builder.Data.Strings;
using Builder.Presentation;
using Builder.Presentation.Documents;
using Builder.Presentation.Extensions;
using Builder.Presentation.Models;
using Builder.Presentation.Models.CharacterSheet;
using Builder.Presentation.Models.CharacterSheet.Content;
using Builder.Presentation.Models.Equipment;
using Builder.Presentation.Models.Helpers;
using Builder.Presentation.Models.Sheet;
using Builder.Presentation.Properties;
using Builder.Presentation.Services;
using Builder.Presentation.Services.Calculator;
using Builder.Presentation.Services.Data;
using Builder.Presentation.Telemetry;
//using Builder.Presentation.UserControls.Spellcasting;
using Builder.Presentation.Utilities;
using Builder.Presentation.ViewModels;
using Builder.Presentation.ViewModels.Shell.Items;

namespace Builder.Presentation.Services
{
    public class CharacterSheetGenerator
    {
        private IExportContentProvider _contentProvider;

        private readonly IEventAggregator _eventAggregator;

        private readonly CharacterManager _manager;

        private readonly Character _character;

        private readonly ElementBaseCollection _elements;

        public CharacterSheetGenerator(CharacterManager manager)
        {
            _eventAggregator = ApplicationManager.Current.EventAggregator;
            _manager = manager;
            _character = _manager.Character;
            _elements = _manager.GetElements();
        }

        private SpellcastingSheetInfo GenerateSpellcastingSheetInfo(SpellcastingInformation information)
        {
            // SpellcasterSelectionControlViewModel spellcasterSectionViewModel = SpellcastingSectionHandler.Current.GetSpellcasterSectionViewModel(information.UniqueIdentifier);
            //IOrderedEnumerable<SelectionElement> spells = SpellcastingSectionHandler.Current.GetSpells(information);
            SpellcastingSheetInfo spellcastingSheetInfo = new SpellcastingSheetInfo();
            spellcastingSheetInfo.SpellcastingClass = information.Name;
            spellcastingSheetInfo.SpellcastingAbility = information.AbilityName;
            //spellcastingSheetInfo.SpellAttackBonus = spellcasterSectionViewModel.InformationHeader.SpellAttackModifier.ToString();
            //spellcastingSheetInfo.SpellSaveDifficultyClass = spellcasterSectionViewModel.InformationHeader.SpellSaveDc.ToString();
            //spellcastingSheetInfo.Spells1st.TotalSlots = spellcasterSectionViewModel.InformationHeader.Slot1;
            //spellcastingSheetInfo.Spells2nd.TotalSlots = spellcasterSectionViewModel.InformationHeader.Slot2;
            //spellcastingSheetInfo.Spells3rd.TotalSlots = spellcasterSectionViewModel.InformationHeader.Slot3;
            //spellcastingSheetInfo.Spells4th.TotalSlots = spellcasterSectionViewModel.InformationHeader.Slot4;
            //spellcastingSheetInfo.Spells5th.TotalSlots = spellcasterSectionViewModel.InformationHeader.Slot5;
            //spellcastingSheetInfo.Spells6th.TotalSlots = spellcasterSectionViewModel.InformationHeader.Slot6;
            //spellcastingSheetInfo.Spells7th.TotalSlots = spellcasterSectionViewModel.InformationHeader.Slot7;
            //spellcastingSheetInfo.Spells8th.TotalSlots = spellcasterSectionViewModel.InformationHeader.Slot8;
            //spellcastingSheetInfo.Spells9th.TotalSlots = spellcasterSectionViewModel.InformationHeader.Slot9;
            SpellcastingSheetInfo spellcastingSheetInfo2 = spellcastingSheetInfo;
            //foreach (SelectionElement item in spells)
            //{
            //    Spell spell = item.Element.AsElement<Spell>();
            //    switch (spell.Level)
            //    {
            //        case 0:
            //            spellcastingSheetInfo2.Cantrips.Add(new SpellcastingSheetInfo.SpellsContainer.SpellPlaceholder(spell.Name, isPrepared: true));
            //            break;
            //        case 1:
            //            spellcastingSheetInfo2.Spells1st.Add(new SpellcastingSheetInfo.SpellsContainer.SpellPlaceholder(spell.Name, item.IsChosen));
            //            break;
            //        case 2:
            //            spellcastingSheetInfo2.Spells2nd.Add(new SpellcastingSheetInfo.SpellsContainer.SpellPlaceholder(spell.Name, item.IsChosen));
            //            break;
            //        case 3:
            //            spellcastingSheetInfo2.Spells3rd.Add(new SpellcastingSheetInfo.SpellsContainer.SpellPlaceholder(spell.Name, item.IsChosen));
            //            break;
            //        case 4:
            //            spellcastingSheetInfo2.Spells4th.Add(new SpellcastingSheetInfo.SpellsContainer.SpellPlaceholder(spell.Name, item.IsChosen));
            //            break;
            //        case 5:
            //            spellcastingSheetInfo2.Spells5th.Add(new SpellcastingSheetInfo.SpellsContainer.SpellPlaceholder(spell.Name, item.IsChosen));
            //            break;
            //        case 6:
            //            spellcastingSheetInfo2.Spells6th.Add(new SpellcastingSheetInfo.SpellsContainer.SpellPlaceholder(spell.Name, item.IsChosen));
            //            break;
            //        case 7:
            //            spellcastingSheetInfo2.Spells7th.Add(new SpellcastingSheetInfo.SpellsContainer.SpellPlaceholder(spell.Name, item.IsChosen));
            //            break;
            //        case 8:
            //            spellcastingSheetInfo2.Spells8th.Add(new SpellcastingSheetInfo.SpellsContainer.SpellPlaceholder(spell.Name, item.IsChosen));
            //            break;
            //        case 9:
            //            spellcastingSheetInfo2.Spells9th.Add(new SpellcastingSheetInfo.SpellsContainer.SpellPlaceholder(spell.Name, item.IsChosen));
            //            break;
            //    }
            //}
            return spellcastingSheetInfo2;
        }

        public FileInfo GenerateNewSheet(string destinationPath, bool generateForPreview)
        {
            Settings settings = ApplicationManager.Current.Settings.Settings;
            List<SpellcastingInformation> list = (from x in CharacterManager.Current.GetSpellcastingInformations()
                                                  where !x.IsExtension
                                                  select x).ToList();
            CharacterSheetEx characterSheetEx = new CharacterSheetEx();
            characterSheetEx.Configuration.IsAttributeDisplayFlipped = settings.CharacterSheetAbilitiesFlipped;
            characterSheetEx.Configuration.IncludeBackgroundPage = true;
            characterSheetEx.Configuration.IncludeEquipmentPage = CharacterManager.Current.Character.Inventory.Items.Count > 15 || true;
            characterSheetEx.Configuration.IncludeNotesPage = CharacterManager.Current.Character.Notes1.Length > 0 || CharacterManager.Current.Character.Notes2.Length > 0;
            characterSheetEx.Configuration.IncludeCompanionPage = CharacterManager.Current.Status.HasCompanion;
            characterSheetEx.Configuration.IncludeSpellcastingPage = list.Any();
            characterSheetEx.Configuration.IsEditable = settings.AllowEditableSheet;
            characterSheetEx.Configuration.IncludeSpellcards = settings.IncludeSpellcards;
            characterSheetEx.Configuration.IncludeItemcards = settings.IncludeItemcards;
            characterSheetEx.Configuration.IncludeAttackCards = settings.SheetIncludeAttackCards;
            characterSheetEx.Configuration.IncludeFeatureCards = settings.SheetIncludeFeatureCards;
            characterSheetEx.Configuration.IncludeFormatting = settings.SheetIncludeFormatting;
            characterSheetEx.Configuration.StartNewSpellCardsPage = settings.SheetStartSpellCardsOnNewPage;
            characterSheetEx.Configuration.StartNewAttackCardsPage = settings.SheetStartAttackCardsOnNewPage;
            characterSheetEx.Configuration.StartNewItemCardsPage = settings.SheetStartItemCardsOnNewPage;
            characterSheetEx.Configuration.StartNewFeatureCardsPage = settings.SheetStartFeatureCardsOnNewPage;
            characterSheetEx.Configuration.UseLegacyDetailsPage = settings.UseLegacyDetailsPage;
            characterSheetEx.Configuration.UseLegacyBackgroundPage = settings.UseLegactBackgroundPage;
            characterSheetEx.Configuration.UseLegacySpellcastingPage = settings.UseLegacySpellcastingPage;
            characterSheetEx.ExportContent = GenerateExportContent(CharacterManager.Current, characterSheetEx.Configuration);
            _contentProvider = new ExportContentGenerator(_manager, characterSheetEx.Configuration);
            if (characterSheetEx.Configuration.IncludeEquipmentPage)
            {
                characterSheetEx.EquipmentSheetExportContent = _contentProvider.GetEquipmentContent();
            }
            if (characterSheetEx.Configuration.IncludeNotesPage)
            {
                characterSheetEx.NotesExportContent = _contentProvider.GetNotesContent();
            }
            if (characterSheetEx.Configuration.IncludeCompanionPage)
            {
                GenerateCompanionSheetExportContent(CharacterManager.Current, characterSheetEx.Configuration, characterSheetEx.ExportContent);
            }
            List<string> list2 = new List<string>();
            foreach (SpellcastingInformation item in list)
            {
                characterSheetEx.SpellcastingPageExportContentCollection.Add(GenerateSpellcastingExportContent(CharacterManager.Current, item, list2));
            }
            try
            {
                if (list.Any())
                {
                    CharacterSheetSpellcastingPageExportContent characterSheetSpellcastingPageExportContent = GenerateOtherSpellcastingExportContent(CharacterManager.Current, list2);
                    if (characterSheetSpellcastingPageExportContent != null)
                    {
                        characterSheetEx.SpellcastingPageExportContentCollection.Add(characterSheetSpellcastingPageExportContent);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "GenerateNewSheet");
                AnalyticsErrorHelper.Exception(ex, null, null, "GenerateNewSheet", 813);
            }
            if (characterSheetEx.Configuration.IncludeSpellcards)
            {
                IEnumerable<Spell> first = (from e in CharacterManager.Current.GetElements()
                                            where e.Type.Equals("Spell")
                                            select e).Cast<Spell>();
                IEnumerable<Spell> preparedSpells = CharacterManager.Current.GetPreparedSpells();
                IEnumerable<Spell> collection = (from x in first.Concat(preparedSpells)
                                                 orderby x.Level, x.Name
                                                 select x).Distinct();
                characterSheetEx.Spells.AddRange(collection);
            }
            if (characterSheetEx.Configuration.IncludeItemcards)
            {
                foreach (RefactoredEquipmentItem item2 in CharacterManager.Current.Character.Inventory.Items)
                {
                    if (item2.ShowCard)
                    {
                        characterSheetEx.Items.Add(item2);
                    }
                }
            }
            _ = characterSheetEx.Configuration.IncludeAttackCards;
            if (characterSheetEx.Configuration.IncludeFeatureCards)
            {
                foreach (ElementBase sortedFeature in new ElementsOrganizer(_manager.GetElements()).GetSortedFeatures(new List<ElementBase>()))
                {
                    if (sortedFeature.SheetDescription.DisplayOnSheet)
                    {
                        characterSheetEx.Features.Add(sortedFeature);
                    }
                }
            }
            if (generateForPreview)
            {
                AnalyticsEventHelper.CharacterSheetPreview(characterSheetEx.Configuration.IsFormFillable, characterSheetEx.Configuration.IncludeFormatting, characterSheetEx.Configuration.IncludeItemcards, characterSheetEx.Configuration.IncludeAttackCards, characterSheetEx.Configuration.IncludeSpellcards, characterSheetEx.Configuration.IncludeFeatureCards, characterSheetEx.Configuration.UseLegacySpellcastingPage);
            }
            else
            {
                AnalyticsEventHelper.CharacterSheetSave(characterSheetEx.Configuration.IsFormFillable, characterSheetEx.Configuration.IncludeFormatting, characterSheetEx.Configuration.IncludeItemcards, characterSheetEx.Configuration.IncludeAttackCards, characterSheetEx.Configuration.IncludeSpellcards, characterSheetEx.Configuration.IncludeFeatureCards, characterSheetEx.Configuration.UseLegacySpellcastingPage);
            }
            return characterSheetEx.Save(destinationPath);
        }

        private MainSheetContent GenerateInformationSheetContent(CharacterSheetConfiguration configuration, Character character)
        {
            MainSheetContent mainSheetContent = new MainSheetContent();
            mainSheetContent.CharacterName = character.Name;
            mainSheetContent.Level = character.Level.ToString();
            mainSheetContent.Class = character.Class;
            mainSheetContent.ClassLevel = mainSheetContent.Class + " (" + mainSheetContent.Level + ")";
            if (_manager.Status.HasMulticlass)
            {
                mainSheetContent.ClassLevel = mainSheetContent.Class ?? "";
            }
            mainSheetContent.Background = character.Background;
            mainSheetContent.PlayerName = character.PlayerName;
            mainSheetContent.Race = character.Race;
            mainSheetContent.Alignment = character.Alignment;
            mainSheetContent.Experience = character.Experience.ToString();
            mainSheetContent.Strength = character.Abilities.Strength.FinalScore.ToString();
            mainSheetContent.Dexterity = character.Abilities.Dexterity.FinalScore.ToString();
            mainSheetContent.Constitution = character.Abilities.Constitution.FinalScore.ToString();
            mainSheetContent.Intelligence = character.Abilities.Intelligence.FinalScore.ToString();
            mainSheetContent.Wisdom = character.Abilities.Wisdom.FinalScore.ToString();
            mainSheetContent.Charisma = character.Abilities.Charisma.FinalScore.ToString();
            mainSheetContent.StrengthModifier = character.Abilities.Strength.ModifierString;
            mainSheetContent.DexterityModifier = character.Abilities.Dexterity.ModifierString;
            mainSheetContent.ConstitutionModifier = character.Abilities.Constitution.ModifierString;
            mainSheetContent.IntelligenceModifier = character.Abilities.Intelligence.ModifierString;
            mainSheetContent.WisdomModifier = character.Abilities.Wisdom.ModifierString;
            mainSheetContent.CharismaModifier = character.Abilities.Charisma.ModifierString;
            mainSheetContent.ProficiencyBonus = character.Proficiency.ToValueString();
            mainSheetContent.StrengthSavingThrow = character.SavingThrows.Strength.FinalBonus.ToValueString();
            mainSheetContent.DexteritySavingThrow = character.SavingThrows.Dexterity.FinalBonus.ToValueString();
            mainSheetContent.ConstitutionSavingThrow = character.SavingThrows.Constitution.FinalBonus.ToValueString();
            mainSheetContent.IntelligenceSavingThrow = character.SavingThrows.Intelligence.FinalBonus.ToValueString();
            mainSheetContent.WisdomSavingThrow = character.SavingThrows.Wisdom.FinalBonus.ToValueString();
            mainSheetContent.CharismaSavingThrow = character.SavingThrows.Charisma.FinalBonus.ToValueString();
            mainSheetContent.StrengthSavingThrowProficient = character.SavingThrows.Strength.IsProficient;
            mainSheetContent.DexteritySavingThrowProficient = character.SavingThrows.Dexterity.IsProficient;
            mainSheetContent.ConstitutionSavingThrowProficient = character.SavingThrows.Constitution.IsProficient;
            mainSheetContent.IntelligenceSavingThrowProficient = character.SavingThrows.Intelligence.IsProficient;
            mainSheetContent.WisdomSavingThrowProficient = character.SavingThrows.Wisdom.IsProficient;
            mainSheetContent.CharismaSavingThrowProficient = character.SavingThrows.Charisma.IsProficient;
            mainSheetContent.Acrobatics = character.Skills.Acrobatics.FinalBonus.ToValueString();
            mainSheetContent.AnimalHandling = character.Skills.AnimalHandling.FinalBonus.ToValueString();
            mainSheetContent.Arcana = character.Skills.Arcana.FinalBonus.ToValueString();
            mainSheetContent.Athletics = character.Skills.Athletics.FinalBonus.ToValueString();
            mainSheetContent.Deception = character.Skills.Deception.FinalBonus.ToValueString();
            mainSheetContent.History = character.Skills.History.FinalBonus.ToValueString();
            mainSheetContent.Insight = character.Skills.Insight.FinalBonus.ToValueString();
            mainSheetContent.Intimidation = character.Skills.Intimidation.FinalBonus.ToValueString();
            mainSheetContent.Investigation = character.Skills.Investigation.FinalBonus.ToValueString();
            mainSheetContent.Medicine = character.Skills.Medicine.FinalBonus.ToValueString();
            mainSheetContent.Nature = character.Skills.Nature.FinalBonus.ToValueString();
            mainSheetContent.Perception = character.Skills.Perception.FinalBonus.ToValueString();
            mainSheetContent.Performance = character.Skills.Performance.FinalBonus.ToValueString();
            mainSheetContent.Persuasion = character.Skills.Persuasion.FinalBonus.ToValueString();
            mainSheetContent.Religion = character.Skills.Religion.FinalBonus.ToValueString();
            mainSheetContent.SleightOfHand = character.Skills.SleightOfHand.FinalBonus.ToValueString();
            mainSheetContent.Stealth = character.Skills.Stealth.FinalBonus.ToValueString();
            mainSheetContent.Survival = character.Skills.Survival.FinalBonus.ToValueString();
            mainSheetContent.AcrobaticsProficient = character.Skills.Acrobatics.IsProficient;
            mainSheetContent.AnimalHandlingProficient = character.Skills.AnimalHandling.IsProficient;
            mainSheetContent.ArcanaProficient = character.Skills.Arcana.IsProficient;
            mainSheetContent.AthleticsProficient = character.Skills.Athletics.IsProficient;
            mainSheetContent.DeceptionProficient = character.Skills.Deception.IsProficient;
            mainSheetContent.HistoryProficient = character.Skills.History.IsProficient;
            mainSheetContent.InsightProficient = character.Skills.Insight.IsProficient;
            mainSheetContent.IntimidationProficient = character.Skills.Intimidation.IsProficient;
            mainSheetContent.InvestigationProficient = character.Skills.Investigation.IsProficient;
            mainSheetContent.MedicineProficient = character.Skills.Medicine.IsProficient;
            mainSheetContent.NatureProficient = character.Skills.Nature.IsProficient;
            mainSheetContent.PerceptionProficient = character.Skills.Perception.IsProficient;
            mainSheetContent.PerformanceProficient = character.Skills.Performance.IsProficient;
            mainSheetContent.PersuasionProficient = character.Skills.Persuasion.IsProficient;
            mainSheetContent.ReligionProficient = character.Skills.Religion.IsProficient;
            mainSheetContent.SleightOfHandProficient = character.Skills.SleightOfHand.IsProficient;
            mainSheetContent.StealthProficient = character.Skills.Stealth.IsProficient;
            mainSheetContent.SurvivalProficient = character.Skills.Survival.IsProficient;
            mainSheetContent.AcrobaticsExpertise = character.Skills.Acrobatics.ProficiencyBonus == character.Proficiency * 2;
            int value = _manager.StatisticsCalculator.StatisticValues.GetValue(_manager.StatisticsCalculator.Names.PerceptionPassive);
            mainSheetContent.PassiveWisdomPerception = (character.Skills.Perception.FinalBonus + value).ToString();
            if (character.Inventory.EquippedArmor != null)
            {
                mainSheetContent.EquippedArmor = character.Inventory.EquippedArmor.ToString();
            }
            if (character.Inventory.EquippedSecondary != null && character.Inventory.IsEquippedShield())
            {
                mainSheetContent.EquippedShield = character.Inventory.EquippedSecondary.ToString();
            }
            mainSheetContent.ConditionalArmorClass = "conditional armor class field";
            mainSheetContent.ArmorClass = character.ArmorClass.ToString();
            mainSheetContent.Initiative = character.Initiative.ToString();
            mainSheetContent.Speed = character.Speed.ToString();
            mainSheetContent.FlySpeed = _manager.StatisticsCalculator.StatisticValues.GetValue("speed:fly").ToString();
            mainSheetContent.ClimbSpeed = _manager.StatisticsCalculator.StatisticValues.GetValue("speed:climb").ToString();
            mainSheetContent.SwimSpeed = _manager.StatisticsCalculator.StatisticValues.GetValue("speed:swim").ToString();
            mainSheetContent.MaximumHitPoints = character.MaxHp.ToString();
            mainSheetContent.CurrentHitPoints = character.MaxHp.ToString();
            mainSheetContent.TemporaryHitPoints = "0";
            string text = "";
            foreach (ClassProgressionManager classProgressionManager2 in CharacterManager.Current.ClassProgressionManagers)
            {
                text += $"/{classProgressionManager2.ProgressionLevel}{classProgressionManager2.HD}";
            }
            mainSheetContent.TotalHitDice = text.TrimStart('/').Trim();
            mainSheetContent.HitDice = "";
            mainSheetContent.DeathSavingThrowSuccess1 = true;
            mainSheetContent.DeathSavingThrowSuccess2 = false;
            mainSheetContent.DeathSavingThrowSuccess3 = false;
            mainSheetContent.DeathSavingThrowFailure1 = false;
            mainSheetContent.DeathSavingThrowFailure2 = false;
            mainSheetContent.DeathSavingThrowFailure3 = false;
            mainSheetContent.Name1 = character.AttacksSection.AttackObject1.Name;
            mainSheetContent.AttackBonus1 = character.AttacksSection.AttackObject1.Bonus;
            mainSheetContent.DamageType1 = character.AttacksSection.AttackObject1.Damage;
            mainSheetContent.Name2 = character.AttacksSection.AttackObject2.Name;
            mainSheetContent.AttackBonus2 = character.AttacksSection.AttackObject2.Bonus;
            mainSheetContent.DamageType2 = character.AttacksSection.AttackObject2.Damage;
            mainSheetContent.Name3 = character.AttacksSection.AttackObject3.Name;
            mainSheetContent.AttackBonus3 = character.AttacksSection.AttackObject3.Bonus;
            mainSheetContent.DamageType3 = character.AttacksSection.AttackObject3.Damage;
            mainSheetContent.AttackAndSpellcastingField = character.AttacksSection.AttacksAndSpellcasting;
            mainSheetContent.Copper = character.Inventory.Coins.Copper.ToString();
            mainSheetContent.Silver = character.Inventory.Coins.Silver.ToString();
            mainSheetContent.Electrum = character.Inventory.Coins.Electrum.ToString();
            mainSheetContent.Gold = character.Inventory.Coins.Gold.ToString();
            mainSheetContent.Platinum = character.Inventory.Coins.Platinum.ToString();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (RefactoredEquipmentItem item in character.Inventory.Items)
            {
                string text2 = ((!string.IsNullOrWhiteSpace(item.AlternativeName)) ? item.AlternativeName : item.DisplayName);
                string text3 = ((item.IsStackable && !string.IsNullOrWhiteSpace(item.AlternativeName)) ? $" ({item.Amount})" : "");
                string text4 = (item.IsEquipped ? " (E)" : "");
                stringBuilder.AppendLine(text2 + text3 + text4);
            }
            mainSheetContent.Equipment = stringBuilder.ToString();
            mainSheetContent.PersonalityTraits = character.FillableBackgroundCharacteristics.Traits.Content;
            mainSheetContent.Ideals = character.FillableBackgroundCharacteristics.Ideals.Content;
            mainSheetContent.Bonds = character.FillableBackgroundCharacteristics.Bonds.Content;
            mainSheetContent.Flaws = character.FillableBackgroundCharacteristics.Flaws.Content;
            ElementsOrganizer elementsOrganizer = new ElementsOrganizer(CharacterManager.Current.GetElements());
            List<Language> list = elementsOrganizer.GetLanguages(includeDuplicates: false).ToList();
            List<ElementBase> list2 = _manager.GetProficiencyList(elementsOrganizer.GetArmorProficiencies(includeDuplicates: false)).ToList();
            List<ElementBase> list3 = _manager.GetProficiencyList(elementsOrganizer.GetWeaponProficiencies(includeDuplicates: false)).ToList();
            List<ElementBase> list4 = _manager.GetProficiencyList(elementsOrganizer.GetToolProficiencies(includeDuplicates: false)).ToList();
            ContentField contentField = new ContentField();
            if (list.Count > 0)
            {
                contentField.Lines.Add(new ContentLine("Languages", string.Join(", ", list.Select((Language x) => x.Name))));
            }
            if (list2.Count > 0)
            {
                IEnumerable<string> values = list2.Select((ElementBase x) => x.Name.Replace("Armor Proficiency", "").Replace("(", "").Replace(")", "")
                    .Trim());
                contentField.Lines.Add(new ContentLine("Armor Proficiencies", string.Join(", ", values), newLineBefore: true));
            }
            if (list3.Count > 0)
            {
                IEnumerable<string> values2 = list3.Select((ElementBase x) => x.Name.Replace("Weapon Proficiency", "").Replace("(", "").Replace(")", "")
                    .Trim());
                contentField.Lines.Add(new ContentLine("Weapon Proficiencies", string.Join(", ", values2), newLineBefore: true));
            }
            if (list4.Count > 0)
            {
                IEnumerable<string> values3 = list4.Select((ElementBase x) => x.Name.Replace("Tool Proficiency", "").Replace("(", "").Replace(")", "")
                    .Trim());
                contentField.Lines.Add(new ContentLine("Tool Proficiencies", string.Join(", ", values3), newLineBefore: true));
            }
            mainSheetContent.ProficienciesAndLanguagesFieldContent = contentField;
            if (!configuration.IncludeFormatting)
            {
                StringBuilder stringBuilder2 = new StringBuilder();
                foreach (ContentLine line in mainSheetContent.ProficienciesAndLanguagesFieldContent.Lines)
                {
                    if (line.NewLineBefore)
                    {
                        stringBuilder2.AppendLine();
                    }
                    stringBuilder2.AppendLine(line.Name + ". " + line.Content);
                }
                mainSheetContent.ProficienciesAndLanguages = stringBuilder2.ToString();
            }
            List<ElementBase> list5 = (from x in _manager.GetElements()
                                       where x.Type == "Vision" || x.Type == "Race" || x.Type == "Sub Race" || x.Type == "Race Variant" || x.Type == "Racial Trait" || x.Type == "Class" || x.Type == "Class Feature" || x.Type == "Archetype" || x.Type == "Archetype Feature" || x.Type == "Feat" || x.Type == "Feat Feature"
                                       where !x.Name.StartsWith("Ability Score Increase")
                                       where !x.Name.StartsWith("Ability Score Improvement")
                                       select x).ToList();
            Logger.Info("====================features post sorting====================");
            List<ElementBase> list6 = new List<ElementBase>();
            List<ElementBase> list7 = new List<ElementBase>();
            foreach (ElementBase item2 in list5)
            {
                if (!list6.Contains(item2))
                {
                    list6.Add(item2);
                    Logger.Info($"{item2}");
                }
                if (!item2.ContainsSelectRules)
                {
                    continue;
                }
                foreach (SelectRule rule in item2.GetSelectRules())
                {
                    for (int i = 1; i <= rule.Attributes.Number; i++)
                    {
                        if (!SelectionRuleExpanderHandler.Current.HasExpander(rule.UniqueIdentifier, i))
                        {
                            continue;
                        }
                        ElementBase registeredElement = SelectionRuleExpanderHandler.Current.GetRegisteredElement(rule, i) as ElementBase;
                        if (!list5.Contains(registeredElement))
                        {
                            continue;
                        }
                        ElementBase elementBase = list5.First((ElementBase x) => x.Id == registeredElement.Id);
                        if (list6.Contains(elementBase))
                        {
                            continue;
                        }
                        ElementBase elementBase2 = CharacterManager.Current.GetElements().Single((ElementBase x) => x.Id == rule.ElementHeader.Id);
                        list6.Add(elementBase);
                        if ((!rule.ElementHeader.Name.StartsWith("Ability Score Increase") && rule.ElementHeader.Type != "Race") || (!rule.ElementHeader.Id.StartsWith("ID_CLASS_FEATURE_FEAT_") && rule.ElementHeader.Type != "Class Feature"))
                        {
                            if (elementBase2.SheetDescription.DisplayOnSheet)
                            {
                                list7.Add(elementBase);
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
            foreach (ElementBase element in list6)
            {
                int num = _manager.Character.Level;
                ClassProgressionManager classProgressionManager = _manager.ClassProgressionManagers.FirstOrDefault((ClassProgressionManager x) => x.GetElements().Contains(element));
                if (classProgressionManager != null)
                {
                    num = classProgressionManager.ProgressionLevel;
                }
                if (!element.SheetDescription.DisplayOnSheet)
                {
                    continue;
                }
                string text5 = "";
                foreach (ElementSheetDescriptions.SheetDescription item3 in element.SheetDescription.OrderBy((ElementSheetDescriptions.SheetDescription x) => x.Level))
                {
                    if (item3.Level <= num)
                    {
                        text5 = item3.Description;
                    }
                }
                string pattern = "\\$\\((.*?)\\)";
                foreach (Match item4 in Regex.Matches(text5, pattern))
                {
                    string value2 = item4.Value;
                    string text6 = item4.Value.Substring(2, item4.Value.Length - 3);
                    string newValue = "==UNKNOWN VALUE==";
                    if (_manager.StatisticsCalculator.StatisticValues.ContainsGroup(text6))
                    {
                        newValue = _manager.StatisticsCalculator.StatisticValues.GetValue(text6).ToString();
                    }
                    else if (_manager.StatisticsCalculator.InlineValues.ContainsKey(text6))
                    {
                        newValue = _manager.StatisticsCalculator.InlineValues[text6];
                    }
                    else
                    {
                        Logger.Warning($"UNKNOWN REPLACE STRING: {value2} in {element}");
                    }
                    text5 = text5.Replace(value2, newValue);
                }
                if (text5.Contains("%"))
                {
                    while (text5.Contains("%"))
                    {
                        int num2 = text5.IndexOf("%", StringComparison.Ordinal);
                        string text7 = text5.Substring(num2 + 1, text5.Length - num2 - 1);
                        int num3 = num2 + text7.IndexOf("%", StringComparison.Ordinal);
                        string text8 = text5.Substring(num2, num3 - num2 + 2);
                        string newValue2 = "UNKNOWN VALUE";
                        if (_manager.StatisticsCalculator.InlineValues.ContainsKey(text8.Replace("%", "")))
                        {
                            newValue2 = _manager.StatisticsCalculator.InlineValues[text8.Replace("%", "")];
                        }
                        else if (_manager.StatisticsCalculator.StatisticValues.ContainsGroup(text8.Replace("%", "")))
                        {
                            newValue2 = _manager.StatisticsCalculator.StatisticValues.GetValue(text8.Replace("%", "")).ToString();
                        }
                        else
                        {
                            Logger.Warning($"UNKNOWN REPLACESTRING:{text8} in {element}");
                        }
                        text5 = text5.Replace(text8, newValue2);
                    }
                }
                bool flag = false;
                if (mainSheetContent.FeaturesFieldContent.Lines.Any())
                {
                    flag = mainSheetContent.FeaturesFieldContent.Lines.Last().Indent;
                }
                string name = (element.SheetDescription.HasAlternateName ? element.SheetDescription.AlternateName : element.Name);
                mainSheetContent.FeaturesFieldContent.Lines.Add(new ContentLine(name, text5, flag || !list7.Contains(element), list7.Contains(element)));
            }
            if (!configuration.IncludeFormatting)
            {
                StringBuilder stringBuilder3 = new StringBuilder();
                foreach (ContentLine line2 in mainSheetContent.FeaturesFieldContent.Lines)
                {
                    if (line2.NewLineBefore)
                    {
                        stringBuilder3.AppendLine();
                    }
                    stringBuilder3.AppendLine((line2.Indent ? "    " : "") + line2.Name + ". " + line2.Content);
                }
                mainSheetContent.FeaturesAndTraitsField = stringBuilder3.ToString().Trim();
            }
            return mainSheetContent;
        }

        private DetailsSheetContent GenerateDetailsSheetContent(Character character)
        {
            DetailsSheetContent detailsSheetContent = new DetailsSheetContent();
            detailsSheetContent.CharacterName = character.Name;
            detailsSheetContent.Age = character.AgeField.Content;
            detailsSheetContent.Height = character.HeightField.Content;
            detailsSheetContent.Weight = character.WeightField.Content;
            detailsSheetContent.Eyes = character.Eyes;
            detailsSheetContent.Skin = character.Skin;
            detailsSheetContent.Hair = character.Hair;
            detailsSheetContent.CharacterAppearance = character.PortraitFilename;
            detailsSheetContent.AlliesAndOrganizations = character.Allies;
            detailsSheetContent.OrganizationName = character.OrganisationName;
            detailsSheetContent.OrganizationSymbol = character.OrganisationSymbol;
            detailsSheetContent.AdditionalFeaturesAndTraits = character.AdditionalFeatures;
            detailsSheetContent.CharacterBackstory = character.BackgroundStory.Content;
            detailsSheetContent.Treasure = character.Inventory.Treasure;
            detailsSheetContent.Trinket = character.Trinket.ToString();
            ElementBase elementBase = CharacterManager.Current.GetElements().FirstOrDefault((ElementBase x) => x.Type.Equals("Background Feature"));
            if (elementBase != null)
            {
                detailsSheetContent.BackgroundFeatureName = (elementBase.SheetDescription.HasAlternateName ? elementBase.SheetDescription.AlternateName : elementBase.Name);
                detailsSheetContent.BackgroundFeature = (elementBase.SheetDescription.Any() ? elementBase.SheetDescription[0].Description : ElementDescriptionGenerator.GeneratePlainDescription(elementBase.Description));
            }
            return detailsSheetContent;
        }

        private SpellcastingSheetContent GenerateSpellcastingSheetContent(SpellcastingInformation information)
        {
            //SpellcasterSelectionControlViewModel spellcasterSectionViewModel = SpellcastingSectionHandler.Current.GetSpellcasterSectionViewModel(information.UniqueIdentifier);
            // IOrderedEnumerable<SelectionElement> spells = SpellcastingSectionHandler.Current.GetSpells(information);
            SpellcastingSheetContent spellcastingSheetContent = new SpellcastingSheetContent();
            spellcastingSheetContent.SpellcastingClass = information.Name;
            spellcastingSheetContent.SpellcastingAbility = information.AbilityName;
            //spellcastingSheetContent.SpellcastingAttackModifier = spellcasterSectionViewModel.InformationHeader.SpellAttackModifier.ToString();
            //spellcastingSheetContent.SpellcastingSave = spellcasterSectionViewModel.InformationHeader.SpellSaveDc.ToString();
            //spellcastingSheetContent.Spells1.SlotsCount = spellcasterSectionViewModel.InformationHeader.Slot1;
            //spellcastingSheetContent.Spells2.SlotsCount = spellcasterSectionViewModel.InformationHeader.Slot2;
            //spellcastingSheetContent.Spells3.SlotsCount = spellcasterSectionViewModel.InformationHeader.Slot3;
            //spellcastingSheetContent.Spells4.SlotsCount = spellcasterSectionViewModel.InformationHeader.Slot4;
            //spellcastingSheetContent.Spells5.SlotsCount = spellcasterSectionViewModel.InformationHeader.Slot5;
            //spellcastingSheetContent.Spells6.SlotsCount = spellcasterSectionViewModel.InformationHeader.Slot6;
            //spellcastingSheetContent.Spells7.SlotsCount = spellcasterSectionViewModel.InformationHeader.Slot7;
            //spellcastingSheetContent.Spells8.SlotsCount = spellcasterSectionViewModel.InformationHeader.Slot8;
            //spellcastingSheetContent.Spells9.SlotsCount = spellcasterSectionViewModel.InformationHeader.Slot9;
            SpellcastingSheetContent spellcastingSheetContent2 = spellcastingSheetContent;
            if (information.Prepare)
            {
                string prepareAmountStatisticName = information.GetPrepareAmountStatisticName();
                spellcastingSheetContent2.SpellcastingPrepareCount = _manager.StatisticsCalculator.StatisticValues.GetValue(prepareAmountStatisticName).ToString();
            }
            else
            {
                spellcastingSheetContent2.SpellcastingPrepareCount = "N/A";
            }
            spellcastingSheetContent2.SpellcastingNotes = "implement spellcasting notes";
            //foreach (SelectionElement item in spells)
            //{
            //    Spell spell = item.Element.AsElement<Spell>();
            //    switch (spell.Level)
            //    {
            //        case 0:
            //            spellcastingSheetContent2.Cantrips.Collection.Add(new SpellcastingSpellContent(spell.Name, isPrepared: true));
            //            break;
            //        case 1:
            //            spellcastingSheetContent2.Spells1.Collection.Add(new SpellcastingSpellContent(spell.Name, item.IsChosen));
            //            break;
            //        case 2:
            //            spellcastingSheetContent2.Spells2.Collection.Add(new SpellcastingSpellContent(spell.Name, item.IsChosen));
            //            break;
            //        case 3:
            //            spellcastingSheetContent2.Spells3.Collection.Add(new SpellcastingSpellContent(spell.Name, item.IsChosen));
            //            break;
            //        case 4:
            //            spellcastingSheetContent2.Spells4.Collection.Add(new SpellcastingSpellContent(spell.Name, item.IsChosen));
            //            break;
            //        case 5:
            //            spellcastingSheetContent2.Spells5.Collection.Add(new SpellcastingSpellContent(spell.Name, item.IsChosen));
            //            break;
            //        case 6:
            //            spellcastingSheetContent2.Spells6.Collection.Add(new SpellcastingSpellContent(spell.Name, item.IsChosen));
            //            break;
            //        case 7:
            //            spellcastingSheetContent2.Spells7.Collection.Add(new SpellcastingSpellContent(spell.Name, item.IsChosen));
            //            break;
            //        case 8:
            //            spellcastingSheetContent2.Spells8.Collection.Add(new SpellcastingSpellContent(spell.Name, item.IsChosen));
            //            break;
            //        case 9:
            //            spellcastingSheetContent2.Spells9.Collection.Add(new SpellcastingSpellContent(spell.Name, item.IsChosen));
            //            break;
            //    }
            //}
            return spellcastingSheetContent2;
        }

        private CharacterSheetExportContent GenerateExportContent(CharacterManager manager, CharacterSheetConfiguration configuration)
        {
            CharacterSheetExportContent characterSheetExportContent = new CharacterSheetExportContent();
            ElementBaseCollection elements = manager.GetElements();
            Character character = manager.Character;
            CharacterInventory inventory = manager.Character.Inventory;
            StatisticValuesGroupCollection statisticValues = manager.StatisticsCalculator.StatisticValues;
            AuroraStatisticStrings names = manager.StatisticsCalculator.Names;
            CharacterStatus status = manager.Status;
            characterSheetExportContent.PlayerName = character.PlayerName;
            characterSheetExportContent.CharacterName = character.Name;
            characterSheetExportContent.Race = character.Race;
            ElementBase elementBase = elements.FirstOrDefault((ElementBase x) => x.Type.Equals("Race Variant"));
            if (elementBase != null)
            {
                characterSheetExportContent.Race = elementBase.GetAlternateName();
            }
            characterSheetExportContent.Alignment = character.Alignment;
            characterSheetExportContent.Deity = character.Deity;
            characterSheetExportContent.Experience = character.Experience.ToString();
            characterSheetExportContent.ProficiencyBonus = character.Proficiency.ToValueString();
            characterSheetExportContent.Level = character.Level.ToString();
            characterSheetExportContent.AttacksCount = statisticValues.GetGroup("extra attack:count", createNonExisting: false)?.Sum().ToString() ?? "1";
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            foreach (ClassProgressionManager classProgressionManager2 in manager.ClassProgressionManagers)
            {
                if (classProgressionManager2.IsMainClass)
                {
                    characterSheetExportContent.MainClass.Level = classProgressionManager2.ProgressionLevel.ToString();
                    characterSheetExportContent.MainClass.Name = classProgressionManager2.ClassElement.GetAlternateName();
                    characterSheetExportContent.MainClass.Archetype = classProgressionManager2.Elements.FirstOrDefault((ElementBase x) => x.Type.Equals("Archetype"))?.GetAlternateName();
                    characterSheetExportContent.MainClass.HitDie = classProgressionManager2.HD;
                    if (dictionary.ContainsKey(classProgressionManager2.HD))
                    {
                        dictionary[classProgressionManager2.HD] += classProgressionManager2.ProgressionLevel;
                    }
                    else
                    {
                        dictionary.Add(classProgressionManager2.HD, classProgressionManager2.ProgressionLevel);
                    }
                    continue;
                }
                CharacterSheetExportContent.ClassExportContent item = new CharacterSheetExportContent.ClassExportContent
                {
                    Level = classProgressionManager2.ProgressionLevel.ToString(),
                    Name = classProgressionManager2.ClassElement.GetAlternateName(),
                    Archetype = classProgressionManager2.Elements.FirstOrDefault((ElementBase x) => x.Type.Equals("Archetype"))?.GetAlternateName(),
                    HitDie = classProgressionManager2.HD
                };
                characterSheetExportContent.Multiclass.Add(item);
                if (dictionary.ContainsKey(classProgressionManager2.HD))
                {
                    dictionary[classProgressionManager2.HD] += classProgressionManager2.ProgressionLevel;
                }
                else
                {
                    dictionary.Add(classProgressionManager2.HD, classProgressionManager2.ProgressionLevel);
                }
            }
            characterSheetExportContent.IsMulticlass = status.HasMulticlass;
            characterSheetExportContent.AbilitiesContent.Strength = character.Abilities.Strength.FinalScore.ToString();
            characterSheetExportContent.AbilitiesContent.Dexterity = character.Abilities.Dexterity.FinalScore.ToString();
            characterSheetExportContent.AbilitiesContent.Constitution = character.Abilities.Constitution.FinalScore.ToString();
            characterSheetExportContent.AbilitiesContent.Intelligence = character.Abilities.Intelligence.FinalScore.ToString();
            characterSheetExportContent.AbilitiesContent.Wisdom = character.Abilities.Wisdom.FinalScore.ToString();
            characterSheetExportContent.AbilitiesContent.Charisma = character.Abilities.Charisma.FinalScore.ToString();
            characterSheetExportContent.AbilitiesContent.StrengthModifier = character.Abilities.Strength.ModifierString;
            characterSheetExportContent.AbilitiesContent.DexterityModifier = character.Abilities.Dexterity.ModifierString;
            characterSheetExportContent.AbilitiesContent.ConstitutionModifier = character.Abilities.Constitution.ModifierString;
            characterSheetExportContent.AbilitiesContent.IntelligenceModifier = character.Abilities.Intelligence.ModifierString;
            characterSheetExportContent.AbilitiesContent.WisdomModifier = character.Abilities.Wisdom.ModifierString;
            characterSheetExportContent.AbilitiesContent.CharismaModifier = character.Abilities.Charisma.ModifierString;
            characterSheetExportContent.AbilitiesContent.StrengthSave = character.SavingThrows.Strength.FinalBonus.ToValueString();
            characterSheetExportContent.AbilitiesContent.DexteritySave = character.SavingThrows.Dexterity.FinalBonus.ToValueString();
            characterSheetExportContent.AbilitiesContent.ConstitutionSave = character.SavingThrows.Constitution.FinalBonus.ToValueString();
            characterSheetExportContent.AbilitiesContent.IntelligenceSave = character.SavingThrows.Intelligence.FinalBonus.ToValueString();
            characterSheetExportContent.AbilitiesContent.WisdomSave = character.SavingThrows.Wisdom.FinalBonus.ToValueString();
            characterSheetExportContent.AbilitiesContent.CharismaSave = character.SavingThrows.Charisma.FinalBonus.ToValueString();
            characterSheetExportContent.AbilitiesContent.StrengthSaveProficient = character.SavingThrows.Strength.IsProficient;
            characterSheetExportContent.AbilitiesContent.DexteritySaveProficient = character.SavingThrows.Dexterity.IsProficient;
            characterSheetExportContent.AbilitiesContent.ConstitutionSaveProficient = character.SavingThrows.Constitution.IsProficient;
            characterSheetExportContent.AbilitiesContent.IntelligenceSaveProficient = character.SavingThrows.Intelligence.IsProficient;
            characterSheetExportContent.AbilitiesContent.WisdomSaveProficient = character.SavingThrows.Wisdom.IsProficient;
            characterSheetExportContent.AbilitiesContent.CharismaSaveProficient = character.SavingThrows.Charisma.IsProficient;
            characterSheetExportContent.SkillsContent.Acrobatics = character.Skills.Acrobatics.FinalBonus.ToValueString();
            characterSheetExportContent.SkillsContent.AnimalHandling = character.Skills.AnimalHandling.FinalBonus.ToValueString();
            characterSheetExportContent.SkillsContent.Arcana = character.Skills.Arcana.FinalBonus.ToValueString();
            characterSheetExportContent.SkillsContent.Athletics = character.Skills.Athletics.FinalBonus.ToValueString();
            characterSheetExportContent.SkillsContent.Deception = character.Skills.Deception.FinalBonus.ToValueString();
            characterSheetExportContent.SkillsContent.History = character.Skills.History.FinalBonus.ToValueString();
            characterSheetExportContent.SkillsContent.Insight = character.Skills.Insight.FinalBonus.ToValueString();
            characterSheetExportContent.SkillsContent.Intimidation = character.Skills.Intimidation.FinalBonus.ToValueString();
            characterSheetExportContent.SkillsContent.Investigation = character.Skills.Investigation.FinalBonus.ToValueString();
            characterSheetExportContent.SkillsContent.Medicine = character.Skills.Medicine.FinalBonus.ToValueString();
            characterSheetExportContent.SkillsContent.Nature = character.Skills.Nature.FinalBonus.ToValueString();
            characterSheetExportContent.SkillsContent.Perception = character.Skills.Perception.FinalBonus.ToValueString();
            characterSheetExportContent.SkillsContent.Performance = character.Skills.Performance.FinalBonus.ToValueString();
            characterSheetExportContent.SkillsContent.Persuasion = character.Skills.Persuasion.FinalBonus.ToValueString();
            characterSheetExportContent.SkillsContent.Religion = character.Skills.Religion.FinalBonus.ToValueString();
            characterSheetExportContent.SkillsContent.SleightOfHand = character.Skills.SleightOfHand.FinalBonus.ToValueString();
            characterSheetExportContent.SkillsContent.Stealth = character.Skills.Stealth.FinalBonus.ToValueString();
            characterSheetExportContent.SkillsContent.Survival = character.Skills.Survival.FinalBonus.ToValueString();
            characterSheetExportContent.SkillsContent.AcrobaticsProficient = character.Skills.Acrobatics.IsProficient;
            characterSheetExportContent.SkillsContent.AnimalHandlingProficient = character.Skills.AnimalHandling.IsProficient;
            characterSheetExportContent.SkillsContent.ArcanaProficient = character.Skills.Arcana.IsProficient;
            characterSheetExportContent.SkillsContent.AthleticsProficient = character.Skills.Athletics.IsProficient;
            characterSheetExportContent.SkillsContent.DeceptionProficient = character.Skills.Deception.IsProficient;
            characterSheetExportContent.SkillsContent.HistoryProficient = character.Skills.History.IsProficient;
            characterSheetExportContent.SkillsContent.InsightProficient = character.Skills.Insight.IsProficient;
            characterSheetExportContent.SkillsContent.IntimidationProficient = character.Skills.Intimidation.IsProficient;
            characterSheetExportContent.SkillsContent.InvestigationProficient = character.Skills.Investigation.IsProficient;
            characterSheetExportContent.SkillsContent.MedicineProficient = character.Skills.Medicine.IsProficient;
            characterSheetExportContent.SkillsContent.NatureProficient = character.Skills.Nature.IsProficient;
            characterSheetExportContent.SkillsContent.PerceptionProficient = character.Skills.Perception.IsProficient;
            characterSheetExportContent.SkillsContent.PerformanceProficient = character.Skills.Performance.IsProficient;
            characterSheetExportContent.SkillsContent.PersuasionProficient = character.Skills.Persuasion.IsProficient;
            characterSheetExportContent.SkillsContent.ReligionProficient = character.Skills.Religion.IsProficient;
            characterSheetExportContent.SkillsContent.SleightOfHandProficient = character.Skills.SleightOfHand.IsProficient;
            characterSheetExportContent.SkillsContent.StealthProficient = character.Skills.Stealth.IsProficient;
            characterSheetExportContent.SkillsContent.SurvivalProficient = character.Skills.Survival.IsProficient;
            characterSheetExportContent.SkillsContent.AcrobaticsExpertise = character.Skills.Acrobatics.IsExpertise(character.Proficiency);
            characterSheetExportContent.SkillsContent.AnimalHandlingExpertise = character.Skills.AnimalHandling.IsExpertise(character.Proficiency);
            characterSheetExportContent.SkillsContent.ArcanaExpertise = character.Skills.Arcana.IsExpertise(character.Proficiency);
            characterSheetExportContent.SkillsContent.AthleticsExpertise = character.Skills.Athletics.IsExpertise(character.Proficiency);
            characterSheetExportContent.SkillsContent.DeceptionExpertise = character.Skills.Deception.IsExpertise(character.Proficiency);
            characterSheetExportContent.SkillsContent.HistoryExpertise = character.Skills.History.IsExpertise(character.Proficiency);
            characterSheetExportContent.SkillsContent.InsightExpertise = character.Skills.Insight.IsExpertise(character.Proficiency);
            characterSheetExportContent.SkillsContent.IntimidationExpertise = character.Skills.Intimidation.IsExpertise(character.Proficiency);
            characterSheetExportContent.SkillsContent.InvestigationExpertise = character.Skills.Investigation.IsExpertise(character.Proficiency);
            characterSheetExportContent.SkillsContent.MedicineExpertise = character.Skills.Medicine.IsExpertise(character.Proficiency);
            characterSheetExportContent.SkillsContent.NatureExpertise = character.Skills.Nature.IsExpertise(character.Proficiency);
            characterSheetExportContent.SkillsContent.PerceptionExpertise = character.Skills.Perception.IsExpertise(character.Proficiency);
            characterSheetExportContent.SkillsContent.PerformanceExpertise = character.Skills.Performance.IsExpertise(character.Proficiency);
            characterSheetExportContent.SkillsContent.PersuasionExpertise = character.Skills.Persuasion.IsExpertise(character.Proficiency);
            characterSheetExportContent.SkillsContent.ReligionExpertise = character.Skills.Religion.IsExpertise(character.Proficiency);
            characterSheetExportContent.SkillsContent.SleightOfHandExpertise = character.Skills.SleightOfHand.IsExpertise(character.Proficiency);
            characterSheetExportContent.SkillsContent.StealthExpertise = character.Skills.Stealth.IsExpertise(character.Proficiency);
            characterSheetExportContent.SkillsContent.SurvivalExpertise = character.Skills.Survival.IsExpertise(character.Proficiency);
            characterSheetExportContent.SkillsContent.PerceptionPassive = $"{statisticValues.GetValue(names.PerceptionPassive) + character.Skills.Perception.FinalBonus}";
            characterSheetExportContent.ArmorClassContent.ArmorClass = character.ArmorClass.ToString();
            characterSheetExportContent.ArmorClassContent.StealthDisadvantage = elements.HasElement(InternalGrants.StealthDisadvantage);
            if (inventory.EquippedArmor != null)
            {
                characterSheetExportContent.ArmorClassContent.EquippedArmor = inventory.EquippedArmor.ToString();
                characterSheetExportContent.ArmorClassContent.ConditionalArmorClass = statisticValues.GetGroup("ac:misc").GetSummery();
            }
            else if (statisticValues.ContainsGroup("ac:calculation"))
            {
                StatisticValuesGroup group = statisticValues.GetGroup("ac:calculation");
                characterSheetExportContent.ArmorClassContent.EquippedArmor = group.GetSummery();
                characterSheetExportContent.ArmorClassContent.ConditionalArmorClass = statisticValues.GetGroup("ac:misc").GetSummery();
            }
            else
            {
                characterSheetExportContent.ArmorClassContent.ConditionalArmorClass = statisticValues.GetGroup("ac").GetSummery();
            }
            if (inventory.EquippedSecondary != null && inventory.IsEquippedShield())
            {
                characterSheetExportContent.ArmorClassContent.EquippedShield = inventory.EquippedSecondary.ToString();
            }
            if (!string.IsNullOrWhiteSpace(characterSheetExportContent.ArmorClassContent.ConditionalArmorClass))
            {
                characterSheetExportContent.ArmorClassContent.ConditionalArmorClass += Environment.NewLine;
            }
            characterSheetExportContent.ArmorClassContent.ConditionalArmorClass += character.ConditionalArmorClassField.ToString().TrimEnd();
            characterSheetExportContent.HitPointsContent.Maximum = character.MaxHp.ToString();
            characterSheetExportContent.HitPointsContent.Current = "";
            characterSheetExportContent.HitPointsContent.Temporary = "";
            characterSheetExportContent.HitPointsContent.DeathSavingThrowFailure1 = false;
            characterSheetExportContent.HitPointsContent.DeathSavingThrowFailure2 = false;
            characterSheetExportContent.HitPointsContent.DeathSavingThrowFailure3 = false;
            characterSheetExportContent.HitPointsContent.DeathSavingThrowSuccess1 = false;
            characterSheetExportContent.HitPointsContent.DeathSavingThrowSuccess2 = false;
            characterSheetExportContent.HitPointsContent.DeathSavingThrowSuccess3 = false;
            characterSheetExportContent.HitPointsContent.HitDice = string.Join("/", dictionary.Select((KeyValuePair<string, int> x) => $"{x.Value}{x.Key}"));
            characterSheetExportContent.ConditionsContent.WalkingSpeed = statisticValues.GetValue(names.Speed).ToString();
            characterSheetExportContent.ConditionsContent.FlySpeed = statisticValues.GetValue(names.SpeedFly).ToString();
            characterSheetExportContent.ConditionsContent.ClimbSpeed = statisticValues.GetValue(names.SpeedClimb).ToString();
            characterSheetExportContent.ConditionsContent.SwimSpeed = statisticValues.GetValue(names.SpeedSwim).ToString();
            characterSheetExportContent.ConditionsContent.BurrowSpeed = statisticValues.GetValue(names.SpeedBurrow).ToString();
            IEnumerable<ElementBase> source = elements.Where((ElementBase x) => x.Type.Equals("Vision")).Distinct();
            characterSheetExportContent.ConditionsContent.Vision = string.Join(", ", source.Select((ElementBase x) => x.Name));
            characterSheetExportContent.ConditionsContent.Exhaustion = "";
            List<ElementBase> source2 = (from x in elements
                                         where x.Type.Equals("Condition")
                                         orderby x.Name
                                         select x).Distinct().ToList();
            List<ElementBase> source3 = source2.Where((ElementBase x) => x.Supports.Contains("Resistance")).ToList();
            List<ElementBase> source4 = source2.Where((ElementBase x) => x.Supports.Contains("Vulnerability")).ToList();
            List<ElementBase> source5 = source2.Where((ElementBase x) => x.Supports.Contains("Immunity")).ToList();
            StringBuilder stringBuilder = new StringBuilder();
            if (source3.Any())
            {
                stringBuilder.AppendLine("Resistances. " + string.Join(", ", source3.Select((ElementBase x) => x.Name.Replace("Resistance", "").Replace("(", "").Replace(")", "")
                    .Trim())));
            }
            if (source4.Any())
            {
                stringBuilder.AppendLine("Vulnerabilities. " + string.Join(", ", source4.Select((ElementBase x) => x.Name.Replace("Vulnerability", "").Replace("(", "").Replace(")", "")
                    .Trim())));
            }
            if (source5.Any())
            {
                stringBuilder.AppendLine("Immunities. " + string.Join(", ", source5.Select((ElementBase x) => x.Name.Replace("Immunity", "").Replace("(", "").Replace(")", "")
                    .Trim())));
            }
            characterSheetExportContent.ConditionsContent.Resistances = stringBuilder.ToString();
            characterSheetExportContent.EquipmentContent.Copper = inventory.Coins.Copper.ToString();
            characterSheetExportContent.EquipmentContent.Silver = inventory.Coins.Silver.ToString();
            characterSheetExportContent.EquipmentContent.Electrum = inventory.Coins.Electrum.ToString();
            characterSheetExportContent.EquipmentContent.Gold = inventory.Coins.Gold.ToString();
            characterSheetExportContent.EquipmentContent.Platinum = inventory.Coins.Platinum.ToString();
            characterSheetExportContent.EquipmentContent.Weight = inventory.EquipmentWeight.ToString();
            foreach (RefactoredEquipmentItem item3 in inventory.Items)
            {
                string item2 = ((!string.IsNullOrWhiteSpace(item3.AlternativeName)) ? item3.AlternativeName : item3.DisplayName);
                characterSheetExportContent.EquipmentContent.Equipment.Add(new Tuple<string, string>(item3.Amount.ToString(), item2));
            }
            characterSheetExportContent.Initiative = character.Initiative.ToValueString();
            characterSheetExportContent.InitiativeAdvantage = elements.HasElement(InternalGrants.InitiativeAdvantage);
            foreach (AttackSectionItem item4 in character.AttacksSection.Items)
            {
                if (item4.IsDisplayed)
                {
                    CharacterSheetExportContent.AttackExportContent attackExportContent = new CharacterSheetExportContent.AttackExportContent();
                    attackExportContent.Name = item4.Name.Content;
                    attackExportContent.Range = item4.Range.Content;
                    attackExportContent.Bonus = item4.Attack.Content;
                    attackExportContent.Damage = item4.Damage.Content;
                    attackExportContent.Description = item4.Description.Content;
                    attackExportContent.Underline = item4.EquipmentItem?.GetUnderline();
                    attackExportContent.AttackBreakdown = item4.DisplayCalculatedAttack;
                    attackExportContent.DamageBreakdown = item4.DisplayCalculatedDamage;
                    attackExportContent.AsCard = item4.IsDisplayedAsCard;
                    characterSheetExportContent.AttacksContent.Add(attackExportContent);
                }
            }
            characterSheetExportContent.AttackAndSpellcastingField = character.AttacksSection.AttacksAndSpellcasting;
            ElementsOrganizer elementsOrganizer = new ElementsOrganizer(elements);
            List<Language> list = elementsOrganizer.GetLanguages(includeDuplicates: false).ToList();
            List<ElementBase> list2 = manager.GetProficiencyList(elementsOrganizer.GetArmorProficiencies(includeDuplicates: false)).ToList();
            List<ElementBase> list3 = manager.GetProficiencyList(elementsOrganizer.GetWeaponProficiencies(includeDuplicates: false)).ToList();
            List<ElementBase> list4 = manager.GetProficiencyList(elementsOrganizer.GetToolProficiencies(includeDuplicates: false)).ToList();
            if (list.Count > 0)
            {
                characterSheetExportContent.Languages = string.Join(", ", list.Select((Language x) => x.Name));
            }
            else
            {
                characterSheetExportContent.Languages = "–";
            }
            if (list2.Count > 0)
            {
                IEnumerable<string> values = list2.Select((ElementBase x) => x.Name.Replace("Armor Proficiency", "").Replace("(", "").Replace(")", "")
                    .Trim());
                characterSheetExportContent.ArmorProficiencies = string.Join(", ", values);
            }
            else
            {
                characterSheetExportContent.ArmorProficiencies = "–";
            }
            if (list3.Count > 0)
            {
                IEnumerable<string> values2 = list3.Select((ElementBase x) => x.Name.Replace("Weapon Proficiency", "").Replace("(", "").Replace(")", "")
                    .Trim());
                characterSheetExportContent.WeaponProficiencies = string.Join(", ", values2);
            }
            else
            {
                characterSheetExportContent.WeaponProficiencies = "–";
            }
            if (list4.Count > 0)
            {
                IEnumerable<string> values3 = list4.Select((ElementBase x) => x.Name.Replace("Tool Proficiency", "").Replace("(", "").Replace(")", "")
                    .Trim());
                characterSheetExportContent.ToolProficiencies = string.Join(", ", values3);
            }
            else
            {
                characterSheetExportContent.ToolProficiencies = "–";
            }
            ContentField contentField = new ContentField();
            List<ElementBase> list5 = new List<ElementBase>();
            foreach (ElementBase element in elementsOrganizer.GetSortedFeaturesExcludingRacialTraits(list5))
            {
                int num = character.Level;
                ClassProgressionManager classProgressionManager = manager.ClassProgressionManagers.FirstOrDefault((ClassProgressionManager x) => x.GetElements().Contains(element));
                if (classProgressionManager != null)
                {
                    num = classProgressionManager.ProgressionLevel;
                }
                if (!element.SheetDescription.DisplayOnSheet)
                {
                    continue;
                }
                string input = "";
                string input2 = (element.SheetDescription.HasUsage ? element.SheetDescription.Usage : "");
                string input3 = (element.SheetDescription.HasAction ? element.SheetDescription.Action : "");
                foreach (ElementSheetDescriptions.SheetDescription item5 in element.SheetDescription.OrderBy((ElementSheetDescriptions.SheetDescription x) => x.Level))
                {
                    if (item5.Level <= num)
                    {
                        input = item5.Description;
                        if (item5.HasUsage)
                        {
                            input2 = item5.Usage;
                        }
                        if (item5.HasAction)
                        {
                            input3 = item5.Action;
                        }
                    }
                }
                input = manager.StatisticsCalculator.ReplaceInline(input);
                input2 = manager.StatisticsCalculator.ReplaceInline(input2);
                input3 = manager.StatisticsCalculator.ReplaceInline(input3);
                bool flag = false;
                if (contentField.Lines.Any())
                {
                    flag = contentField.Lines.Last().Indent;
                }
                string text = "";
                if (!string.IsNullOrWhiteSpace(input3))
                {
                    text = "(" + input3 + ((!string.IsNullOrWhiteSpace(input2)) ? ("—" + input2) : "") + ")";
                }
                else if (!string.IsNullOrWhiteSpace(input2))
                {
                    text = "(" + input2 + ")";
                }
                string text2 = element.GetAlternateName();
                if (Settings.Default.SheetFormattingActionSuffixBold)
                {
                    text2 = (string.IsNullOrWhiteSpace(text) ? text2 : (text2 + " " + text));
                }
                else
                {
                    input = (string.IsNullOrWhiteSpace(text) ? input : (text + " " + input));
                }
                contentField.Lines.Add(new ContentLine(text2.Trim(), input, flag || !list5.Contains(element), list5.Contains(element)));
            }
            if (configuration.IncludeFormatting)
            {
                StringBuilder stringBuilder2 = new StringBuilder();
                foreach (ContentLine line in contentField.Lines)
                {
                    if ((!line.NewLineBefore || !line.Indent) && line.NewLineBefore && !string.IsNullOrWhiteSpace(stringBuilder2.ToString()))
                    {
                        stringBuilder2.Append("<p>&nbsp;</p>");
                    }
                    string text3 = line.Content.Replace(Environment.NewLine, "<br>&nbsp;  &nbsp;");
                    stringBuilder2.Append("<p>" + (line.Indent ? "&nbsp;    &nbsp;" : "") + "<strong><em>" + line.Name + ".</em></strong> " + text3 + "</p>");
                }
                characterSheetExportContent.Features = stringBuilder2.ToString().Trim();
            }
            else
            {
                StringBuilder stringBuilder3 = new StringBuilder();
                foreach (ContentLine line2 in contentField.Lines)
                {
                    if (line2.NewLineBefore)
                    {
                        stringBuilder3.AppendLine();
                    }
                    stringBuilder3.AppendLine((line2.Indent ? "    " : "") + line2.Name + ". " + line2.Content);
                }
                characterSheetExportContent.Features = stringBuilder3.ToString().Trim();
            }
            GenerateTemporaryRacialTraitsExportContent(configuration, elementsOrganizer, character, manager, characterSheetExportContent);
            characterSheetExportContent.BackgroundContent.Name = character.Background;
            ElementBase elementBase2 = elements.FirstOrDefault((ElementBase x) => x.Type.Equals("Background Variant"));
            if (elementBase2 != null)
            {
                characterSheetExportContent.BackgroundContent.Name = elementBase2.GetAlternateName();
            }
            characterSheetExportContent.BackgroundContent.PersonalityTrait = character.FillableBackgroundCharacteristics.Traits.Content;
            characterSheetExportContent.BackgroundContent.Ideal = character.FillableBackgroundCharacteristics.Ideals.Content;
            characterSheetExportContent.BackgroundContent.Bond = character.FillableBackgroundCharacteristics.Bonds.Content;
            characterSheetExportContent.BackgroundContent.Flaw = character.FillableBackgroundCharacteristics.Flaws.Content;
            characterSheetExportContent.BackgroundContent.Trinket = character.Trinket.Content;
            characterSheetExportContent.BackgroundContent.Story = character.BackgroundStory.Content;
            characterSheetExportContent.BackgroundContent.FeatureName = character.BackgroundFeatureName.ToString();
            characterSheetExportContent.BackgroundContent.FeatureDescription = character.BackgroundFeatureDescription.ToString();
            characterSheetExportContent.AppearanceContent.Portrait = character.PortraitFilename;
            characterSheetExportContent.AppearanceContent.Gender = character.Gender;
            characterSheetExportContent.AppearanceContent.Age = character.AgeField.Content;
            characterSheetExportContent.AppearanceContent.Height = character.HeightField.Content;
            characterSheetExportContent.AppearanceContent.Weight = character.WeightField.Content;
            characterSheetExportContent.AppearanceContent.Eyes = character.Eyes;
            characterSheetExportContent.AppearanceContent.Skin = character.Skin;
            characterSheetExportContent.AppearanceContent.Hair = character.Hair;
            characterSheetExportContent.AlliesAndOrganizations = character.Allies;
            characterSheetExportContent.OrganizationName = character.OrganisationName;
            characterSheetExportContent.OrganizationSymbol = character.OrganisationSymbol;
            characterSheetExportContent.AdditionalFeaturesAndTraits = character.AdditionalFeatures;
            characterSheetExportContent.EquipmentContent.AdditionalTreasure = character.Inventory.Treasure;
            characterSheetExportContent.Treasure = character.Inventory.Treasure;
            return characterSheetExportContent;
        }

        private void GenerateTemporaryRacialTraitsExportContent(CharacterSheetConfiguration configuration, ElementsOrganizer organizer, Character character, CharacterManager manager, CharacterSheetExportContent export)
        {
            ContentField contentField = new ContentField();
            List<ElementBase> list = new List<ElementBase>();
            foreach (ElementBase element in organizer.GetSortedRacialTraits(list))
            {
                int num = character.Level;
                ClassProgressionManager classProgressionManager = manager.ClassProgressionManagers.FirstOrDefault((ClassProgressionManager x) => x.GetElements().Contains(element));
                if (classProgressionManager != null)
                {
                    num = classProgressionManager.ProgressionLevel;
                }
                if (!element.SheetDescription.DisplayOnSheet)
                {
                    continue;
                }
                string input = "";
                string input2 = (element.SheetDescription.HasUsage ? element.SheetDescription.Usage : "");
                string input3 = (element.SheetDescription.HasAction ? element.SheetDescription.Action : "");
                foreach (ElementSheetDescriptions.SheetDescription item in element.SheetDescription.OrderBy((ElementSheetDescriptions.SheetDescription x) => x.Level))
                {
                    if (item.Level <= num)
                    {
                        input = item.Description;
                        if (item.HasUsage)
                        {
                            input2 = item.Usage;
                        }
                        if (item.HasAction)
                        {
                            input3 = item.Action;
                        }
                    }
                }
                input = manager.StatisticsCalculator.ReplaceInline(input);
                input2 = manager.StatisticsCalculator.ReplaceInline(input2);
                input3 = manager.StatisticsCalculator.ReplaceInline(input3);
                bool flag = false;
                if (contentField.Lines.Any())
                {
                    flag = contentField.Lines.Last().Indent;
                }
                string text = "";
                if (!string.IsNullOrWhiteSpace(input3))
                {
                    text = "(" + input3 + ((!string.IsNullOrWhiteSpace(input2)) ? ("—" + input2) : "") + ")";
                }
                else if (!string.IsNullOrWhiteSpace(input2))
                {
                    text = "(" + input2 + ")";
                }
                string text2 = element.GetAlternateName();
                if (Settings.Default.SheetFormattingActionSuffixBold)
                {
                    text2 = (string.IsNullOrWhiteSpace(text) ? text2 : (text2 + " " + text));
                }
                else
                {
                    input = (string.IsNullOrWhiteSpace(text) ? input : (text + " " + input));
                }
                contentField.Lines.Add(new ContentLine(text2.Trim(), input, flag || !list.Contains(element), list.Contains(element)));
            }
            if (configuration.IncludeFormatting)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (ContentLine line in contentField.Lines)
                {
                    if ((!line.NewLineBefore || !line.Indent) && line.NewLineBefore && !string.IsNullOrWhiteSpace(stringBuilder.ToString()))
                    {
                        stringBuilder.Append("<p>&nbsp;</p>");
                    }
                    string text3 = line.Content.Replace(Environment.NewLine, "<br>&nbsp;  &nbsp;");
                    stringBuilder.Append("<p>" + (line.Indent ? "&nbsp;    &nbsp;" : "") + "<strong><em>" + line.Name + ".</em></strong> " + text3 + "</p>");
                }
                stringBuilder.Append("<p>&nbsp;</p>");
                export.TemporaryRacialTraits = stringBuilder.ToString();
                return;
            }
            StringBuilder stringBuilder2 = new StringBuilder();
            foreach (ContentLine line2 in contentField.Lines)
            {
                if (line2.NewLineBefore)
                {
                    stringBuilder2.AppendLine();
                }
                stringBuilder2.AppendLine((line2.Indent ? "    " : "") + line2.Name + ". " + line2.Content);
            }
            stringBuilder2.AppendLine();
            export.TemporaryRacialTraits = stringBuilder2.ToString();
        }

        private void GenerateCompanionSheetExportContent(CharacterManager manager, CharacterSheetConfiguration configuration, CharacterSheetExportContent export)
        {
            Companion companion = manager.Character.Companion;
            StatisticValuesGroupCollection statisticValues = manager.StatisticsCalculator.StatisticValues;
            export.CompanionName = companion.CompanionName.Content;
            export.CompanionKind = companion.Element.Name;
            export.CompanionBuild = companion.DisplayBuild;
            export.CompanionChallenge = companion.Element.Challenge;
            export.CompanionAppearance = "";
            export.CompanionSkills = companion.Element.Skills;
            export.CompanionInitiative = companion.Statistics.Initiative.ToValueString();
            export.CompanionProficiency = companion.Statistics.Proficiency.ToString();
            export.CompanionPortrait = companion.Portrait.Content;
            export.CompanionSpeedString = $"{companion.Statistics.Speed} ft.";
            if (companion.Statistics.SpeedFly > 0)
            {
                export.CompanionSpeedString += $", fly {companion.Statistics.SpeedFly} ft.";
            }
            if (companion.Statistics.SpeedClimb > 0)
            {
                export.CompanionSpeedString += $", climb {companion.Statistics.SpeedClimb} ft.";
            }
            if (companion.Statistics.SpeedSwim > 0)
            {
                export.CompanionSpeedString += $", swim {companion.Statistics.SpeedSwim} ft.";
            }
            if (companion.Statistics.SpeedBurrow > 0)
            {
                export.CompanionSpeedString += $", burrow {companion.Statistics.SpeedBurrow} ft.";
            }
            export.CompanionAbilitiesContent.Strength = companion.Abilities.Strength.FinalScore.ToString();
            export.CompanionAbilitiesContent.Dexterity = companion.Abilities.Dexterity.FinalScore.ToString();
            export.CompanionAbilitiesContent.Constitution = companion.Abilities.Constitution.FinalScore.ToString();
            export.CompanionAbilitiesContent.Intelligence = companion.Abilities.Intelligence.FinalScore.ToString();
            export.CompanionAbilitiesContent.Wisdom = companion.Abilities.Wisdom.FinalScore.ToString();
            export.CompanionAbilitiesContent.Charisma = companion.Abilities.Charisma.FinalScore.ToString();
            export.CompanionAbilitiesContent.StrengthModifier = companion.Abilities.Strength.ModifierString;
            export.CompanionAbilitiesContent.DexterityModifier = companion.Abilities.Dexterity.ModifierString;
            export.CompanionAbilitiesContent.ConstitutionModifier = companion.Abilities.Constitution.ModifierString;
            export.CompanionAbilitiesContent.IntelligenceModifier = companion.Abilities.Intelligence.ModifierString;
            export.CompanionAbilitiesContent.WisdomModifier = companion.Abilities.Wisdom.ModifierString;
            export.CompanionAbilitiesContent.CharismaModifier = companion.Abilities.Charisma.ModifierString;
            export.CompanionAbilitiesContent.StrengthSave = companion.SavingThrows.Strength.FinalBonus.ToValueString();
            export.CompanionAbilitiesContent.DexteritySave = companion.SavingThrows.Dexterity.FinalBonus.ToValueString();
            export.CompanionAbilitiesContent.ConstitutionSave = companion.SavingThrows.Constitution.FinalBonus.ToValueString();
            export.CompanionAbilitiesContent.IntelligenceSave = companion.SavingThrows.Intelligence.FinalBonus.ToValueString();
            export.CompanionAbilitiesContent.WisdomSave = companion.SavingThrows.Wisdom.FinalBonus.ToValueString();
            export.CompanionAbilitiesContent.CharismaSave = companion.SavingThrows.Charisma.FinalBonus.ToValueString();
            export.CompanionAbilitiesContent.StrengthSaveProficient = companion.SavingThrows.Strength.IsProficient;
            export.CompanionAbilitiesContent.DexteritySaveProficient = companion.SavingThrows.Dexterity.IsProficient;
            export.CompanionAbilitiesContent.ConstitutionSaveProficient = companion.SavingThrows.Constitution.IsProficient;
            export.CompanionAbilitiesContent.IntelligenceSaveProficient = companion.SavingThrows.Intelligence.IsProficient;
            export.CompanionAbilitiesContent.WisdomSaveProficient = companion.SavingThrows.Wisdom.IsProficient;
            export.CompanionAbilitiesContent.CharismaSaveProficient = companion.SavingThrows.Charisma.IsProficient;
            export.CompanionSkillsContent.Acrobatics = companion.Skills.Acrobatics.FinalBonus.ToValueString();
            export.CompanionSkillsContent.AnimalHandling = companion.Skills.AnimalHandling.FinalBonus.ToValueString();
            export.CompanionSkillsContent.Arcana = companion.Skills.Arcana.FinalBonus.ToValueString();
            export.CompanionSkillsContent.Athletics = companion.Skills.Athletics.FinalBonus.ToValueString();
            export.CompanionSkillsContent.Deception = companion.Skills.Deception.FinalBonus.ToValueString();
            export.CompanionSkillsContent.History = companion.Skills.History.FinalBonus.ToValueString();
            export.CompanionSkillsContent.Insight = companion.Skills.Insight.FinalBonus.ToValueString();
            export.CompanionSkillsContent.Intimidation = companion.Skills.Intimidation.FinalBonus.ToValueString();
            export.CompanionSkillsContent.Investigation = companion.Skills.Investigation.FinalBonus.ToValueString();
            export.CompanionSkillsContent.Medicine = companion.Skills.Medicine.FinalBonus.ToValueString();
            export.CompanionSkillsContent.Nature = companion.Skills.Nature.FinalBonus.ToValueString();
            export.CompanionSkillsContent.Perception = companion.Skills.Perception.FinalBonus.ToValueString();
            export.CompanionSkillsContent.Performance = companion.Skills.Performance.FinalBonus.ToValueString();
            export.CompanionSkillsContent.Persuasion = companion.Skills.Persuasion.FinalBonus.ToValueString();
            export.CompanionSkillsContent.Religion = companion.Skills.Religion.FinalBonus.ToValueString();
            export.CompanionSkillsContent.SleightOfHand = companion.Skills.SleightOfHand.FinalBonus.ToValueString();
            export.CompanionSkillsContent.Stealth = companion.Skills.Stealth.FinalBonus.ToValueString();
            export.CompanionSkillsContent.Survival = companion.Skills.Survival.FinalBonus.ToValueString();
            export.CompanionSkillsContent.AcrobaticsProficient = companion.Skills.Acrobatics.IsProficient;
            export.CompanionSkillsContent.AnimalHandlingProficient = companion.Skills.AnimalHandling.IsProficient;
            export.CompanionSkillsContent.ArcanaProficient = companion.Skills.Arcana.IsProficient;
            export.CompanionSkillsContent.AthleticsProficient = companion.Skills.Athletics.IsProficient;
            export.CompanionSkillsContent.DeceptionProficient = companion.Skills.Deception.IsProficient;
            export.CompanionSkillsContent.HistoryProficient = companion.Skills.History.IsProficient;
            export.CompanionSkillsContent.InsightProficient = companion.Skills.Insight.IsProficient;
            export.CompanionSkillsContent.IntimidationProficient = companion.Skills.Intimidation.IsProficient;
            export.CompanionSkillsContent.InvestigationProficient = companion.Skills.Investigation.IsProficient;
            export.CompanionSkillsContent.MedicineProficient = companion.Skills.Medicine.IsProficient;
            export.CompanionSkillsContent.NatureProficient = companion.Skills.Nature.IsProficient;
            export.CompanionSkillsContent.PerceptionProficient = companion.Skills.Perception.IsProficient;
            export.CompanionSkillsContent.PerformanceProficient = companion.Skills.Performance.IsProficient;
            export.CompanionSkillsContent.PersuasionProficient = companion.Skills.Persuasion.IsProficient;
            export.CompanionSkillsContent.ReligionProficient = companion.Skills.Religion.IsProficient;
            export.CompanionSkillsContent.SleightOfHandProficient = companion.Skills.SleightOfHand.IsProficient;
            export.CompanionSkillsContent.StealthProficient = companion.Skills.Stealth.IsProficient;
            export.CompanionSkillsContent.SurvivalProficient = companion.Skills.Survival.IsProficient;
            export.CompanionSkillsContent.AcrobaticsExpertise = companion.Skills.Acrobatics.IsExpertise(companion.Statistics.Proficiency);
            export.CompanionSkillsContent.AnimalHandlingExpertise = companion.Skills.AnimalHandling.IsExpertise(companion.Statistics.Proficiency);
            export.CompanionSkillsContent.ArcanaExpertise = companion.Skills.Arcana.IsExpertise(companion.Statistics.Proficiency);
            export.CompanionSkillsContent.AthleticsExpertise = companion.Skills.Athletics.IsExpertise(companion.Statistics.Proficiency);
            export.CompanionSkillsContent.DeceptionExpertise = companion.Skills.Deception.IsExpertise(companion.Statistics.Proficiency);
            export.CompanionSkillsContent.HistoryExpertise = companion.Skills.History.IsExpertise(companion.Statistics.Proficiency);
            export.CompanionSkillsContent.InsightExpertise = companion.Skills.Insight.IsExpertise(companion.Statistics.Proficiency);
            export.CompanionSkillsContent.IntimidationExpertise = companion.Skills.Intimidation.IsExpertise(companion.Statistics.Proficiency);
            export.CompanionSkillsContent.InvestigationExpertise = companion.Skills.Investigation.IsExpertise(companion.Statistics.Proficiency);
            export.CompanionSkillsContent.MedicineExpertise = companion.Skills.Medicine.IsExpertise(companion.Statistics.Proficiency);
            export.CompanionSkillsContent.NatureExpertise = companion.Skills.Nature.IsExpertise(companion.Statistics.Proficiency);
            export.CompanionSkillsContent.PerceptionExpertise = companion.Skills.Perception.IsExpertise(companion.Statistics.Proficiency);
            export.CompanionSkillsContent.PerformanceExpertise = companion.Skills.Performance.IsExpertise(companion.Statistics.Proficiency);
            export.CompanionSkillsContent.PersuasionExpertise = companion.Skills.Persuasion.IsExpertise(companion.Statistics.Proficiency);
            export.CompanionSkillsContent.ReligionExpertise = companion.Skills.Religion.IsExpertise(companion.Statistics.Proficiency);
            export.CompanionSkillsContent.SleightOfHandExpertise = companion.Skills.SleightOfHand.IsExpertise(companion.Statistics.Proficiency);
            export.CompanionSkillsContent.StealthExpertise = companion.Skills.Stealth.IsExpertise(companion.Statistics.Proficiency);
            export.CompanionSkillsContent.SurvivalExpertise = companion.Skills.Survival.IsExpertise(companion.Statistics.Proficiency);
            export.CompanionSkillsContent.PerceptionPassive = companion.Skills.Perception.FinalPassiveBonus.ToString();
            export.CompanionHitPointsContent.Maximum = companion.Statistics.MaxHp.ToString();
            export.CompanionArmorClassContent.ArmorClass = companion.Statistics.ArmorClass.ToString();
            export.CompanionConditionsContent.WalkingSpeed = companion.Statistics.Speed.ToString();
            export.CompanionConditionsContent.FlySpeed = companion.Statistics.SpeedFly.ToString();
            export.CompanionConditionsContent.ClimbSpeed = companion.Statistics.SpeedClimb.ToString();
            export.CompanionConditionsContent.SwimSpeed = companion.Statistics.SpeedSwim.ToString();
            export.CompanionConditionsContent.BurrowSpeed = companion.Statistics.SpeedBurrow.ToString();
            export.CompanionFeatures = GetCompanionFeaturesContent(configuration, companion, statisticValues, manager.StatisticsCalculator.InlineValues);
            export.CompanionStats = GetCompanionStatsContent(configuration, companion, statisticValues, manager.StatisticsCalculator.InlineValues);
            export.CompanionOwner = companion.Element.Aquisition.GetParentHeader().Name ?? manager.Character.Name;
        }

        private CharacterSheetSpellcastingPageExportContent GenerateSpellcastingExportContent(CharacterManager manager, SpellcastingInformation information, List<string> addedSpells)
        {
            CharacterSheetSpellcastingPageExportContent characterSheetSpellcastingPageExportContent = new CharacterSheetSpellcastingPageExportContent();
            Character character = manager.Character;
            StatisticValuesGroupCollection statisticValues = manager.StatisticsCalculator.StatisticValues;
            CharacterStatus status = manager.Status;
            //SpellcasterSelectionControlViewModel spellcasterSectionViewModel = SpellcastingSectionHandler.Current.GetSpellcasterSectionViewModel(information.UniqueIdentifier);
            //IOrderedEnumerable<SelectionElement> spells = SpellcastingSectionHandler.Current.GetSpells(information);
            characterSheetSpellcastingPageExportContent.SpellcastingClass = information.Name;
            try
            {
                ClassProgressionManager classProgressionManager = manager.ClassProgressionManagers.FirstOrDefault((ClassProgressionManager c) => c.SpellcastingInformations.Select((SpellcastingInformation x) => x.UniqueIdentifier).Contains(information.UniqueIdentifier));
                characterSheetSpellcastingPageExportContent.SpellcastingArchetype = (classProgressionManager.HasArchetype() ? classProgressionManager.GetElements().FirstOrDefault((ElementBase x) => x.Type.Equals("Archetype")) : null)?.Name ?? "";
            }
            catch (Exception)
            {
            }
            characterSheetSpellcastingPageExportContent.Ability = information.AbilityName;
            //characterSheetSpellcastingPageExportContent.AttackBonus = spellcasterSectionViewModel.InformationHeader.SpellAttackModifier.ToValueString();
            //characterSheetSpellcastingPageExportContent.Save = spellcasterSectionViewModel.InformationHeader.SpellSaveDc.ToString();
            characterSheetSpellcastingPageExportContent.PrepareCount = (information.Prepare ? statisticValues.GetValue(information.GetPrepareAmountStatisticName()).ToString() : "N/A");
            characterSheetSpellcastingPageExportContent.Notes = "";
            characterSheetSpellcastingPageExportContent.IsMulticlassSpellcaster = status.HasMulticlassSpellSlots;
            //characterSheetSpellcastingPageExportContent.Spells1.AvailableSlots = spellcasterSectionViewModel.InformationHeader.Slot1;
            //characterSheetSpellcastingPageExportContent.Spells2.AvailableSlots = spellcasterSectionViewModel.InformationHeader.Slot2;
            //characterSheetSpellcastingPageExportContent.Spells3.AvailableSlots = spellcasterSectionViewModel.InformationHeader.Slot3;
            //characterSheetSpellcastingPageExportContent.Spells4.AvailableSlots = spellcasterSectionViewModel.InformationHeader.Slot4;
            //characterSheetSpellcastingPageExportContent.Spells5.AvailableSlots = spellcasterSectionViewModel.InformationHeader.Slot5;
            //characterSheetSpellcastingPageExportContent.Spells6.AvailableSlots = spellcasterSectionViewModel.InformationHeader.Slot6;
            //characterSheetSpellcastingPageExportContent.Spells7.AvailableSlots = spellcasterSectionViewModel.InformationHeader.Slot7;
            //characterSheetSpellcastingPageExportContent.Spells8.AvailableSlots = spellcasterSectionViewModel.InformationHeader.Slot8;
            //characterSheetSpellcastingPageExportContent.Spells9.AvailableSlots = spellcasterSectionViewModel.InformationHeader.Slot9;
            characterSheetSpellcastingPageExportContent.Cantrips.Level = 0;
            characterSheetSpellcastingPageExportContent.Spells1.Level = 1;
            characterSheetSpellcastingPageExportContent.Spells2.Level = 2;
            characterSheetSpellcastingPageExportContent.Spells3.Level = 3;
            characterSheetSpellcastingPageExportContent.Spells4.Level = 4;
            characterSheetSpellcastingPageExportContent.Spells5.Level = 5;
            characterSheetSpellcastingPageExportContent.Spells6.Level = 6;
            characterSheetSpellcastingPageExportContent.Spells7.Level = 7;
            characterSheetSpellcastingPageExportContent.Spells8.Level = 8;
            characterSheetSpellcastingPageExportContent.Spells9.Level = 9;
            if (status.HasMulticlassSpellSlots)
            {
                //int slot = character.MulticlassSpellSlots.Slot1;
                //int slot2 = character.MulticlassSpellSlots.Slot2;
                //int slot3 = character.MulticlassSpellSlots.Slot3;
                //int slot4 = character.MulticlassSpellSlots.Slot4;
                //int slot5 = character.MulticlassSpellSlots.Slot5;
                //int slot6 = character.MulticlassSpellSlots.Slot6;
                //int slot7 = character.MulticlassSpellSlots.Slot7;
                //int slot8 = character.MulticlassSpellSlots.Slot8;
                //int slot9 = character.MulticlassSpellSlots.Slot9;
                //characterSheetSpellcastingPageExportContent.Spells1.AvailableSlots = slot;
                //characterSheetSpellcastingPageExportContent.Spells2.AvailableSlots = slot2;
                //characterSheetSpellcastingPageExportContent.Spells3.AvailableSlots = slot3;
                //characterSheetSpellcastingPageExportContent.Spells4.AvailableSlots = slot4;
                //characterSheetSpellcastingPageExportContent.Spells5.AvailableSlots = slot5;
                //characterSheetSpellcastingPageExportContent.Spells6.AvailableSlots = slot6;
                //characterSheetSpellcastingPageExportContent.Spells7.AvailableSlots = slot7;
                //characterSheetSpellcastingPageExportContent.Spells8.AvailableSlots = slot8;
                //characterSheetSpellcastingPageExportContent.Spells9.AvailableSlots = slot9;
            }
            bool sheetIncludeNonPreparedSpells = ApplicationManager.Current.Settings.Settings.SheetIncludeNonPreparedSpells;
            //foreach (SelectionElement item in spells)
            //{
            //    Spell spell = item.Element.AsElement<Spell>();
            //    bool alwaysPrepared = spell.Aquisition.WasGranted && (spell.Aquisition.GrantRule.Setters.GetSetter("prepared")?.ValueAsBool() ?? false);
            //    CharacterSheetSpellcastingPageExportContent.SpellExportContent spellExportContent = new CharacterSheetSpellcastingPageExportContent.SpellExportContent
            //    {
            //        IsPrepared = item.IsChosen,
            //        AlwaysPrepared = alwaysPrepared,
            //        Name = spell.Name,
            //        CastingTime = spell.CastingTime,
            //        Description = ElementDescriptionGenerator.GeneratePlainDescription(spell.Description),
            //        Range = spell.Range,
            //        Level = spell.Level.ToString(),
            //        Duration = spell.Duration,
            //        Components = spell.GetComponentsString(),
            //        Concentration = spell.IsConcentration,
            //        Ritual = spell.IsRitual,
            //        School = spell.MagicSchool,
            //        Subtitle = spell.Underline
            //    };
            //    if (spell.Level <= 0 || !information.Prepare || spellExportContent.IsPrepared || sheetIncludeNonPreparedSpells)
            //    {
            //        addedSpells.Add(spell.Name);
            //        switch (spell.Level)
            //        {
            //            case 0:
            //                characterSheetSpellcastingPageExportContent.Cantrips.Spells.Add(spellExportContent);
            //                break;
            //            case 1:
            //                characterSheetSpellcastingPageExportContent.Spells1.Spells.Add(spellExportContent);
            //                break;
            //            case 2:
            //                characterSheetSpellcastingPageExportContent.Spells2.Spells.Add(spellExportContent);
            //                break;
            //            case 3:
            //                characterSheetSpellcastingPageExportContent.Spells3.Spells.Add(spellExportContent);
            //                break;
            //            case 4:
            //                characterSheetSpellcastingPageExportContent.Spells4.Spells.Add(spellExportContent);
            //                break;
            //            case 5:
            //                characterSheetSpellcastingPageExportContent.Spells5.Spells.Add(spellExportContent);
            //                break;
            //            case 6:
            //                characterSheetSpellcastingPageExportContent.Spells6.Spells.Add(spellExportContent);
            //                break;
            //            case 7:
            //                characterSheetSpellcastingPageExportContent.Spells7.Spells.Add(spellExportContent);
            //                break;
            //            case 8:
            //                characterSheetSpellcastingPageExportContent.Spells8.Spells.Add(spellExportContent);
            //                break;
            //            case 9:
            //                characterSheetSpellcastingPageExportContent.Spells9.Spells.Add(spellExportContent);
            //                break;
            //        }
            //    }
            //}
            return characterSheetSpellcastingPageExportContent;
        }

        private CharacterSheetSpellcastingPageExportContent GenerateOtherSpellcastingExportContent(CharacterManager manager, List<string> includedSpells)
        {
            CharacterSheetSpellcastingPageExportContent characterSheetSpellcastingPageExportContent = new CharacterSheetSpellcastingPageExportContent();
            List<ElementBase> source = manager.GetElements().ToList();
            List<ElementBase> source2 = source.Where((ElementBase x) => x.Type.Equals("Spell") && !includedSpells.Contains(x.Name)).ToList();
            if (!source2.Any())
            {
                return null;
            }
            characterSheetSpellcastingPageExportContent.SpellcastingClass = "Other Spells";
            characterSheetSpellcastingPageExportContent.Cantrips.Level = 0;
            characterSheetSpellcastingPageExportContent.Spells1.Level = 1;
            characterSheetSpellcastingPageExportContent.Spells2.Level = 2;
            characterSheetSpellcastingPageExportContent.Spells3.Level = 3;
            characterSheetSpellcastingPageExportContent.Spells4.Level = 4;
            characterSheetSpellcastingPageExportContent.Spells5.Level = 5;
            characterSheetSpellcastingPageExportContent.Spells6.Level = 6;
            characterSheetSpellcastingPageExportContent.Spells7.Level = 7;
            characterSheetSpellcastingPageExportContent.Spells8.Level = 8;
            characterSheetSpellcastingPageExportContent.Spells9.Level = 9;
            foreach (Spell item in source2.Cast<Spell>())
            {
                CharacterSheetSpellcastingPageExportContent.SpellExportContent spellExportContent = new CharacterSheetSpellcastingPageExportContent.SpellExportContent
                {
                    IsPrepared = false,
                    AlwaysPrepared = false,
                    Name = item.Name,
                    CastingTime = item.CastingTime,
                    Description = ElementDescriptionGenerator.GeneratePlainDescription(item.Description),
                    Range = item.Range,
                    Level = item.Level.ToString(),
                    Duration = item.Duration,
                    Components = item.GetComponentsString(),
                    Concentration = item.IsConcentration,
                    Ritual = item.IsRitual,
                    School = item.MagicSchool,
                    Subtitle = item.Underline
                };
                try
                {
                    string text = "";
                    ElementHeader parent = item.Aquisition.GetParentHeader();
                    if (parent != null)
                    {
                        text = parent.Name;
                        if (parent.Name.StartsWith("Additional Spell,") || (parent.Name.StartsWith("Additional ") && parent.Name.Contains("Spell,")))
                        {
                            text = parent.Name.Replace(item.Name, "").Trim().Trim(',')
                                .Trim();
                        }
                        if (parent.Type.Equals("Racial Trait"))
                        {
                            ElementHeader elementHeader = source.FirstOrDefault((ElementBase x) => x.Id == parent.Id)?.Aquisition.GetParentHeader();
                            if (elementHeader != null)
                            {
                                text = elementHeader.Name;
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        spellExportContent.Name = item.Name + " (" + text + ")";
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, "GenerateOtherSpellcastingExportContent");
                    AnalyticsErrorHelper.Exception(ex, null, null, "GenerateOtherSpellcastingExportContent", 2917);
                }
                switch (item.Level)
                {
                    case 0:
                        characterSheetSpellcastingPageExportContent.Cantrips.Spells.Add(spellExportContent);
                        break;
                    case 1:
                        characterSheetSpellcastingPageExportContent.Spells1.Spells.Add(spellExportContent);
                        break;
                    case 2:
                        characterSheetSpellcastingPageExportContent.Spells2.Spells.Add(spellExportContent);
                        break;
                    case 3:
                        characterSheetSpellcastingPageExportContent.Spells3.Spells.Add(spellExportContent);
                        break;
                    case 4:
                        characterSheetSpellcastingPageExportContent.Spells4.Spells.Add(spellExportContent);
                        break;
                    case 5:
                        characterSheetSpellcastingPageExportContent.Spells5.Spells.Add(spellExportContent);
                        break;
                    case 6:
                        characterSheetSpellcastingPageExportContent.Spells6.Spells.Add(spellExportContent);
                        break;
                    case 7:
                        characterSheetSpellcastingPageExportContent.Spells7.Spells.Add(spellExportContent);
                        break;
                    case 8:
                        characterSheetSpellcastingPageExportContent.Spells8.Spells.Add(spellExportContent);
                        break;
                    case 9:
                        characterSheetSpellcastingPageExportContent.Spells9.Spells.Add(spellExportContent);
                        break;
                }
            }
            return characterSheetSpellcastingPageExportContent;
        }

        private string GetCompanionFeaturesContent(CharacterSheetConfiguration configuration, Companion companion, StatisticValuesGroupCollection statistics, Dictionary<string, string> inlineStatistics)
        {
            List<ElementBase> source = DataManager.Current.ElementsCollection.Where((ElementBase x) => x.Type.Equals("Companion Trait") || x.Type.Equals("Companion Action") || x.Type.Equals("Companion Reaction")).ToList();
            List<ElementBase> list = new List<ElementBase>();
            foreach (string trait in companion.Element.Traits)
            {
                ElementBase elementBase = source.FirstOrDefault((ElementBase x) => x.Id.Equals(trait));
                if (elementBase != null)
                {
                    list.Add(elementBase);
                }
            }
            foreach (string action in companion.Element.Actions)
            {
                ElementBase elementBase2 = source.FirstOrDefault((ElementBase x) => x.Id.Equals(action));
                if (elementBase2 != null)
                {
                    list.Add(elementBase2);
                }
            }
            foreach (string reaction in companion.Element.Reactions)
            {
                ElementBase elementBase3 = source.FirstOrDefault((ElementBase x) => x.Id.Equals(reaction));
                if (elementBase3 != null)
                {
                    list.Add(elementBase3);
                }
            }
            ContentField contentField = new ContentField();
            foreach (ElementBase item in list)
            {
                if (!item.SheetDescription.DisplayOnSheet)
                {
                    continue;
                }
                string text = item.SheetDescription[0]?.Description ?? "";
                foreach (Match item2 in Regex.Matches(text, "\\$\\((.*?)\\)"))
                {
                    string value = item2.Value;
                    string text2 = item2.Value.Substring(2, item2.Value.Length - 3);
                    string newValue = "==INVALID REPLACEMENT: " + value + " VALUE==";
                    if (statistics.ContainsGroup(text2))
                    {
                        newValue = statistics.GetValue(text2).ToString();
                    }
                    else if (inlineStatistics.ContainsKey(text2))
                    {
                        newValue = inlineStatistics[text2];
                    }
                    else
                    {
                        Logger.Warning($"UNKNOWN REPLACE STRING: {value} in {item}");
                    }
                    text = text.Replace(value, newValue);
                }
                string text3 = item.GetAlternateName();
                if (item.SheetDescription.HasUsage)
                {
                    text3 = text3 + " (" + item.SheetDescription.Usage + ")";
                }
                contentField.Lines.Add(new ContentLine(text3, text, contentField.Lines.Any()));
            }
            StringBuilder stringBuilder = new StringBuilder();
            if (configuration.IncludeFormatting)
            {
                foreach (ContentLine line in contentField.Lines)
                {
                    if (line.NewLineBefore && !string.IsNullOrWhiteSpace(stringBuilder.ToString()))
                    {
                        stringBuilder.Append("<p>&nbsp;</p>");
                    }
                    string text4 = line.Content.Replace(Environment.NewLine, "<br>&nbsp;  &nbsp;");
                    stringBuilder.Append("<p>" + (line.Indent ? "&nbsp;    &nbsp;" : "") + "<strong><em>" + line.Name + ".</em></strong> " + text4 + "</p>");
                }
            }
            else
            {
                foreach (ContentLine line2 in contentField.Lines)
                {
                    if (line2.NewLineBefore)
                    {
                        stringBuilder.AppendLine();
                    }
                    stringBuilder.AppendLine((line2.Indent ? "    " : "") + line2.Name + ". " + line2.Content);
                }
            }
            return stringBuilder.ToString().Trim();
        }

        private string GetCompanionStatsContent(CharacterSheetConfiguration configuration, Companion companion, StatisticValuesGroupCollection statistics, Dictionary<string, string> inlineStatistics)
        {
            ContentField contentField = new ContentField();
            string text = string.Join(", ", from x in companion.SavingThrows.GetCollection()
                                            where x.IsProficient
                                            select x.Name + " " + x.FinalBonus.ToValueString());
            if (!string.IsNullOrWhiteSpace(text))
            {
                text = text.Replace(" Saving Throw", "");
                contentField.Lines.Add(new ContentLine("Saving Throws", text, contentField.Lines.Any()));
            }
            string text2 = string.Join(", ", from x in companion.Skills.GetCollection()
                                             where x.IsProficient
                                             select x.Name + " " + x.FinalBonus.ToValueString());
            if (!string.IsNullOrWhiteSpace(text2))
            {
                contentField.Lines.Add(new ContentLine("Skills", text2, contentField.Lines.Any()));
            }
            if (companion.Element.HasDamageVulnerabilities)
            {
                contentField.Lines.Add(new ContentLine("Damage Vulnerabilities", companion.Element.DamageVulnerabilities, contentField.Lines.Any()));
            }
            if (companion.Element.HasDamageResistances)
            {
                contentField.Lines.Add(new ContentLine("Damage Resistances", companion.Element.DamageResistances, contentField.Lines.Any()));
            }
            if (companion.Element.HasDamageImmunities)
            {
                contentField.Lines.Add(new ContentLine("Damage Immunities", companion.Element.DamageImmunities, contentField.Lines.Any()));
            }
            if (companion.Element.HasConditionVulnerabilities)
            {
                contentField.Lines.Add(new ContentLine("Condition Vulnerabilities", companion.Element.ConditionVulnerabilities, contentField.Lines.Any()));
            }
            if (companion.Element.HasConditionResistances)
            {
                contentField.Lines.Add(new ContentLine("Condition Resistances", companion.Element.ConditionResistances, contentField.Lines.Any()));
            }
            if (companion.Element.HasConditionImmunities)
            {
                contentField.Lines.Add(new ContentLine("Condition Immunities", companion.Element.ConditionImmunities, contentField.Lines.Any()));
            }
            if (companion.Element.HasSenses)
            {
                contentField.Lines.Add(new ContentLine("Senses", companion.Element.Senses, contentField.Lines.Any()));
            }
            contentField.Lines.Add(new ContentLine("Languages", companion.Element.Languages, contentField.Lines.Any()));
            StringBuilder stringBuilder = new StringBuilder();
            if (configuration.IncludeFormatting)
            {
                foreach (ContentLine line in contentField.Lines)
                {
                    if (line.NewLineBefore && !string.IsNullOrWhiteSpace(stringBuilder.ToString()))
                    {
                        stringBuilder.Append("<p>&nbsp;</p>");
                    }
                    string text3 = line.Content.Replace(Environment.NewLine, "<br>&nbsp;  &nbsp;");
                    stringBuilder.Append("<p>" + (line.Indent ? "&nbsp;    &nbsp;" : "") + "<strong><em>" + line.Name + ".</em></strong> " + text3 + "</p>");
                }
            }
            else
            {
                foreach (ContentLine line2 in contentField.Lines)
                {
                    if (line2.NewLineBefore)
                    {
                        stringBuilder.AppendLine();
                    }
                    stringBuilder.AppendLine((line2.Indent ? "    " : "") + line2.Name + ". " + line2.Content);
                }
            }
            return stringBuilder.ToString().Trim();
        }
    }
}
