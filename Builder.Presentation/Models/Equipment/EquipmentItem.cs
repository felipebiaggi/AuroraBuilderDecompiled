using Builder.Core;
using Builder.Data;
using Builder.Data.Elements;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace Builder.Presentation.Models.Equipment
{
    public class EquipmentItem : ObservableObject
    {
        private string _name;

        private bool _isStackable;

        private bool _isSilvered;

        private int _amount;

        private bool _isEquippable;

        private bool _isEquipped;

        private string _equippedLocation;

        public string Identifier { get; protected set; }

        public Item Item { get; }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetProperty(ref _name, value, "Name");
                OnPropertyChanged("DisplayName");
            }
        }

        public bool IsStackable
        {
            get
            {
                return _isStackable;
            }
            set
            {
                SetProperty(ref _isStackable, value, "IsStackable");
            }
        }

        public bool IsSilvered
        {
            get
            {
                return _isSilvered;
            }
            set
            {
                SetProperty(ref _isSilvered, value, "IsSilvered");
            }
        }

        public int Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                SetProperty(ref _amount, value, "Amount");
                OnPropertyChanged("DisplayName");
            }
        }

        public bool IsEquippable
        {
            get
            {
                return _isEquippable;
            }
            set
            {
                SetProperty(ref _isEquippable, value, "IsEquippable");
            }
        }

        public bool IsEquipped
        {
            get
            {
                return _isEquipped;
            }
            set
            {
                SetProperty(ref _isEquipped, value, "IsEquipped");
                OnPropertyChanged("IsActivated");
            }
        }

        public string EquippedLocation
        {
            get
            {
                return _equippedLocation;
            }
            set
            {
                SetProperty(ref _equippedLocation, value, "EquippedLocation");
            }
        }

        public virtual string DisplayName => Name;

        public ObservableCollection<EquipmentItem> StashedItems { get; } = new EquipmentItemCollection();

        public Item AdornerItem { get; set; }

        public bool IsAdorned => AdornerItem != null;

        public EquipmentItem(Item item, Item adorner = null)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            Identifier = Guid.NewGuid().ToString();
            Item item2 = item.Copy();
            Item adornerItem = adorner?.Copy();
            Item = item2;
            AdornerItem = adornerItem;
            Name = item.Name;
            Amount = 1;
            IsStackable = item.IsStackable;
            IsEquippable = item.IsEquippable;
        }

        public override string ToString()
        {
            return Name + (IsSilvered ? " (silver)" : "") + (IsStackable ? $" ({Amount})" : "") + (Debugger.IsAttached ? (" " + Identifier) : "");
        }
    }
}
