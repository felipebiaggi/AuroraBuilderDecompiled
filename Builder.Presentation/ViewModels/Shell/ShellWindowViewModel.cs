using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Builder.Core;
using Builder.Core.Events;
using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Rules;
using Builder.Presentation;
using Builder.Presentation.Commands;
using Builder.Presentation.Data;
using Builder.Presentation.Events.Application;
using Builder.Presentation.Events.Character;
using Builder.Presentation.Events.Shell;
using Builder.Presentation.Extensions;
using Builder.Presentation.Models;
using Builder.Presentation.Models.Collections;
using Builder.Presentation.Properties;
using Builder.Presentation.Services;
using Builder.Presentation.Services.Calculator;
using Builder.Presentation.Services.Data;
using Builder.Presentation.Telemetry;
using Builder.Presentation.ViewModels.Base;
using Builder.Presentation.ViewModels.Shell;
using Builder.Presentation.ViewModels.Shell.Manage;
using Builder.Presentation.Views;
//using Builder.Presentation.Views.Dialogs;
using Builder.Presentation.Views.Sliders;
using Microsoft.Win32;

namespace Builder.Presentation.ViewModels.Shell
{
    public sealed class ShellWindowViewModel : ViewModelBase, ISubscriber<MainWindowStatusUpdateEvent>, ISubscriber<CharacterNameChangedEvent>, ISubscriber<CharacterBuildChangedEvent>, ISubscriber<CharacterManagerElementRegistered>, ISubscriber<CharacterManagerElementUnregistered>, ISubscriber<CharacterSavedEvent>, ISubscriber<SettingsChangedEvent>, ISubscriber<AdditionalContentUpdatedEvent>
    {
        private string _statusMessage;

        private int _progressPercentage;

        private bool _isProgressVisible;

        private bool _isProgressIndeterminate;

        private bool _enableDeveloperTools;

        private bool _updateAvailable;

        private string _characterName;

        private string _characterBuild;

        private string _characterPortraitUri;

        private CharacterFile _selectedCharacter;

        private string _loadedFilepath;

        private bool _isCharacterLoaded;

        private bool _applicationInitialized;

        private int _listViewItemSize = 150;

        private bool _isCharacterInformationBlockVisible;

        private string _characterSummery;

        private string _updateNotificationContent = "1";

        private string _characterBackgroundBuildString;

        private string _characterAlignmentBuildString;

        private bool _showDonateButton = true;

        private bool _isCharacterLoadedFully;

        private string _updatesHeader = "NO UPDATE AVAILABLE";

        private string _characterLevelBuild;

        private string _characterClassBuild;

        private bool _isQuickSearchBarEnabled;

        private bool _isContentUpdated;

        private string _contentUpdatedMessage;

        public SelectionRuleNavigationService SelectionRuleNavigationService { get; }

        public SpellContentViewModel SpellContentViewModel { get; set; } = new SpellContentViewModel();

        //public CharacterInformationSliderViewModel CharacterInformationSliderViewModel { get; }

        public string CharacterSummery
        {
            get
            {
                return _characterSummery;
            }
            set
            {
                SetProperty(ref _characterSummery, value, "CharacterSummery");
            }
        }

        public string Version => Resources.ApplicationVersion;

        public bool EnableDeveloperTools
        {
            get
            {
                return _enableDeveloperTools;
            }
            set
            {
                SetProperty(ref _enableDeveloperTools, value, "EnableDeveloperTools");
            }
        }

        public bool UpdateAvailable
        {
            get
            {
                return _updateAvailable;
            }
            set
            {
                SetProperty(ref _updateAvailable, value, "UpdateAvailable");
            }
        }

        public string UpdateNotificationContent
        {
            get
            {
                return _updateNotificationContent;
            }
            set
            {
                SetProperty(ref _updateNotificationContent, value, "UpdateNotificationContent");
            }
        }

        public string StatusMessage
        {
            get
            {
                return _statusMessage;
            }
            set
            {
                SetProperty(ref _statusMessage, value, "StatusMessage");
            }
        }

        public int ProgressPercentage
        {
            get
            {
                return _progressPercentage;
            }
            set
            {
                SetProperty(ref _progressPercentage, value, "ProgressPercentage");
            }
        }

        public bool IsProgressVisible
        {
            get
            {
                return _isProgressVisible;
            }
            set
            {
                SetProperty(ref _isProgressVisible, value, "IsProgressVisible");
            }
        }

        public bool IsProgressIndeterminate
        {
            get
            {
                return _isProgressIndeterminate;
            }
            set
            {
                SetProperty(ref _isProgressIndeterminate, value, "IsProgressIndeterminate");
            }
        }

        public ObservableCollection<CharacterFile> Characters { get; } = new ObservableCollection<CharacterFile>();

        public CharacterFile SelectedCharacter
        {
            get
            {
                return _selectedCharacter;
            }
            set
            {
                SetProperty(ref _selectedCharacter, value, "SelectedCharacter");
            }
        }

        public string LoadedFilepath
        {
            get
            {
                return _loadedFilepath;
            }
            set
            {
                SetProperty(ref _loadedFilepath, value, "LoadedFilepath");
            }
        }

        public bool IsCharacterLoaded
        {
            get
            {
                return _isCharacterLoaded;
            }
            set
            {
                SetProperty(ref _isCharacterLoaded, value, "IsCharacterLoaded");
            }
        }

        public Character Character => CharacterManager.Current.Character;

        public CharacterManager CharacterManager => CharacterManager.Current;

        public string CharacterName
        {
            get
            {
                return _characterName;
            }
            set
            {
                SetProperty(ref _characterName, value, "CharacterName");
            }
        }

