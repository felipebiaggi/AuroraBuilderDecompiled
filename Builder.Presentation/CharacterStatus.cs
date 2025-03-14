using System;
using Builder.Core;
using Builder.Presentation;


namespace Builder.Presentation
{
    public class CharacterStatus : ObservableObject
    {
        private bool _isNew;

        private bool _isLoaded;

        private bool _hasChanges;

        private bool _isUserPortrait;

        private bool _hasMainClass;

        private bool _canMulticlass;

        private bool _hasMulticlass;

        private bool _canLevelUp = true;

        private bool _canLevelDown;

        private bool _hasSpellcasting;

        private bool _hasMulticlassSpellSlots;

        private bool _hasCompanion;

        private bool _hasDragonmark;

        public bool IsNew
        {
            get
            {
                return _isNew;
            }
            set
            {
                SetProperty(ref _isNew, value, "IsNew");
                OnStatusChanged();
            }
        }

        public bool IsLoaded
        {
            get
            {
                return _isLoaded;
            }
            set
            {
                SetProperty(ref _isLoaded, value, "IsLoaded");
                OnStatusChanged();
            }
        }

        public bool HasChanges
        {
            get
            {
                return _hasChanges;
            }
            set
            {
                SetProperty(ref _hasChanges, value, "HasChanges");
                OnStatusChanged();
            }
        }

        public bool IsUserPortrait
        {
            get
            {
                return _isUserPortrait;
            }
            set
            {
                SetProperty(ref _isUserPortrait, value, "IsUserPortrait");
                OnStatusChanged();
            }
        }

        public bool HasMainClass
        {
            get
            {
                return _hasMainClass;
            }
            set
            {
                SetProperty(ref _hasMainClass, value, "HasMainClass");
                OnStatusChanged();
            }
        }

        public bool CanMulticlass
        {
            get
            {
                return _canMulticlass;
            }
            set
            {
                SetProperty(ref _canMulticlass, value, "CanMulticlass");
                OnStatusChanged();
            }
        }

        public bool HasMulticlass
        {
            get
            {
                return _hasMulticlass;
            }
            set
            {
                SetProperty(ref _hasMulticlass, value, "HasMulticlass");
                OnStatusChanged();
            }
        }

        public bool CanLevelUp
        {
            get
            {
                return _canLevelUp;
            }
            set
            {
                SetProperty(ref _canLevelUp, value, "CanLevelUp");
                OnStatusChanged();
            }
        }

        public bool CanLevelDown
        {
            get
            {
                return _canLevelDown;
            }
            set
            {
                SetProperty(ref _canLevelDown, value, "CanLevelDown");
                OnStatusChanged();
            }
        }

        public bool HasSpellcasting
        {
            get
            {
                return _hasSpellcasting;
            }
            set
            {
                SetProperty(ref _hasSpellcasting, value, "HasSpellcasting");
                OnStatusChanged();
            }
        }

        public bool HasMulticlassSpellSlots
        {
            get
            {
                return _hasMulticlassSpellSlots;
            }
            set
            {
                SetProperty(ref _hasMulticlassSpellSlots, value, "HasMulticlassSpellSlots");
                OnStatusChanged();
            }
        }

        public bool HasCompanion
        {
            get
            {
                return _hasCompanion;
            }
            set
            {
                SetProperty(ref _hasCompanion, value, "HasCompanion");
                OnStatusChanged();
            }
        }

        public bool HasDragonmark
        {
            get
            {
                return _hasDragonmark;
            }
            set
            {
                SetProperty(ref _hasDragonmark, value, "HasDragonmark");
            }
        }

        public event EventHandler<CharacterStatusChangedEventArgs> StatusChanged;

        protected virtual void OnStatusChanged()
        {
            this.StatusChanged?.Invoke(this, new CharacterStatusChangedEventArgs(CharacterManager.Current.Character, this));
        }
    }
}
