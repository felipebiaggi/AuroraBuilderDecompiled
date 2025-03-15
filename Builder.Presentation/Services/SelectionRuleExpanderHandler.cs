using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Builder.Core.Events;
using Builder.Core.Logging;
using Builder.Data.Rules;
using Builder.Presentation;
using Builder.Presentation.Events.Global;
using Builder.Presentation.Extensions;
using Builder.Presentation.Interfaces;
using Builder.Presentation.Services;
//using Builder.Presentation.UserControls;
using Builder.Presentation.ViewModels;

namespace Builder.Presentation.Services
{
    public class SelectionRuleExpanderHandler : ISubscriber<CharacterManagerSelectionRuleCreated>, ISubscriber<CharacterManagerSelectionRuleDeleted>
    {
        private object _expanderLock = new object();

        private readonly List<ISupportExpanders> _supports;

        private readonly ObservableCollection<ISelectionRuleExpander> _expanders;

        public static SelectionRuleExpanderHandler Current { get; } = new SelectionRuleExpanderHandler();

        private SelectionRuleExpanderHandler()
        {
            Logger.Initializing("SelectionRuleExpanderHandler");
            _supports = new List<ISupportExpanders>();
            _expanders = new ObservableCollection<ISelectionRuleExpander>();
            ApplicationManager.Current.EventAggregator.Subscribe(this);
        }

        public void RegisterSupport(ISupportExpanders support)
        {
            if (_supports.Contains(support))
            {
                throw new ArgumentException("already registered " + support.Name);
            }
            if (support.Expanders == null)
            {
                support.Expanders = new ObservableCollection<ISelectionRuleExpander>();
            }
            foreach (ISelectionRuleExpander item in _expanders.Where((ISelectionRuleExpander expander) => support.Listings.Contains(expander.SelectionRule.Attributes.Type)))
            {
                support.Expanders.Add(item);
            }
            _supports.Add(support);
            Logger.Debug("registered {0} with handler", support.Name);
        }

        [Obsolete]
        public void UnregisterSupport(ISupportExpanders support)
        {
            if (_supports.Contains(support))
            {
                _supports.Remove(support);
            }
            Logger.Debug("unregistered {0} with handler", support.Name);
        }

        public bool HasExpander(string uniqueIdentifier)
        {
            return _expanders.Any((ISelectionRuleExpander x) => x.SelectionRule.UniqueIdentifier == uniqueIdentifier);
        }

        public bool HasExpander(string uniqueIdentifier, int number)
        {
            return _expanders.Any((ISelectionRuleExpander x) => x.SelectionRule.UniqueIdentifier == uniqueIdentifier && x.Number == number);
        }

        public void OnHandleEvent(CharacterManagerSelectionRuleCreated args)
        {
            for (int i = 1; i <= args.SelectionRule.Attributes.Number; i++)
            {
                ISelectionRuleExpander selectionRuleExpander;
                if (args.SelectionRule.Attributes.IsList)
                {
                    // selectionRuleExpander = new ListItemsSelectionRuleExpander(args.SelectionRule, i);
                    foreach (ISupportExpanders item in _supports.Where((ISupportExpanders s) => s.Listings.Contains(args.SelectionRule.ElementHeader.Type)))
                    {
                        //item.AddExpander(selectionRuleExpander);
                        //Logger.Debug("expander '{0}' assigned to support '{1}'", selectionRuleExpander.SelectionRule, item.Name);
                    }
                }
                else
                {
                    switch (args.SelectionRule.Attributes.Type)
                    {
                        case "Deity":
                        case "Alignment":
                        case "Ability Score Improvement":
                            //selectionRuleExpander = new SelectionRuleComboBox(args.SelectionRule, i);
                            break;
                        default:
                            //selectionRuleExpander = new SelectionRuleExpander(args.SelectionRule, i);
                            break;
                    }
                    foreach (ISupportExpanders item2 in _supports.Where((ISupportExpanders s) => s.Listings.Contains(args.SelectionRule.Attributes.Type)))
                    {
                        //item2.AddExpander(selectionRuleExpander);
                        //Logger.Info("expander '{0}' assigned to support '{1}'", selectionRuleExpander.SelectionRule, item2.Name);
                    }
                }
                //_expanders.Add(selectionRuleExpander);
            }
        }

