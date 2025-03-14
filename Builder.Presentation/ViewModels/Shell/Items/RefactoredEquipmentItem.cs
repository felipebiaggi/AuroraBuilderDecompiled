using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Data.Rules;
using Builder.Presentation.Models.Equipment;
using Builder.Presentation.Services.Data;
using Builder.Presentation.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Builder.Presentation.ViewModels.Shell.Items
{
    public class RefactoredEquipmentItem : EquipmentItem
    {
        private bool _isAttuned;

        private bool _showCard = true;

        private string _alternativeName;

        private string _notes;

        private string _notesWatermark;

        private ElementHeader _aquisitionParent;

        private bool _hasAquisitionParent;

        private bool _includeInEquipmentPageInventory;

        private bool _includeInEquipmentPageDescriptionSidebar;

        private bool _includeIndividualPriceOnEquipmentPage;

        private bool _includeInTreasure;

        private bool _isStored;

        private InventoryStorage _storage;

        private string _displayPrice;

        private string _displayWeight;

        public override string DisplayName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(AlternativeName))
                {
                    return AlternativeName + (base.IsStackable ? $" ({base.Amount})" : "");
                }
                if (base.AdornerItem == null)
                {
                    return base.DisplayName;
                }
                return GetAdornedName() + ((base.IsStackable && base.Amount > 1) ? $" ({base.Amount})" : "");
            }
        }

        public bool IsAttunable
        {
            get
            {
                if (!base.Item.RequiresAttunement)
                {
                    if (base.IsAdorned)
                    {
                        return base.AdornerItem.RequiresAttunement;
                    }
                    return false;
                }
                return true;
            }
        }

        public bool IsAttuned
        {
            get
            {
                return _isAttuned;
            }
            set
            {
                SetProperty(ref _isAttuned, value, "IsAttuned");
                OnPropertyChanged("IsActivated");
                OnPropertyChanged("Underline");
            }
        }

        public bool IsActivated
        {
            get
            {
                if (!IsAttuned)
                {
                    return base.IsEquipped;
                }
                return true;
            }
        }

        private bool IsItemRegistered { get; set; }

        private bool IsAdornedItemRegisted { get; set; }

        public bool ShowCard
        {
            get
            {
                return _showCard;
            }
            set
            {
                SetProperty(ref _showCard, value, "ShowCard");
            }
        }

        public string AlternativeName
        {
            get
            {
                return _alternativeName;
            }
            set
            {
                SetProperty(ref _alternativeName, value, "AlternativeName");
                OnPropertyChanged("DisplayName");
            }
        }

        public string Notes
        {
            get
            {
                return _notes;
            }
            set
            {
                SetProperty(ref _notes, value, "Notes");
                OnPropertyChanged("HasAdditionalNotes");
                IncludeInEquipmentPageDescriptionSidebar = true;
            }
        }

        public string NotesWatermark
        {
            get
            {
                return _notesWatermark;
            }
            set
            {
                SetProperty(ref _notesWatermark, value, "NotesWatermark");
            }
        }

        public bool HasAdditionalNotes => !string.IsNullOrWhiteSpace(Notes);

        public ElementHeader AquisitionParent
        {
            get
            {
                return _aquisitionParent;
            }
            set
            {
                SetProperty(ref _aquisitionParent, value, "AquisitionParent");
            }
        }

        public bool HasAquisitionParent
        {
            get
            {
                return _hasAquisitionParent;
            }
            set
            {
                SetProperty(ref _hasAquisitionParent, value, "HasAquisitionParent");
            }
        }

        public string Underline
        {
            get
            {
                List<string> list = new List<string>();
                string underline = GetUnderline();
                if (!string.IsNullOrWhiteSpace(underline))
                {
                    list.Add(underline);
                }
                else if (base.Item is ArmorElement armorElement)
                {
                    list.Add(armorElement.Name);
                }
                if (!string.IsNullOrWhiteSpace(base.EquippedLocation))
                {
                    list.Add(base.EquippedLocation);
                }
                if (!list.Any() && !string.IsNullOrWhiteSpace(base.Item.ItemType))
                {
                    list.Add(base.Item.ItemType);
                }
                if (!list.Any() && !string.IsNullOrWhiteSpace(base.Item.Category))
                {
                    list.Add(base.Item.Category);
                }
                return string.Join(" • ", list);
            }
        }

        public bool IncludeInEquipmentPageInventory
        {
            get
            {
                return _includeInEquipmentPageInventory;
            }
            set
            {
                SetProperty(ref _includeInEquipmentPageInventory, value, "IncludeInEquipmentPageInventory");
            }
        }

        public bool IncludeInEquipmentPageDescriptionSidebar
        {
            get
            {
                return _includeInEquipmentPageDescriptionSidebar;
            }
            set
            {
                SetProperty(ref _includeInEquipmentPageDescriptionSidebar, value, "IncludeInEquipmentPageDescriptionSidebar");
            }
        }

        public bool IncludeIndividualPriceOnEquipmentPage
        {
            get
            {
                return _includeIndividualPriceOnEquipmentPage;
            }
            set
            {
                SetProperty(ref _includeIndividualPriceOnEquipmentPage, value, "IncludeIndividualPriceOnEquipmentPage");
            }
        }

        public bool IncludeInTreasure
        {
            get
            {
                return _includeInTreasure;
            }
            set
            {
                SetProperty(ref _includeInTreasure, value, "IncludeInTreasure");
            }
        }

        public bool IsStored
        {
            get
            {
                return _isStored;
            }
            set
            {
                SetProperty(ref _isStored, value, "IsStored");
            }
        }

        public InventoryStorage Storage
        {
            get
            {
                return _storage;
            }
            set
            {
                SetProperty(ref _storage, value, "Storage");
            }
        }

        public string StorageDisplayName
        {
            get
            {
                if (!IsStored)
                {
                    return "";
                }
                return Storage?.Name;
            }
        }

        public string DisplayPrice
        {
            get
            {
                return _displayPrice;
            }
            set
            {
                SetProperty(ref _displayPrice, value, "DisplayPrice");
            }
        }

        public string DisplayWeight
        {
            get
            {
                return _displayWeight;
            }
            set
            {
                SetProperty(ref _displayWeight, value, "DisplayWeight");
            }
        }

        public RefactoredEquipmentItem(Item item, Item adorner = null)
            : base(item, adorner)
        {
            if (base.Item.ContainsSelectRules)
            {
                foreach (SelectRule selectRule in base.Item.GetSelectRules())
                {
                    selectRule.RenewIdentifier();
                }
            }
            if (base.AdornerItem != null && base.AdornerItem.ContainsSelectRules)
            {
                foreach (SelectRule selectRule2 in base.AdornerItem.GetSelectRules())
                {
                    selectRule2.RenewIdentifier();
                }
            }
            AlternativeName = "";
            Notes = "";
            NotesWatermark = ElementDescriptionGenerator.GeneratePlainDescription(item.Description);
            if (base.AdornerItem != null)
            {
                NotesWatermark = ElementDescriptionGenerator.GeneratePlainDescription(base.AdornerItem.Description);
            }
            IncludeInEquipmentPageInventory = true;
            if (base.Item.HideFromInventory)
            {
                IncludeInEquipmentPageInventory = false;
                ShowCard = false;
            }
            else if (base.AdornerItem != null && base.AdornerItem.HideFromInventory)
            {
                IncludeInEquipmentPageInventory = false;
                ShowCard = false;
            }
            if (!IncludeInEquipmentPageInventory)
            {
                IncludeInEquipmentPageDescriptionSidebar = false;
            }
            IncludeInEquipmentPageDescriptionSidebar = RaiseEquippedConditionForSidebar();
            IncludeIndividualPriceOnEquipmentPage = base.Item.IsValuable;
            IncludeInTreasure = IncludeIndividualPriceOnEquipmentPage || base.Item.Category.Equals("Valuables");
            DisplayPrice = item.DisplayPrice;
            if (base.IsAdorned && base.AdornerItem is MagicItemElement magicItemElement && magicItemElement.OverrideCost)
            {
                DisplayPrice = base.AdornerItem.DisplayPrice;
            }
            OnPropertyChanged("Underline");
        }

        public void SetIdentifier(string identifier)
        {
            base.Identifier = identifier;
        }

        [Obsolete]
        public void AttuneItem()
        {
            Register();
        }

        [Obsolete]
        public void UnAttuneItem()
        {
            Unregister();
        }

        private string GetAdornedName()
        {
            if (string.IsNullOrWhiteSpace(base.AdornerItem.NameFormat))
            {
                return base.AdornerItem.Name;
            }
            string text = base.AdornerItem.NameFormat;
            foreach (Match item in Regex.Matches(base.AdornerItem.NameFormat, "\\$\\((.*?)\\)"))
            {
                switch (item.Value.Substring(2, item.Value.Length - 3))
                {
                    case "parent":
                        text = text.Replace(item.Value, base.Item.Name);
                        break;
                    case "enhancement":
                        text = text.Replace(item.Value, base.AdornerItem.Enhancement);
                        break;
                }
            }
            foreach (Match item2 in Regex.Matches(base.AdornerItem.NameFormat, "{{(.*?)}}"))
            {
                switch (item2.Value.Substring(2, item2.Value.Length - 4).Trim())
                {
                    case "parent":
                        text = text.Replace(item2.Value, base.Item.Name);
                        break;
                    case "enhancement":
                        text = text.Replace(item2.Value, base.AdornerItem.Enhancement);
                        break;
                }
            }
            return text;
        }

        [Obsolete]
        private void Register()
        {
            if (base.Item != null)
            {
                CharacterManager.Current.RegisterElement(base.Item);
            }
            if (base.AdornerItem != null)
            {
                CharacterManager.Current.RegisterElement(base.AdornerItem);
            }
            IsAttuned = true;
        }

        [Obsolete]
        private void Unregister()
        {
            if (base.Item != null)
            {
                CharacterManager.Current.UnregisterElement(base.Item);
            }
            if (base.AdornerItem != null)
            {
                CharacterManager.Current.UnregisterElement(base.AdornerItem);
            }
            IsAttuned = false;
        }

        private void RegisterItem()
        {
            if (!IsItemRegistered)
            {
                Logger.Info($"registering item: {base.Item}");
                CharacterManager.Current.RegisterElement(base.Item);
                IsItemRegistered = true;
            }
        }

        private void UnRegisterItem()
        {
            if (IsItemRegistered)
            {
                Logger.Info($"unregistering item: {base.Item}");
                CharacterManager.Current.UnregisterElement(base.Item);
                IsItemRegistered = false;
            }
        }

        private void RegisterAdorner()
        {
            if (base.IsAdorned && !IsAdornedItemRegisted)
            {
                Logger.Info($"registering adorner item: {base.AdornerItem}");
                CharacterManager.Current.RegisterElement(base.AdornerItem);
                IsAdornedItemRegisted = true;
            }
        }

        private void UnRegisterAdorner()
        {
            if (base.IsAdorned && IsAdornedItemRegisted)
            {
                Logger.Info($"unregistering adorner item: {base.AdornerItem}");
                CharacterManager.Current.UnregisterElement(base.AdornerItem);
                IsAdornedItemRegisted = false;
            }
        }

        public void Activate(bool equip, bool attune)
        {
            if (equip && !base.IsEquippable)
            {
                Logger.Warning($"request equip item {this} while not equippable");
            }
            if (attune && !IsAttunable)
            {
                Logger.Warning($"request attune item {this} while not attunable");
                attune = false;
            }
            if (base.IsAdorned)
            {
                if (equip && base.Item.IsEquippable)
                {
                    RegisterItem();
                    if (!base.Item.RequiresAttunement)
                    {
                        RegisterAdorner();
                    }
                    base.IsEquipped = true;
                }
                if (attune && base.AdornerItem.RequiresAttunement)
                {
                    RegisterAdorner();
                    IsAttuned = true;
                }
            }
            else
            {
                if (equip && base.Item.IsEquippable)
                {
                    if (base.Item.Type.Equals("Armor") || base.Item.Type.Equals("Weapon"))
                    {
                        RegisterItem();
                    }
                    if (!base.Item.RequiresAttunement)
                    {
                        RegisterItem();
                    }
                    base.IsEquipped = true;
                }
                if (attune && base.Item.RequiresAttunement)
                {
                    RegisterItem();
                    IsAttuned = true;
                }
            }
            if (RaiseEquippedConditionForSidebar())
            {
                IncludeInEquipmentPageDescriptionSidebar = true;
            }
            OnPropertyChanged("Underline");
        }

        public void Deactivate()
        {
            UnRegisterAdorner();
            UnRegisterItem();
            base.IsEquipped = false;
            IsAttuned = false;
            base.EquippedLocation = string.Empty;
            IncludeInEquipmentPageDescriptionSidebar = false;
        }

        public void DeactivateAttunement()
        {
            if (!IsAttuned)
            {
                Logger.Warning($"trying to unattune {this} which was not attuned.");
            }
            if (base.IsAdorned)
            {
                UnRegisterAdorner();
                IsAttuned = false;
            }
            else
            {
                UnRegisterItem();
                IsAttuned = false;
            }
        }

        public bool IsArmorTarget()
        {
            if (!base.Item.Slots.Contains("armor"))
            {
                return base.Item.Slots.Contains("body");
            }
            return true;
        }

        public bool IsTwoHandTarget()
        {
            if (!base.Item.Slot.Equals("twohand"))
            {
                return base.Item.Slots.Contains("twohand");
            }
            return true;
        }

        public bool IsOneHandTarget()
        {
            if (!base.Item.Slot.Equals("onehand"))
            {
                return base.Item.Slots.Contains("onehand");
            }
            return true;
        }

        public bool IsPrimaryTarget()
        {
            return base.Item.Slots.Contains("primary");
        }

        public bool IsSecondaryTarget()
        {
            return base.Item.Slots.Contains("secondary");
        }

        public string GetUnderline()
        {
            List<string> list = new List<string>();
            if (base.Item is WeaponElement weaponElement)
            {
                foreach (ElementBase item in (from x in DataManager.Current.ElementsCollection
                                              where x.Type.Equals("Weapon Category")
                                              orderby x.Name
                                              select x).ToList())
                {
                    if (weaponElement.Supports.Contains(item.Id))
                    {
                        if (item.Name.Equals("firearm", StringComparison.InvariantCultureIgnoreCase))
                        {
                            list.Add(item.Name ?? "");
                        }
                        else
                        {
                            list.Add(item.Name + " Weapon");
                        }
                    }
                }
            }
            return string.Join(", ", list);
        }

        public override string ToString()
        {
            if (base.IsAdorned)
            {
                return GetAdornedName();
            }
            return base.DisplayName;
        }

        public bool RaiseEquippedConditionForSidebar()
        {
            if (base.Item.HideFromInventory || (base.IsAdorned && base.AdornerItem.HideFromInventory))
            {
                return false;
            }
            if (base.IsEquipped || IsAttuned)
            {
                if (!base.IsAdorned && (base.Item.Type.Equals("Weapon") || base.Item.Type.Equals("Armor")))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public string GetEquipmentPageName()
        {
            string text = base.Item.Name;
            if (!string.IsNullOrWhiteSpace(AlternativeName))
            {
                text = AlternativeName;
            }
            if (base.IsAdorned)
            {
                text = GetAdornedName();
            }
            if (IncludeIndividualPriceOnEquipmentPage)
            {
                return text + " (" + base.Item.DisplayPrice + ")";
            }
            return text ?? "";
        }

        public void Store(InventoryStorage storage, bool deactivate = true)
        {
            if (Storage != null)
            {
                Retrieve();
            }
            if (deactivate)
            {
                Deactivate();
            }
            Storage = storage;
            Storage.PropertyChanged += Storage_PropertyChanged;
            Storage.StoredItems.Add(this);
            IsStored = true;
            OnPropertyChanged("StorageDisplayName");
        }

        private void Storage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("StorageDisplayName");
        }

        public void Retrieve()
        {
            if (IsStored)
            {
                Storage.StoredItems.Remove(this);
                IsStored = false;
                Storage.PropertyChanged -= Storage_PropertyChanged;
                Storage = null;
                OnPropertyChanged("StorageDisplayName");
            }
        }

        private bool ExcludeEncumbrance()
        {
            if (!base.IsAdorned || !base.AdornerItem.ExcludeFromEncumbrance)
            {
                return base.Item.ExcludeFromEncumbrance;
            }
            return true;
        }

        public decimal GetWeight()
        {
            if (ExcludeEncumbrance())
            {
                Logger.Warning($"ExcludeEncumbrance: {this}");
                DisplayWeight = "—";
                return 0m;
            }
            decimal calculableWeight = base.Item.CalculableWeight;
            DisplayWeight = base.Item.DisplayWeight;
            if (base.IsAdorned && base.AdornerItem is MagicItemElement magicItemElement && magicItemElement.OverrideWeight)
            {
                calculableWeight = magicItemElement.CalculableWeight;
                DisplayWeight = magicItemElement.DisplayWeight;
            }
            if (base.Item.ItemType == "Vehicle")
            {
                Logger.Warning($"{base.Item} needs a excludeEncumbrance=\"true\" setter attribute");
            }
            decimal num = default(decimal);
            if (base.IsStackable && base.Amount > 1)
            {
                return num + calculableWeight * (decimal)base.Amount;
            }
            return num + calculableWeight;
        }
    }

}
