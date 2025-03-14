using Builder.Core;
using Builder.Presentation.Events.Application;
using Builder.Presentation.Events.Shell;
using Builder.Presentation.Properties;
using Builder.Presentation.Services.Data;
using Builder.Presentation.ViewModels.Base;
using MahApps.Metro;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;


namespace Builder.Presentation.ViewModels
{
    public sealed class SettingsWindowViewModel : ViewModelBase
    {
        private string _styleSheet;

        private string _changelog;

        private string _openGamingLicence;

        private bool _startupCheckForUpdates;

        private bool _startupLoadCustomFiles;

        private SelectionItem _selectedSelectionExpanderItemsSize;

        private bool _startupCheckForContentUpdated;

        private bool _autoNavigateNextSelectionWhenAvailable;

        private string _customRootDirectory;

        private string _additionalCustomDirectory;

        private Accent _selectedAccent;

        private AppTheme _selectedTheme;

        private int _charactersCollectionSize;

        private bool _characterPanelAbilitiesIsExpanded;

        private bool _characterPanelStatisticsIsExpanded;

        private bool _characterPanelSavingThrowsIsExpanded;

        private bool _characterPanelSkillsIsExpanded;

        private bool _characterPanelQuickActionsIsExpanded;

        private bool _enableQuickSearchBar;

        private SelectionItem _selectedAbilityGenerateOption;

        private bool _useDefaultAbilityScoreMaximum;

        private string _defaultFontSize;

        private string _defaultPlayerName;

        private bool _allowEditableSheet;

        private bool _includeSpellcards;

        private bool _includeItemcards;

        private bool _flippedAbilities;

        private bool _includeAttackCards;

        private bool _includeFeatureCards;

        private bool _includeFormatting;

        private bool _useLegacySpellcastingPage;

        private bool _startItemCardsOnNewPage;

        private bool _startAttackCardsOnNewPage;

        private bool _startFeatureCardsOnNewPage;

        private bool _sheetFormattingSuffixActionsBold;

        private bool _sheetDescriptionAbbreviateUsage;

        private bool _sheetIncludeNonPreparedSpells;

        private bool _useLegacyDetailsPage;

        private bool _useLegacyBackgroundPage;

        private bool _displayRemoveLevelConfirmation;

        private bool _requestAddAttackWhenEquippingWeapon;

        public string StyleSheet
        {
            get
            {
                return _styleSheet;
            }
            set
            {
                SetProperty(ref _styleSheet, value, "StyleSheet");
            }
        }

        public string Changelog
        {
            get
            {
                return _changelog;
            }
            set
            {
                SetProperty(ref _changelog, value, "Changelog");
            }
        }

        public string OpenGamingLicence
        {
            get
            {
                return _openGamingLicence;
            }
            set
            {
                SetProperty(ref _openGamingLicence, value, "OpenGamingLicence");
            }
        }

        public string Version { get; }

        public string AssemblyVersion { get; }

        public ObservableCollection<Accent> Accents { get; } = new ObservableCollection<Accent>();

        public ObservableCollection<AppTheme> Themes { get; } = new ObservableCollection<AppTheme>();

        public ObservableCollection<string> FontSizeCollection { get; } = new ObservableCollection<string>();

        public ObservableCollection<SelectionItem> AbilitiesGenerationSelectionItems { get; } = new ObservableCollection<SelectionItem>();

        public ObservableCollection<SelectionItem> SelectionExpanderItemsSize { get; } = new ObservableCollection<SelectionItem>();

        public bool StartupCheckForUpdates
        {
            get
            {
                return _startupCheckForUpdates;
            }
            set
            {
                SetProperty(ref _startupCheckForUpdates, value, "StartupCheckForUpdates");
            }
        }

        public bool StartupLoadCustomFiles
        {
            get
            {
                return _startupLoadCustomFiles;
            }
            set
            {
                SetProperty(ref _startupLoadCustomFiles, value, "StartupLoadCustomFiles");
            }
        }

        public bool StartupCheckForContentUpdated
        {
            get
            {
                return _startupCheckForContentUpdated;
            }
            set
            {
                SetProperty(ref _startupCheckForContentUpdated, value, "StartupCheckForContentUpdated");
            }
        }

