using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Builder.Core;
using Builder.Core.Events;
using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Data.Extensions;
using Builder.Data.Rules;
using Builder.Presentation;
using Builder.Presentation.Elements;
using Builder.Presentation.Events.Character;
using Builder.Presentation.Events.Global;
using Builder.Presentation.Events.Shell;
using Builder.Presentation.Services;
using Builder.Presentation.Services.Calculator;
using Builder.Presentation.Services.Data;
using Builder.Presentation.Services.Sources;
using Builder.Presentation.UserControls;
using Builder.Presentation.ViewModels;
using Builder.Presentation.ViewModels.Base;

namespace Builder.Presentation.ViewModels
{
    public class SelectionRuleExpanderViewModel : ViewModelBase, ISubscriber<CharacterManagerElementRegistered>, ISubscriber<CharacterManagerElementUnregistered>, ISubscriber<CharacterManagerSelectionRuleDeleted>, ISubscriber<ReprocessCharacterEvent>, IDisposable
    {
        private SelectionCollectionService _service;

        private bool _disposed;

        private bool _initialized;

        private string _header;

        private bool _isEnabled;

        private bool _isExpanded;

        private SelectRule _selectionRule;

        private ElementBaseCollection _selectionElements;

        private ElementBase _selectedElement;

        private string _registeredElementId;

        private int _initialLevel = 1;

        private ElementBaseCollection _baseCollection;

        private ElementBaseCollection _baseSupportsCollection;

        private ExpressionInterpreter _interpreter = new ExpressionInterpreter();

        private int _aquisitionLevel;

        private SpellcastingInformation _information;

        private int _retrainLevel;

        private bool _isOptional;

        private SelectionElement _selectedSelectionElement;

        public bool RegisterOnSelection { get; set; }

        public bool IsComboBoxExpander => RegisterOnSelection;

