using Builder.Core;
using Builder.Core.Logging;
using Builder.Data.Elements;
using Builder.Presentation.Extensions;
using Builder.Presentation.Models.Collections;
using Builder.Presentation.Models.Equipment;
using Builder.Presentation.Models.NewFolder1;
using Builder.Presentation.Services;
using Builder.Presentation.Services.Calculator;
using Builder.Presentation.ViewModels.Shell.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Builder.Presentation.Models.Helpers
{
    public class AttackSectionItem : ObservableObject
    {
        private readonly Spell _spell;

        private readonly SpellcastingInformation _information;

        private readonly RefactoredEquipmentItem _equipment;

        private bool _isAutomaticAddition;

        private bool _isDisplayed;

        private bool _isDisplayedAsCard;

        private string _displayCalculatedAttack;

        private string _displayCalculatedDamage;

        private AbilityItem _linkedAbility;

        public bool IsAutomaticAddition
        {
            get
            {
                return _isAutomaticAddition;
            }
            set
            {
                SetProperty(ref _isAutomaticAddition, value, "IsAutomaticAddition");
            }
        }

        public bool IsDisplayed
        {
            get
            {
                return _isDisplayed;
            }
            set
            {
                SetProperty(ref _isDisplayed, value, "IsDisplayed");
            }
        }

        public bool IsDisplayedAsCard
        {
            get
            {
                return _isDisplayedAsCard;
            }
            set
            {
                SetProperty(ref _isDisplayedAsCard, value, "IsDisplayedAsCard");
            }
        }

        public string DisplayCalculatedAttack
        {
            get
            {
                return _displayCalculatedAttack;
            }
            set
            {
                SetProperty(ref _displayCalculatedAttack, value, "DisplayCalculatedAttack");
            }
        }

        public string DisplayCalculatedDamage
        {
            get
            {
                return _displayCalculatedDamage;
            }
            set
            {
                SetProperty(ref _displayCalculatedDamage, value, "DisplayCalculatedDamage");
            }
        }

        public ObservableCollection<AbilityItem> Abilities { get; } = new ObservableCollection<AbilityItem>();

        public AbilityItem LinkedAbility
        {
            get
            {
                return _linkedAbility;
            }
            set
            {
                SetProperty(ref _linkedAbility, value, "LinkedAbility");
                if (_linkedAbility != null)
                {
                    UpdateCalculations();
                }
            }
        }

        public FillableField Name { get; } = new FillableField();

        public FillableField Range { get; } = new FillableField();

        public FillableField Attack { get; } = new FillableField();

        public FillableField Damage { get; } = new FillableField();

        public FillableField Description { get; } = new FillableField();

        public RefactoredEquipmentItem EquipmentItem => _equipment;

        public AttackSectionItem(string name)
            : this(name, string.Empty)
        {
        }

        public AttackSectionItem(string name, string description)
        {
            Name.Content = name;
            Description.Content = description;
            Range.Content = "";
            Attack.Content = "";
            Damage.Content = "";
            IsDisplayed = true;
            IsDisplayedAsCard = false;
        }

        public AttackSectionItem(RefactoredEquipmentItem equipment, bool initializeAbility = true)
        {
            Abilities.Add(CharacterManager.Current.Character.Abilities.Strength);
            Abilities.Add(CharacterManager.Current.Character.Abilities.Dexterity);
            Abilities.Add(CharacterManager.Current.Character.Abilities.Constitution);
            Abilities.Add(CharacterManager.Current.Character.Abilities.Intelligence);
            Abilities.Add(CharacterManager.Current.Character.Abilities.Wisdom);
            Abilities.Add(CharacterManager.Current.Character.Abilities.Charisma);
            _equipment = equipment;
            UpdateCalculations(initializeAbility);
            IsDisplayed = true;
            IsDisplayedAsCard = equipment.ShowCard;
            IsAutomaticAddition = true;
        }

        public AttackSectionItem(Spell spell, SpellcastingInformation information)
        {
            _spell = spell;
            _information = information;
            Abilities.Add(CharacterManager.Current.Character.Abilities.Strength);
            Abilities.Add(CharacterManager.Current.Character.Abilities.Dexterity);
            Abilities.Add(CharacterManager.Current.Character.Abilities.Constitution);
            Abilities.Add(CharacterManager.Current.Character.Abilities.Intelligence);
            Abilities.Add(CharacterManager.Current.Character.Abilities.Wisdom);
            Abilities.Add(CharacterManager.Current.Character.Abilities.Charisma);
            UpdateCalculations(setLinkedAbility: true);
            IsDisplayed = true;
            IsDisplayedAsCard = false;
            IsAutomaticAddition = true;
        }

        public Dictionary<string, int> GetCalculatedDamage(RefactoredEquipmentItem equipment)
        {
            CharacterManager current = CharacterManager.Current;
            List<Tuple<string, int>> list = new List<Tuple<string, int>>();
            bool num = equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_SIMPLE_MELEE") || equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_MARTIAL_MELEE") || equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_MELEE");
            bool flag = equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_SIMPLE_RANGED") || equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_MARTIAL_RANGED") || equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_RANGED") || equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_FIREARM") || equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_FIREARMS");
            if (LinkedAbility != null)
            {
                list.Add(new Tuple<string, int>($"{LinkedAbility} Modifier", LinkedAbility.Modifier));
            }
            if (num)
            {
                StatisticValuesGroup group = current.StatisticsCalculator.StatisticValues.GetGroup("melee:damage");
                if (group.GetValues().Count > 0)
                {
                    list.Add(new Tuple<string, int>(group.GetSummery(includeValues: false), group.Sum()));
                }
            }
            else if (flag)
            {
                StatisticValuesGroup group2 = current.StatisticsCalculator.StatisticValues.GetGroup("ranged:damage");
                if (group2.GetValues().Count > 0)
                {
                    list.Add(new Tuple<string, int>(group2.GetSummery(includeValues: false), group2.Sum()));
                }
            }
            StatisticValuesGroup group3 = current.StatisticsCalculator.StatisticValues.GetGroup(equipment.Item.Name.ToLower() + ":damage");
            if (group3.GetValues().Count > 0)
            {
                list.Add(new Tuple<string, int>(group3.GetSummery(includeValues: false), group3.Sum()));
            }
            if (equipment.IsAdorned && int.TryParse(equipment.AdornerItem.Enhancement, out var result))
            {
                list.Add(new Tuple<string, int>("Enhancement", result));
            }
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            foreach (Tuple<string, int> item in list)
            {
                if (dictionary.ContainsKey(item.Item1))
                {
                    dictionary[item.Item1] += item.Item2;
                }
                else
                {
                    dictionary.Add(item.Item1, item.Item2);
                }
            }
            return dictionary;
        }

        public Dictionary<string, int> GetCalculatedAttackBonus(RefactoredEquipmentItem equipment)
        {
            CharacterManager current = CharacterManager.Current;
            ElementsOrganizer elementsOrganizer = new ElementsOrganizer(current.GetElements());
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            int num = 0;
            string text = equipment.Item.ElementSetters.GetSetter("proficiency")?.Value;
            if (text != null && (from x in elementsOrganizer.GetProficiencies()
                                 select x.Id).Contains(text.Trim()))
            {
                num += current.Character.Proficiency;
                dictionary.Add("Proficiency", current.Character.Proficiency);
            }
            bool num2 = equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_SIMPLE_MELEE") || equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_MARTIAL_MELEE") || equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_MELEE");
            bool flag = equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_SIMPLE_RANGED") || equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_MARTIAL_RANGED") || equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_RANGED") || equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_FIREARM") || equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_FIREARMS");
            if (LinkedAbility != null)
            {
                num += LinkedAbility.Modifier;
                dictionary.Add(LinkedAbility.Name + " Modifier", LinkedAbility.Modifier);
            }
            if (num2)
            {
                StatisticValuesGroup group = current.StatisticsCalculator.StatisticValues.GetGroup("melee:attack");
                if (group.GetValues().Count > 0)
                {
                    dictionary.Add(group.GetSummery(includeValues: false), group.Sum());
                }
            }
            else if (flag)
            {
                StatisticValuesGroup group2 = current.StatisticsCalculator.StatisticValues.GetGroup("ranged:attack");
                if (group2.GetValues().Count > 0)
                {
                    dictionary.Add(group2.GetSummery(includeValues: false), group2.Sum());
                }
            }
            StatisticValuesGroup group3 = current.StatisticsCalculator.StatisticValues.GetGroup(equipment.Item.Name.ToLower() + ":attack");
            if (group3.GetValues().Count > 0)
            {
                dictionary.Add(group3.GetSummery(includeValues: false), group3.Sum());
            }
            if (equipment.IsAdorned && int.TryParse(equipment.AdornerItem.Enhancement, out var result))
            {
                num += result;
                dictionary.Add("Enhancement", result);
            }
            return dictionary;
        }

        public Dictionary<string, int> GetCalculatedSpellAttackBonus(SpellcastingInformation information)
        {
            CharacterManager current = CharacterManager.Current;
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            string spellAttackStatisticName = information.GetSpellAttackStatisticName();
            StatisticValuesGroup group = current.StatisticsCalculator.StatisticValues.GetGroup(spellAttackStatisticName);
            dictionary.Add(group.GetSummery(includeValues: false), group.Sum());
            StatisticValuesGroup group2 = current.StatisticsCalculator.StatisticValues.GetGroup("spellcasting:attack");
            dictionary.Add(group2.GetSummery(includeValues: false), group2.Sum());
            return dictionary;
        }

        public void SetLinkedAbility(string abilityName)
        {
            AbilityItem abilityItem = Abilities.FirstOrDefault((AbilityItem x) => x.Name.Equals(abilityName, StringComparison.OrdinalIgnoreCase));
            if (abilityItem != null)
            {
                LinkedAbility = abilityItem;
            }
            else
            {
                MessageDialogService.Show($"unable to set linked ability '{abilityName}' on {_equipment}");
            }
        }

        private void DeterminePrimaryAbility(RefactoredEquipmentItem equipment)
        {
            AbilitiesCollection abilities = CharacterManager.Current.Character.Abilities;
            bool flag = equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_SIMPLE_MELEE") || equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_MARTIAL_MELEE") || equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_MELEE");
            bool flag2 = equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_SIMPLE_RANGED") || equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_MARTIAL_RANGED") || equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_RANGED") || equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_FIREARM") || equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_CATEGORY_FIREARMS");
            if (!flag && !flag2)
            {
                flag = true;
                SetLinkedAbility(abilities.Strength.Name);
            }
            if (flag)
            {
                if (equipment.Item.Supports.Contains("ID_INTERNAL_WEAPON_PROPERTY_FINESSE") && abilities.Dexterity.Modifier > abilities.Strength.Modifier)
                {
                    SetLinkedAbility(abilities.Dexterity.Name);
                }
                else
                {
                    SetLinkedAbility(abilities.Strength.Name);
                }
            }
            else if (flag2)
            {
                SetLinkedAbility(abilities.Dexterity.Name);
            }
        }

        public void UpdateCalculations(bool setLinkedAbility = false)
        {
            try
            {
                if (_equipment != null)
                {
                    if (setLinkedAbility)
                    {
                        DeterminePrimaryAbility(_equipment);
                    }
                    else if (LinkedAbility == null)
                    {
                        DeterminePrimaryAbility(_equipment);
                    }
                    CharacterInventory inventory = CharacterManager.Current.Character.Inventory;
                    Name.OriginalContent = _equipment.DisplayName;
                    Range.OriginalContent = _equipment.Item.Range ?? "5 ft";
                    if (!string.IsNullOrWhiteSpace(_equipment.Notes))
                    {
                        Description.OriginalContent = _equipment.Notes;
                    }
                    else
                    {
                        Description.OriginalContent = _equipment.Item.DisplayWeaponProperties;
                    }
                    Dictionary<string, int> calculatedAttackBonus = GetCalculatedAttackBonus(_equipment);
                    Attack.OriginalContent = calculatedAttackBonus.Sum((KeyValuePair<string, int> x) => x.Value).ToValueString() + " vs AC";
                    DisplayCalculatedAttack = string.Join(", ", from x in calculatedAttackBonus
                                                                where x.Value > 0
                                                                select $"{x.Key} {x.Value}");
                    Dictionary<string, int> calculatedDamage = GetCalculatedDamage(_equipment);
                    int num = calculatedDamage.Sum((KeyValuePair<string, int> x) => x.Value);
                    bool flag = num < 0;
                    Damage.OriginalContent = string.Format("{0}{1}{2} {3}", _equipment.Item.Damage, flag ? "" : "+", num, _equipment.Item.DamageType);
                    DisplayCalculatedDamage = string.Join(", ", from x in calculatedDamage
                                                                where x.Value > 0
                                                                select $"{x.Key} {x.Value}");
                    if (_equipment.IsEquipped && _equipment.Item.HasVersatile && inventory.IsEquippedVersatile())
                    {
                        Damage.OriginalContent = string.Format("{0}{1}{2} {3} (Versatile)", _equipment.Item.Versatile, flag ? "" : "+", num, _equipment.Item.DamageType);
                    }
                }
                else if (_spell != null && _information != null)
                {
                    Name.OriginalContent = _spell.Name;
                    Range.OriginalContent = _spell.Range;
                    Dictionary<string, int> calculatedSpellAttackBonus = GetCalculatedSpellAttackBonus(_information);
                    Attack.OriginalContent = calculatedSpellAttackBonus.Sum((KeyValuePair<string, int> x) => x.Value).ToValueString() + " " + _information.AbilityName.Substring(0, 3).ToUpper() + " vs AC";
                    DisplayCalculatedAttack = string.Join(", ", from x in calculatedSpellAttackBonus
                                                                where x.Value > 0
                                                                select $"{x.Key} {x.Value}");
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "UpdateCalculations");
            }
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