        public bool AutoNavigateNextSelectionWhenAvailable
        {
            get
            {
                return _autoNavigateNextSelectionWhenAvailable;
            }
            set
            {
                SetProperty(ref _autoNavigateNextSelectionWhenAvailable, value, "AutoNavigateNextSelectionWhenAvailable");
            }
        }

        public SelectionItem SelectedSelectionExpanderItemsSize
        {
            get
            {
                return _selectedSelectionExpanderItemsSize;
            }
            set
            {
                SetProperty(ref _selectedSelectionExpanderItemsSize, value, "SelectedSelectionExpanderItemsSize");
            }
        }

        public string CustomRootDirectory
        {
            get
            {
                return _customRootDirectory;
            }
            set
            {
                SetProperty(ref _customRootDirectory, value, "CustomRootDirectory");
            }
        }

        public string AdditionalCustomDirectory
        {
            get
            {
                return _additionalCustomDirectory;
            }
            set
            {
                SetProperty(ref _additionalCustomDirectory, value, "AdditionalCustomDirectory");
            }
        }

        public Accent SelectedAccent
        {
            get
            {
                return _selectedAccent;
            }
            set
            {
                SetProperty(ref _selectedAccent, value, "SelectedAccent");
                if (_selectedAccent != null)
                {
                    SetAccent();
                }
            }
        }

        public AppTheme SelectedTheme
        {
            get
            {
                return _selectedTheme;
            }
            set
            {
                SetProperty(ref _selectedTheme, value, "SelectedTheme");
                if (_selectedTheme != null)
                {
                    SetTheme(saveTheme: false);
                }
            }
        }

        public int CharactersCollectionSize
        {
            get
            {
                return _charactersCollectionSize;
            }
            set
            {
                SetProperty(ref _charactersCollectionSize, value, "CharactersCollectionSize");
            }
        }

        public bool CharacterPanelAbilitiesIsExpanded
        {
            get
            {
                return _characterPanelAbilitiesIsExpanded;
            }
            set
            {
                SetProperty(ref _characterPanelAbilitiesIsExpanded, value, "CharacterPanelAbilitiesIsExpanded");
            }
        }

        public bool CharacterPanelStatisticsIsExpanded
        {
            get
            {
                return _characterPanelStatisticsIsExpanded;
            }
            set
            {
                SetProperty(ref _characterPanelStatisticsIsExpanded, value, "CharacterPanelStatisticsIsExpanded");
            }
        }

        public bool CharacterPanelSavingThrowsIsExpanded
        {
            get
            {
                return _characterPanelSavingThrowsIsExpanded;
            }
            set
            {
                SetProperty(ref _characterPanelSavingThrowsIsExpanded, value, "CharacterPanelSavingThrowsIsExpanded");
            }
        }

        public bool CharacterPanelSkillsIsExpanded
        {
            get
            {
                return _characterPanelSkillsIsExpanded;
            }
            set
            {
                SetProperty(ref _characterPanelSkillsIsExpanded, value, "CharacterPanelSkillsIsExpanded");
            }
        }

        public bool CharacterPanelQuickActionsIsExpanded
        {
            get
            {
                return _characterPanelQuickActionsIsExpanded;
            }
            set
            {
                SetProperty(ref _characterPanelQuickActionsIsExpanded, value, "CharacterPanelQuickActionsIsExpanded");
            }
        }

        public bool EnableQuickSearchBar
        {
            get
            {
                return _enableQuickSearchBar;
            }
            set
            {
                SetProperty(ref _enableQuickSearchBar, value, "EnableQuickSearchBar");
            }
        }

        public SelectionItem SelectedAbilityGenerateOption
        {
            get
            {
                return _selectedAbilityGenerateOption;
            }
            set
            {
                SetProperty(ref _selectedAbilityGenerateOption, value, "SelectedAbilityGenerateOption");
            }
        }

        public bool UseDefaultAbilityScoreMaximum
        {
            get
            {
                return _useDefaultAbilityScoreMaximum;
            }
            set
            {
                SetProperty(ref _useDefaultAbilityScoreMaximum, value, "UseDefaultAbilityScoreMaximum");
            }
        }

        public string DefaultFontSize
        {
            get
            {
                return _defaultFontSize;
            }
            set
            {
                SetProperty(ref _defaultFontSize, value, "DefaultFontSize");
            }
        }

        public string DefaultPlayerName
        {
            get
            {
                return _defaultPlayerName;
            }
            set
            {
                SetProperty(ref _defaultPlayerName, value, "DefaultPlayerName");
            }
        }