        public string Header
        {
            get
            {
                return _header;
            }
            set
            {
                SetProperty(ref _header, value, "Header");
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                SetProperty(ref _isEnabled, value, "IsEnabled");
            }
        }

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                SetProperty(ref _isExpanded, value, "IsExpanded");
                if (!IsExpanded || !ElementRegistered)
                {
                    return;
                }
                try
                {
                    SelectedElement = SelectionElements.Single((ElementBase e) => e.Id == RegisteredElementId);
                }
                catch (Exception ex)
                {
                    MessageDialogService.ShowException(ex, ex.GetType().ToString(), "Unable to set selected element '" + RegisteredElementId + "'");
                }
            }
        }

        public SelectRule SelectionRule
        {
            get
            {
                return _selectionRule;
            }
            set
            {
                SetProperty(ref _selectionRule, value, "SelectionRule");
            }
        }

        public ElementBaseCollection SelectionElements
        {
            get
            {
                return _selectionElements;
            }
            set
            {
                SetProperty(ref _selectionElements, value, "SelectionElements");
            }
        }

        public ElementBase SelectedElement
        {
            get
            {
                return _selectedElement;
            }
            set
            {
                SetProperty(ref _selectedElement, value, "SelectedElement");
                base.EventAggregator.Send(new ElementDescriptionDisplayRequestEvent(SelectedElement));
                if (RegisterOnSelection && _selectedElement != null)
                {
                    RegisterSelection();
                }
            }
        }

        public string RegisteredElementId
        {
            get
            {
                return _registeredElementId;
            }
            set
            {
                SetProperty(ref _registeredElementId, value, "RegisteredElementId");
                OnPropertyChanged("ElementRegistered", "RegisteredElement");
            }
        }

        public bool ElementRegistered => !string.IsNullOrWhiteSpace(_registeredElementId);

        public ElementBase RegisteredElement
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_registeredElementId))
                {
                    return null;
                }
                return SelectionElements.FirstOrDefault((ElementBase e) => e.Id == _registeredElementId);
            }
        }

        public RelayCommand RegisterElementCommand => new RelayCommand(RegisterSelection);

        public RelayCommand UnregisterElementCommand => new RelayCommand(UnregisterRegisteredElement);

        public ICommand ManageSourcesCommand => new RelayCommand(ManageSources);

        private CharacterManager Manager { get; set; }

        public bool ContainsCantrips { get; set; }

        // public ElementFilter Filter { get; set; }

        public bool EnableFilter { get; set; }

        public int RetrainLevel
        {
            get
            {
                return _retrainLevel;
            }
            set
            {
                SetProperty(ref _retrainLevel, value, "RetrainLevel");
            }
        }

        public bool IsOptional
        {
            get
            {
                return _isOptional;
            }
            set
            {
                SetProperty(ref _isOptional, value, "IsOptional");
            }
        }

        public SelectionElementCollection SelectionElementsCollection { get; } = new SelectionElementCollection();

        public SelectionElement SelectedSelectionElement
        {
            get
            {
                return _selectedSelectionElement;
            }
            set
            {
                SetProperty(ref _selectedSelectionElement, value, "SelectedSelectionElement");
                SelectedElement = _selectedSelectionElement?.Element;
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Dispose();
                    _baseCollection.Clear();
                    _baseSupportsCollection.Clear();
                    _interpreter = null;
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public SelectionRuleExpanderViewModel()
        {
            if (base.IsInDesignMode)
            {
                InitializeDesignData();
            }
        }

        public SelectionRuleExpanderViewModel(SelectRule selectionRule)
        {
            SubscribeWithEventAggregator();
            SelectionRule = selectionRule;
            SelectionElements = new ElementBaseCollection();
            IsOptional = selectionRule.Attributes.Optional;
            Header = (string.IsNullOrWhiteSpace(_selectionRule.Attributes.Name) ? _selectionRule.ElementHeader.Name : _selectionRule.Attributes.Name);
            if (IsOptional)
            {
                Header += " (optional)";
            }
            IsExpanded = true;
            IsEnabled = true;
            Logger.Debug("Expander Created: [{0}]", _selectionRule.Attributes.Name);
        }

        public override async Task InitializeAsync(InitializationArguments args)
        {
            try
            {
                if (SelectionRule.Attributes.Type == "Spell")
                {
                    IsExpanded = false;
                }
                Stopwatch.StartNew();
                SetSupportedElements(initial: true, SelectionRule);
                _initialized = true;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "InitializeAsync");
                MessageDialogService.ShowException(ex);
            }
            await base.InitializeAsync(args);
        }

        private void ManageSources()
        {
            base.EventAggregator.Send(new SelectionRuleNavigationArgs(NavigationLocation.StartSources));
        }

        private async Task<ElementBaseCollection> GetSupported(SelectRule rule)
        {
            ElementBaseCollection collection = _baseCollection;
            await Task.Run(delegate
            {
                collection.GetSupportedElements(rule);
            });
            return collection;
        }

        private void SetSupportedElements(bool initial, SelectRule rule)
        {
            if (rule.Attributes.Type.Equals("Spell"))
            {
                GenerateBaseCollections(initial, rule);
            }
            else
            {
                _interpreter.InitializeWithSelectionRule(rule);
                if (initial)
                {
                    _baseCollection = new ElementBaseCollection(DataManager.Current.ElementsCollection.Where((ElementBase element) => element.Type.Equals(rule.Attributes.Type)));
                    _initialLevel = rule.Attributes.RequiredLevel;
                }
                if (initial && rule.Attributes.ContainsSupports())
                {
                    string text = rule.Attributes.Supports;
                    if (rule.Attributes.ContainsDynamicSupports() && rule.Attributes.Supports.Contains("$$SPELLSLOT"))
                    {
                        List<string> list = new List<string>();
                        for (int i = 1; i <= 9; i++)
                        {
                            if (CharacterManager.Current.StatisticsCalculator.StatisticValues.ContainsGroup("spellcasting:slot:" + i) && CharacterManager.Current.StatisticsCalculator.StatisticValues.GetValue("spellcasting:slot:" + i) > 0)
                            {
                                list.Add(i.ToString());
                            }
                        }
                        if (list.Any())
                        {
                            text = text.Replace("$$SPELLSLOT", string.Join("||", list));
                        }
                    }
                    IEnumerable<ElementBase> elements = _interpreter.EvaluateSupportsExpression(text, _baseCollection, rule.Attributes.SupportsElementIdRange());
                    _baseSupportsCollection = new ElementBaseCollection(elements);
                }
                else if (initial)
                {
                    _baseSupportsCollection = new ElementBaseCollection(_baseCollection);
                }
            }
            ElementBaseCollection elementBaseCollection = new ElementBaseCollection(_baseSupportsCollection);
            SourcesManager sourcesManager = CharacterManager.Current.SourcesManager;
            List<string> list2 = sourcesManager.GetUndefinedRestrictedSourceNames().ToList();
            List<string> list3 = sourcesManager.GetRestrictedElementIds().ToList();
            ElementBaseCollection elementBaseCollection2 = new ElementBaseCollection();
            foreach (ElementBase item in elementBaseCollection)
            {
                if (list3.Contains(item.Id))
                {
                    elementBaseCollection2.Add(item);
                }
                else if (list2.Contains(item.Source))
                {
                    elementBaseCollection2.Add(item);
                }
            }
            foreach (ElementBase item2 in elementBaseCollection2)
            {
                elementBaseCollection.Remove(item2);
            }
            ElementBaseCollection elements2 = CharacterManager.Current.GetElements();
            foreach (ElementBase item3 in elements2.Where((ElementBase e) => e.Type.Equals(rule.Attributes.Type)))
            {
                if (elementBaseCollection.Contains(item3) && !item3.AllowDuplicate && !item3.Id.Equals(RegisteredElementId))
                {
                    elementBaseCollection.RemoveElement(item3.Id);
                }
            }
            ElementBaseCollection elementBaseCollection3 = new ElementBaseCollection();
            switch (rule.Attributes.Type)
            {
                case "Spell":
                    elementBaseCollection3.AddRange(from Spell x in elementBaseCollection
                                                    orderby x.Level, x.Name
                                                    select x);
                    break;
                case "Alignment":
                    elementBaseCollection3.AddRange(elementBaseCollection);
                    break;
                default:
                    if (!string.IsNullOrWhiteSpace(rule.Attributes.Name) && rule.Attributes.Name.Contains("Ability Score"))
                    {
                        if (rule.Attributes.Type == "Racial Trait" || rule.Attributes.Type == "Class Feature" || rule.Attributes.Type == "Ability Score Improvement")
                        {
                            elementBaseCollection3.AddRange(elementBaseCollection);
                            break;
                        }
                        elementBaseCollection3.AddRange(elementBaseCollection.OrderBy((ElementBase x) => x.Name));
                    }
                    else
                    {
                        elementBaseCollection3.AddRange(elementBaseCollection.OrderBy((ElementBase x) => x.Name));
                    }
                    break;
            }
            elementBaseCollection = elementBaseCollection3;
            List<ElementBase> list4 = new List<ElementBase>();
            if (EnableFilter)
            {
                // list4 = Filter.Filter(elementBaseCollection.ToList());
            }
            SelectionElementsCollection.Clear();
            if (_baseSupportsCollection.Any((ElementBase x) => x.HasRequirements))
            {
                ElementBaseCollection elementBaseCollection4 = new ElementBaseCollection();
                List<string> elementsIDs = elements2.Select((ElementBase e) => e.Id).ToList();
                foreach (ElementBase item4 in elementBaseCollection)
                {
                    if (item4.HasRequirements)
                    {
                        if (_interpreter.EvaluateElementRequirementsExpression(item4.Requirements, elementsIDs))
                        {
                            elementBaseCollection4.Add(item4);
                            SelectionElementsCollection.Add(new SelectionElement(item4)
                            {
                                IsHighlighted = (list4.Contains(item4) && EnableFilter)
                            });
                        }
                        else if (!base.IsInDebugMode)
                        {
                        }
                    }
                    else
                    {
                        elementBaseCollection4.Add(item4);
                        SelectionElementsCollection.Add(new SelectionElement(item4)
                        {
                            IsHighlighted = (list4.Contains(item4) && EnableFilter)
                        });
                    }
                }
                elementBaseCollection = elementBaseCollection4;
            }
            else
            {
                foreach (ElementBase item5 in elementBaseCollection)
                {
                    SelectionElementsCollection.Add(new SelectionElement(item5)
                    {
                        IsHighlighted = (list4.Contains(item5) && EnableFilter)
                    });
                }
            }
            if (IsComboBoxExpander)
            {
                _selectedSelectionElement = SelectionElementsCollection.FirstOrDefault((SelectionElement x) => x.Element == _selectedElement);
                OnPropertyChanged("SelectedSelectionElement");
            }
            if (!IsComboBoxExpander)
            {
                SelectedElement = null;
            }
            SelectionElements.Clear();
            SelectionElements.AddRange(elementBaseCollection);
            if (_registeredElementId != null)
            {
                ElementBase elementBase = SelectionElements.FirstOrDefault((ElementBase element) => element.Id == _registeredElementId);
                if (elementBase != null)
                {
                    _selectedElement = elementBase;
                    SelectionElement selectionElement = SelectionElementsCollection.FirstOrDefault((SelectionElement x) => x.Element.Id.Equals(_selectedElement.Id));
                    if (selectionElement != null)
                    {
                        selectionElement.IsChosen = true;
                    }
                }
                else
                {
                    UnregisterRegisteredElement(fromReevaluation: true);
                }
            }
            try
            {
                IsEnabled = true;
                if (initial && rule.Attributes.ContainsDefaultSelection())
                {
                    ElementBase elementBase2 = SelectionElements.SingleOrDefault((ElementBase x) => x.Id == rule.Attributes.Default);
                    if (elementBase2 != null)
                    {
                        _selectedElement = elementBase2;
                        RegisterSelection();
                        if (SelectionElements.Count() == 1)
                        {
                            IsEnabled = false;
                        }
                    }
                }
                if (rule.Attributes.ContainsDefaultSelection() && SelectionElements.Count() == 1 && RegisteredElement != null)
                {
                    IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Warning($"{ex.GetType()} while trying to set the [default:{rule.Attributes.Default}] element on {rule} ");
                Logger.Exception(ex, "SetSupportedElements");
            }
            if (!IsExpanded && !ElementRegistered && rule.Attributes.Type != "Spell")
            {
                IsExpanded = true;
            }
            rule.Attributes.ContainsDefaultSelection();
            if (rule.Attributes.Optional && SelectionElements.Count == 0)
            {
                IsEnabled = false;
                IsExpanded = false;
            }
            foreach (SelectionElement item6 in SelectionElementsCollection)
            {
                if (item6.Element.HasPrerequisites)
                {
                    item6.DisplayShortDescription = item6.Element.Prerequisite;
                }
            }
            OnPropertyChanged("SelectedSelectionElement");
        }

        protected bool SupportContainsDynamicExpression(string supports)
        {
            if (string.IsNullOrWhiteSpace(supports))
            {
                return false;
            }
            string[] all = DynamicSupportExpressions.All;
            foreach (string value in all)
            {
                if (supports.Contains(value))
                {
                    return true;
                }
            }
            return false;
        }

        protected void GenerateBaseCollections(bool initial, SelectRule rule)
        {
            if (initial)
            {
                if (Manager == null)
                {
                    Manager = CharacterManager.Current;
                }
                if (_interpreter == null)
                {
                    _interpreter = new ExpressionInterpreter();
                }
                _interpreter.InitializeWithSelectionRule(rule);
                _baseCollection = new ElementBaseCollection(DataManager.Current.ElementsCollection.Where((ElementBase element) => element.Type.Equals(rule.Attributes.Type)));
                _aquisitionLevel = rule.Attributes.RequiredLevel;
                _baseSupportsCollection = new ElementBaseCollection();
            }
            if (SupportContainsDynamicExpression(rule.Attributes.Supports) || RetrainLevel > 0)
            {
                _baseSupportsCollection.Clear();
                List<string> list = new List<string>();
                string text = rule.Attributes.Supports;
                SpellcastingInformation spellcastingInformation = Manager.GetSpellcastingInformations().FirstOrDefault((SpellcastingInformation x) => x.Name.Equals(rule.Attributes.SpellcastingName) && !x.IsExtension);
                if (spellcastingInformation != null)
                {
                    _information = spellcastingInformation;
                }
                if (spellcastingInformation == null)
                {
                    if (_information == null)
                    {
                        throw new Exception($"{rule} (in {rule.ElementHeader.Name}) does not have a spellcasting attribute with the name of the spellcasting class (e.g. Wizard or Arcane Trickster), this is required when using the dynamic support expressions");
                    }
                    spellcastingInformation = _information;
                }
                if (rule.Attributes.Supports.Contains("$(spellcasting:list)"))
                {
                    List<SpellcastingInformation> list2 = (from x in Manager.GetSpellcastingInformations()
                                                           where (x.Name.Equals(rule.Attributes.SpellcastingName) || x.AssignToAllSpellcastingClasses) && x.IsExtension
                                                           select x).ToList();
                    if (list2.Any())
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append("(" + spellcastingInformation.InitialSupportedSpellsExpression.Supports + ")");
                        foreach (SpellcastingInformation item in list2)
                        {
                            foreach (SpellcastingInformation.SpellcastingList extendedSupportedSpellsExpression in item.ExtendedSupportedSpellsExpressions)
                            {
                                if (extendedSupportedSpellsExpression.IsId)
                                {
                                    if (extendedSupportedSpellsExpression.Supports.Contains(","))
                                    {
                                        list.AddRange(from x in extendedSupportedSpellsExpression.Supports.Split(',')
                                                      select x.Trim());
                                    }
                                    else
                                    {
                                        list.Add(extendedSupportedSpellsExpression.Supports);
                                    }
                                }
                                else
                                {
                                    stringBuilder.Append("||(" + extendedSupportedSpellsExpression.Supports + ")");
                                }
                            }
                        }
                        text = text.Replace("$(spellcasting:list)", $"({stringBuilder})");
                    }
                    else
                    {
                        text = text.Replace("$(spellcasting:list)", spellcastingInformation.InitialSupportedSpellsExpression.Supports);
                    }
                }
                if (rule.Attributes.Supports.Contains("$(spellcasting:slots)") || RetrainLevel > 0)
                {
                    StatisticValuesGroupCollection statisticValuesGroupCollection = Manager.StatisticsCalculator.CalculateValuesAtLevel((RetrainLevel > 0) ? RetrainLevel : _aquisitionLevel, Manager.GetElements());
                    List<int> activeSpellSlots = new List<int>();
                    for (int num = 9; num >= 1; num--)
                    {
                        if (statisticValuesGroupCollection.ContainsGroup(spellcastingInformation.GetSlotStatisticName(num)) || activeSpellSlots.Any())
                        {
                            if (statisticValuesGroupCollection.GetValue(spellcastingInformation.GetSlotStatisticName(num)) > 0)
                            {
                                activeSpellSlots.Add(num);
                            }
                            else
                            {
                                activeSpellSlots.Add(num);
                            }
                        }
                    }
                    if (!activeSpellSlots.Any())
                    {
                        activeSpellSlots.Add(99);
                    }
                    if (list.Any())
                    {
                        foreach (string id in list)
                        {
                            ElementBase elementBase = _baseCollection.FirstOrDefault((ElementBase x) => x.Id.Equals(id) && activeSpellSlots.Contains(x.AsElement<Spell>().Level));
                            if (elementBase != null)
                            {
                                _baseSupportsCollection.Add(elementBase);
                            }
                        }
                        list.Clear();
                    }
                    if (!rule.Attributes.Supports.Contains("$(spellcasting:slots)"))
                    {
                        string text2 = string.Join("||", activeSpellSlots);
                        for (int i = 1; i <= 9; i++)
                        {
                            if (text.Contains(i.ToString()))
                            {
                                text = text.Replace(i.ToString(), "(" + text2 + ")");
                                break;
                            }
                        }
                    }
                    text = text.Replace("$(spellcasting:slots)", "(" + string.Join("||", activeSpellSlots) + ")");
                }
                else if (!text.Contains("0") && list.Any())
                {
                    int num2 = -1;
                    if (_baseSupportsCollection.Any())
                    {
                        num2 = (from Spell x in _baseSupportsCollection
                                select x.Level).Max();
                    }
                    foreach (string id2 in list)
                    {
                        ElementBase elementBase2 = _baseCollection.FirstOrDefault((ElementBase x) => x.Id.Equals(id2));
                        if (elementBase2 != null && num2 > 0)
                        {
                            _ = (elementBase2 as Spell).Level;
                        }
                    }
                }
                if (SupportContainsDynamicExpression(text) && Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                IEnumerable<ElementBase> enumerable = _interpreter.EvaluateSupportsExpression(text, _baseCollection, rule.Attributes.SupportsElementIdRange());
                if (text.Contains("0"))
                {
                    enumerable.Any((ElementBase x) => x.AsElement<Spell>().Level > 0);
                }
                if (_baseSupportsCollection.Any())
                {
                    foreach (ElementBase elementBase3 in enumerable)
                    {
                        if (_baseSupportsCollection.FirstOrDefault((ElementBase x) => x.Id.Equals(elementBase3.Id)) == null)
                        {
                            _baseSupportsCollection.Add(elementBase3);
                        }
                    }
                }
                else
                {
                    _baseSupportsCollection.AddRange(enumerable);
                }
                text.Contains("0");
                if (list.Any())
                {
                    int num3 = 0;
                    int num4 = -1;
                    if (_baseSupportsCollection.Any())
                    {
                        num4 = (from Spell x in _baseSupportsCollection
                                select x.Level).Max();
                    }
                    List<string> list3 = _baseSupportsCollection.Select((ElementBase x) => x.Id).ToList();
                    foreach (string id3 in list)
                    {
                        ElementBase elementBase4 = _baseCollection.FirstOrDefault((ElementBase x) => x.Id.Equals(id3));
                        if (elementBase4 == null || list3.Contains(elementBase4.Id))
                        {
                            continue;
                        }
                        if (num4 > 0)
                        {
                            Spell spell = elementBase4 as Spell;
                            if (spell.Level > num4 || spell.Level == 0)
                            {
                                continue;
                            }
                        }
                        if (num4 != 0 || (elementBase4 as Spell).Level <= num4)
                        {
                            _baseSupportsCollection.Add(elementBase4);
                            num3++;
                        }
                    }
                    Logger.Info($"added {num3} from extended range to {text} with max level set to {num4}");
                    list.Clear();
                }
            }
            else if (rule.Attributes.ContainsSupports())
            {
                if (initial)
                {
                    IEnumerable<ElementBase> elements = _interpreter.EvaluateSupportsExpression(rule.Attributes.Supports, _baseCollection, rule.Attributes.SupportsElementIdRange());
                    _baseSupportsCollection = new ElementBaseCollection(elements);
                }
            }
            else if (initial)
            {
                _baseSupportsCollection = new ElementBaseCollection(_baseCollection);
            }
            ContainsCantrips = _baseSupportsCollection.Any((ElementBase x) => x.Type.Equals("Spell") && x.Supports.Contains("0"));
        }

        public int GetAquisitionLevel()
        {
            return _aquisitionLevel;
        }

        public void ActivateFilter()
        {
            SetSupportedElements(initial: false, SelectionRule);
        }

        private void GetAvailableElements(bool initial)
        {
            _ = SelectionRule;
        }

        private void PopulateAvailableElements(IEnumerable<ElementBase> availableElements, bool initial, SelectRule rule)
        {
            SelectionElementCollection selectionElementCollection = new SelectionElementCollection();
            foreach (ElementBase availableElement in availableElements)
            {
                selectionElementCollection.Add(new SelectionElement(availableElement));
            }
        }

        public ElementBase GetRegisteredElement()
        {
            return RegisteredElement;
        }

        private void RegisterSelection()
        {
            if (SelectedElement != null && (!ElementRegistered || !(RegisteredElementId == SelectedElement.Id)))
            {
                if (ElementRegistered)
                {
                    UnregisterRegisteredElement(fromReevaluation: false);
                }
                RegisteredElementId = SelectedElement.Id;
                ElementBase elementBase = SelectionElements.First((ElementBase e) => e.Id == SelectedElement.Id);
                elementBase.Aquisition.SelectedBy(SelectionRule);
                ElementBase elementBase2 = CharacterManager.Current.RegisterElement(elementBase);
                RegisteredElementId = elementBase2.Id;
                IsExpanded = !ElementRegistered;
            }
        }

        private void UnregisterRegisteredElement()
        {
            UnregisterRegisteredElement(fromReevaluation: false);
        }

        private void UnregisterRegisteredElement(bool fromReevaluation)
        {
            if (!ElementRegistered)
            {
                throw new ArgumentException("unable to unregister when nothing has been registered");
            }
            ElementBase elementBase = DataManager.Current.ElementsCollection.GetElement(RegisteredElementId);
            if (elementBase == null)
            {
                elementBase = DataManager.Current.ElementsCollection.FirstOrDefault((ElementBase x) => x.Id.Equals(RegisteredElementId));
            }
            if (fromReevaluation)
            {
                base.EventAggregator.Send(new MainWindowStatusUpdateEvent("Your selection (" + elementBase.Name + ") in " + Header + " was removed due to loss of requirement."));
            }
            RegisteredElementId = null;
            CharacterManager.Current.UnregisterElement(elementBase);
            IsExpanded = !ElementRegistered;
        }

        public async void SetSelectionAndRegister(string id)
        {
            try
            {
                ElementBase elementBase = SelectionElements.FirstOrDefault((ElementBase x) => x.Id == id);
                if (elementBase == null)
                {
                    int count;
                    for (count = 0; count < 25; count++)
                    {
                        await Task.Delay(100);
                        elementBase = SelectionElements.FirstOrDefault((ElementBase x) => x.Id == id);
                        if (elementBase != null)
                        {
                            break;
                        }
                    }
                    Logger.Warning($"it took {count * 100}ms to register {id}");
                }
                if (elementBase == null)
                {
                    throw new InvalidOperationException("Unable to find the element with id: " + id + " maybe it has been renamed since the character was saved. ");
                }
                SelectedElement = elementBase;
                RegisterSelection();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "SetSelectionAndRegister");
            }
        }

        public void OnHandleEvent(CharacterManagerElementRegistered args)
        {
            if (RegisteredElement != null && RegisteredElement == args.Element)
            {
                Logger.Debug("not populating the {0} expander after this selection", SelectionRule);
            }
            else
            {
                SetSupportedElements(initial: false, SelectionRule);
            }
        }

        public void OnHandleEvent(CharacterManagerElementUnregistered args)
        {
            if (args.Element.HasSpellcastingInformation)
            {
                Logger.Warning("//here it might fail from getting the proper list");
            }
            SetSupportedElements(initial: false, SelectionRule);
        }

        public void OnHandleEvent(CharacterManagerSelectionRuleDeleted args)
        {
            if (SelectionRule == null)
            {
                Logger.Warning("selection rule empty on '{0}' expander", Header);
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                Header += " - RULE MISSING";
                IsEnabled = false;
            }
            else
            {
                IsEnabled = true;
                if (args.SelectionRule.UniqueIdentifier == SelectionRule.UniqueIdentifier && ElementRegistered)
                {
                    UnregisterRegisteredElement(fromReevaluation: false);
                }
            }
        }

        public void Repopulate()
        {
            SetSupportedElements(initial: false, SelectionRule);
        }

        protected override void InitializeDesignData()
        {
            base.InitializeDesignData();
            IsEnabled = true;
            Header = "Language (Optional)";
            IsExpanded = true;
            _selectionElements = new ElementBaseCollection
        {
            new ElementBase
            {
                ElementHeader = new ElementHeader("Common", "Language", "Player’s Handbook", "1")
            },
            new ElementBase
            {
                ElementHeader = new ElementHeader("Gnomish", "Language", "Dungeon Master's Guide", "2")
            },
            new ElementBase
            {
                ElementHeader = new ElementHeader("Elvish", "Language", "Player’s Handbook", "3")
            },
            new ElementBase
            {
                ElementHeader = new ElementHeader("Fireball", "Spell", "Sword Coast Adventurer's Guide", "4")
            },
            new ElementBase
            {
                ElementHeader = new ElementHeader("Skilled", "Feat", "PHB", "5")
            }
        };
            SelectedElement = _selectionElements[1];
            RegisteredElementId = _selectedElement.Id;
            foreach (ElementBase selectionElement in _selectionElements)
            {
                SelectionElementsCollection.Add(new SelectionElement(selectionElement));
            }
            SelectedSelectionElement = SelectionElementsCollection.FirstOrDefault();
            SelectedSelectionElement.IsEnabled = true;
        }

        private void DepricatedPopulateSelectionElements(bool isInitialPopulating)
        {
            try
            {
                _ = SelectionRule;
                string supportString = SelectionRule.Attributes.Supports;
                SelectionRule.ElementHeader.Name.Contains("Expertise");
                List<ElementBase> list = (from e in DataManager.Current.ElementsCollection
                                          where e.Type == SelectionRule.Attributes.Type
                                          select e into x
                                          orderby x.Source, x.Name
                                          select x).ToList();
                ElementBaseCollection elementBaseCollection = new ElementBaseCollection();
                if (SelectionRule.Attributes.ContainsSupports())
                {
                    if (supportString.Contains(',') && supportString.Contains('|'))
                    {
                        throw new NotSupportedException();
                    }
                    if (supportString.Contains(',') && !supportString.Contains('|'))
                    {
                        elementBaseCollection.AddRange(list);
                        foreach (string item in from s in supportString.Split(',')
                                                select s.Trim())
                        {
                            string str = item;
                            elementBaseCollection = new ElementBaseCollection(elementBaseCollection.Where((ElementBase e) => e.Supports.Contains(str)));
                        }
                    }
                    else if (!supportString.Contains(',') && supportString.Contains('|'))
                    {
                        string[] array = supportString.Split('|');
                        foreach (string id in array)
                        {
                            ElementBase elementBase = list.FirstOrDefault((ElementBase e) => e.Id == id.Trim());
                            if (elementBase == null)
                            {
                                Logger.Warning($"unable to find id:{id.Trim()} for populating {SelectionRule}, maybe a typo in the ID");
                            }
                            else
                            {
                                elementBaseCollection.Add(elementBase);
                            }
                        }
                    }
                    else
                    {
                        ElementBaseCollection elementBaseCollection2 = elementBaseCollection;
                        IEnumerable<ElementBase> elements;
                        if (!SelectionRule.Attributes.ContainsSupports())
                        {
                            IEnumerable<ElementBase> enumerable = list;
                            elements = enumerable;
                        }
                        else
                        {
                            elements = list.Where((ElementBase e) => e.Supports.Contains(supportString));
                        }
                        elementBaseCollection2.AddRange(elements);
                    }
                }
                else
                {
                    ElementBaseCollection elementBaseCollection3 = elementBaseCollection;
                    IEnumerable<ElementBase> elements2;
                    if (!SelectionRule.Attributes.ContainsSupports())
                    {
                        IEnumerable<ElementBase> enumerable = list;
                        elements2 = enumerable;
                    }
                    else
                    {
                        elements2 = list.Where((ElementBase e) => e.Supports.Contains(supportString));
                    }
                    elementBaseCollection3.AddRange(elements2);
                }
                foreach (ElementBase item2 in CharacterManager.Current.GetElements().ToList())
                {
                    if (elementBaseCollection.Contains(item2) && !item2.AllowDuplicate && item2.Id != _registeredElementId)
                    {
                        elementBaseCollection.RemoveElement(item2.Id);
                    }
                }
                try
                {
                    if (SelectedElement != null)
                    {
                        _ = _registeredElementId;
                        SelectedElement = null;
                    }
                    foreach (string item3 in SelectionElements.Select((ElementBase x) => x.Id).ToList())
                    {
                        SelectionElements.RemoveElement(item3);
                    }
                    SelectionElements.AddRange(elementBaseCollection);
                    if (!string.IsNullOrWhiteSpace(_registeredElementId))
                    {
                        _selectedElement = SelectionElements.Single((ElementBase x) => x.Id == _registeredElementId);
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    ElementBaseCollection elementBaseCollection4 = new ElementBaseCollection();
                    foreach (ElementBase selectionElement in SelectionElements)
                    {
                        if (!elementBaseCollection.Contains(selectionElement))
                        {
                            elementBaseCollection4.Add(selectionElement);
                        }
                    }
                    Logger.Exception(ex, "DepricatedPopulateSelectionElements");
                }
                if (!_initialized && !string.IsNullOrWhiteSpace(SelectionRule.Attributes.Default))
                {
                    ElementBase elementBase2 = SelectionElements.FirstOrDefault((ElementBase e) => e.Id == SelectionRule.Attributes.Default);
                    if (elementBase2 != null)
                    {
                        try
                        {
                            SelectedElement = elementBase2;
                            RegisterSelection();
                        }
                        catch (Exception ex2)
                        {
                            Logger.Exception(ex2, "DepricatedPopulateSelectionElements");
                            MessageDialogService.ShowException(ex2);
                        }
                    }
                }
                if (string.IsNullOrWhiteSpace(_registeredElementId) && !IsExpanded)
                {
                    IsExpanded = true;
                }
            }
            catch (Exception ex3)
            {
                Logger.Exception(ex3, "DepricatedPopulateSelectionElements");
                MessageDialogService.ShowException(ex3);
            }
        }

        public void OnHandleEvent(ReprocessCharacterEvent args)
        {
            if (!(SelectionRule.Attributes.Type == "Spell"))
            {
                SetSupportedElements(initial: false, SelectionRule);
            }
        }
    }
}