        public string CharacterBuild
        {
            get
            {
                return _characterBuild;
            }
            private set
            {
                SetProperty(ref _characterBuild, value, "CharacterBuild");
            }
        }

        public string CharacterPortraitUri
        {
            get
            {
                return _characterPortraitUri;
            }
            set
            {
                SetProperty(ref _characterPortraitUri, value, "CharacterPortraitUri");
            }
        }

        public bool ApplicationInitialized
        {
            get
            {
                return _applicationInitialized;
            }
            set
            {
                SetProperty(ref _applicationInitialized, value, "ApplicationInitialized");
            }
        }

        public int ListViewItemSize
        {
            get
            {
                return _listViewItemSize;
            }
            set
            {
                SetProperty(ref _listViewItemSize, value, "ListViewItemSize");
            }
        }

        public bool IsCharacterInformationBlockVisible
        {
            get
            {
                return _isCharacterInformationBlockVisible;
            }
            set
            {
                SetProperty(ref _isCharacterInformationBlockVisible, value, "IsCharacterInformationBlockVisible");
            }
        }

        public string CharacterBackgroundBuildString
        {
            get
            {
                return _characterBackgroundBuildString;
            }
            set
            {
                SetProperty(ref _characterBackgroundBuildString, value, "CharacterBackgroundBuildString");
            }
        }

        public string CharacterAlignmentBuildString
        {
            get
            {
                return _characterAlignmentBuildString;
            }
            set
            {
                SetProperty(ref _characterAlignmentBuildString, value, "CharacterAlignmentBuildString");
            }
        }

        public bool ShowDonateButton
        {
            get
            {
                return _showDonateButton;
            }
            set
            {
                SetProperty(ref _showDonateButton, value, "ShowDonateButton");
            }
        }

        public ICommand NewCharacterCommand => new RelayCommand(NewCharacter);

        public ICommand SaveCommand => new RelayCommand(SaveCharacter);

        public ICommand LoadCommand => new RelayCommand(LoadCharacter);

        public ICommand DeleteCommand => new RelayCommand(DeleteCharacter);

        public ICommand LevelUpCommand => new RelayCommand(LevelUp);

        public ICommand LevelDownCommand => new RelayCommand(LevelDown);

        public ICommand ChangePortraitCommand => new RelayCommand(ChangePortrait);

        public ICommand ToggleFavoriteCharacterCommand => new RelayCommand(ToggleFavoriteCharacter);

        public ICommand UpdateCommand => new RelayCommand(Update);

        public ICommand JumpListSettingsCommand => new RelayCommand(JumpListSettings);

        public ICommand CreateSummeryCommand => new RelayCommand(CreateSummery);

        public ICommand CreateCharacterSheetCommand => new RelayCommand(CreateCharacterSheet);

        public ICommand OpenCustomDocumentsFolderCommand => new RelayCommand(OpenCustomDocumentsFolder);

        public ICommand OpenCharacterFileCommand => new RelayCommand<string>(OpenCharacterFile);

        public bool IsCharacterLoadedFully
        {
            get
            {
                return _isCharacterLoadedFully;
            }
            set
            {
                SetProperty(ref _isCharacterLoadedFully, value, "IsCharacterLoadedFully");
            }
        }

        public string UpdatesHeader
        {
            get
            {
                return _updatesHeader;
            }
            set
            {
                SetProperty(ref _updatesHeader, value, "UpdatesHeader");
            }
        }

        public string CharacterLevelBuild
        {
            get
            {
                return _characterLevelBuild;
            }
            set
            {
                SetProperty(ref _characterLevelBuild, value, "CharacterLevelBuild");
            }
        }

        public string CharacterClassBuild
        {
            get
            {
                return _characterClassBuild;
            }
            set
            {
                SetProperty(ref _characterClassBuild, value, "CharacterClassBuild");
            }
        }

        public ICommand CharacterSheetPreviousPageCommand => new RelayCommand(CharacterSheetPreviousPage);

        public ICommand CharacterSheetNextPageCommand => new RelayCommand(CharacterSheetNextPage);

        public bool IsQuickSearchBarEnabled
        {
            get
            {
                return _isQuickSearchBarEnabled;
            }
            set
            {
                SetProperty(ref _isQuickSearchBarEnabled, value, "IsQuickSearchBarEnabled");
            }
        }

        public ICommand SaveDocumentAsCommand => new RelayCommand(SaveDocumentAs);

        public bool IsContentUpdated
        {
            get
            {
                return _isContentUpdated;
            }
            set
            {
                SetProperty(ref _isContentUpdated, value, "IsContentUpdated");
            }
        }

        public string ContentUpdatedMessage
        {
            get
            {
                return _contentUpdatedMessage;
            }
            set
            {
                SetProperty(ref _contentUpdatedMessage, value, "ContentUpdatedMessage");
            }
        }

        public ICommand RestartCommand => new RelayCommand(RestartAfterContentUpdated);

        public ICommand EditGroupCommand => new RelayCommand<object>(EditGroup);

        public ICommand EditCharacterGroupCommand => new RelayCommand<object>(EditCharacterGroup);

        public ICommand ApplyRestrictionsCommand { get; }

        public ICommand OpenDeveloperToolsCommand => new RelayCommand(OpenDeveloperTools);

        public ICommand OpenToolsCommand => new RelayCommand(OpenTools);