        public bool AllowEditableSheet
        {
            get
            {
                return _allowEditableSheet;
            }
            set
            {
                SetProperty(ref _allowEditableSheet, value, "AllowEditableSheet");
            }
        }

        public bool FlippedAbilities
        {
            get
            {
                return _flippedAbilities;
            }
            set
            {
                SetProperty(ref _flippedAbilities, value, "FlippedAbilities");
            }
        }

        public bool IncludeSpellcards
        {
            get
            {
                return _includeSpellcards;
            }
            set
            {
                SetProperty(ref _includeSpellcards, value, "IncludeSpellcards");
            }
        }

        public bool IncludeItemcards
        {
            get
            {
                return _includeItemcards;
            }
            set
            {
                SetProperty(ref _includeItemcards, value, "IncludeItemcards");
            }
        }

        public bool IncludeAttackCards
        {
            get
            {
                return _includeAttackCards;
            }
            set
            {
                SetProperty(ref _includeAttackCards, value, "IncludeAttackCards");
            }
        }

        public bool IncludeFeatureCards
        {
            get
            {
                return _includeFeatureCards;
            }
            set
            {
                SetProperty(ref _includeFeatureCards, value, "IncludeFeatureCards");
            }
        }

        public bool IncludeFormatting
        {
            get
            {
                return _includeFormatting;
            }
            set
            {
                SetProperty(ref _includeFormatting, value, "IncludeFormatting");
            }
        }

        public bool SheetFormattingSuffixActionsBold
        {
            get
            {
                return _sheetFormattingSuffixActionsBold;
            }
            set
            {
                SetProperty(ref _sheetFormattingSuffixActionsBold, value, "SheetFormattingSuffixActionsBold");
            }
        }

        public bool UseLegacySpellcastingPage
        {
            get
            {
                return _useLegacySpellcastingPage;
            }
            set
            {
                SetProperty(ref _useLegacySpellcastingPage, value, "UseLegacySpellcastingPage");
            }
        }

        public bool StartItemCardsOnNewPage
        {
            get
            {
                return _startItemCardsOnNewPage;
            }
            set
            {
                SetProperty(ref _startItemCardsOnNewPage, value, "StartItemCardsOnNewPage");
            }
        }

        public bool StartAttackCardsOnNewPage
        {
            get
            {
                return _startAttackCardsOnNewPage;
            }
            set
            {
                SetProperty(ref _startAttackCardsOnNewPage, value, "StartAttackCardsOnNewPage");
            }
        }

        public bool StartFeatureCardsOnNewPage
        {
            get
            {
                return _startFeatureCardsOnNewPage;
            }
            set
            {
                SetProperty(ref _startFeatureCardsOnNewPage, value, "StartFeatureCardsOnNewPage");
            }
        }

        public bool SheetDescriptionAbbreviateUsage
        {
            get
            {
                return _sheetDescriptionAbbreviateUsage;
            }
            set
            {
                SetProperty(ref _sheetDescriptionAbbreviateUsage, value, "SheetDescriptionAbbreviateUsage");
            }
        }

        public bool SheetIncludeNonPreparedSpells
        {
            get
            {
                return _sheetIncludeNonPreparedSpells;
            }
            set
            {
                SetProperty(ref _sheetIncludeNonPreparedSpells, value, "SheetIncludeNonPreparedSpells");
            }
        }

        public bool UseLegacyDetailsPage
        {
            get
            {
                return _useLegacyDetailsPage;
            }
            set
            {
                SetProperty(ref _useLegacyDetailsPage, value, "UseLegacyDetailsPage");
            }
        }

        public bool UseLegacyBackgroundPage
        {
            get
            {
                return _useLegacyBackgroundPage;
            }
            set
            {
                SetProperty(ref _useLegacyBackgroundPage, value, "UseLegacyBackgroundPage");
            }
        }

        public bool DisplayRemoveLevelConfirmation
        {
            get
            {
                return _displayRemoveLevelConfirmation;
            }
            set
            {
                SetProperty(ref _displayRemoveLevelConfirmation, value, "DisplayRemoveLevelConfirmation");
            }
        }

        public bool RequestAddAttackWhenEquippingWeapon
        {
            get
            {
                return _requestAddAttackWhenEquippingWeapon;
            }
            set
            {
                SetProperty(ref _requestAddAttackWhenEquippingWeapon, value, "RequestAddAttackWhenEquippingWeapon");
            }
        }

