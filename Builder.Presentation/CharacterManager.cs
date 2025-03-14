using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Builder.Core.Events;
using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Data.Extensions;
using Builder.Data.Rules;
using Builder.Data.Strings;
using Builder.Presentation;
using Builder.Presentation.Events.Character;
using Builder.Presentation.Events.Global;
using Builder.Presentation.Events.Shell;
using Builder.Presentation.Extensions;
using Builder.Presentation.Models;
using Builder.Presentation.Models.Collections;
using Builder.Presentation.Properties;
using Builder.Presentation.Services;
using Builder.Presentation.Services.Calculator;
using Builder.Presentation.Services.Data;
using Builder.Presentation.Services.Sources;
// using Builder.Presentation.UserControls.Spellcasting;
using Builder.Presentation.Utilities;
using Builder.Presentation.ViewModels;
// using Builder.Presentation.Views;

namespace Builder.Presentation
{
    public sealed class CharacterManager
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly ProgressionManager _progressionManager;

        private static Random _rnd = new Random();

        public static CharacterManager Current { get; } = new CharacterManager();

        public StatisticsHandler2 StatisticsCalculator { get; private set; }

        public Character Character { get; } = new Character();

        public CharacterStatus Status { get; } = new CharacterStatus();

        public CharacterFile File { get; set; }

        public ObservableCollection<ClassProgressionManager> ClassProgressionManagers { get; } = new ObservableCollection<ClassProgressionManager>();

        public SourcesManager SourcesManager { get; } = new SourcesManager();

        public ObservableCollection<ElementBase> Elements
        {
            get
            {
                ElementBaseCollection elementBaseCollection = new ElementBaseCollection(_progressionManager.Elements);
                foreach (ClassProgressionManager classProgressionManager in ClassProgressionManagers)
                {
                    elementBaseCollection.AddRange(classProgressionManager.Elements);
                }
                return elementBaseCollection;
            }
        }

        public ObservableCollection<SelectRule> SelectionRules
        {
            get
            {
                ObservableCollection<SelectRule> observableCollection = new ObservableCollection<SelectRule>(_progressionManager.SelectionRules);
                foreach (ClassProgressionManager classProgressionManager in ClassProgressionManagers)
                {
                    foreach (SelectRule selectionRule in classProgressionManager.SelectionRules)
                    {
                        observableCollection.Add(selectionRule);
                    }
                }
                return observableCollection;
            }
        }

        private CharacterManager()
        {
            _eventAggregator = ApplicationManager.Current.EventAggregator;
            _progressionManager = new ProgressionManager();
            _progressionManager.SelectionRuleCreated += _progressionManager_SelectionRuleCreated;
            _progressionManager.SelectionRuleRemoved += _progressionManager_SelectionRuleRemoved;
            _progressionManager.SpellcastingInformationCreated += _progressionManager_SpellcastingSectionCreated;
            _progressionManager.SpellcastingInformationRemoved += _progressionManager_SpellcastingSectionRemoved;
            StatisticsCalculator = new StatisticsHandler2(this);
            Character.PropertyChanged += Character_PropertyChanged;
        }

        private void _progressionManager_SelectionRuleCreated(object sender, SelectRule e)
        {
            _eventAggregator.Send(new CharacterManagerSelectionRuleCreated(e));
        }

        private void _progressionManager_SelectionRuleRemoved(object sender, SelectRule e)
        {
            _eventAggregator.Send(new CharacterManagerSelectionRuleDeleted(e));
        }

        private void _progressionManager_SpellcastingSectionCreated(object sender, SpellcastingInformation e)
        {
            _eventAggregator.Send(new SpellcastingInformationCreatedEvent(e));
            if (sender is ProgressionManager && !(sender is ClassProgressionManager) && e.IsExtension)
            {
                foreach (SpellcastingInformation spellcastingInformation in GetSpellcastingInformations())
                {
                    if (!spellcastingInformation.IsExtension && (spellcastingInformation.Name.Equals(e.Name, StringComparison.OrdinalIgnoreCase) || e.AssignToAllSpellcastingClasses))
                    {
                        spellcastingInformation.MergeExtended(e.ExtendedSupportedSpellsExpressions);
                    }
                }
            }
            Status.HasSpellcasting = GetSpellcastingInformations().Any((SpellcastingInformation x) => !x.IsExtension);
        }

        private void _progressionManager_SpellcastingSectionRemoved(object sender, SpellcastingInformation e)
        {
            _eventAggregator.Send(new SpellcastingInformationRemovedEvent(e));
            if (sender is ProgressionManager && !(sender is ClassProgressionManager) && e.IsExtension)
            {
                foreach (SpellcastingInformation spellcastingInformation in GetSpellcastingInformations())
                {
                    if (!spellcastingInformation.IsExtension && (spellcastingInformation.Name.Equals(e.Name, StringComparison.OrdinalIgnoreCase) || e.AssignToAllSpellcastingClasses))
                    {
                        spellcastingInformation.Unmerge(e.ExtendedSupportedSpellsExpressions);
                    }
                }
            }
            Status.HasSpellcasting = GetSpellcastingInformations().Any((SpellcastingInformation x) => !x.IsExtension);
        }

