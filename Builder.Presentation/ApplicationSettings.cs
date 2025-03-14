using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Builder.Core;
using Builder.Core.Events;
using Builder.Core.Logging;
using Builder.Presentation.Commands.Settings;
using Builder.Presentation.Events.Application;
using Builder.Presentation.Properties;
using Builder.Presentation.Services;
using Builder.Presentation.Telemetry;
using Builder.Presentation.ViewModels;

namespace Builder.Presentation
{
    public class ApplicationSettings : ObservableObject, ISubscriber<SettingsChangedEvent>
    {
        private readonly IEventAggregator _eventAggregator;

        private bool _isSaveSettingsOnChangeEnabled;

        private bool _characterSheetAbilitiesFlipped;

        internal Settings Settings { get; }

        public bool IsSaveSettingsOnChangeEnabled
        {
            get
            {
                return _isSaveSettingsOnChangeEnabled;
            }
            set
            {
                SetProperty(ref _isSaveSettingsOnChangeEnabled, value, "IsSaveSettingsOnChangeEnabled");
            }
        }

        public ICommand Set3d6GenerateOptionCommand => new RelayCommand(Set3d6GenerateOption);

        public ICommand Set4d6GenerateOptionCommand => new RelayCommand(Set4d6GenerateOption);

        public ICommand SetPointBuyGenerateOptionCommand => new RelayCommand(SetPointBuyGenerateOption);

        public ICommand SetArrayGenerateOptionCommand => new RelayCommand(SetArrayGenerateOption);

        public ICommand ToggleAllowEditableSheetCommand => new RelayCommand(ToggleAllowEditableSheet);

        public ICommand ToggleAutoGenerateSheetCommand => new RelayCommand(ToggleAutoGenerateSheet);

        public ICommand ToggleAutoSelectionNavigationCommand => new RelayCommand(ToggleAutoSelectionNavigation);

        public ObservableCollection<SelectionItem> AbilitiesGenerationSelectionItems { get; } = new ObservableCollection<SelectionItem>();

        public bool IncludeItemcards
        {
            get
            {
                return Settings.IncludeItemcards;
            }
            set
            {
                Settings.IncludeItemcards = value;
                OnPropertyChanged("IncludeItemcards");
                Save();
            }
        }

        public bool AllowEditableSheet
        {
            get
            {
                return Settings.AllowEditableSheet;
            }
            set
            {
                Settings.AllowEditableSheet = value;
                OnPropertyChanged("AllowEditableSheet");
                Save();
            }
        }

        public bool CharacterSheetAbilitiesFlipped
        {
            get
            {
                return Settings.CharacterSheetAbilitiesFlipped;
            }
            set
            {
                Settings.CharacterSheetAbilitiesFlipped = value;
                OnPropertyChanged("CharacterSheetAbilitiesFlipped");
                Save();
            }
        }

        public bool CharacterSheetIncludeSpellCards
        {
            get
            {
                return Settings.IncludeSpellcards;
            }
            set
            {
                Settings.IncludeSpellcards = value;
                OnPropertyChanged("CharacterSheetIncludeSpellCards");
                Save();
            }
        }

        public string PlayerName
        {
            get
            {
                return Settings.PlayerName;
            }
            set
            {
                Settings.PlayerName = value;
                OnPropertyChanged("PlayerName");
            }
        }

        public bool GenerateSheetOnCharacterChangedRegistered
        {
            get
            {
                return Settings.GenerateSheetOnCharacterChangedRegistered;
            }
            set
            {
                Settings.GenerateSheetOnCharacterChangedRegistered = value;
                OnPropertyChanged("GenerateSheetOnCharacterChangedRegistered");
            }
        }

        public bool AutoNavigateNextSelectionWhenAvailable
        {
            get
            {
                return Settings.AutoNavigateNextSelectionWhenAvailable;
            }
            set
            {
                Settings.AutoNavigateNextSelectionWhenAvailable = value;
                OnPropertyChanged("AutoNavigateNextSelectionWhenAvailable");
            }
        }

