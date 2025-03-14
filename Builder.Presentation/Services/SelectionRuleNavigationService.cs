using Builder.Core;
using Builder.Core.Events;
using Builder.Data.Rules;
using Builder.Presentation.Events.Character;
using Builder.Presentation.Events.Global;
using Builder.Presentation.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Builder.Presentation.Services
{
    public class SelectionRuleNavigationService : ObservableObject, ISubscriber<CharacterManagerSelectionRuleCreated>, ISubscriber<CharacterManagerSelectionRuleDeleted>, ISubscriber<CharacterManagerElementRegistered>, ISubscriber<CharacterManagerElementUnregistered>, ISubscriber<CharacterNameChangedEvent>, ISubscriber<ReprocessCharacterEvent>, ISubscriber<ListSelectionRuleRegisteredEvent>, ISubscriber<ListSelectionRuleUnregisteredEvent>
    {
        private readonly IEventAggregator _eventAggregator;

        private bool _isEnabled;

        private bool _isNextAvailable;

        private bool _isPreviousAvailable;

        private int _selectionCount;

        private int _selectionsRemaining;

        private SelectRule _firstRequiredSelectionRule;

        public ICommand NavigateNextCommand => new RelayCommand(NavigateNext);

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                SetProperty(ref _isEnabled, value, "IsEnabled");
                if (_isEnabled)
                {
                    Eval();
                }
            }
        }

        public bool IsNextAvailable
        {
            get
            {
                return _isNextAvailable;
            }
            set
            {
                SetProperty(ref _isNextAvailable, value, "IsNextAvailable");
            }
        }

        public bool IsPreviousAvailable
        {
            get
            {
                return _isPreviousAvailable;
            }
            set
            {
                SetProperty(ref _isPreviousAvailable, value, "IsPreviousAvailable");
            }
        }

        public int SelectionCount
        {
            get
            {
                return _selectionCount;
            }
            set
            {
                SetProperty(ref _selectionCount, value, "SelectionCount");
            }
        }

        public int SelectionsRemaining
        {
            get
            {
                return _selectionsRemaining;
            }
            set
            {
                SetProperty(ref _selectionsRemaining, value, "SelectionsRemaining");
            }
        }

        public SelectRule FirstRequiredSelectionRule
        {
            get
            {
                return _firstRequiredSelectionRule;
            }
            set
            {
                SetProperty(ref _firstRequiredSelectionRule, value, "FirstRequiredSelectionRule");
            }
        }

        public string DisplayNext
        {
            get
            {
                if (!IsEnabled)
                {
                    return "N/A";
                }
                return "SELECTION AVAILABLE";
            }
        }

        public string DisplayNextWithCounter
        {
            get
            {
                if (!IsEnabled)
                {
                    return "N/A";
                }
                return $"{SelectionCount - SelectionsRemaining}/{SelectionCount} SELECTION AVAILABLE";
            }
        }

        public SelectionRuleNavigationService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _isEnabled = true;
        }

        private void NavigateNext()
        {
            if (!IsEnabled)
            {
                return;
            }
            List<IGrouping<string, SelectRule>> list = (from x in CharacterManager.Current.SelectionRules.ToList()
                                                        group x by x.Attributes.Type into x
                                                        orderby x.Key
                                                        select x).ToList();
            bool flag = false;
            foreach (IGrouping<string, SelectRule> item in list.Where((IGrouping<string, SelectRule> x) => x.Key == "Race" || x.Key == "Sub Race" || x.Key == "Race Variant" || x.Key == "Racial Trait"))
            {
                foreach (SelectRule item2 in item)
                {
                    flag = TryFocusRule(item2);
                    if (flag)
                    {
                        break;
                    }
                }
                if (flag)
                {
                    break;
                }
            }
            if (!flag)
            {
                foreach (IGrouping<string, SelectRule> item3 in list.Where((IGrouping<string, SelectRule> x) => x.Key == "Class" || x.Key == "Class Feature"))
                {
                    foreach (SelectRule item4 in item3)
                    {
                        flag = TryFocusRule(item4);
                        if (flag)
                        {
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
            }
            if (!flag)
            {
                foreach (IGrouping<string, SelectRule> item5 in list.Where((IGrouping<string, SelectRule> x) => x.Key == "Archetype" || x.Key == "Archetype Feature"))
                {
                    foreach (SelectRule item6 in item5)
                    {
                        flag = TryFocusRule(item6);
                        if (flag)
                        {
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
            }
            if (!flag)
            {
                foreach (IGrouping<string, SelectRule> item7 in list.Where((IGrouping<string, SelectRule> x) => x.Key == "Multiclass"))
                {
                    foreach (SelectRule item8 in item7)
                    {
                        flag = TryFocusRule(item8);
                        if (flag)
                        {
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
            }
            if (!flag)
            {
                foreach (IGrouping<string, SelectRule> item9 in list.Where((IGrouping<string, SelectRule> x) => x.Key == "Background" || x.Key == "Background Feature" || x.Key == "Background Variant" || x.Key == "Background Characteristics"))
                {
                    foreach (SelectRule item10 in item9)
                    {
                        flag = TryFocusRule(item10);
                        if (flag)
                        {
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
            }
            if (!flag)
            {
                foreach (IGrouping<string, SelectRule> item11 in list.Where((IGrouping<string, SelectRule> x) => x.Key == "List"))
                {
                    foreach (SelectRule item12 in item11)
                    {
                        if (item12.ElementHeader.Type == "Background" || item12.ElementHeader.Type == "Background Characteristics")
                        {
                            flag = TryFocusRule(item12);
                        }
                        if (flag)
                        {
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
            }
            if (!flag)
            {
                foreach (IGrouping<string, SelectRule> item13 in list.Where((IGrouping<string, SelectRule> x) => x.Key == "Ability Score Improvement"))
                {
                    foreach (SelectRule item14 in item13)
                    {
                        flag = TryFocusRule(item14);
                        if (flag)
                        {
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
            }
            if (!flag)
            {
                foreach (IGrouping<string, SelectRule> item15 in list.Where((IGrouping<string, SelectRule> x) => x.Key == "Language"))
                {
                    foreach (SelectRule item16 in item15)
                    {
                        flag = TryFocusRule(item16);
                        if (flag)
                        {
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
            }
            if (!flag)
            {
                foreach (IGrouping<string, SelectRule> item17 in list.Where((IGrouping<string, SelectRule> x) => x.Key == "Proficiency"))
                {
                    foreach (SelectRule item18 in item17)
                    {
                        flag = TryFocusRule(item18);
                        if (flag)
                        {
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
            }
            if (!flag)
            {
                foreach (IGrouping<string, SelectRule> item19 in list.Where((IGrouping<string, SelectRule> x) => x.Key == "Feat"))
                {
                    foreach (SelectRule item20 in item19)
                    {
                        flag = TryFocusRule(item20);
                        if (flag)
                        {
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
            }
            if (!flag)
            {
                foreach (IGrouping<string, SelectRule> item21 in list.Where((IGrouping<string, SelectRule> x) => x.Key == "Companion" || x.Key == "Companion Feature" || x.Key == "Companion Trait" || x.Key == "Companion Action"))
                {
                    foreach (SelectRule item22 in item21)
                    {
                        flag = TryFocusRule(item22);
                        if (flag)
                        {
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
            }
            if (!flag)
            {
                foreach (IGrouping<string, SelectRule> item23 in list.Where((IGrouping<string, SelectRule> x) => x.Key == "Spell"))
                {
                    foreach (SelectRule item24 in item23)
                    {
                        flag = TryFocusRule(item24);
                        if (flag)
                        {
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
            }
            if (flag)
            {
                return;
            }
            foreach (IGrouping<string, SelectRule> item25 in list)
            {
                foreach (SelectRule item26 in item25)
                {
                    flag = TryFocusRule(item26);
                    if (flag)
                    {
                        break;
                    }
                }
                if (flag)
                {
                    break;
                }
            }
        }

        private bool TryFocusRule(SelectRule rule, int number = 1)
        {
            if (number > 1)
            {
                if (SelectionRuleExpanderHandler.Current.RequiresSelection(rule, number))
                {
                    SelectionRuleExpanderHandler.Current.FocusExpander(rule, number);
                    string statusMessage = "The '" + rule.Attributes.Name + "' sectionselection option.";
                    ApplicationManager.Current.SendStatusMessage(statusMessage);
                    return true;
                }
            }
            else
            {
                for (int i = 0; i < rule.Attributes.Number; i++)
                {
                    if (SelectionRuleExpanderHandler.Current.RequiresSelection(rule, i + 1))
                    {
                        SelectionRuleExpanderHandler.Current.FocusExpander(rule, i + 1);
                        string statusMessage2 = "Choose one from the '" + rule.Attributes.Name + "' selection option.";
                        ApplicationManager.Current.SendStatusMessage(statusMessage2);
                        return true;
                    }
                }
            }
            return false;
        }

        private async void Eval()
        {
            if (!IsEnabled)
            {
                return;
            }
            SelectionCount = 0;
            SelectionsRemaining = 0;
            bool flag = false;
            foreach (SelectRule selectionRule in CharacterManager.Current.SelectionRules)
            {
                SelectionCount += selectionRule.Attributes.Number;
                for (int i = 0; i < selectionRule.Attributes.Number; i++)
                {
                    if (SelectionRuleExpanderHandler.Current.RequiresSelection(selectionRule, i + 1))
                    {
                        if (!flag)
                        {
                            FirstRequiredSelectionRule = selectionRule;
                        }
                        flag = true;
                        SelectionsRemaining++;
                    }
                }
            }
            IsNextAvailable = SelectionsRemaining > 0;
            if (!flag)
            {
                FirstRequiredSelectionRule = null;
            }
            _eventAggregator.Send(new NavigationServiceEvaluationEvent(SelectionsRemaining, SelectionCount, FirstRequiredSelectionRule));
            OnPropertyChanged("DisplayNext");
            if (IsNextAvailable && Settings.Default.AutoNavigateNextSelectionWhenAvailable)
            {
                await Task.Delay(250);
                NavigateNext();
            }
        }

        public void OnHandleEvent(CharacterManagerSelectionRuleCreated args)
        {
            Eval();
        }

        public void OnHandleEvent(CharacterManagerSelectionRuleDeleted args)
        {
            Eval();
        }

        public void OnHandleEvent(CharacterManagerElementRegistered args)
        {
            Eval();
        }

        public void OnHandleEvent(CharacterManagerElementUnregistered args)
        {
            Eval();
        }

        public void OnHandleEvent(CharacterNameChangedEvent args)
        {
        }

        public void OnHandleEvent(ListSelectionRuleRegisteredEvent args)
        {
            Eval();
        }

        public void OnHandleEvent(ListSelectionRuleUnregisteredEvent args)
        {
            Eval();
        }

        public void OnHandleEvent(ReprocessCharacterEvent args)
        {
        }

        public override string ToString()
        {
            return DisplayNext;
        }
    }
}