        private void Character_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
            if (propertyName != null && propertyName == "Experience" && Status.IsLoaded)
            {
                LevelElement upcomingLevelElement = GetUpcomingLevelElement();
                if (upcomingLevelElement == null || Character.Experience < upcomingLevelElement.RequiredExperience)
                {
                    return;
                }
                _eventAggregator.Send(new MainWindowStatusUpdateEvent("You have earned enough experience to advance to the next level."));
            }
            Status.HasChanges = true;
        }

        public ElementBase RegisterElement(ElementBase element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            Logger.Info("Registering Element: " + element);
            switch (element.Type)
            {
                case "Level":
                    {
                        LevelElement levelElement = (LevelElement)element;
                        if (levelElement.Level > Character.Level)
                        {
                            Character.Level = levelElement.Level;
                            if (Character.Experience < levelElement.RequiredExperience)
                            {
                                Character.Experience = levelElement.RequiredExperience;
                            }
                        }
                        _progressionManager.ProgressionLevel = Character.Level;
                        _progressionManager.Elements.Add(element);
                        _progressionManager.Process(element);
                        _progressionManager.ProcessExistingElements();
                        break;
                    }
                case "Class":
                    {
                        ClassProgressionManager classProgressionManager = ClassProgressionManagers.FirstOrDefault((ClassProgressionManager x) => x.IsMainClass);
                        if (classProgressionManager == null)
                        {
                            classProgressionManager = new ClassProgressionManager(isMainClass: true, isMulticlass: false, 1, element);
                            classProgressionManager.LevelElements.AddRange(_progressionManager.Elements.Where((ElementBase x) => x.Type.Equals("Level")));
                            classProgressionManager.ProgressionLevel = Character.Level;
                            classProgressionManager.SelectionRuleCreated += _progressionManager_SelectionRuleCreated;
                            classProgressionManager.SelectionRuleRemoved += _progressionManager_SelectionRuleRemoved;
                            classProgressionManager.SpellcastingInformationCreated += _progressionManager_SpellcastingSectionCreated;
                            classProgressionManager.SpellcastingInformationRemoved += _progressionManager_SpellcastingSectionRemoved;
                            ClassProgressionManagers.Add(classProgressionManager);
                        }
                        else
                        {
                            classProgressionManager.RemoveClass();
                            classProgressionManager.SetClass(element);
                        }
                        if (!element.AllowMultipleElements && classProgressionManager.Elements.Any((ElementBase e) => e.Type == element.Type))
                        {
                            UnregisterElement(classProgressionManager.Elements.FirstOrDefault((ElementBase e) => e.Type == element.Type));
                        }
                        classProgressionManager.Elements.Add(element);
                        classProgressionManager.Process(element);
                        classProgressionManager.ProcessExistingElements();
                        Status.HasMainClass = true;
                        break;
                    }
                case "Multiclass":
                    {
                        string id = element.Aquisition.SelectRule.UniqueIdentifier;
                        ClassProgressionManager multiclassProgressionManager = ClassProgressionManagers.FirstOrDefault((ClassProgressionManager x) => x.SelectRule.UniqueIdentifier.Equals(id));
                        if (multiclassProgressionManager == null)
                        {
                            multiclassProgressionManager = new ClassProgressionManager(isMainClass: false, isMulticlass: true, element.Aquisition.SelectRule.Attributes.RequiredLevel, element);
                            multiclassProgressionManager.SelectionRuleCreated += _progressionManager_SelectionRuleCreated;
                            multiclassProgressionManager.SelectionRuleRemoved += _progressionManager_SelectionRuleRemoved;
                            multiclassProgressionManager.ProgressionLevel = 1;
                            multiclassProgressionManager.SpellcastingInformationCreated += _progressionManager_SpellcastingSectionCreated;
                            multiclassProgressionManager.SpellcastingInformationRemoved += _progressionManager_SpellcastingSectionRemoved;
                            multiclassProgressionManager.LevelElements.Add(_progressionManager.Elements.FirstOrDefault((ElementBase x) => x.Type.Equals("Level") && x.ElementSetters.GetSetter("Level").ValueAsInteger() == multiclassProgressionManager.StartingLevel));
                            ClassProgressionManagers.Add(multiclassProgressionManager);
                        }
                        else
                        {
                            multiclassProgressionManager.SetClass(element);
                        }
                        if (element.ContainsSelectRules)
                        {
                            foreach (SelectRule selectRule in element.GetSelectRules())
                            {
                                if (multiclassProgressionManager.SelectionRules.Contains(selectRule) && Debugger.IsAttached)
                                {
                                    Debugger.Break();
                                }
                            }
                        }
                        multiclassProgressionManager.Elements.Add(element);
                        multiclassProgressionManager.Process(element);
                        multiclassProgressionManager.ProcessExistingElements();
                        Status.HasMulticlass = true;
                        break;
                    }
                case "Class Feature":
                case "Archetype":
                case "Archetype Feature":
                    {
                        ProgressionManager progressionManager = null;
                        if (element.Aquisition.WasSelected)
                        {
                            progressionManager = ClassProgressionManagers.FirstOrDefault((ClassProgressionManager x) => x.SelectionRules.Contains(element.Aquisition.SelectRule));
                        }
                        else if (element.Aquisition.WasGranted)
                        {
                            progressionManager = ClassProgressionManagers.FirstOrDefault((ClassProgressionManager x) => (from e in x.GetElements()
                                                                                                                         select e.Id).Contains(element.Aquisition.GrantRule.ElementHeader.Id));
                        }
                        else if (Debugger.IsAttached)
                        {
                            Debugger.Break();
                        }
                        if (progressionManager == null)
                        {
                            progressionManager = _progressionManager;
                        }
                        if (!element.AllowMultipleElements && Debugger.IsAttached)
                        {
                            Debugger.Break();
                        }
                        if (!element.AllowMultipleElements && progressionManager.Elements.Any((ElementBase e) => e.Type == element.Type))
                        {
                            Logger.Warning("{0} not allowed for multiple elements", element);
                            UnregisterElement(progressionManager.Elements.FirstOrDefault((ElementBase e) => e.Type == element.Type));
                        }
                        progressionManager.Elements.Add(element);
                        progressionManager.Process(element);
                        progressionManager.ProcessExistingElements();
                        break;
                    }
                case "Option":
                    _progressionManager.Elements.Add(element);
                    _progressionManager.Process(element);
                    _progressionManager.ProcessExistingElements();
                    if (!element.AsElement<Option>().IsInternal)
                    {
                    }
                    break;
                default:
                    _ = element.AllowMultipleElements;
                    if (!element.AllowMultipleElements && _progressionManager.Elements.Any((ElementBase e) => e.Type == element.Type))
                    {
                        Logger.Warning("{0} not allowed for multiple elements", element);
                        UnregisterElement(_progressionManager.Elements.FirstOrDefault((ElementBase e) => e.Type == element.Type));
                    }
                    _progressionManager.Elements.Add(element);
                    _progressionManager.Process(element);
                    _progressionManager.ProcessExistingElements();
                    break;
            }
            _progressionManager.ProcessExistingElements();
            foreach (ClassProgressionManager classProgressionManager2 in ClassProgressionManagers)
            {
                classProgressionManager2.ProcessExistingElements();
            }
            SetCharacterDetails();
            _eventAggregator.Send(new CharacterManagerElementRegistered(element));
            _eventAggregator.Send(new CharacterManagerElementsUpdated(element, CharacterManagerUpdateReason.ElementRegistered));
            if (element.Type == "Race" || element.Type == "Sub Race" || element.Type == "Class" || element.Type == "Multiclass" || element.Type == "Archetype" || element.Type == "Level")
            {
                _eventAggregator.Send(new CharacterBuildChangedEvent(Character));
                if (element.Type == "Race" || element.Type == "Sub Race")
                {
                    SetPortrait(element);
                }
            }
            if (Status.IsLoaded && ApplicationManager.Current.Settings.Settings.GenerateSheetOnCharacterChangedRegistered)
            {
                GenerateCharacterSheet();
            }
            return element;
        }

        public ElementBase UnregisterElement(ElementBase element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            Logger.Info("Unregistering Element: " + element);
            switch (element.Type)
            {
                case "Level":
                    {
                        LevelElement levelElement = (LevelElement)element;
                        if (levelElement.Level == Character.Level)
                        {
                            Character.Level = levelElement.Level - 1;
                            Character.Experience = levelElement.RequiredExperience - 50;
                        }
                        _progressionManager.ProgressionLevel = Character.Level;
                        _progressionManager.Clean(element);
                        _progressionManager.Elements.Remove(element);
                        break;
                    }
                case "Class":
                    {
                        ClassProgressionManager classProgressionManager2 = ClassProgressionManagers.FirstOrDefault((ClassProgressionManager x) => x.ClassElement == element);
                        if (classProgressionManager2 != null)
                        {
                            classProgressionManager2.Clean(element);
                            classProgressionManager2.Elements.Remove(element);
                        }
                        else
                        {
                            _progressionManager.Clean(element);
                        }
                        break;
                    }
                case "Multiclass":
                    {
                        ClassProgressionManager classProgressionManager = ClassProgressionManagers.FirstOrDefault((ClassProgressionManager x) => x.ClassElement == element);
                        if (classProgressionManager != null)
                        {
                            classProgressionManager.Clean(element);
                            classProgressionManager.Elements.Remove(element);
                            if (!classProgressionManager.IsObsolete)
                            {
                                classProgressionManager.RemoveClass();
                                if (!classProgressionManager.Elements.Any() && !classProgressionManager.SelectionRules.Any())
                                {
                                }
                            }
                        }
                        else
                        {
                            _progressionManager.Clean(element);
                        }
                        break;
                    }
                case "Archetype":
                case "Archetype Feature":
                case "Class Feature":
                    {
                        ProgressionManager progressionManager = null;
                        foreach (ClassProgressionManager classProgressionManager3 in ClassProgressionManagers)
                        {
                            if (classProgressionManager3.Elements.Contains(element))
                            {
                                progressionManager = classProgressionManager3;
                                break;
                            }
                        }
                        if (progressionManager == null)
                        {
                            Logger.Warning($"trying to unregister {element} from the normal progression manager");
                            progressionManager = _progressionManager;
                        }
                        progressionManager.Clean(element);
                        progressionManager.Elements.Remove(element);
                        break;
                    }
                case "Option":
                    _progressionManager.Clean(element);
                    _progressionManager.Elements.Remove(element);
                    if (!element.AsElement<Option>().IsInternal)
                    {
                    }
                    break;
                default:
                    _progressionManager.Clean(element);
                    _progressionManager.Elements.Remove(element);
                    break;
            }
            _progressionManager.ProcessExistingElements();
            foreach (ClassProgressionManager classProgressionManager4 in ClassProgressionManagers)
            {
                classProgressionManager4.ProcessExistingElements();
            }
            SetCharacterDetails();
            _eventAggregator.Send(new CharacterManagerElementUnregistered(element));
            _eventAggregator.Send(new CharacterManagerElementsUpdated(element, CharacterManagerUpdateReason.ElementUnregistered));
            if (element.Type == "Race" || element.Type == "Sub Race" || element.Type == "Class" || element.Type == "Multiclass" || element.Type == "Archetype" || element.Type == "Level")
            {
                _eventAggregator.Send(new CharacterBuildChangedEvent(Character));
            }
            if (Status.IsLoaded && ApplicationManager.Current.Settings.Settings.GenerateSheetOnCharacterChangedRegistered)
            {
                GenerateCharacterSheet();
            }
            return element;
        }

        public async Task<Character> New(bool initializeFirstLevel)
        {
            Logger.Info("creating a new character");
            Stopwatch sw = Stopwatch.StartNew();
            Logger.Info("unregister all remaining elements");
            foreach (ClassProgressionManager classProgressionManager in ClassProgressionManagers)
            {
                while (classProgressionManager.Elements.Any())
                {
                    UnregisterElement(classProgressionManager.Elements.Last());
                }
            }
            ClassProgressionManagers.Clear();
            while (_progressionManager.Elements.Any())
            {
                UnregisterElement(_progressionManager.Elements.Last());
            }
            _progressionManager.ProgressionLevel = 0;
            while (true)
            {
                int expandersCount = SelectionRuleExpanderHandler.Current.GetExpandersCount();
                if (expandersCount == 0)
                {
                    break;
                }
                await Task.Delay(10);
                Logger.Debug("post=cleanup | expanders remaining: {0}", expandersCount);
            }
            SelectionRuleExpanderHandler.Current.RemoveAllExpanders();
            if (Settings.Default.ApplyDefaultSourceRestrictionsOnNewCharacter)
            {
                SourcesManager.LoadDefaults();
            }
            Character.ResetEntryFields();
            if (initializeFirstLevel)
            {
                LevelUp();
                Character.Name = "New Character";
                Character.PlayerName = ApplicationManager.Current.Settings.PlayerName;
                Character.PortraitFilename = Path.Combine(DataManager.Current.UserDocumentsPortraitsDirectory, "default-portrait.png");
            }
            Status.IsUserPortrait = !initializeFirstLevel;
            Status.IsLoaded = true;
            Status.IsNew = initializeFirstLevel;
            Status.HasChanges = false;
            Status.CanLevelUp = true;
            Status.CanLevelDown = false;
            Status.HasMainClass = false;
            Status.HasMulticlass = false;
            Status.HasSpellcasting = false;
            Status.HasMulticlassSpellSlots = false;
            Logger.Info("finished initializing the new character ({0}ms)", sw.ElapsedMilliseconds);
            return Character;
        }

        public ElementBase LevelUp()
        {
            List<ElementBase> list = DataManager.Current.ElementsCollection.Where((ElementBase e) => e.Type == "Level").ToList();
            if (_progressionManager.Elements.Any((ElementBase x) => x.Type.Equals("Level")))
            {
                ElementBase elementBase = _progressionManager.Elements.Last((ElementBase e) => e.Type == "Level");
                int num = list.IndexOf(elementBase);
                if (num + 1 > list.Count)
                {
                    return null;
                }
                if (elementBase == list.Last())
                {
                    Status.CanLevelUp = false;
                    MessageDialogService.Show("You have reached the maximum level available (" + elementBase.Name + ")");
                    return null;
                }
                ElementBase element = list[num + 1];
                ElementBase elementBase2 = RegisterElement(element);
                if (elementBase2 == list.Last())
                {
                    Status.CanLevelUp = false;
                }
                return elementBase2;
            }
            return RegisterElement(list.First());
        }

        public void LevelDown()
        {
            if (!Status.CanLevelDown)
            {
                Logger.Warning("LevelDown !Status.CanLevelDown");
                return;
            }
            ElementBase lastLevel = _progressionManager.Elements.Last((ElementBase x) => x.Type.Equals("Level"));
            ClassProgressionManager classProgressionManager = ClassProgressionManagers.FirstOrDefault((ClassProgressionManager x) => x.LevelElements.Contains(lastLevel));
            if (classProgressionManager != null)
            {
                if (classProgressionManager.LevelElements.Remove(lastLevel))
                {
                    classProgressionManager.ProgressionLevel--;
                }
                if (classProgressionManager.ProgressionLevel == 0)
                {
                    classProgressionManager.IsObsolete = true;
                }
                if (classProgressionManager.IsObsolete)
                {
                    while (classProgressionManager.Elements.Any())
                    {
                        UnregisterElement(classProgressionManager.Elements.Last());
                    }
                    if (classProgressionManager.SelectionRules.Any())
                    {
                        foreach (SelectRule selectionRule in classProgressionManager.SelectionRules)
                        {
                            _progressionManager_SelectionRuleRemoved(classProgressionManager, selectionRule);
                        }
                    }
                }
            }
            UnregisterElement(lastLevel);
            if (classProgressionManager != null && classProgressionManager.IsObsolete)
            {
                while (classProgressionManager.Elements.Any())
                {
                    Logger.Warning("classProgressionManager.Elements.Any() after unregistering level element");
                    UnregisterElement(classProgressionManager.Elements.Last());
                }
                if (classProgressionManager.SelectionRules.Any())
                {
                    Logger.Warning("classProgressionManager.SelectionRules.Any() after unregistering level element");
                    foreach (SelectRule selectionRule2 in classProgressionManager.SelectionRules)
                    {
                        _progressionManager_SelectionRuleRemoved(classProgressionManager, selectionRule2);
                    }
                }
                classProgressionManager.SelectionRuleCreated -= _progressionManager_SelectionRuleCreated;
                classProgressionManager.SelectionRuleRemoved -= _progressionManager_SelectionRuleRemoved;
                ClassProgressionManagers.Remove(classProgressionManager);
                Status.HasMulticlass = ClassProgressionManagers.Any((ClassProgressionManager x) => x.IsMulticlass);
            }
            lastLevel = _progressionManager.Elements.Last((ElementBase x) => x.Type.Equals("Level"));
            if (lastLevel.AsElement<LevelElement>().Level == 1)
            {
                Status.CanLevelDown = false;
            }
            Status.CanLevelUp = lastLevel.AsElement<LevelElement>().Level != 20;
        }

        public void LevelUpMain()
        {
            if (!Status.CanLevelUp)
            {
                Logger.Warning("LevelUpMain !Status.CanLevelUp");
                MessageDialogService.Show($"You have reached the maximum level available ({Character.Level})");
                return;
            }
            if (Status.HasMainClass)
            {
                ClassProgressionManager classProgressionManager = ClassProgressionManagers.Single((ClassProgressionManager x) => x.IsMainClass);
                if (Character.Level == 20)
                {
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }
                    return;
                }
                classProgressionManager.ProgressionLevel++;
                ElementBase elementBase = LevelUp();
                if (elementBase != null)
                {
                    classProgressionManager.LevelElements.Add(elementBase);
                }
            }
            else
            {
                LevelUp();
            }
            Status.CanLevelDown = true;
        }

        public void LevelUpMulti(Multiclass element)
        {
            if (!Status.CanLevelUp)
            {
                Logger.Warning("LevelUpMulti !Status.CanLevelUp");
                return;
            }
            try
            {
                if (Status.HasMulticlass)
                {
                    ClassProgressionManager classProgressionManager = ClassProgressionManagers.Single((ClassProgressionManager x) => x.IsMulticlass && x.ClassElement.Id.Equals(element.Id));
                    if (Character.Level == 20 && Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }
                    classProgressionManager.ProgressionLevel++;
                    ElementBase elementBase = LevelUp();
                    if (elementBase != null)
                    {
                        classProgressionManager.LevelElements.Add(elementBase);
                        Status.CanLevelDown = true;
                    }
                }
                else if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "LevelUpMulti");
                MessageDialogService.ShowException(ex);
            }
        }

        public void NewMulticlass()
        {
            if (!Status.CanLevelUp)
            {
                Logger.Warning("NewMulticlass !Status.CanLevelUp");
                return;
            }
            try
            {
                if (Status.HasMainClass)
                {
                    LevelElement level = LevelUp() as LevelElement;
                    if (level != null)
                    {
                        ElementBase item = DataManager.Current.ElementsCollection.Where((ElementBase x) => x.Type.Equals("Grants")).Single((ElementBase x) => x.Id == $"ID_INTERNAL_MULTICLASS_LEVEL_{level.Level}");
                        level.RuleElements.Add(item);
                        ReprocessCharacter();
                        Status.CanLevelDown = true;
                    }
                }
                else if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "NewMulticlass");
                MessageDialogService.ShowException(ex);
            }
        }

        public void ReprocessCharacter(bool generateSheet = false)
        {
            _progressionManager.ProcessExistingElements();
            foreach (ClassProgressionManager classProgressionManager in ClassProgressionManagers)
            {
                classProgressionManager.ProcessExistingElements();
            }
            _eventAggregator.Send(new ReprocessCharacterEvent());
            SetCharacterDetails();
            if (generateSheet && Status.IsLoaded && ApplicationManager.Current.Settings.Settings.GenerateSheetOnCharacterChangedRegistered)
            {
                GenerateCharacterSheet();
            }
        }

        public FileInfo GenerateCharacterSheetPreview()
        {
            bool hasChanges = Status.HasChanges;
            ReprocessCharacter();
            if (!hasChanges)
            {
                Status.HasChanges = false;
            }
            CharacterSheetGenerator characterSheetGenerator = new CharacterSheetGenerator(this);
            string tempFileName = Path.GetTempFileName();
            FileInfo fileInfo = characterSheetGenerator.GenerateNewSheet(tempFileName, generateForPreview: true);
            // _eventAggregator.Send(new CharacterSheetPreviewEvent(fileInfo));
            return fileInfo;
        }

        public FileInfo GenerateCharacterSheet()
        {
            try
            {
                return GenerateCharacterSheetPreview();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "GenerateCharacterSheet");
                _eventAggregator.Send(new MainWindowStatusUpdateEvent("Unable to generate a preview of your character sheet. (" + ex.Message + ")"));
                if (Debugger.IsAttached)
                {
                    MessageDialogService.ShowException(ex);
                }
            }
            return null;
        }

        [Obsolete]
        public void SetCharacterDetailsAfterAbilitiesChange()
        {
            if (!Status.IsLoaded)
            {
                return;
            }
            _progressionManager.ProcessExistingElements();
            SetCharacterDetails();
            if (!Status.IsLoaded || !Settings.Default.GenerateSheetOnCharacterChangedRegistered || Debugger.IsAttached)
            {
                return;
            }
            try
            {
                GenerateCharacterSheet();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "SetCharacterDetailsAfterAbilitiesChange");
            }
        }

        public LevelElement GetCurrentLevelElement()
        {
            if (!Elements.Any())
            {
                return null;
            }
            return Elements.Last((ElementBase x) => x.Type.Equals("Level"))?.AsElement<LevelElement>();
        }

        public LevelElement GetUpcomingLevelElement()
        {
            LevelElement currentLevelElement = GetCurrentLevelElement();
            if (currentLevelElement == null)
            {
                return null;
            }
            if (currentLevelElement.Level == 20)
            {
                return null;
            }
            List<ElementBase> list = DataManager.Current.ElementsCollection.Where((ElementBase x) => x.Type.Equals("Level")).ToList();
            return list[list.IndexOf(currentLevelElement) + 1].AsElement<LevelElement>();
        }

        private void SetCharacterDetails()
        {
            int progressionLevel = _progressionManager.ProgressionLevel;
            if (StatisticsCalculator == null)
            {
                StatisticsCalculator = new StatisticsHandler2(this);
            }
            StatisticValuesGroupCollection seed = StatisticsCalculator.CreateSeed(progressionLevel, this);
            StatisticValuesGroupCollection statisticValuesGroupCollection = StatisticsCalculator.CalculateValues(GetElements(), seed);
            AuroraStatisticStrings auroraStatisticStrings = new AuroraStatisticStrings();
            Character.Proficiency = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.Proficiency);
            AbilitiesCollection abilities = Character.Abilities;
            abilities.Strength.AdditionalScore = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.Strength);
            abilities.Dexterity.AdditionalScore = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.Dexterity);
            abilities.Constitution.AdditionalScore = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.Constitution);
            abilities.Intelligence.AdditionalScore = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.Intelligence);
            abilities.Wisdom.AdditionalScore = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.Wisdom);
            abilities.Charisma.AdditionalScore = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.Charisma);
            abilities.Strength.AdditionalSummery = string.Join(", ", from x in statisticValuesGroupCollection.GetGroup(auroraStatisticStrings.Strength).GetValues()
                                                                     select $"{x.Key} ({x.Value})");
            abilities.Dexterity.AdditionalSummery = string.Join(", ", from x in statisticValuesGroupCollection.GetGroup(auroraStatisticStrings.Dexterity).GetValues()
                                                                      select $"{x.Key} ({x.Value})");
            abilities.Constitution.AdditionalSummery = string.Join(", ", from x in statisticValuesGroupCollection.GetGroup(auroraStatisticStrings.Constitution).GetValues()
                                                                         select $"{x.Key} ({x.Value})");
            abilities.Intelligence.AdditionalSummery = string.Join(", ", from x in statisticValuesGroupCollection.GetGroup(auroraStatisticStrings.Intelligence).GetValues()
                                                                         select $"{x.Key} ({x.Value})");
            abilities.Wisdom.AdditionalSummery = string.Join(", ", from x in statisticValuesGroupCollection.GetGroup(auroraStatisticStrings.Wisdom).GetValues()
                                                                   select $"{x.Key} ({x.Value})");
            abilities.Charisma.AdditionalSummery = string.Join(", ", from x in statisticValuesGroupCollection.GetGroup(auroraStatisticStrings.Charisma).GetValues()
                                                                     select $"{x.Key} ({x.Value})");
            if (abilities.Strength.UseOverrideScore())
            {
                abilities.Strength.AdditionalSummery = string.Join(", ", from x in statisticValuesGroupCollection.GetGroup(auroraStatisticStrings.StrengthSet).GetValues()
                                                                         select $"{x.Key} ({x.Value})");
            }
            if (abilities.Dexterity.UseOverrideScore())
            {
                abilities.Dexterity.AdditionalSummery = string.Join(", ", from x in statisticValuesGroupCollection.GetGroup(auroraStatisticStrings.DexteritySet).GetValues()
                                                                          select $"{x.Key} ({x.Value})");
            }
            if (abilities.Constitution.UseOverrideScore())
            {
                abilities.Constitution.AdditionalSummery = string.Join(", ", from x in statisticValuesGroupCollection.GetGroup(auroraStatisticStrings.ConstitutionSet).GetValues()
                                                                             select $"{x.Key} ({x.Value})");
            }
            if (abilities.Intelligence.UseOverrideScore())
            {
                abilities.Intelligence.AdditionalSummery = string.Join(", ", from x in statisticValuesGroupCollection.GetGroup(auroraStatisticStrings.IntelligenceSet).GetValues()
                                                                             select $"{x.Key} ({x.Value})");
            }
            if (abilities.Wisdom.UseOverrideScore())
            {
                abilities.Wisdom.AdditionalSummery = string.Join(", ", from x in statisticValuesGroupCollection.GetGroup(auroraStatisticStrings.WisdomSet).GetValues()
                                                                       select $"{x.Key} ({x.Value})");
            }
            if (abilities.Charisma.UseOverrideScore())
            {
                abilities.Charisma.AdditionalSummery = string.Join(", ", from x in statisticValuesGroupCollection.GetGroup(auroraStatisticStrings.CharismaSet).GetValues()
                                                                         select $"{x.Key} ({x.Value})");
            }
            Character.SavingThrows.Strength.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.StrengthSaveProficiency);
            Character.SavingThrows.Dexterity.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.DexteritySaveProficiency);
            Character.SavingThrows.Constitution.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.ConstitutionSaveProficiency);
            Character.SavingThrows.Intelligence.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.IntelligenceSaveProficiency);
            Character.SavingThrows.Wisdom.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.WisdomSaveProficiency);
            Character.SavingThrows.Charisma.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.CharismaSaveProficiency);
            Character.SavingThrows.Strength.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.StrengthSaveMisc);
            Character.SavingThrows.Dexterity.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.DexteritySaveMisc);
            Character.SavingThrows.Constitution.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.ConstitutionSaveMisc);
            Character.SavingThrows.Intelligence.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.IntelligenceSaveMisc);
            Character.SavingThrows.Wisdom.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.WisdomSaveMisc);
            Character.SavingThrows.Charisma.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.CharismaSaveMisc);
            Character.Skills.Acrobatics.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.AcrobaticsProficiency);
            Character.Skills.AnimalHandling.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.AnimalHandlingProficiency);
            Character.Skills.Arcana.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.ArcanaProficiency);
            Character.Skills.Athletics.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.AthleticsProficiency);
            Character.Skills.Deception.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.DeceptionProficiency);
            Character.Skills.History.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.HistoryProficiency);
            Character.Skills.Insight.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.InsightProficiency);
            Character.Skills.Intimidation.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.IntimidationProficiency);
            Character.Skills.Investigation.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.InvestigationProficiency);
            Character.Skills.Medicine.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.MedicineProficiency);
            Character.Skills.Nature.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.NatureProficiency);
            Character.Skills.Perception.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.PerceptionProficiency);
            Character.Skills.Performance.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.PerformanceProficiency);
            Character.Skills.Persuasion.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.PersuasionProficiency);
            Character.Skills.Religion.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.ReligionProficiency);
            Character.Skills.SleightOfHand.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.SleightOfHandProficiency);
            Character.Skills.Stealth.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.StealthProficiency);
            Character.Skills.Survival.ProficiencyBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.SurvivalProficiency);
            Character.Skills.Acrobatics.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.AcrobaticsMisc);
            Character.Skills.AnimalHandling.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.AnimalHandlingMisc);
            Character.Skills.Arcana.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.ArcanaMisc);
            Character.Skills.Athletics.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.AthleticsMisc);
            Character.Skills.Deception.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.DeceptionMisc);
            Character.Skills.History.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.HistoryMisc);
            Character.Skills.Insight.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.InsightMisc);
            Character.Skills.Intimidation.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.IntimidationMisc);
            Character.Skills.Investigation.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.InvestigationMisc);
            Character.Skills.Medicine.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.MedicineMisc);
            Character.Skills.Nature.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.NatureMisc);
            Character.Skills.Perception.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.PerceptionMisc);
            Character.Skills.Performance.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.PerformanceMisc);
            Character.Skills.Persuasion.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.PersuasionMisc);
            Character.Skills.Religion.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.ReligionMisc);
            Character.Skills.SleightOfHand.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.SleightOfHandMisc);
            Character.Skills.Stealth.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.StealthMisc);
            Character.Skills.Survival.MiscBonus = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.SurvivalMisc);
            Character.ArmorClass = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.ArmorClass);
            Character.Initiative = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.Initiative);
            Character.Speed = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.Speed);
            Character.MaxHp = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.HitPointsStarting) + statisticValuesGroupCollection.GetValue(auroraStatisticStrings.HitPoints);
            Character.Inventory.MaxAttunedItemCount = statisticValuesGroupCollection.GetValue(auroraStatisticStrings.AttunementMax);
            if (Status.IsLoaded)
            {
                foreach (StatisticValuesGroup item in statisticValuesGroupCollection.Where((StatisticValuesGroup x) => x.GroupName.StartsWith("equipment:")).ToList())
                {
                    string id = item.GroupName.Replace("equipment:", "");
                    int amount = item.Sum();
                    string name = "";
                    foreach (KeyValuePair<string, int> value in item.GetValues())
                    {
                        name = value.Key;
                    }
                    Character.Inventory.AddFromStatistics(new ElementHeader(name, "", "", ""), id, amount);
                }
            }
            ElementsOrganizer elementsOrganizer = SetCharacterBuild();
            Status.HasDragonmark = elementsOrganizer.GetTypes("Grants").Any((ElementBase x) => x.Id.Equals(InternalGrants.Dragonmark));
            if (Status.HasDragonmark)
            {
                Dragonmark dragonmark = elementsOrganizer.GetTypes<Dragonmark>("Dragonmark").FirstOrDefault();
                Character.Dragonmark = dragonmark.Name;
            }
            else
            {
                Character.Dragonmark = "";
            }
            bool canMulticlass = elementsOrganizer.GetTypes("Grants").Any((ElementBase x) => x.Id.Equals(InternalGrants.MulticlassPrerequisite));
            Status.CanMulticlass = canMulticlass;
            ElementBase elementBase = elementsOrganizer.GetTypes("Background").FirstOrDefault();
            Character.Background = ((elementBase != null) ? elementBase.Name : "");
            if (elementBase != null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (XmlNode item2 in elementBase.ElementNode["description"])
                {
                    if (!(item2.Name == "p") || item2.InnerXml.Contains("<span class=\"feature\">") || item2.InnerXml.Contains("<span class=\"emphasis\">") || item2.InnerText.Contains("Skill Proficiencies:"))
                    {
                        break;
                    }
                    stringBuilder.AppendLine(item2.GetInnerText());
                    stringBuilder.AppendLine();
                }
                ElementBase elementBase2 = elementsOrganizer.GetTypes("Background Variant").FirstOrDefault();
                if (elementBase2 != null)
                {
                    foreach (XmlNode item3 in elementBase2.ElementNode["description"])
                    {
                        if (item3.Name == "p")
                        {
                            stringBuilder.AppendLine(item3.GetInnerText());
                            stringBuilder.AppendLine();
                            continue;
                        }
                        break;
                    }
                    Character.Background = elementBase2.GetAlternateName();
                }
                Character.BackgroundStory.OriginalContent = stringBuilder.ToString().Trim();
                int num = elementsOrganizer.GetTypes("Background Feature").Count();
                ElementBase elementBase3 = elementsOrganizer.GetTypes("Background Feature").FirstOrDefault();
                if (num > 1)
                {
                    ElementBase elementBase4 = elementsOrganizer.GetTypes("Background Feature").FirstOrDefault((ElementBase x) => x.Supports.Contains("Background Feature") && x.Supports.Contains("Primary"));
                    if (elementBase4 != null)
                    {
                        elementBase3 = elementBase4;
                    }
                    else
                    {
                        ElementBase elementBase5 = elementsOrganizer.GetTypes("Background Feature").FirstOrDefault((ElementBase x) => x.Supports.Contains("Background Feature"));
                        if (elementBase5 != null)
                        {
                            elementBase3 = elementBase5;
                        }
                    }
                }
                if (elementBase3 != null)
                {
                    Character.BackgroundFeatureName.OriginalContent = elementBase3.GetAlternateName();
                    Character.BackgroundFeatureDescription.OriginalContent = (elementBase3.SheetDescription.Any() ? elementBase3.SheetDescription[0].Description : ElementDescriptionGenerator.GeneratePlainDescription(elementBase3.Description).Trim());
                }
            }
            else
            {
                Character.BackgroundStory.OriginalContent = string.Empty;
                Character.BackgroundFeatureName.OriginalContent = string.Empty;
                Character.BackgroundFeatureDescription.OriginalContent = string.Empty;
            }
            try
            {
                IEnumerable<SelectRule> enumerable = _progressionManager.SelectionRules.Where((SelectRule x) => x.ElementHeader.Type == "Background" && x.Attributes.IsList);
                string text = "";
                string text2 = "";
                string text3 = "";
                string text4 = "";
                bool flag = false;
                foreach (SelectRule selectionRule in enumerable)
                {
                    foreach (KeyValuePair<string, SelectionRuleListItem> selectionRuleListItem in elementsOrganizer.Elements.First((ElementBase x) => x.Id == selectionRule.ElementHeader.Id).SelectionRuleListItems)
                    {
                        if (!flag)
                        {
                            switch (selectionRuleListItem.Key)
                            {
                                case "Personality Trait:1":
                                    text += selectionRuleListItem.Value.Text;
                                    break;
                                case "Personality Trait:2":
                                    text = text + Environment.NewLine + selectionRuleListItem.Value.Text;
                                    break;
                                case "Ideal:1":
                                    text2 += selectionRuleListItem.Value.Text;
                                    break;
                                case "Bond:1":
                                    text3 += selectionRuleListItem.Value.Text;
                                    break;
                                case "Flaw:1":
                                    text4 += selectionRuleListItem.Value.Text;
                                    break;
                            }
                        }
                    }
                    flag = true;
                }
                Character.FillableBackgroundCharacteristics.Traits.OriginalContent = text.Trim();
                Character.FillableBackgroundCharacteristics.Ideals.OriginalContent = text2;
                Character.FillableBackgroundCharacteristics.Bonds.OriginalContent = text3;
                Character.FillableBackgroundCharacteristics.Flaws.OriginalContent = text4;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "SetCharacterDetails");
            }
            ElementBase elementBase6 = elementsOrganizer.GetTypes<ElementBase>("Alignment").FirstOrDefault();
            Character.Alignment = ((elementBase6 != null) ? elementBase6.Name : "");
            if (Status.HasSpellcasting)
            {
                List<ElementBase> source = elementsOrganizer.GetTypes<ElementBase>("Grants").ToList();
                int num2 = source.Count((ElementBase x) => x.Id.Equals(InternalGrants.MulticlassSpellcastingSlotsFull));
                int num3 = source.Count((ElementBase x) => x.Id.Equals(InternalGrants.MulticlassSpellcastingSlotsHalf) || x.Id.Equals(InternalGrants.MulticlassSpellcastingSlotsHalfUp));
                int num4 = source.Count((ElementBase x) => x.Id.Equals(InternalGrants.MulticlassSpellcastingSlotsThird) || x.Id.Equals(InternalGrants.MulticlassSpellcastingSlotsThirdUp));
                int num5 = source.Count((ElementBase x) => x.Id.Equals(InternalGrants.MulticlassSpellcastingSlotsFourth) || x.Id.Equals(InternalGrants.MulticlassSpellcastingSlotsFourthUp));
                int num6 = source.Count((ElementBase x) => x.Id.Equals(InternalGrants.MulticlassSpellcastingSlotsFifth));
                int num7 = num2 + num3 + num4 + num5 + num6;
                Status.HasMulticlassSpellSlots = num7 >= 2;
                if (Status.HasMulticlassSpellSlots)
                {
                    int num8 = statisticValuesGroupCollection.GetGroup("multiclass:spellcasting:level").Sum();
                    Character.MulticlassSpellcasterLevel = num8;
                    StatisticValuesGroupCollection statisticValuesGroupCollection2 = new StatisticsHandler2(this).CalculateValuesAtLevel(num8, new ElementBaseCollection(elementsOrganizer.Elements));
                    //Character.MulticlassSpellSlots.Slot1 = statisticValuesGroupCollection2.GetGroup("multiclass:spellcasting:slot:1").Sum();
                    //Character.MulticlassSpellSlots.Slot2 = statisticValuesGroupCollection2.GetGroup("multiclass:spellcasting:slot:2").Sum();
                    //Character.MulticlassSpellSlots.Slot3 = statisticValuesGroupCollection2.GetGroup("multiclass:spellcasting:slot:3").Sum();
                    //Character.MulticlassSpellSlots.Slot4 = statisticValuesGroupCollection2.GetGroup("multiclass:spellcasting:slot:4").Sum();
                    //Character.MulticlassSpellSlots.Slot5 = statisticValuesGroupCollection2.GetGroup("multiclass:spellcasting:slot:5").Sum();
                    //Character.MulticlassSpellSlots.Slot6 = statisticValuesGroupCollection2.GetGroup("multiclass:spellcasting:slot:6").Sum();
                    //Character.MulticlassSpellSlots.Slot7 = statisticValuesGroupCollection2.GetGroup("multiclass:spellcasting:slot:7").Sum();
                    //Character.MulticlassSpellSlots.Slot8 = statisticValuesGroupCollection2.GetGroup("multiclass:spellcasting:slot:8").Sum();
                    //Character.MulticlassSpellSlots.Slot9 = statisticValuesGroupCollection2.GetGroup("multiclass:spellcasting:slot:9").Sum();
                }
            }
            else
            {
                Status.HasMulticlassSpellSlots = false;
            }
            if (Status.HasCompanion)
            {
                // Character.Companion.Statistics.Update(statisticValuesGroupCollection);
            }
            Character.Inventory.CalculateWeight();
            Current.Status.HasChanges = true;
        }

        private ElementsOrganizer SetCharacterBuild()
        {
            ElementsOrganizer elementsOrganizer = new ElementsOrganizer(GetElements());
            Race race = elementsOrganizer.GetTypes<Race>("Race").FirstOrDefault();
            ElementBase elementBase = elementsOrganizer.GetTypes<ElementBase>("Sub Race").FirstOrDefault();
            Character.Race = ((race != null) ? race.GetAlternateName() : "");
            if (elementBase != null)
            {
                Character.Race = elementBase.GetAlternateName();
            }
            Character.HeightField.OriginalContent = race?.BaseHeight ?? "";
            Character.WeightField.OriginalContent = race?.BaseWeight ?? "";
            Class mainclass = elementsOrganizer.GetTypes<Class>("Class").FirstOrDefault();
            List<Multiclass> list = elementsOrganizer.GetTypes<Multiclass>("Multiclass").ToList();
            Character.Class = ((mainclass != null) ? mainclass.GetAlternateName() : "");
            if (list.Any())
            {
                int? num = ClassProgressionManagers.FirstOrDefault((ClassProgressionManager x) => x.ClassElement == mainclass)?.ProgressionLevel;
                Character.Class += $" ({num})";
                foreach (Multiclass multiclass in list)
                {
                    Character character = Character;
                    character.Class = character.Class + " / " + multiclass.GetAlternateName();
                    int? num2 = ClassProgressionManagers.FirstOrDefault((ClassProgressionManager x) => x.ClassElement == multiclass)?.ProgressionLevel;
                    Character.Class += $" ({num2})";
                }
            }
            _ = Status.HasMulticlass;
            Deity deity = elementsOrganizer.GetTypes<Deity>("Deity").FirstOrDefault();
            Character.Deity = ((deity != null) ? deity.GetAlternateName() : "");
            return elementsOrganizer;
        }

        public ElementBaseCollection GetElements()
        {
            ElementBaseCollection elements = _progressionManager.GetElements();
            foreach (ClassProgressionManager classProgressionManager in ClassProgressionManagers)
            {
                elements.AddRange(classProgressionManager.GetElements());
            }
            return elements;
        }

        [Obsolete]
        public IEnumerable<StatisticRule> GetStatisticRules()
        {
            return (from e in GetElements()
                    where e.ContainsStatisticRules
                    select e).SelectMany((ElementBase e) => e.GetStatisticRules());
        }

        public IEnumerable<ElementBase> GetProficiencyList(IEnumerable<ElementBase> collection)
        {
            ElementBaseCollection elementBaseCollection = new ElementBaseCollection();
            foreach (ElementBase item in collection)
            {
                bool flag = false;
                foreach (ElementBase item2 in elementBaseCollection)
                {
                    if (item2.ContainsRuleElement(item))
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    elementBaseCollection.Add(item);
                }
            }
            return elementBaseCollection;
        }

        public IEnumerable<StatisticRule> GetStatisticRules2()
        {
            List<StatisticRule> list = new List<StatisticRule>();
            list.AddRange(_progressionManager.GetStatisticRules());
            foreach (ClassProgressionManager classProgressionManager in ClassProgressionManagers)
            {
                list.AddRange(classProgressionManager.GetStatisticRules());
            }
            return list;
        }

        public IEnumerable<StatisticRule> GetStatisticRulesAtLevel(int level)
        {
            List<StatisticRule> list = new List<StatisticRule>();
            list.AddRange(_progressionManager.GetStatisticRulesAtLevel(level));
            foreach (ClassProgressionManager classProgressionManager in ClassProgressionManagers)
            {
                list.AddRange(classProgressionManager.GetStatisticRulesAtLevel(level));
            }
            return list;
        }

        public IEnumerable<StatisticRule> GetInlineStatisticRules2()
        {
            List<StatisticRule> list = new List<StatisticRule>();
            list.AddRange(_progressionManager.GetInlineStatisticRules());
            foreach (ClassProgressionManager classProgressionManager in ClassProgressionManagers)
            {
                list.AddRange(classProgressionManager.GetInlineStatisticRules());
            }
            return list;
        }

        [Obsolete]
        public bool AllowMulticlass()
        {
            return GetElements().Any((ElementBase x) => x.Id.Equals(InternalOptions.AllowMulticlassing));
        }

        public IEnumerable<SpellcastingInformation> GetSpellcastingInformations()
        {
            List<SpellcastingInformation> list = new List<SpellcastingInformation>();
            list.AddRange(_progressionManager.SpellcastingInformations);
            foreach (ClassProgressionManager classProgressionManager in ClassProgressionManagers)
            {
                list.AddRange(classProgressionManager.SpellcastingInformations);
            }
            return list;
        }

        public IEnumerable<Spell> GetPreparedSpells()
        {
            List<Spell> list = new List<Spell>();
            SpellcastingSectionHandler current = SpellcastingSectionHandler.Current;
            foreach (SpellcastingInformation item in from x in GetSpellcastingInformations()
                                                     where !x.IsExtension
                                                     select x)
            {
                //foreach (SelectionElement item2 in from x in current.GetSpellcasterSection(item.UniqueIdentifier).GetViewModel<SpellcasterSelectionControlViewModel>().KnownSpells
                //                                   where x.IsChosen
                //                                   orderby x.Element.AsElement<Spell>().Level, x.Element.Name
                //                                   select x)
                {
                    // Spell spell = item2.Element.AsElement<Spell>();
                    // spell.Aquisition.PrepareParent = item.Name;
                    // list.Add(spell);
                }
            }
            return list;
        }

        private void SetPortrait(ElementBase element)
        {
            if (Status.IsUserPortrait)
            {
                return;
            }
            try
            {
                IEnumerable<string> source = from x in Directory.GetFiles(DataManager.Current.UserDocumentsPortraitsDirectory)
                                             select x.ToLower();
                List<string> list = new List<string>();
                List<string> list2 = source.Where((string x) => x.Contains(element.Name.Replace("-", " ").ToLower())).ToList();
                if (list2.Any())
                {
                    string text = Character.Gender.ToLower();
                    List<string> list3 = new List<string>();
                    if (text != null)
                    {
                        _ = text == "male";
                    }
                    list.AddRange(list3.Any() ? list3 : list2);
                    int index = _rnd.Next(list.Count);
                    string portraitFilename = list[index];
                    Character.PortraitFilename = portraitFilename;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "SetPortrait");
            }
        }

        [Obsolete("not yet implemented")]
        public string GenerateCharacterName()
        {
            throw new NotImplementedException();
        }

        public ProgressionManager GetProgressManager(SelectRule selectionRule)
        {
            if (_progressionManager.SelectionRules.Contains(selectionRule))
            {
                return _progressionManager;
            }
            foreach (ClassProgressionManager classProgressionManager in ClassProgressionManagers)
            {
                if (classProgressionManager.SelectionRules.Contains(selectionRule))
                {
                    return classProgressionManager;
                }
            }
            return null;
        }

        public bool ContainsOption(string id)
        {
            return (from x in _progressionManager.GetElements()
                    select x.Id).Contains(id);
        }

        public bool ContainsAverageHitPointsOption()
        {
            return ContainsOption(InternalOptions.AllowAverageHitPoints);
        }

        public bool ContainsMulticlassOption()
        {
            return ContainsOption(InternalOptions.AllowMulticlassing);
        }

        public bool ContainsFeatsOption()
        {
            return ContainsOption(InternalOptions.AllowFeats);
        }
    }

}