        public bool StartupCheckForContentUpdated
        {
            get
            {
                return Settings.StartupCheckForContentUpdated;
            }
            set
            {
                Settings.StartupCheckForContentUpdated = value;
                OnPropertyChanged("StartupCheckForContentUpdated");
                Save();
            }
        }

        public ICommand AcitvateDarkThemeCommand => new ActivateDarkThemeCommand();

        public ICommand ActivateLightThemeCommand => new ActivateLightThemeCommand();

        public ICommand ActivateDefaultAccentCommand => new ActivateDefaultAccentCommand();

        public ApplicationSettings(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            AbilitiesGenerationSelectionItems.Add(new SelectionItem("Roll 3d6", 0));
            AbilitiesGenerationSelectionItems.Add(new SelectionItem("Roll 4d6 - Discard Lowest", 1));
            AbilitiesGenerationSelectionItems.Add(new SelectionItem("Standard Array (15, 14, 13, 12, 10, 8)", 2));
            AbilitiesGenerationSelectionItems.Add(new SelectionItem("Point Buy", 3));
            Settings = Settings.Default;
            _eventAggregator.Subscribe(this);
        }

        private void Set3d6GenerateOption()
        {
            Settings.AbilitiesGenerationOption = 0;
            Save();
        }

        private void Set4d6GenerateOption()
        {
            Settings.AbilitiesGenerationOption = 1;
            Save();
        }

        private void SetPointBuyGenerateOption()
        {
            Settings.AbilitiesGenerationOption = 3;
            Save();
        }

        private void SetArrayGenerateOption()
        {
            Settings.AbilitiesGenerationOption = 2;
            Save();
        }

        private void ToggleAllowEditableSheet()
        {
            AllowEditableSheet = !AllowEditableSheet;
            Save();
        }

        private void ToggleAutoGenerateSheet()
        {
            GenerateSheetOnCharacterChangedRegistered = !GenerateSheetOnCharacterChangedRegistered;
            Save();
        }

        private void ToggleAutoSelectionNavigation()
        {
            AutoNavigateNextSelectionWhenAvailable = !AutoNavigateNextSelectionWhenAvailable;
            Save();
        }

        public int GetSelectionExpanderGridRowHeight()
        {
            int result = 21;
            try
            {
                switch ((ContentSize)Settings.SelectionExpanderGridRowSize)
                {
                    case ContentSize.Small:
                        result = 17;
                        break;
                    case ContentSize.Medium:
                        result = 21;
                        break;
                    case ContentSize.Large:
                        result = 25;
                        break;
                    default:
                        result = 21;
                        Settings.SelectionExpanderGridRowSize = 2;
                        Save();
                        break;
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "GetSelectionExpanderGridRowHeight");
                return result;
            }
        }

        public void Save(bool raiseSettingsChanged = true)
        {
            try
            {
                Settings.Save();
                if (raiseSettingsChanged)
                {
                    RaiseSettingsChanged();
                }
            }
            catch (ArgumentException ex)
            {
                Logger.Exception(ex, "Save");
                AnalyticsErrorHelper.Exception(ex, null, null, "Save", 269);
            }
            catch (Exception ex2)
            {
                Logger.Exception(ex2, "Save");
                AnalyticsErrorHelper.Exception(ex2, null, null, "Save", 274);
                MessageDialogService.ShowException(ex2);
            }
        }

        public void Reload()
        {
            Settings.Reload();
        }

        public void Reset()
        {
            Settings.Reset();
        }

        public void RaiseSettingsChanged()
        {
            _eventAggregator.Send(new SettingsChangedEvent());
        }

        void ISubscriber<SettingsChangedEvent>.OnHandleEvent(SettingsChangedEvent args)
        {
            OnPropertyChanged("CharacterSheetAbilitiesFlipped");
            OnPropertyChanged("CharacterSheetIncludeSpellCards");
            OnPropertyChanged("StartupCheckForContentUpdated");
        }
    }

}
