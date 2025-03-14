using Builder.Presentation.ViewModels.Base;

namespace Builder.Presentation.ViewModels.Shell
{
    public class StartSectionViewModel : ViewModelBase
    {
        private bool _isStartSectionEnabled;

        private bool _isBuildSectionEnabled;

        private bool _isMagicSectionEnabled;

        private bool _isEquipmentSectionEnabled;

        private bool _isManageCharacterSectionEnabled;

        private bool _isCharacterSheetSectionEnabled;

        public bool IsStartSectionEnabled
        {
            get
            {
                return _isStartSectionEnabled;
            }
            set
            {
                SetProperty(ref _isStartSectionEnabled, value, "IsStartSectionEnabled");
            }
        }

        public bool IsBuildSectionEnabled
        {
            get
            {
                return _isBuildSectionEnabled;
            }
            set
            {
                SetProperty(ref _isBuildSectionEnabled, value, "IsBuildSectionEnabled");
            }
        }

        public bool IsMagicSectionEnabled
        {
            get
            {
                return _isMagicSectionEnabled;
            }
            set
            {
                SetProperty(ref _isMagicSectionEnabled, value, "IsMagicSectionEnabled");
            }
        }

        public bool IsEquipmentSectionEnabled
        {
            get
            {
                return _isEquipmentSectionEnabled;
            }
            set
            {
                SetProperty(ref _isEquipmentSectionEnabled, value, "IsEquipmentSectionEnabled");
            }
        }

        public bool IsManageCharacterSectionEnabled
        {
            get
            {
                return _isManageCharacterSectionEnabled;
            }
            set
            {
                SetProperty(ref _isManageCharacterSectionEnabled, value, "IsManageCharacterSectionEnabled");
            }
        }

        public bool IsCharacterSheetSectionEnabled
        {
            get
            {
                return _isCharacterSheetSectionEnabled;
            }
            set
            {
                SetProperty(ref _isCharacterSheetSectionEnabled, value, "IsCharacterSheetSectionEnabled");
            }
        }

        public StartSectionViewModel()
        {
            _isStartSectionEnabled = false;
            _isBuildSectionEnabled = false;
            _isMagicSectionEnabled = false;
            _isEquipmentSectionEnabled = false;
            _isManageCharacterSectionEnabled = false;
            _isCharacterSheetSectionEnabled = false;
        }
    }
}