        public ShellWindowViewModel()
        {
            SelectionRuleNavigationService = new SelectionRuleNavigationService(base.EventAggregator);
            StatusMessage = "READY";
            CollectionView obj = (CollectionView)CollectionViewSource.GetDefaultView(Characters);
            PropertyGroupDescription item = new PropertyGroupDescription("CollectionGroupName");
            obj.GroupDescriptions.Add(item);
            if (base.IsInDesignMode)
            {
                InitializeDesignData();
                return;
            }
            // CharacterInformationSliderViewModel = new CharacterInformationSliderViewModel(this);
            if (ApplicationManager.Current.IsInDeveloperMode)
            {
                EnableDeveloperTools = true;
            }
            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(DataManager.Current.UserDocumentsRootDirectory);
            fileSystemWatcher.Created += _watcher_Created;
            fileSystemWatcher.Changed += _watcher_Changed;
            fileSystemWatcher.Deleted += _watcher_Deleted;
            fileSystemWatcher.Filter = "*.dnd5e";
            fileSystemWatcher.EnableRaisingEvents = true;
            LoadedFilepath = "no character file loaded";
            IsCharacterInformationBlockVisible = true;
            ListViewItemSize = Builder.Presentation.Properties.Settings.Default.CharactersCollectionSize;
            ApplyRestrictionsCommand = new ApplyRestrictionsCommand(CharacterManager);
            ShowDonateButton = !base.Settings.Settings.Bundle;
            SubscribeWithEventAggregator();
            CharacterManager.Current.Status.StatusChanged += Status_StatusChanged;
        }

        private void Status_StatusChanged(object sender, CharacterStatusChangedEventArgs e)
        {
            if (e.Status.IsLoaded)
            {
                IsCharacterLoaded = e.Status.IsLoaded;
            }
        }

        private void OpenCharacterFile(string parameter)
        {
            if (!File.Exists(parameter))
            {
                MessageDialogService.Show("File does not exist.");
                return;
            }
            SelectedCharacter = Characters.FirstOrDefault((CharacterFile x) => x.FilePath.Equals(parameter));
            if (SelectedCharacter != null)
            {
                LoadCharacter();
                return;
            }
            try
            {
                CharacterFile characterFile = DataManager.Current.LoadCharacterFile(parameter);
                Characters.Add(characterFile);
                SelectedCharacter = characterFile;
                LoadCharacter();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "OpenCharacterFile");
                MessageDialogService.ShowException(ex);
            }
        }

        private void OpenCustomDocumentsFolder()
        {
            Process.Start(DataManager.Current.UserDocumentsCustomElementsDirectory);
        }