        public void OnHandleEvent(CharacterManagerSelectionRuleDeleted args)
        {
            for (int i = 1; i <= args.SelectionRule.Attributes.Number; i++)
            {
                ISelectionRuleExpander selectionRuleExpander = _expanders.FirstOrDefault((ISelectionRuleExpander ex) => ex.UniqueIdentifier == args.SelectionRule.UniqueIdentifier);
                if (selectionRuleExpander == null)
                {
                    Logger.Warning($"expander you want to delete doesn't exist ({args.SelectionRule}) | parent:: {args.SelectionRule.ElementHeader.Id}");
                    break;
                }
                if (args.SelectionRule.Attributes.IsList)
                {
                    foreach (ISupportExpanders item in _supports.Where((ISupportExpanders s) => s.Listings.Contains(args.SelectionRule.ElementHeader.Type)))
                    {
                        item.Expanders.Remove(selectionRuleExpander);
                        Logger.Info("list expander '{0}' removed from support '{1}'", selectionRuleExpander.SelectionRule.ElementHeader.Name, item.Name);
                    }
                }
                else
                {
                    foreach (ISupportExpanders item2 in _supports.Where((ISupportExpanders s) => s.Listings.Contains(args.SelectionRule.Attributes.Type)))
                    {
                        if (item2.Expanders.Remove(selectionRuleExpander))
                        {
                            Logger.Info("selection rule expander '{0}' removed from support '{1}' ", selectionRuleExpander.SelectionRule.Attributes.Name, item2.Name);
                        }
                        else
                        {
                            Logger.Warning("!selection rule expander '{0}' WAS NOT removed from support '{1}' ", selectionRuleExpander.SelectionRule.Attributes.Name, item2.Name);
                        }
                    }
                }
                _expanders.Remove(selectionRuleExpander);
                Logger.Info("expander '{0}' removed from handler", selectionRuleExpander.SelectionRule.ElementHeader.Name);
                //if (selectionRuleExpander is SelectionRuleExpander)
                //{
                //    (selectionRuleExpander as SelectionRuleExpander).GetViewModel<SelectionRuleExpanderViewModel>().IsEnabled = false;
                //}
            }
        }

        public bool HasRegisteredElement(SelectRule selectionRule)
        {
            if (!HasExpander(selectionRule.UniqueIdentifier))
            {
                return false;
            }
            ISelectionRuleExpander selectionRuleExpander = _expanders.Single((ISelectionRuleExpander x) => x.SelectionRule.UniqueIdentifier == selectionRule.UniqueIdentifier);
            if (selectionRuleExpander.SelectionRule.Attributes.IsList)
            {
                //return (selectionRuleExpander as ListItemsSelectionRuleExpander).GetViewModel<ListItemSelectionRuleExpanderViewModel>().SelectionMade;
            }
            //if (selectionRuleExpander is SelectionRuleComboBox)
            //{
            //    return (selectionRuleExpander as SelectionRuleComboBox).GetViewModel<SelectionRuleComboBoxViewModel>().ElementRegistered;
            //}
            //return (selectionRuleExpander as SelectionRuleExpander).GetViewModel<SelectionRuleExpanderViewModel>().ElementRegistered;
            return false;
        }

        public int GetExpanderCount(SelectRule selectionRule)
        {
            return _expanders.Count((ISelectionRuleExpander x) => x.SelectionRule.UniqueIdentifier == selectionRule.UniqueIdentifier);
        }

        public object GetRegisteredElement(SelectRule selectionRule, int number = 1)
        {
            //ISelectionRuleExpander selectionRuleExpander = _expanders.Single((ISelectionRuleExpander x) => x.SelectionRule.UniqueIdentifier == selectionRule.UniqueIdentifier && x.Number == number);
            //if (selectionRule.Attributes.IsList)
            //{
            //    return (selectionRuleExpander as ListItemsSelectionRuleExpander).GetViewModel<ListItemSelectionRuleExpanderViewModel>().RegisteredItem;
            //}
            //if (!(selectionRuleExpander is SelectionRuleExpander))
            //{
            //    if (selectionRuleExpander is SelectionRuleComboBox)
            //    {
            //        return (selectionRuleExpander as SelectionRuleComboBox).GetViewModel<SelectionRuleComboBoxViewModel>().RegisteredElement;
            //    }
            //    throw new ArgumentException($"GetRegisteredElement unknown expander: {selectionRuleExpander}");
            //}
            //return (selectionRuleExpander as SelectionRuleExpander).GetViewModel<SelectionRuleExpanderViewModel>().RegisteredElement;
            return false;
        }

        private IEnumerable<ISelectionRuleExpander> GetExpanders(string uniqueIdentifier)
        {
            return _expanders.Where((ISelectionRuleExpander x) => x.SelectionRule.UniqueIdentifier == uniqueIdentifier);
        }

        public void SetRegisteredElement(SelectRule selectionRule, string id, int number = 1)
        {
            _expanders.Single((ISelectionRuleExpander x) => x.SelectionRule.UniqueIdentifier == selectionRule.UniqueIdentifier && x.Number == number).SetSelection(id);
        }

