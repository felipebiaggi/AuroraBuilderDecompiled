using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using Builder.Core;
using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Data.Strings;
using Builder.Presentation;
using Builder.Presentation.Models;
using Builder.Presentation.Models.Equipment;
using Builder.Presentation.Models.Helpers;
using Builder.Presentation.Services;
using Builder.Presentation.Services.Data;
using Builder.Presentation.ViewModels.Shell.Items;

namespace Builder.Presentation.Models.Equipment
{
    public class CharacterInventory : ObservableObject
    {
        private readonly ExpressionInterpreter _expressionInterpreter;

        private RefactoredEquipmentItem _equippedArmor;

        private RefactoredEquipmentItem _equippedPrimary;

        private RefactoredEquipmentItem _equippedSecondary;

        private decimal _equipmentWeight;

        private int _attunedItemCount;

        private int _maxAttunedItemCount;

        private string _equipment;

        private string _treasure;

        private string _questItems;

        public Character Character => CharacterManager.Current.Character;

        public Coinage Coins { get; }

        public ObservableCollection<RefactoredEquipmentItem> Items { get; }

        public RefactoredEquipmentItem EquippedArmor
        {
            get
            {
                return _equippedArmor;
            }
            set
            {
                SetProperty(ref _equippedArmor, value, "EquippedArmor");
                CalculateAttunedItemCount();
            }
        }

        public RefactoredEquipmentItem EquippedPrimary
        {
            get
            {
                return _equippedPrimary;
            }
            set
            {
                SetProperty(ref _equippedPrimary, value, "EquippedPrimary");
                OnPropertyChanged("IsEquippedTwoHanded");
                OnPropertyChanged("IsEquippedVersatile");
                CalculateAttunedItemCount();
            }
        }

        public RefactoredEquipmentItem EquippedSecondary
        {
            get
            {
                return _equippedSecondary;
            }
            set
            {
                SetProperty(ref _equippedSecondary, value, "EquippedSecondary");
                OnPropertyChanged("IsEquippedTwoHanded");
                OnPropertyChanged("IsEquippedVersatile");
                OnPropertyChanged("IsEquippedShield");
                CalculateAttunedItemCount();
            }
        }

        public decimal EquipmentWeight
        {
            get
            {
                return _equipmentWeight;
            }
            set
            {
                SetProperty(ref _equipmentWeight, value, "EquipmentWeight");
            }
        }

        public int AttunedItemCount
        {
            get
            {
                return _attunedItemCount;
            }
            set
            {
                SetProperty(ref _attunedItemCount, value, "AttunedItemCount");
            }
        }

        public int MaxAttunedItemCount
        {
            get
            {
                return _maxAttunedItemCount;
            }
            set
            {
                SetProperty(ref _maxAttunedItemCount, value, "MaxAttunedItemCount");
            }
        }

        public string Equipment
        {
            get
            {
                return _equipment;
            }
            set
            {
                SetProperty(ref _equipment, value, "Equipment");
            }
        }

        public string Treasure
        {
            get
            {
                return _treasure;
            }
            set
            {
                SetProperty(ref _treasure, value, "Treasure");
            }
        }

        public string QuestItems
        {
            get
            {
                return _questItems;
            }
            set
            {
                SetProperty(ref _questItems, value, "QuestItems");
            }
        }

        public InventoryStorage StoredItems1 { get; set; }

        public InventoryStorage StoredItems2 { get; set; }

        public CharacterInventory()
        {
            _expressionInterpreter = new ExpressionInterpreter();
            _maxAttunedItemCount = 3;
            Coins = new Coinage();
            Items = new ObservableCollection<RefactoredEquipmentItem>();
            Items.CollectionChanged += Items_CollectionChanged;
            StoredItems1 = new InventoryStorage();
            StoredItems2 = new InventoryStorage();
        }

        public void CalculateAttunedItemCount()
        {
            AttunedItemCount = Items.Count((RefactoredEquipmentItem equipment) => equipment.IsAttunable && equipment.IsAttuned);
        }

        public decimal CalculateWeight()
        {
            return CalculateWeight(Items);
        }

        public decimal CalculateWeight(IEnumerable<RefactoredEquipmentItem> equipmentItems)
        {
            decimal num = 0.0m;
            foreach (RefactoredEquipmentItem equipmentItem in equipmentItems)
            {
                if (!equipmentItem.IsStored)
                {
                    num += equipmentItem.GetWeight();
                }
                else
                {
                    Logger.Warning($"not includeing stored item: {equipmentItem}");
                }
            }
            num += CalculateCoinsWeight();
            EquipmentWeight = num;
            return num;
        }

        public decimal CalculateCoinsWeight()
        {
            return (decimal)(0 + Coins.Copper + Coins.Silver + Coins.Electrum + Coins.Gold + Coins.Platinum) / 50m;
        }