        public RelayCommand SetDefaultsSettingsCommand => new RelayCommand(SetDefaultSettings);

        public RelayCommand ApplySettingsCommand => new RelayCommand(ApplySettings);

        public RelayCommand SaveSettingsCommand => new RelayCommand(SaveSettings);

        public RelayCommand CancelSettingsCommand => new RelayCommand(CancelSettings);

        public RelayCommand BrowseCustomRootDirectoryCommand => new RelayCommand(BrowseCustomRootDirectory);

        public RelayCommand BrowseAdditionalCustomDirectoryCommand => new RelayCommand(BrowseAdditionalCustomDirectory);

        public ICommand ClearAdditionalCustomDirectoryCommand => new RelayCommand(ClearAdditionalCustomDirectory);

        public SettingsWindowViewModel()
        {
            Version = Resources.ApplicationVersion;
            AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            bool flag = Builder.Presentation.Properties.Settings.Default.Theme.Contains("Dark");
            StyleSheet = (flag ? DataManager.Current.GetResourceWebDocument("stylesheet-dark.css") : DataManager.Current.GetResourceWebDocument("stylesheet.css"));
            OpenGamingLicence = DataManager.Current.GetResourceWebDocument("description-ogl.html");
            int[] array = new int[5] { 6, 7, 8, 9, 10 };
            foreach (int num in array)
            {
                FontSizeCollection.Add(num.ToString());
            }
            AbilitiesGenerationSelectionItems.Add(new SelectionItem("Roll 3d6", 0));
            AbilitiesGenerationSelectionItems.Add(new SelectionItem("Roll 4d6 Discard Lowest", 1));
            AbilitiesGenerationSelectionItems.Add(new SelectionItem("Standard Array", 2));
            AbilitiesGenerationSelectionItems.Add(new SelectionItem("Point Buy System", 3));
            SelectionExpanderItemsSize.Add(new SelectionItem("Small", 1));
            SelectionExpanderItemsSize.Add(new SelectionItem("Medium (Default)", 2));
            SelectionExpanderItemsSize.Add(new SelectionItem("Large", 3));
            if (base.IsInDesignMode)
            {
                InitializeDesignData();
                return;
            }
            string[] array2 = new string[10] { "Black", "Brown", "Default", "Purple", "Green", "Aqua", "Blue", "Mauve", "Pink", "Red" };
            foreach (string accentName in array2)
            {
                Accents.Add(ThemeManager.Accents.Single((Accent x) => x.Name.Equals("Aurora " + accentName, StringComparison.OrdinalIgnoreCase)));
            }
            array2 = new string[2] { "Aurora Light", "Aurora Dark" };
            foreach (string theme in array2)
            {
                Themes.Add(ThemeManager.AppThemes.Single((AppTheme x) => x.Name.Equals(theme, StringComparison.OrdinalIgnoreCase)));
            }
            PopulateProperties();
        }

        private void ClearAdditionalCustomDirectory()
        {
            AdditionalCustomDirectory = "";
        }

