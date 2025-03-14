using Builder.Core;
using Builder.Core.Events;
using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Rules;
using Builder.Presentation.Events.Global;
using Builder.Presentation.Services;
using Builder.Presentation.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Builder.Presentation.ViewModels
{
    public sealed class ListItemSelectionRuleExpanderViewModel : ViewModelBase, ISubscriber<CharacterManagerSelectionRuleDeleted>
    {
        private ElementBase _parentElement;

        private SelectRule _selectionRule;

        private readonly int _number;

        private string _header;

        private bool _isExpanded;

        private bool _selectionMade;

        private SelectionRuleListItem _selectedItem;

        private SelectionRuleListItem _registeredItem;

        public ObservableCollection<SelectionRuleListItem> SelectionItems { get; set; }

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

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                if (SetProperty(ref _isExpanded, value, "IsExpanded") && _isExpanded)
                {
                    if (_selectionMade)
                    {
                        SelectedItem = RegisteredItem;
                    }
                    else
                    {
                        SelectedItem = null;
                    }
                }
            }
        }

        public SelectionRuleListItem SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                SetProperty(ref _selectedItem, value, "SelectedItem");
            }
        }

        public bool SelectionMade
        {
            get
            {
                return _selectionMade;
            }
            set
            {
                SetProperty(ref _selectionMade, value, "SelectionMade");
            }
        }

        public SelectionRuleListItem RegisteredItem
        {
            get
            {
                return _registeredItem;
            }
            set
            {
                SetProperty(ref _registeredItem, value, "RegisteredItem");
            }
        }

        public RelayCommand SetCommand => new RelayCommand(RegisterSelection);

        public RelayCommand UnsetCommand => new RelayCommand(UnregisteredSelection);

        public ListItemSelectionRuleExpanderViewModel()
        {
            ApplicationManager.Current.EventAggregator.Subscribe(this);
            if (base.IsInDesignMode)
            {
                InitializeDesignData();
            }
        }

        public ListItemSelectionRuleExpanderViewModel(SelectRule selectionRule, int number)
        {
            ApplicationManager.Current.EventAggregator.Subscribe(this);
            _selectionRule = selectionRule;
            _number = number;
            SelectionItems = new ObservableCollection<SelectionRuleListItem>();
        }

        public override async Task InitializeAsync(InitializationArguments args)
        {
            try
            {
                IsExpanded = true;
                SelectedItem = null;
                Header = (string.IsNullOrWhiteSpace(SelectionRule.Attributes.Name) ? SelectionRule.ElementHeader.Type.ToUpper() : string.Format("{0}", SelectionRule.Attributes.Name, SelectionRule.ElementHeader.Name).ToUpper());
                if (_selectionRule.Attributes.Optional)
                {
                    Header += " (optional)";
                }
                if (!SelectionRule.Attributes.IsList)
                {
                    throw new Exception("wrong expander chosen");
                }
                foreach (SelectionRuleListItem listItem in SelectionRule.Attributes.ListItems)
                {
                    SelectionItems.Add(listItem);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "InitializeAsync");
                MessageDialogService.ShowException(ex, ToString());
            }
            await base.InitializeAsync(args);
        }

        public string GetKey()
        {
            return $"{SelectionRule.Attributes.Name}:{_number}";
        }

        private void RegisterSelection()
        {
            if (RegisteredItem != null)
            {
                Logger.Info("trying to register with registered element set");
            }
            if (SelectedItem.ID == RegisteredItem?.ID)
            {
                return;
            }
            if (SelectionMade)
            {
                UnregisteredSelection();
            }
            try
            {
                ElementBase elementBase = CharacterManager.Current.GetElements().First((ElementBase e) => e.Id == SelectionRule.ElementHeader.Id);
                if (elementBase.SelectionRuleListItems.ContainsKey(GetKey()))
                {
                    Logger.Warning("'{0}' SelectionRuleListItems already contains the key '{1}'", elementBase.Name, GetKey());
                }
                else
                {
                    elementBase.SelectionRuleListItems.Add(GetKey(), SelectedItem);
                }
                RegisteredItem = new SelectionRuleListItem(SelectedItem.ID, SelectedItem.Text);
                SelectionMade = true;
                IsExpanded = false;
                base.EventAggregator.Send(new ListSelectionRuleRegisteredEvent(SelectionRule));
                CharacterManager.Current.ReprocessCharacter();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "RegisterSelection");
                MessageDialogService.ShowException(ex);
            }
        }

        private void UnregisteredSelection()
        {
            if (RegisteredItem == null)
            {
                Logger.Info("trying to unregister NULL");
                return;
            }
            ElementBaseCollection elements = CharacterManager.Current.GetElements();
            if (!elements.Any() && Debugger.IsAttached)
            {
                Debugger.Break();
            }
            ElementBase elementBase = elements.FirstOrDefault((ElementBase e) => e.Id == SelectionRule.ElementHeader.Id);
            if (elementBase == null)
            {
                Logger.Warning("old element removed and not able to unregister selection");
            }
            else if (!elementBase.SelectionRuleListItems.Remove(GetKey()))
            {
                Logger.Info("FAILED TO REMOVE " + GetKey());
            }
            else
            {
                Logger.Info("REMOVED " + GetKey());
            }
            RegisteredItem = null;
            IsExpanded = true;
            SelectionMade = false;
            base.EventAggregator.Send(new ListSelectionRuleUnregisteredEvent(SelectionRule));
            CharacterManager.Current.ReprocessCharacter();
        }

        protected override void InitializeDesignData()
        {
            base.InitializeDesignData();
            _header = "Specialty";
            SelectionRule = new SelectRule(new ElementHeader("parent name", "parent type", "handbook", "id"));
            SelectionRule.Attributes.Type = "List";
            SelectionItems = new ObservableCollection<SelectionRuleListItem>();
            SelectionRule.Attributes.ListItems = new List<SelectionRuleListItem>();
            SelectionRule.Attributes.ListItems.Add(new SelectionRuleListItem(1, "item 1"));
            SelectionRule.Attributes.ListItems.Add(new SelectionRuleListItem(2, "item 2"));
            SelectionRule.Attributes.ListItems.Add(new SelectionRuleListItem(3, "item 3"));
            SelectionRule.Attributes.ListItems.Add(new SelectionRuleListItem(4, "item 4"));
            SelectionRule.Attributes.ListItems.Add(new SelectionRuleListItem(5, "item 5"));
            foreach (SelectionRuleListItem listItem in SelectionRule.Attributes.ListItems)
            {
                SelectionItems.Add(listItem);
            }
            SelectedItem = SelectionRule.Attributes.ListItems[2];
            RegisteredItem = _selectedItem;
            SelectionMade = true;
            IsExpanded = true;
        }

        public void OnHandleEvent(CharacterManagerSelectionRuleDeleted args)
        {
            _ = SelectionRule.UniqueIdentifier == args.SelectionRule.UniqueIdentifier;
        }
    }
}