        public bool IsEquippedPrimary(EquipmentItem equipment)
        {
            if (EquippedPrimary != null)
            {
                return EquippedPrimary.Identifier.Equals(equipment.Identifier);
            }
            return false;
        }

        public bool IsEquippedSecondary(EquipmentItem equipment)
        {
            if (EquippedSecondary != null)
            {
                return EquippedSecondary.Identifier.Equals(equipment.Identifier);
            }
            return false;
        }

        public bool IsEquippedTwoHanded()
        {
            if (EquippedPrimary == null || EquippedSecondary == null)
            {
                return false;
            }
            if (EquippedPrimary.Item.HasVersatile)
            {
                return false;
            }
            return EquippedPrimary.Identifier.Equals(EquippedSecondary.Identifier);
        }

        public bool IsEquippedVersatile()
        {
            if (EquippedPrimary == null || EquippedSecondary == null)
            {
                return false;
            }
            if (!EquippedPrimary.Item.HasVersatile)
            {
                return false;
            }
            return EquippedPrimary.Identifier.Equals(EquippedSecondary.Identifier);
        }

        public bool IsEquippedShield()
        {
            if (EquippedSecondary == null)
            {
                return false;
            }
            if (EquippedSecondary.Item.Supports.Contains(InternalElementID.ArmorGroup.Shield))
            {
                return true;
            }
            if (EquippedSecondary.Item.AttemptGetSetterValue("armor", out var setter))
            {
                return setter.Value.ToLower().Equals("shield");
            }
            if (EquippedSecondary.Item.ElementSetters.ContainsSetter("armor"))
            {
                return EquippedSecondary.Item.ElementSetters.GetSetter("armor").Value.Equals("shield", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public bool AllowMoreAttunement()
        {
            return AttunedItemCount < Character.Inventory.MaxAttunedItemCount;
        }

        public void ClearInventory()
        {
            while (Items.Any())
            {
                Items.FirstOrDefault()?.Retrieve();
                Items.FirstOrDefault()?.Deactivate();
                Items.RemoveAt(0);
            }
            Coins.Clear();
            Equipment = string.Empty;
            Treasure = string.Empty;
            QuestItems = string.Empty;
            EquipmentWeight = 0m;
            AttunedItemCount = 0;
            MaxAttunedItemCount = 3;
            EquippedArmor = null;
            EquippedPrimary = null;
            EquippedSecondary = null;
            StoredItems1.Name = "#1";
            StoredItems1.StoredItems.Clear();
            StoredItems2.Name = "#2";
            StoredItems2.StoredItems.Clear();
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CalculateWeight();
        }

        public void EquipArmor(RefactoredEquipmentItem equipment)
        {
            EquippedArmor = equipment;
            EquippedArmor.EquippedLocation = "Armor";
            EquippedArmor.Activate(equip: true, equipment.IsAttunable);
            CalculateAttunedItemCount();
            ApplicationManager.Current.EventAggregator.Send(new ReprocessCharacterEvent());
        }

        public void EquipPrimary(RefactoredEquipmentItem equipment, bool twohanded = false)
        {
            EquippedPrimary = equipment;
            EquippedPrimary.EquippedLocation = "Primary Hand";
            if (twohanded)
            {
                EquippedSecondary = equipment;
                EquippedSecondary.EquippedLocation = "Two-Handed";
                if (!equipment.IsTwoHandTarget() && equipment.Item.HasVersatile)
                {
                    EquippedSecondary.EquippedLocation = "Two-Handed (Versatile)";
                }
            }
            EquippedPrimary.Activate(equip: true, equipment.IsAttunable);
            CalculateAttunedItemCount();
            PromtAddToAttacksIfEnabled(equipment);
            ApplicationManager.Current.EventAggregator.Send(new ReprocessCharacterEvent());
        }

        public void EquipSecondary(RefactoredEquipmentItem equipment)
        {
            EquippedSecondary = equipment;
            EquippedSecondary.EquippedLocation = "Secondary Hand";
            EquippedSecondary.Activate(equip: true, equipment.IsAttunable);
            CalculateAttunedItemCount();
            if (!IsEquippedShield())
            {
                PromtAddToAttacksIfEnabled(equipment);
            }
            ApplicationManager.Current.EventAggregator.Send(new ReprocessCharacterEvent());
        }

        public void UnequipArmor()
        {
            if (EquippedArmor != null)
            {
                RefactoredEquipmentItem equippedArmor = EquippedArmor;
                EquippedArmor = null;
                equippedArmor.Deactivate();
                CalculateAttunedItemCount();
            }
        }

        public void UnequipPrimary()
        {
            if (EquippedPrimary != null)
            {
                RefactoredEquipmentItem equippedPrimary = EquippedPrimary;
                if (IsEquippedVersatile())
                {
                    EquippedSecondary = null;
                }
                else if (IsEquippedTwoHanded())
                {
                    EquippedSecondary = null;
                }
                EquippedPrimary = null;
                equippedPrimary.Deactivate();
                CalculateAttunedItemCount();
            }
        }

        public void UnequipSecondary()
        {
            if (EquippedSecondary != null)
            {
                RefactoredEquipmentItem equippedSecondary = EquippedSecondary;
                if (IsEquippedVersatile())
                {
                    EquippedSecondary = null;
                }
                else if (IsEquippedTwoHanded())
                {
                    EquippedSecondary = null;
                    UnequipPrimary();
                }
                else
                {
                    EquippedSecondary = null;
                    equippedSecondary.Deactivate();
                }
                CalculateAttunedItemCount();
            }
        }

        public bool AddFromStatistics(ElementHeader parent, string id, int amount = 1)
        {
            if (!(DataManager.Current.ElementsCollection.GetElement(id) is Item item))
            {
                return false;
            }
            if (Items.Any((RefactoredEquipmentItem x) => x.HasAquisitionParent && x.AquisitionParent.Name.Equals(parent.Name) && (x.Item.Id.Equals(id) || (x.IsAdorned && x.AdornerItem.Id.Equals(id)))))
            {
                return false;
            }
            RefactoredEquipmentItem refactoredEquipmentItem;
            if (item.Type.Equals("Magic Item"))
            {
                switch (item.ItemType)
                {
                    case "Weapon":
                        {
                            ArmorElement armorElement2 = GetSupportedArmorElements(item.ElementSetters.GetSetter("weapon").Value).FirstOrDefault();
                            refactoredEquipmentItem = ((armorElement2 != null) ? new RefactoredEquipmentItem(armorElement2.Copy(), item.Copy()) : new RefactoredEquipmentItem(item.Copy()));
                            break;
                        }
                    case "Armor":
                        {
                            ArmorElement armorElement = GetSupportedArmorElements(item.ElementSetters.GetSetter("armor").Value).FirstOrDefault();
                            refactoredEquipmentItem = ((armorElement != null) ? new RefactoredEquipmentItem(armorElement.Copy(), item.Copy()) : new RefactoredEquipmentItem(item.Copy()));
                            break;
                        }
                    default:
                        refactoredEquipmentItem = new RefactoredEquipmentItem(item.Copy());
                        break;
                }
            }
            else
            {
                if (!item.Type.Equals("Item") && !item.Type.Equals("Weapon") && !item.Type.Equals("Armor"))
                {
                    return false;
                }
                refactoredEquipmentItem = new RefactoredEquipmentItem(item.Copy());
            }
            if (item.IsStackable)
            {
                refactoredEquipmentItem.Amount = amount;
            }
            refactoredEquipmentItem.Notes = "This item added by '" + parent.Name + "'.";
            refactoredEquipmentItem.AquisitionParent = parent;
            refactoredEquipmentItem.HasAquisitionParent = refactoredEquipmentItem.AquisitionParent != null;
            Items.Add(refactoredEquipmentItem);
            return true;
        }

        public IEnumerable<WeaponElement> GetSupportedWeaponElements(string supportExpression)
        {
            IEnumerable<WeaponElement> elements = DataManager.Current.ElementsCollection.Where((ElementBase x) => x.Type == "Weapon").Cast<WeaponElement>();
            return _expressionInterpreter.EvaluateSupportsExpression(supportExpression, elements).ToList();
        }

        public IEnumerable<ArmorElement> GetSupportedArmorElements(string supportExpression)
        {
            IEnumerable<ArmorElement> elements = DataManager.Current.ElementsCollection.Where((ElementBase x) => x.Type == "Armor").Cast<ArmorElement>();
            return _expressionInterpreter.EvaluateSupportsExpression(supportExpression, elements).ToList();
        }

        private void PromtAddToAttacksIfEnabled(RefactoredEquipmentItem equipment)
        {
            if (ApplicationManager.Current.Settings.Settings.RequestAddAttackWhenEquippingWeapon && CharacterManager.Current.Status.IsLoaded && Character.AttacksSection.Items.All((AttackSectionItem x) => x.EquipmentItem != equipment))
            {
                if (MessageBox.Show($"You have equipped '{equipment}' which is not yet added to your attacks. Do you want to add it now?", "Aurora - Attacks Section", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Character.AttacksSection.Items.Add(new AttackSectionItem(equipment));
                    ApplicationManager.Current.SendStatusMessage($"{equipment} added to your attacks.");
                }
                else
                {
                    ApplicationManager.Current.SendStatusMessage("");
                }
            }
        }

        public InventoryStorage GetStorage(string storageName)
        {
            if (StoredItems1.Name.Equals(storageName))
            {
                return StoredItems1;
            }
            if (StoredItems2.Name.Equals(storageName))
            {
                return StoredItems2;
            }
            return null;
        }
    }
}