        private async void NewCharacter()
        {
            if (CharacterManager.Current.Status.HasChanges)
            {
                switch (MessageBox.Show((string.IsNullOrWhiteSpace(Character.Name) ? "The current character" : Character.Name) + " has unsaved changes. Do you want to save these changes?", Resources.ApplicationName, MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation))
                {
                    default:
                        return;
                    case MessageBoxResult.Yes:
                        CharacterManager.File.Save();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            IsCharacterInformationBlockVisible = true;
            Logger.Info("Creating a New Character");
            await CharacterManager.Current.New(initializeFirstLevel: true);
            CharacterManager.File = new CharacterFile("UNSAVED CHARACTER")
            {
                FileName = "NEW",
                IsNew = true
            };
            IsCharacterLoaded = true;
            base.EventAggregator.Send(new CharacterLoadingCompletedEvent());
            CharacterManager.Current.Status.HasChanges = false;
            StatusMessage = "Created new character";
        }

        [Obsolete]
        private void SaveCharacter()
        {
            try
            {
                CharacterManager.File.InitializeDisplayPropertiesFromCharacter(Character);
                // bool? flag = new SaveCharacterWindow(CharacterManager.File).ShowDialog();
                //if (flag.HasValue && flag.Value)
                //{
                //    StatusMessage = $"'{Character}' Saved";
                //    CharacterManager.Current.Status.HasChanges = false;
                //}
                //else
                //{
                //    StatusMessage = "Canceled Saved";
                //}
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "SaveCharacter");
                MessageDialogService.ShowException(ex);
                StatusMessage = "Error on saving current character. (" + ex.Message + ")";
            }
        }

        private async void LoadCharacter()
        {
            try
            {
                if (SelectedCharacter == null)
                {
                    StatusMessage = "Select a character to load.";
                    return;
                }
                if (CharacterManager.Current.Status.HasChanges)
                {
                    switch (MessageBox.Show((string.IsNullOrWhiteSpace(Character.Name) ? "The current character" : Character.Name) + " has unsaved changes. Do you want to save these changes?", Resources.ApplicationName, MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation))
                    {
                        case MessageBoxResult.Yes:
                            CharacterManager.File.Save();
                            break;
                        default:
                            StatusMessage = "Loading '" + SelectedCharacter.DisplayName + "' Canceled";
                            return;
                        case MessageBoxResult.No:
                            break;
                    }
                }
                IsCharacterLoaded = false;
                StatusMessage = "Loading '" + SelectedCharacter.DisplayName + "'";
                SelectionRuleNavigationService.IsEnabled = false;
                IsCharacterInformationBlockVisible = true;
                if ((await SelectedCharacter.Load()).Success)
                {
                    IsCharacterLoadedFully = true;
                    Logger.Info($"IsCharacterLoadedFully: {IsCharacterLoadedFully}");
                }
                else
                {
                    IsCharacterLoadedFully = false;
                    Logger.Warning($"IsCharacterLoadedFully: {IsCharacterLoadedFully}");
                }
                CharacterManager.File = SelectedCharacter;
                LoadedFilepath = CharacterManager.File.FilePath;
                ShellWindowViewModel shellWindowViewModel = this;
                bool isCharacterLoaded = (CharacterManager.Current.Status.IsLoaded = true);
                shellWindowViewModel.IsCharacterLoaded = isCharacterLoaded;
                CharacterManager.Current.Status.HasChanges = false;
                base.EventAggregator.Send(new CharacterLoadingCompletedEvent());
            }
            catch (Exception ex)
            {
                AnalyticsErrorHelper.Exception(ex, null, null, "LoadCharacter", 515);
                string message = "There was an error while loading the character.  \r\n" + ex.Message;
                ProgressPercentage = 0;
                Logger.Exception(ex, "LoadCharacter");
                MessageDialogService.Show(message);
            }
            finally
            {
                SelectionRuleNavigationService.IsEnabled = true;
                if (IsCharacterLoaded)
                {
                    StatusMessage = "Loaded '" + SelectedCharacter.DisplayName + "'";
                }
                await Task.Delay(2000);
                // base.EventAggregator.Send(new CharacterLoadingSliderEventArgs(open: false));
                await Task.Delay(1000);
                IsProgressVisible = false;
            }
        }

        private void DeleteCharacter()
        {
            if (SelectedCharacter == null)
            {
                StatusMessage = "Select a character to delete.";
                return;
            }
            try
            {
                switch (MessageBox.Show("Are you sure you want to delete " + SelectedCharacter.DisplayName + "?", Resources.ApplicationName, MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation))
                {
                    case MessageBoxResult.Yes:
                        {
                            string filePath = SelectedCharacter.FilePath;
                            if (Characters.Contains(SelectedCharacter))
                            {
                                Characters.Remove(SelectedCharacter);
                            }
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                            StatusMessage = "Deleted " + filePath;
                            DataManager.Current.RemoveNonExistingCharacterFileLocations();
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "DeleteCharacter");
                MessageDialogService.ShowException(ex);
                StatusMessage = "Unable to delete character. " + ex.Message;
            }
        }

        private void ChangePortrait()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png"
                };
                bool? flag = openFileDialog.ShowDialog();
                if (!flag.HasValue || !flag.Value)
                {
                    return;
                }
                string userDocumentsPortraitsDirectory = DataManager.Current.UserDocumentsPortraitsDirectory;
                FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                string text = Path.Combine(userDocumentsPortraitsDirectory, fileInfo.Name.ToLower());
                if (!File.Exists(text))
                {
                    using (Bitmap bitmap = new Bitmap(Image.FromFile(fileInfo.FullName, useEmbeddedColorManagement: false)))
                    {
                        bitmap.Save(text);
                    }
                }
                Character.PortraitFilename = text;
                CharacterManager.Current.Status.IsUserPortrait = true;
            }
            catch (IOException ex)
            {
                Logger.Exception(ex, "ChangePortrait");
                MessageBox.Show(ex.Message, "IO Exception @ ChangePortrait");
            }
            catch (Exception ex2)
            {
                Logger.Exception(ex2, "ChangePortrait");
                MessageBox.Show(ex2.Message, "Exception @ ChangePortrait");
            }
        }

        private void Update()
        {
            Logger.Warning("not implemented method Update");
        }

        private void JumpListSettings()
        {
            // new PreferencesWindow().ShowDialog();
            ListViewItemSize = Builder.Presentation.Properties.Settings.Default.CharactersCollectionSize;
        }

        private void LevelUp()
        {
            CharacterManager.Current.LevelUp();
            StatusMessage = $"Your character is now level {Character.Level}";
        }

        private void LevelDown()
        {
            CharacterManager.Current.LevelDown();
            StatusMessage = $"Your character is now level {Character.Level}";
        }

        private void ToggleFavoriteCharacter()
        {
            if (SelectedCharacter != null)
            {
                SelectedCharacter.IsFavorite = !SelectedCharacter.IsFavorite;
            }
        }

        private void CreateSummery()
        {
            if (!IsCharacterLoaded)
            {
                CharacterSummery = "======================================== NO CHARACTER LOADED ========================================";
                return;
            }
            List<ElementBase> source = CharacterManager.Current.GetElements().ToList();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("======================================== CHARACTER SUMMARY ========================================");
            stringBuilder.AppendLine("Name: " + Character.Name);
            stringBuilder.AppendLine("Race: " + Character.Race);
            stringBuilder.AppendLine("Class: " + Character.Class);
            stringBuilder.AppendLine("Background: " + Character.Background);
            stringBuilder.AppendLine("Alignment: " + Character.Alignment);
            stringBuilder.AppendLine($"Experience: {Character.Experience}");
            stringBuilder.AppendLine("PlayerName: " + Character.PlayerName);
            stringBuilder.AppendLine("==========");
            stringBuilder.AppendLine("Age: " + Character.Age);
            stringBuilder.AppendLine("Height: " + Character.Height);
            stringBuilder.AppendLine("Weight: " + Character.Weight);
            stringBuilder.AppendLine("Eyes: " + Character.Eyes);
            stringBuilder.AppendLine("Skin: " + Character.Skin);
            stringBuilder.AppendLine("Hair: " + Character.Hair);
            stringBuilder.AppendLine("==========");
            stringBuilder.AppendLine($"ArmorClass: {Character.ArmorClass}");
            stringBuilder.AppendLine($"Initiative: {Character.Initiative}");
            stringBuilder.AppendLine($"Speed: {Character.Speed}");
            stringBuilder.AppendLine($"HP: {Character.MaxHp}");
            ElementBase elementBase = source.FirstOrDefault((ElementBase x) => x.Type == "Class");
            if (elementBase != null)
            {
                stringBuilder.AppendLine(string.Format("hd: {0}", elementBase.ElementSetters.GetSetter("hd")));
                stringBuilder.AppendLine("======================================== SETTERS ========================================");
                foreach (ElementSetters.Setter elementSetter in elementBase.ElementSetters)
                {
                    stringBuilder.AppendLine(elementSetter.Name + ":" + elementSetter.Value);
                }
            }
            AbilitiesCollection abilities = Character.Abilities;
            stringBuilder.AppendLine("==========");
            stringBuilder.AppendLine(abilities.Strength.Abbreviation.ToUpper() + ": " + abilities.Strength.AbilityAndModifierString);
            stringBuilder.AppendLine(abilities.Dexterity.Abbreviation.ToUpper() + ": " + abilities.Dexterity.AbilityAndModifierString);
            stringBuilder.AppendLine(abilities.Constitution.Abbreviation.ToUpper() + ": " + abilities.Constitution.AbilityAndModifierString);
            stringBuilder.AppendLine(abilities.Intelligence.Abbreviation.ToUpper() + ": " + abilities.Intelligence.AbilityAndModifierString);
            stringBuilder.AppendLine(abilities.Wisdom.Abbreviation.ToUpper() + ": " + abilities.Wisdom.AbilityAndModifierString);
            stringBuilder.AppendLine(abilities.Charisma.Abbreviation.ToUpper() + ": " + abilities.Charisma.AbilityAndModifierString);
            stringBuilder.AppendLine("======================================== SAVING THROWS ========================================");
            StatisticsHandler2 statisticsCalculator = CharacterManager.Current.StatisticsCalculator;
            int value = statisticsCalculator.StatisticValues.GetValue(statisticsCalculator.Names.StrengthSaveProficiency);
            int value2 = statisticsCalculator.StatisticValues.GetValue(statisticsCalculator.Names.DexteritySaveProficiency);
            int value3 = statisticsCalculator.StatisticValues.GetValue(statisticsCalculator.Names.ConstitutionSaveProficiency);
            int value4 = statisticsCalculator.StatisticValues.GetValue(statisticsCalculator.Names.IntelligenceSaveProficiency);
            int value5 = statisticsCalculator.StatisticValues.GetValue(statisticsCalculator.Names.WisdomSaveProficiency);
            int value6 = statisticsCalculator.StatisticValues.GetValue(statisticsCalculator.Names.CharismaSaveProficiency);
            stringBuilder.AppendLine((abilities.Strength.Modifier + value).ToValueString() + " " + abilities.Strength.Name);
            stringBuilder.AppendLine((abilities.Dexterity.Modifier + value2).ToValueString() + " " + abilities.Dexterity.Name);
            stringBuilder.AppendLine((abilities.Constitution.Modifier + value3).ToValueString() + " " + abilities.Constitution.Name);
            stringBuilder.AppendLine((abilities.Intelligence.Modifier + value4).ToValueString() + " " + abilities.Intelligence.Name);
            stringBuilder.AppendLine((abilities.Wisdom.Modifier + value5).ToValueString() + " " + abilities.Wisdom.Name);
            stringBuilder.AppendLine((abilities.Charisma.Modifier + value6).ToValueString() + " " + abilities.Charisma.Name);
            foreach (ElementBase item in source.Where((ElementBase x) => x.Type == "Proficiency" && x.Name.StartsWith("Saving Throw")))
            {
                stringBuilder.AppendLine(item.ToString());
            }
            stringBuilder.AppendLine("======================================== SKILLS ========================================");
            foreach (SkillItem item2 in Character.Skills.GetCollection())
            {
                stringBuilder.AppendLine(item2.FinalBonus.ToValueString() + " " + item2.Name + " (" + item2.KeyAbility.Abbreviation + ")");
            }
            stringBuilder.AppendLine("==========");
            foreach (ElementBase item3 in source.Where((ElementBase x) => x.Type == "Proficiency" && x.Name.StartsWith("Skill")))
            {
                stringBuilder.AppendLine(item3.ToString());
            }
            stringBuilder.AppendLine("======================================== PROFICIENCY ========================================");
            foreach (ElementBase item4 in source.Where((ElementBase x) => x.Type == "Proficiency" && !x.Name.StartsWith("Skill") && !x.Name.StartsWith("Saving Throw")))
            {
                stringBuilder.AppendLine(item4.ToString());
            }
            stringBuilder.AppendLine("======================================== LANGUAGES ========================================");
            foreach (ElementBase item5 in source.Where((ElementBase x) => x.Type == "Language"))
            {
                stringBuilder.AppendLine(item5.ToString());
            }
            stringBuilder.AppendLine("======================================== FEATURES & TRAITS ========================================");
            foreach (ElementBase item6 in source.Where((ElementBase x) => x.Type == "Class Feature" || x.Type == "Archetype" || x.Type == "Archetype Feature" || x.Type == "Feat" || x.Type == "Racial Trait" || x.Type == "Vision"))
            {
                stringBuilder.AppendLine($"{item6}");
            }
            stringBuilder.AppendLine("======================================== EQUIPMENT ========================================");
            stringBuilder.AppendLine("======================================== BACKGROUNDS ========================================");
            foreach (ElementBase item7 in source.Where((ElementBase x) => x.Type == "Background" || x.Type == "Background Variant" || x.Type == "Background Feature"))
            {
                stringBuilder.AppendLine(item7.ToString());
            }
            IEnumerable<SelectRule> enumerable = CharacterManager.Current.SelectionRules.Where((SelectRule x) => x.ElementHeader.Type == "Background" && x.Attributes.IsList);
            string text = "";
            string text2 = "";
            string text3 = "";
            string text4 = "";
            bool flag = false;
            foreach (SelectRule selectionRule in enumerable)
            {
                foreach (KeyValuePair<string, SelectionRuleListItem> selectionRuleListItem in CharacterManager.Current.GetElements().First((ElementBase x) => x.Id == selectionRule.ElementHeader.Id).SelectionRuleListItems)
                {
                    if (!flag)
                    {
                        switch (selectionRuleListItem.Key)
                        {
                            case "Personality Trait":
                                text += selectionRuleListItem.Value.Text;
                                break;
                            case "Ideal":
                                text2 += selectionRuleListItem.Value.Text;
                                break;
                            case "Bond":
                                text3 += selectionRuleListItem.Value.Text;
                                break;
                            case "Flaw":
                                text4 += selectionRuleListItem.Value.Text;
                                break;
                        }
                    }
                }
                flag = true;
            }
            stringBuilder.AppendLine("Personality Traits: " + text);
            stringBuilder.AppendLine("Ideals: " + text2);
            stringBuilder.AppendLine("Bonds: " + text3);
            stringBuilder.AppendLine("Flaws: " + text4);
            stringBuilder.AppendLine("======================================== ADDITIONAL FEATURES & TRAITS ========================================");
            stringBuilder.AppendLine("======================================== TREASURE ========================================");
            stringBuilder.AppendLine("======================================== INLINE VALUES ========================================");
            foreach (KeyValuePair<string, string> inlineValue in CharacterManager.Current.StatisticsCalculator.InlineValues)
            {
                stringBuilder.AppendLine(inlineValue.Key + ": " + inlineValue.Value);
            }
            stringBuilder.AppendLine("======================================== STATISTIC VALUES ========================================");
            foreach (StatisticValuesGroup item8 in from x in CharacterManager.StatisticsCalculator.StatisticValues
                                                   where x.GetValues().Any()
                                                   orderby x.GroupName
                                                   select x)
            {
                string text5 = $"{item8.GroupName} = {item8.Sum()}";
                string text6 = string.Join(", ", from x in item8.GetValues()
                                                 select $"{x.Key} ({x.Value})");
                stringBuilder.AppendLine(text5.PadRight(60) + " " + text6);
            }
            CharacterSummery = stringBuilder.ToString();
        }

        private void CreateCharacterSheet()
        {
            try
            {
                CharacterManager.Current.GenerateCharacterSheet();
            }
            catch (Exception ex)
            {
                MessageDialogService.ShowException(ex);
            }
        }

        public override async Task InitializeAsync(InitializationArguments args)
        {
            ApplicationInitialized = false;
            foreach (CharacterFile item in from x in DataManager.Current.LoadCharacterFiles()
                                           orderby !x.IsFavorite, x.DisplayName
                                           select x)
            {
                Characters.Add(item);
            }
            CollectionView obj = (CollectionView)CollectionViewSource.GetDefaultView(Characters);
            obj.SortDescriptions.Add(new SortDescription("CollectionGroupName", ListSortDirection.Ascending));
            obj.SortDescriptions.Add(new SortDescription("IsFavorite", ListSortDirection.Descending));
            obj.SortDescriptions.Add(new SortDescription("DisplayName", ListSortDirection.Ascending));
            obj.Refresh();
            UpdateApplicationCharacterGroups();
            EnableDeveloperTools = ApplicationManager.Current.IsInDeveloperMode;
            ApplicationInitialized = true;
            CharacterManager.Current.Status.HasChanges = false;
            await Task.Delay(1000);
            IsQuickSearchBarEnabled = true;
            bool flag = await CheckUpdatesAvailableVersion();
            ApplicationManager.Current.UpdateAvailable = flag;
            UpdateAvailable = flag || base.IsInDebugMode;
        }

        private void SendSources()
        {
        }

        private async Task<bool> CheckUpdatesAvailableVersion(bool https = true)
        {
            try
            {
                ProgressPercentage = 25;
                StatusMessage = "";
                IsProgressVisible = true;
                Version local = new Version(Resources.ApplicationVersion);
                using (HttpClient client = new HttpClient())
                {
                    string applicationVersionUrl = Resources.ApplicationVersionUrl;
                    string text = await client.GetStringAsync(applicationVersionUrl);
                    ProgressPercentage = 50;
                    if (new Version(text).CompareTo(local) == 1)
                    {
                        StatusMessage = "Update Available (" + text + ")";
                        UpdatesHeader = "Update Available (" + text + ")";
                        UpdateNotificationContent = text ?? "";
                        MessageDialogService.Show("A new version is available for download.", "Update Available (" + text + ")");
                        base.EventAggregator.Send(new ShowSliderEvent(Slider.UpdateChangelog));
                        return true;
                    }
                    StatusMessage = "";
                    UpdateNotificationContent = "1";
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "CheckUpdatesAvailableVersion");
                StatusMessage = ex.Message;
            }
            finally
            {
                ProgressPercentage = 100;
                IsProgressVisible = false;
            }
            return false;
        }

        public void OnHandleEvent(CharacterNameChangedEvent args)
        {
            CharacterName = args.Name;
            Character.Name = args.Name;
        }

        public void OnHandleEvent(CharacterBuildChangedEvent args)
        {
            if (string.IsNullOrWhiteSpace(args.Character.Race) && !string.IsNullOrWhiteSpace(args.Character.Class))
            {
                CharacterBuild = $"Level {args.Character.Level} {args.Character.Class}";
            }
            else if (!string.IsNullOrWhiteSpace(args.Character.Race) && string.IsNullOrWhiteSpace(args.Character.Class))
            {
                CharacterBuild = $"Level {args.Character.Level} {args.Character.Race}";
            }
            else
            {
                CharacterBuild = $"Level {args.Character.Level} {args.Character.Race} {args.Character.Class}";
            }
            GenerateBuildStrings();
        }

        private void GenerateBuildStrings()
        {
            CharacterLevelBuild = $"Level {Character.Level}";
            if (string.IsNullOrWhiteSpace(Character.Race) && !string.IsNullOrWhiteSpace(Character.Class))
            {
                CharacterClassBuild = Character.Class ?? "";
            }
            else if (!string.IsNullOrWhiteSpace(Character.Race) && string.IsNullOrWhiteSpace(Character.Class))
            {
                CharacterClassBuild = Character.Race ?? "";
            }
            else
            {
                CharacterClassBuild = Character.Race + " " + Character.Class;
            }
        }

        public void OnHandleEvent(MainWindowStatusUpdateEvent args)
        {
            StatusMessage = args.StatusMessage;
            ProgressPercentage = args.ProgressPercentage;
            IsProgressIndeterminate = args.IsIndeterminateProgress;
            if (args.ProgressPercentage > 0 || args.IsIndeterminateProgress || ProgressPercentage > 0)
            {
                IsProgressVisible = true;
            }
            else
            {
                HandleHidingProgress();
            }
        }

        private async void HandleHidingProgress()
        {
            await Task.Delay(2500);
            if (ProgressPercentage == -1)
            {
                IsProgressVisible = false;
            }
        }

        private async void _watcher_Created(object sender, FileSystemEventArgs e)
        {
            Logger.Info("FileSystemWatcher => Created: {0} | {1}", e.FullPath, e.ChangeType);
            await Application.Current.Dispatcher.BeginInvoke((Action)async delegate
            {
                CharacterFile characterFile = DataManager.Current.LoadCharacterFile(e.FullPath);
                bool flag = false;
                foreach (CharacterFile character in Characters)
                {
                    if (character.FilePath.Equals(characterFile.FilePath))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    Characters.Add(characterFile);
                }
                StatusMessage = characterFile.DisplayName + " (" + e.Name + ") was saved to the character folder, added to the list of characters.";
            });
        }

        private async void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Logger.Info("FileSystemWatcher => Changed: {0} | {1}", e.FullPath, e.ChangeType);
            await Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                CharacterFile characterFile = DataManager.Current.LoadCharacterFile(e.FullPath);
                StatusMessage = characterFile.DisplayName + " (" + e.Name + ") was changed in the character folder, updated the character in the list.";
            });
            StatusMessage = e.Name + " was changed in the character folder.";
        }

        private async void _watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (Application.Current.Dispatcher != null)
                {
                    await Application.Current.Dispatcher.BeginInvoke((Action)delegate
                    {
                        CharacterFile characterFile = Characters.FirstOrDefault((CharacterFile x) => x.FilePath == e.FullPath);
                        if (characterFile != null)
                        {
                            if (SelectedCharacter == characterFile)
                            {
                                SelectedCharacter = null;
                            }
                            Characters.Remove(characterFile);
                        }
                    });
                }
                StatusMessage = e.Name + " was deleted from the character folder.";
            }
            catch (Exception ex)
            {
                AnalyticsErrorHelper.Exception(ex, null, null, "_watcher_Deleted", 1287);
                MessageDialogService.ShowException(ex);
            }
        }

        protected override void InitializeDesignData()
        {
            StatusMessage = "Status Message";
            ProgressPercentage = 67;
            IsProgressVisible = true;
            ListViewItemSize = 150;
            IsCharacterInformationBlockVisible = true;
            EnableDeveloperTools = true;
            IsCharacterLoaded = true;
            string[] characterNames = DesignData.CharacterNames;
            string[] portraitFilenames = DesignData.PortraitFilenames;
            Random random = new Random();
            for (int i = 0; i < 20; i++)
            {
                CharacterFile characterFile = new CharacterFile("C:\\users\\a\\fake\\directory\\path\\5e Character Builder\\designdata.dnd5e")
                {
                    DisplayName = characterNames[random.Next(characterNames.Length)],
                    DisplayLevel = (i + 1).ToString(),
                    DisplayPortraitFilePath = portraitFilenames[random.Next(portraitFilenames.Length)],
                    IsFavorite = (random.Next(5) == 1),
                    DisplayRace = "Changeling",
                    DisplayClass = "Wizard",
                    DisplayBackground = "Sage",
                    DisplayVersion = "1.17.1201",
                    CollectionGroupName = ((i % 2 == 0) ? "Group 1" : "Group 2")
                };
                characterFile.FileName = characterFile.DisplayName.ToLower() + ".dnd5e";
                Characters.Add(characterFile);
            }
            CharacterManager.File = Characters[0];
            SelectedCharacter = Characters[0];
            LoadedFilepath = portraitFilenames[0];
            Character.Name = "Seiðr";
            Character.Level = 7;
            Character.Background = "Uthgardt Tribe Member";
            Character.Class = "Blood Hunter";
            Character.Race = "Lizardfolk";
            Character.PortraitFilename = portraitFilenames[0];
            Character.Experience = 23000;
            Character.Alignment = "Neutral Good";
            Character.Gender = "Female";
            Character.Proficiency = 4;
            Character.Initiative = 6;
            Character.ArmorClass = 21;
            Character.Speed = 35;
            Character.MaxHp = 109;
            CharacterBuild = $"Level {Character.Level} {Character.Race} {Character.Class}";
            Character.Companion.Name = "Guenhwyvar";
            Character.Companion.CompanionName.OriginalContent = "Guenhwyvar";
            Character.Companion.DisplayName = "Black Panther";
            Character.Companion.DisplayBuild = "Medium beast, unaligned";
            Character.Companion.Portrait.Content = portraitFilenames[1];
            Character.Companion.Initiative.OriginalContent = "6";
            Character.Companion.ArmorClass.OriginalContent = "11";
            Character.Companion.Speed.OriginalContent = "35";
            Character.Companion.MaxHp.OriginalContent = "3";
            SelectionRuleNavigationService.IsNextAvailable = true;
        }

        private void CharacterSheetPreviousPage()
        {
            base.EventAggregator.Send(new CharacterSheetPreviousPageEvent());
        }

        private void CharacterSheetNextPage()
        {
            base.EventAggregator.Send(new CharacterSheetNextPageEvent());
        }

        public void OnHandleEvent(CharacterManagerElementRegistered args)
        {
        }

        public void OnHandleEvent(CharacterManagerElementUnregistered args)
        {
        }

        public void OnHandleEvent(CharacterSavedEvent args)
        {
            if (args.File.IsNew)
            {
                CharacterManager.File = args.File;
                LoadedFilepath = CharacterManager.File.FilePath;
                CharacterManager.File.IsNew = false;
                CharacterManager.Current.Status.HasChanges = false;
            }
            bool flag = false;
            foreach (CharacterFile character in Characters)
            {
                if (character.FilePath.Equals(args.File.FilePath))
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                Characters.Add(args.File);
            }
            ((CollectionView)CollectionViewSource.GetDefaultView(Characters)).Refresh();
        }

        void ISubscriber<SettingsChangedEvent>.OnHandleEvent(SettingsChangedEvent args)
        {
            IsQuickSearchBarEnabled = args.Settings.QuickSearchBarEnabled;
            ListViewItemSize = args.Settings.CharactersCollectionSize;
            ShowDonateButton = !base.Settings.Settings.Bundle;
        }

        private void SaveDocumentAs()
        {
            try
            {
                string text = Path.GetInvalidFileNameChars().Aggregate(Character.Name, (string current, char invalidChar) => current.Replace(invalidChar.ToString(), ""));
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = "pdf";
                saveFileDialog.AddExtension = true;
                saveFileDialog.FileName = text.ToLower().Trim();
                saveFileDialog.InitialDirectory = DataManager.Current.UserDocumentsRootDirectory;
                saveFileDialog.Title = "Save Character Sheet (" + Character.Name + ")";
                saveFileDialog.Filter = "PDF (*.pdf)|*.pdf";
                if (saveFileDialog.ShowDialog() == true)
                {
                    string fileName = saveFileDialog.FileName;
                    CharacterManager.Current.ReprocessCharacter();
                    FileInfo fileInfo = new CharacterSheetGenerator(CharacterManager.Current).GenerateNewSheet(fileName, generateForPreview: false);
                    base.EventAggregator.Send(new CharacterSheetSavedEvent(fileName));
                    _ = base.Settings.Settings.CharacterSheetOpenOnSave;
                    Process.Start(fileInfo.FullName);
                }
            }
            catch (IOException ex)
            {
                MessageDialogService.ShowException(ex);
            }
            catch (Exception ex2)
            {
                MessageDialogService.ShowException(ex2);
            }
        }

        public void OnHandleEvent(AdditionalContentUpdatedEvent args)
        {
            IsContentUpdated = args.IsUpdated;
            ContentUpdatedMessage = args.Message;
        }

        private void RestartAfterContentUpdated()
        {
            if (MessageBox.Show("Your content files have been updated, do you want to restart the application to reload the content?", "Aurora Builder", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private List<string> GetGroups()
        {
            return (from x in Characters.Select((CharacterFile x) => x.CollectionGroupName).Distinct()
                    orderby x
                    select x).ToList();
        }

        private void UpdateApplicationCharacterGroups()
        {
            ApplicationManager.Current.CharacterGroups.Clear();
            foreach (string group in GetGroups())
            {
                ApplicationManager.Current.CharacterGroups.Add(group);
            }
        }

        private async void EditGroup(object parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException();
            }
            string group = parameter.ToString();
            // EditCharacterGroupWindow editCharacterGroupWindow = new EditCharacterGroupWindow(group, GetGroups());
            //if (editCharacterGroupWindow.ShowDialog() != true)
            //{
            //    return;
            //}
            try
            {
                //string newGroup = editCharacterGroupWindow.NewGroupName;
                await Task.Run(delegate
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        foreach (CharacterFile character in Characters)
                        {
                            if (character.CollectionGroupName.Equals(group))
                            {
                                //character.UpdateGroupName(newGroup);
                            }
                        }
                    });
                });
                ((CollectionView)CollectionViewSource.GetDefaultView(Characters)).Refresh();
            }
            catch (Exception ex)
            {
                MessageDialogService.ShowException(ex);
            }
            UpdateApplicationCharacterGroups();
        }

        private void EditCharacterGroup(object parameter)
        {
            if (!(parameter is CharacterFile characterFile))
            {
                return;
            }
            //EditCharacterGroupWindow editCharacterGroupWindow = new EditCharacterGroupWindow(characterFile.CollectionGroupName, GetGroups());
            //if (editCharacterGroupWindow.ShowDialog() == true)
            //{
            //    try
            //    {
            //        string newGroupName = editCharacterGroupWindow.NewGroupName;
            //        characterFile.UpdateGroupName(newGroupName);
            //        ((CollectionView)CollectionViewSource.GetDefaultView(Characters)).Refresh();
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageDialogService.ShowException(ex);
            //    }
            //    UpdateApplicationCharacterGroups();
            //}
        }

        private void OpenDeveloperTools()
        {
        }

        private void OpenTools()
        {
        }
    }
}
