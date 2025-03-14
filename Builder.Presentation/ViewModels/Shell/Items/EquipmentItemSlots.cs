using Builder.Core;
using System;
using System.Linq;

namespace Builder.Presentation.ViewModels.Shell.Items
{
    [Obsolete]
    public class EquipmentItemSlots : ObservableObject
    {
        private readonly CharacterManager _manager;

        private RefactoredEquipmentItem _equippedArmor;

        private RefactoredEquipmentItem _equippedPrimary;

        private RefactoredEquipmentItem _equippedSecondary;

        public RefactoredEquipmentItem EquippedArmor
        {
            get
            {
                return _equippedArmor;
            }
            private set
            {
                SetProperty(ref _equippedArmor, value, "EquippedArmor");
                _ = _equippedArmor;
                OnEquippedChanged();
            }
        }

        public RefactoredEquipmentItem EquippedPrimary
        {
            get
            {
                return _equippedPrimary;
            }
            private set
            {
                SetProperty(ref _equippedPrimary, value, "EquippedPrimary");
                OnPropertyChanged("IsEquippedVersatile");
                OnEquippedChanged();
            }
        }

        public RefactoredEquipmentItem EquippedSecondary
        {
            get
            {
                return _equippedSecondary;
            }
            private set
            {
                SetProperty(ref _equippedSecondary, value, "EquippedSecondary");
                OnPropertyChanged("IsEquippedVersatile");
                OnPropertyChanged("IsEquippedShield");
                OnEquippedChanged();
            }
        }

        public bool IsEquippedTwoHander
        {
            get
            {
                if (EquippedPrimary != null && EquippedSecondary != null)
                {
                    return EquippedPrimary.Identifier.Equals(EquippedSecondary.Identifier);
                }
                return false;
            }
        }

        public bool IsEquippedVersatile
        {
            get
            {
                if (EquippedPrimary != null && EquippedSecondary != null)
                {
                    return EquippedPrimary.Identifier.Equals(EquippedSecondary.Identifier);
                }
                return false;
            }
        }

        public bool IsEquippedShield
        {
            get
            {
                if (EquippedSecondary != null)
                {
                    if (EquippedSecondary.Item.ElementSetters.ContainsSetter("armor"))
                    {
                        return EquippedSecondary.Item.ElementSetters.GetSetter("armor").Value.Equals("shield", StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                }
                return false;
            }
        }

        public event EventHandler EquippedChanged;

        public EquipmentItemSlots()
        {
            _manager = CharacterManager.Current;
        }

        public void EquipWeapon(RefactoredEquipmentItem equipment, bool versatile = false)
        {
            string text = (equipment.Item.HasMultipleSlots ? equipment.Item.Slots.FirstOrDefault() : equipment.Item.Slot);
            switch (text)
            {
                default:
                    _ = text == "twohand";
                    break;
                case "armor":
                case "body":
                    EquippedArmor = equipment;
                    break;
                case "onehand":
                    break;
            }
        }

        private void Equip(RefactoredEquipmentItem equipment)
        {
            if (equipment != null)
            {
                _manager.RegisterElement(equipment.Item);
                if (equipment.IsAdorned)
                {
                    _manager.RegisterElement(equipment.AdornerItem);
                }
                equipment.IsEquipped = true;
                equipment.IsAttuned = true;
            }
        }

        private void Unequip(RefactoredEquipmentItem equipment)
        {
            if (equipment != null)
            {
                if (equipment.IsAdorned)
                {
                    _manager.UnregisterElement(equipment.AdornerItem);
                }
                _manager.UnregisterElement(equipment.Item);
                equipment.EquippedLocation = string.Empty;
                equipment.IsEquipped = false;
                equipment.IsAttuned = false;
            }
        }

        protected virtual void OnEquippedChanged()
        {
            this.EquippedChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