        private void BrowseCustomRootDirectory()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = CustomRootDirectory
            };
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                CustomRootDirectory = folderBrowserDialog.SelectedPath;
            }
        }

        private void BrowseAdditionalCustomDirectory()
        {
            _ = AdditionalCustomDirectory;
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = AdditionalCustomDirectory
            };
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                AdditionalCustomDirectory = folderBrowserDialog.SelectedPath;
            }
        }

        private void SetDefaultSettings()
        {
            Builder.Presentation.Properties.Settings.Default.Reset();
            Builder.Presentation.Properties.Settings.Default.ConfigurationUpgradeRequired = false;
            Builder.Presentation.Properties.Settings.Default.Save();
            ResetSettings();
        }

        private void ApplySettings()
        {
            Settings @default = Builder.Presentation.Properties.Settings.Default;
            @default.StartupCheckForUpdates = StartupCheckForUpdates;
            @default.StartupLoadCustomFiles = StartupLoadCustomFiles;
            @default.SelectionExpanderGridRowSize = SelectedSelectionExpanderItemsSize.Value;
            @default.AutoNavigateNextSelectionWhenAvailable = AutoNavigateNextSelectionWhenAvailable;
            @default.RequestAddAttackWhenEquippingWeapon = RequestAddAttackWhenEquippingWeapon;
            @default.DocumentsRootDirectory = CustomRootDirectory;
            @default.AdditionalCustomDirectory = AdditionalCustomDirectory;
            @default.Accent = SelectedAccent?.Name ?? "Aurora Default";
            @default.Theme = SelectedTheme.Name;
            @default.CharactersCollectionSize = CharactersCollectionSize;
            @default.CharacterPanelAbilityScoresExpanded = CharacterPanelAbilitiesIsExpanded;
            @default.CharacterPanelStatisticsExpanded = CharacterPanelStatisticsIsExpanded;
            @default.CharacterPanelSavingThrowsExpanded = CharacterPanelSavingThrowsIsExpanded;
            @default.CharacterPanelSkillsExpanded = CharacterPanelSkillsIsExpanded;
            @default.CharacterPanelQuickActionsExpanded = CharacterPanelQuickActionsIsExpanded;
            @default.QuickSearchBarEnabled = EnableQuickSearchBar;
            @default.DisplayRemoveLevelConfirmation = DisplayRemoveLevelConfirmation;
            @default.StartupCheckForContentUpdated = StartupCheckForContentUpdated;
            @default.AbilitiesGenerationOption = SelectedAbilityGenerateOption.Value;
            @default.UseDefaultAbilityScoreMaximum = UseDefaultAbilityScoreMaximum;
            @default.PlayerName = DefaultPlayerName;
            @default.DefaultFontSize = DefaultFontSize;
            @default.AllowEditableSheet = AllowEditableSheet;
            @default.CharacterSheetAbilitiesFlipped = FlippedAbilities;
            @default.IncludeSpellcards = IncludeSpellcards;
            @default.IncludeItemcards = IncludeItemcards;
            @default.SheetIncludeAttackCards = IncludeAttackCards;
            @default.SheetIncludeFeatureCards = IncludeFeatureCards;
            @default.SheetIncludeFormatting = IncludeFormatting;
            @default.SheetFormattingActionSuffixBold = SheetFormattingSuffixActionsBold;
            @default.UseLegacyDetailsPage = UseLegacyDetailsPage;
            @default.UseLegactBackgroundPage = UseLegacyBackgroundPage;
            @default.UseLegacySpellcastingPage = UseLegacySpellcastingPage;
            @default.SheetStartItemCardsOnNewPage = StartItemCardsOnNewPage;
            @default.SheetStartAttackCardsOnNewPage = StartAttackCardsOnNewPage;
            @default.SheetStartFeatureCardsOnNewPage = StartFeatureCardsOnNewPage;
            @default.SheetDescriptionAbbreviateUsage = SheetDescriptionAbbreviateUsage;
            @default.SheetIncludeNonPreparedSpells = SheetIncludeNonPreparedSpells;
            @default.Save();
            ApplicationManager.Current.EventAggregator.Send(new SettingsChangedEvent());
            SetAccent();
            SetTheme(saveTheme: true);
        }

        private void SaveSettings()
        {
            ApplySettings();
            base.EventAggregator.Send(new MainWindowStatusUpdateEvent("Your preferences have been saved."));
        }

        private void CancelSettings()
        {
            ResetSettings();
            ApplySettings();
        }

        private void ResetSettings()
        {
            Builder.Presentation.Properties.Settings.Default.Reload();
            PopulateProperties();
        }

        private void SetAccent()
        {
            ApplicationManager.Current.SetAccent(SelectedAccent?.Name);
        }

        private void SetTheme(bool saveTheme)
        {
            ApplicationManager.Current.SetTheme(SelectedTheme.Name, saveTheme);
        }

        private void PopulateProperties()
        {
            Settings settings = Builder.Presentation.Properties.Settings.Default;
            StartupCheckForUpdates = settings.StartupCheckForUpdates;
            StartupLoadCustomFiles = settings.StartupLoadCustomFiles;
            SelectedSelectionExpanderItemsSize = SelectionExpanderItemsSize.FirstOrDefault((SelectionItem x) => x.Value == settings.SelectionExpanderGridRowSize) ?? SelectionExpanderItemsSize.FirstOrDefault((SelectionItem x) => x.Value == 2);
            AutoNavigateNextSelectionWhenAvailable = settings.AutoNavigateNextSelectionWhenAvailable;
            CustomRootDirectory = settings.DocumentsRootDirectory;
            AdditionalCustomDirectory = settings.AdditionalCustomDirectory;
            DisplayRemoveLevelConfirmation = settings.DisplayRemoveLevelConfirmation;
            RequestAddAttackWhenEquippingWeapon = settings.RequestAddAttackWhenEquippingWeapon;
            StartupCheckForContentUpdated = settings.StartupCheckForContentUpdated;
            SelectedTheme = Themes.FirstOrDefault((AppTheme x) => x.Name.Equals(settings.Theme));
            SelectedAccent = Accents.FirstOrDefault((Accent x) => x.Name.Equals(settings.Accent));
            if (SelectedAccent == null)
            {
                SelectedAccent = Accents.FirstOrDefault((Accent x) => x.Name.Equals("aurora default", StringComparison.OrdinalIgnoreCase));
            }
            CharactersCollectionSize = settings.CharactersCollectionSize;
            CharacterPanelAbilitiesIsExpanded = settings.CharacterPanelAbilityScoresExpanded;
            CharacterPanelStatisticsIsExpanded = settings.CharacterPanelStatisticsExpanded;
            CharacterPanelSavingThrowsIsExpanded = settings.CharacterPanelSavingThrowsExpanded;
            CharacterPanelSkillsIsExpanded = settings.CharacterPanelSkillsExpanded;
            CharacterPanelQuickActionsIsExpanded = settings.CharacterPanelQuickActionsExpanded;
            EnableQuickSearchBar = settings.QuickSearchBarEnabled;
            SelectedAbilityGenerateOption = AbilitiesGenerationSelectionItems.FirstOrDefault((SelectionItem x) => x.Value == settings.AbilitiesGenerationOption);
            UseDefaultAbilityScoreMaximum = settings.UseDefaultAbilityScoreMaximum;
            DefaultPlayerName = settings.PlayerName;
            DefaultFontSize = settings.DefaultFontSize;
            AllowEditableSheet = settings.AllowEditableSheet;
            FlippedAbilities = settings.CharacterSheetAbilitiesFlipped;
            IncludeSpellcards = settings.IncludeSpellcards;
            IncludeItemcards = settings.IncludeItemcards;
            IncludeAttackCards = settings.SheetIncludeAttackCards;
            IncludeFeatureCards = settings.SheetIncludeFeatureCards;
            IncludeFormatting = settings.SheetIncludeFormatting;
            SheetFormattingSuffixActionsBold = settings.SheetFormattingActionSuffixBold;
            UseLegacyDetailsPage = settings.UseLegacyDetailsPage;
            UseLegacyBackgroundPage = settings.UseLegactBackgroundPage;
            UseLegacySpellcastingPage = settings.UseLegacySpellcastingPage;
            StartItemCardsOnNewPage = settings.SheetStartItemCardsOnNewPage;
            StartAttackCardsOnNewPage = settings.SheetStartAttackCardsOnNewPage;
            StartFeatureCardsOnNewPage = settings.SheetStartFeatureCardsOnNewPage;
            SheetDescriptionAbbreviateUsage = settings.SheetDescriptionAbbreviateUsage;
            SheetIncludeNonPreparedSpells = settings.SheetIncludeNonPreparedSpells;
        }

        protected override void InitializeDesignData()
        {
            StartupLoadCustomFiles = true;
            string[] array = new string[10] { "Black", "Brown", "Default", "Purple", "Green", "Aqua", "Blue", "Mauve", "Pink", "Red" };
            foreach (string text in array)
            {
                Accents.Add(new Accent("Aurora " + text, new Uri("pack://application:,,,/Aurora.Presentation;component/Styles/Accents/" + text + ".xaml")));
            }
            Themes.Add(new AppTheme("Aurora Light", new Uri("pack://application:,,,/Aurora.Presentation;component/Styles/Theme/AuroraLight.xaml")));
            Themes.Add(new AppTheme("Aurora Dark", new Uri("pack://application:,,,/Aurora.Presentation;component/Styles/Theme/AuroraDark.xaml")));
            CharactersCollectionSize = 150;
            CharacterPanelAbilitiesIsExpanded = true;
            CharacterPanelQuickActionsIsExpanded = true;
            SelectedAbilityGenerateOption = AbilitiesGenerationSelectionItems.FirstOrDefault();
            DefaultFontSize = "8";
            IncludeSpellcards = true;
            base.InitializeDesignData();
        }
    }
}