        [Obsolete]
        private void SetExpander(SelectRule rule, string id)
        {
            ISelectionRuleExpander selectionRuleExpander = _expanders.Single((ISelectionRuleExpander e) => e.UniqueIdentifier == rule.UniqueIdentifier);
            if (selectionRuleExpander == null)
            {
                Logger.Warning("unable to set the expander for the selection rule: {0} ", rule.Attributes.Name);
                throw new ArgumentException("unable to set the expander for the selection rule", rule.Attributes.Name);
            }
            selectionRuleExpander.SetSelection(id);
        }

        public int GetExpandersCount()
        {
            return _expanders.Count;
        }

        public void FocusExpander(SelectRule rule, int number = 1)
        {
            ISelectionRuleExpander selectionRuleExpander = _expanders.FirstOrDefault((ISelectionRuleExpander x) => x.SelectionRule.UniqueIdentifier.Equals(rule.UniqueIdentifier) && x.Number == number);
            if (selectionRuleExpander != null)
            {
                NavigationLocation location = NavigationLocation.None;
                switch (rule.Attributes.IsList ? rule.ElementHeader.Type : rule.Attributes.Type)
                {
                    case "Race":
                    case "Sub Race":
                    case "Racial Trait":
                    case "Race Variant":
                        location = NavigationLocation.BuildRace;
                        break;
                    case "Class":
                    case "Class Feature":
                    case "Archetype":
                    case "Archetype Feature":
                    case "Multiclass":
                        location = NavigationLocation.BuildClass;
                        break;
                    case "Background":
                    case "Background Feature":
                    case "Background Characteristics":
                    case "Background Variant":
                        location = NavigationLocation.BuildBackground;
                        break;
                    case "Ability Score Improvement":
                        location = NavigationLocation.BuildAbilities;
                        break;
                    case "Language":
                        location = NavigationLocation.BuildLanguages;
                        break;
                    case "Proficiency":
                        location = NavigationLocation.BuildProficiencies;
                        break;
                    case "Feat":
                    case "Feat Feature":
                        location = NavigationLocation.BuildFeats;
                        break;
                    case "Spell":
                        location = NavigationLocation.MagicSpells;
                        break;
                    case "Alignment":
                    case "Deity":
                        location = NavigationLocation.ManageCharacter;
                        break;
                    case "Companion":
                    case "Companion Feature":
                        location = NavigationLocation.BuildCompanion;
                        break;
                }
                ApplicationManager.Current.EventAggregator.Send(new SelectionRuleNavigationArgs(location));
                selectionRuleExpander.FocusExpander();
                Logger.Info($"FocusExpander: {selectionRuleExpander}");
            }
        }

        public void RetrainSpellExpander(SelectRule rule, int number, int retrainLevel)
        {
            ISelectionRuleExpander selectionRuleExpander = _expanders.FirstOrDefault((ISelectionRuleExpander x) => x.SelectionRule.UniqueIdentifier.Equals(rule.UniqueIdentifier) && x.Number == number);
            if (selectionRuleExpander != null)
            {
                //SelectionRuleExpanderViewModel viewModel = (selectionRuleExpander as SelectionRuleExpander).GetViewModel<SelectionRuleExpanderViewModel>();
                //viewModel.RetrainLevel = retrainLevel;
                //viewModel.Repopulate();
            }
        }

        public void RemoveAllExpanders()
        {
            foreach (ISupportExpanders support in _supports)
            {
                if (support.Expanders.Count > 0)
                {
                    Logger.Warning($"removing left over expanders from {support}");
                }
                support.Expanders.Clear();
            }
            if (_expanders.Count > 0)
            {
                Logger.Warning("removing left over expanders from handler");
            }
            _expanders.Clear();
        }

        public bool RequiresSelection(SelectRule rule, int number = 1)
        {
            if (rule.Attributes.Optional)
            {
                return false;
            }
            foreach (ISelectionRuleExpander expander in GetExpanders(rule.UniqueIdentifier))
            {
                if (expander.Number == number && !expander.IsSelectionMade())
                {
                    return true;
                }
            }
            return false;
        }

        public int GetRetrainLevel(SelectRule rule, int number)
        {
            try
            {
                ISelectionRuleExpander selectionRuleExpander = _expanders.FirstOrDefault((ISelectionRuleExpander x) => x.SelectionRule.UniqueIdentifier.Equals(rule.UniqueIdentifier) && x.Number == number);
                if (selectionRuleExpander == null)
                {
                    return 0;
                }
                //if (selectionRuleExpander is SelectionRuleExpander)
                //{
                //    return (selectionRuleExpander as SelectionRuleExpander).GetViewModel<SelectionRuleExpanderViewModel>().RetrainLevel;
                //}
                //if (selectionRuleExpander is ListItemsSelectionRuleExpander)
                //{
                //    return 0;
                //}
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "GetRetrainLevel");
            }
            return 0;
        }
    }
}
