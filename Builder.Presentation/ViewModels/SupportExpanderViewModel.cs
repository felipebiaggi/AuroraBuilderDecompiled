using Builder.Data.Rules;
using Builder.Presentation.Interfaces;
using Builder.Presentation.Services;
using Builder.Presentation.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

namespace Builder.Presentation.ViewModels
{
    public abstract class SupportExpanderViewModel : ViewModelBase, ISupportExpanders
    {
        private object _lock = new object();

        private ISelectionRuleExpander _selectedExpander;

        public string Name { get; }

        public IEnumerable<string> Listings { get; set; }

        public ObservableCollection<ISelectionRuleExpander> Expanders { get; set; }

        public string Selects
        {
            get
            {
                IEnumerable<string> values = Expanders.Select((ISelectionRuleExpander x) => x.SelectionRule?.ToString() + " [supports:" + x.SelectionRule.Attributes.Supports + "]");
                return string.Join("\r\n", values);
            }
        }

        public virtual bool HasExpanders => Expanders.Any();

        public ISelectionRuleExpander SelectedExpander
        {
            get
            {
                return _selectedExpander;
            }
            set
            {
                SetProperty(ref _selectedExpander, value, "SelectedExpander");
            }
        }

        protected SupportExpanderViewModel(IEnumerable<string> listings)
        {
            Listings = listings;
            Name = GetType().Name;
            Expanders = new ObservableCollection<ISelectionRuleExpander>();
            Expanders.CollectionChanged += Expanders_CollectionChanged;
            SelectionRuleExpanderHandler.Current.RegisterSupport(this);
        }

        private void Expanders_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Selects", "HasExpanders");
            OnPropertyChanged("HasExpanders");
        }

        public void AddExpander(ISelectionRuleExpander expander)
        {
            SelectRule rule = expander.SelectionRule;
            lock (_lock)
            {
                if (Expanders.Count == 0)
                {
                    Expanders.Add(expander);
                    return;
                }
                string type = (rule.Attributes.IsList ? rule.ElementHeader.Type : rule.Attributes.Type);
                if (type == "Class" || type == "Multiclass")
                {
                    Expanders.Add(expander);
                    return;
                }
                bool num = Expanders.Any((ISelectionRuleExpander x) => x.SelectionRule.Attributes.Type == "Multiclass");
                bool flag = Expanders.Select((ISelectionRuleExpander e) => e.SelectionRule.ElementHeader.Id).Contains(rule.ElementHeader.Id);
                bool flag2 = Expanders.Select((ISelectionRuleExpander e) => e.SelectionRule.Attributes.Type).Contains(type);
                List<string> list = Listings.ToList();
                if (num)
                {
                    List<ClassProgressionManager> source = CharacterManager.Current.ClassProgressionManagers.OrderBy((ClassProgressionManager x) => x.StartingLevel).ToList();
                    ClassProgressionManager manager = source.FirstOrDefault((ClassProgressionManager x) => x.SelectionRules.Contains(rule));
                    if (manager?.ClassElement == null)
                    {
                        Expanders.Add(expander);
                        return;
                    }
                    ISelectionRuleExpander selectionRuleExpander = Expanders.FirstOrDefault((ISelectionRuleExpander x) => x.SelectionRule == manager.SelectRule);
                    int num2 = Expanders.IndexOf(selectionRuleExpander);
                    if (Expanders.Last() == selectionRuleExpander)
                    {
                        Expanders.Add(expander);
                        return;
                    }
                    for (int i = num2; i < Expanders.Count; i++)
                    {
                        int num3 = i + 1;
                        if (num3 == Expanders.Count)
                        {
                            Expanders.Add(expander);
                            return;
                        }
                        string type2 = Expanders[num3].SelectionRule.Attributes.Type;
                        if (type2 == "Multiclass")
                        {
                            Expanders.Insert(num3, expander);
                            return;
                        }
                        if (list.IndexOf(rule.Attributes.Type) < list.IndexOf(type2))
                        {
                            Expanders.Insert(num3, expander);
                            return;
                        }
                    }
                    Expanders.Add(expander);
                    return;
                }
                if (flag2)
                {
                    int num4 = Expanders.IndexOf(Expanders.Last((ISelectionRuleExpander e) => e.SelectionRule.Attributes.Type == type));
                    if (flag)
                    {
                        num4 = Expanders.IndexOf(Expanders.Last((ISelectionRuleExpander e) => e.SelectionRule.ElementHeader.Id == rule.ElementHeader.Id));
                    }
                    if (rule.Attributes.IsList)
                    {
                        ISelectionRuleExpander selectionRuleExpander2 = Expanders.LastOrDefault((ISelectionRuleExpander e) => e.SelectionRule.ElementHeader.Type == type);
                        if (selectionRuleExpander2 != null)
                        {
                            num4 = Expanders.IndexOf(selectionRuleExpander2);
                        }
                    }
                    int num5 = num4 + 1;
                    if (num5 == Expanders.Count)
                    {
                        Expanders.Add(expander);
                    }
                    else
                    {
                        Expanders.Insert(num5, expander);
                    }
                    return;
                }
                int num6 = 0;
                if (flag)
                {
                    num6 = Expanders.IndexOf(Expanders.Last((ISelectionRuleExpander e) => e.SelectionRule.ElementHeader.Id == rule.ElementHeader.Id));
                }
                for (int j = num6; j < Expanders.Count; j++)
                {
                    int num7 = j + 1;
                    if (num7 == Expanders.Count)
                    {
                        Expanders.Add(expander);
                        return;
                    }
                    string type3 = Expanders[num7].SelectionRule.Attributes.Type;
                    if (list.IndexOf(rule.Attributes.Type) < list.IndexOf(type3))
                    {
                        Expanders.Insert(num7, expander);
                        return;
                    }
                }
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                Expanders.Add(expander);
            }
        }
    }
}
