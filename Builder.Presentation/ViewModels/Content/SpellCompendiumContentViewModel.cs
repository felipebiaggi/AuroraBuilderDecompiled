using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Builder.Core;
using Builder.Core.Events;
using Builder.Core.Logging;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Presentation.Services.Data;
using Builder.Presentation.ViewModels;
using Builder.Presentation.ViewModels.Base;
using Builder.Presentation.Views;



namespace Builder.Presentation.ViewModels.Content
{
    public class SpellCompendiumContentViewModel : ViewModelBase, ISubscriber<ElementsCollectionPopulatedEvent>, ISubscriber<QuickSearchBarEventArgs>
    {
        private Spell _selectedSpell;

        private string _filterName;

        private string _selectedLevel;

        private string _selectedSchool;

        private string _selectedClass;

        private string _selectedSource;

        private bool _supressFilter;

        public ObservableCollection<Spell> SpellElements { get; }

        public ObservableCollection<Spell> FilteredSpellElements { get; set; }

        public Spell SelectedSpell
        {
            get
            {
                return _selectedSpell;
            }
            set
            {
                SetProperty(ref _selectedSpell, value, "SelectedSpell");
                if (_selectedSpell != null)
                {
                    base.EventAggregator.Send(new SpellcastingElementDescriptionDisplayRequestEvent(_selectedSpell));
                }
            }
        }

        public ObservableCollection<string> Levels { get; set; }

        public ObservableCollection<string> Schools { get; set; }

        public ObservableCollection<string> Classes { get; set; }

        public ObservableCollection<string> Sources { get; set; }

        public string SelectedLevel
        {
            get
            {
                return _selectedLevel;
            }
            set
            {
                SetProperty(ref _selectedLevel, value, "SelectedLevel");
                Filter();
            }
        }

        public string SelectedSchool
        {
            get
            {
                return _selectedSchool;
            }
            set
            {
                SetProperty(ref _selectedSchool, value, "SelectedSchool");
                Filter();
            }
        }

        public string SelectedClass
        {
            get
            {
                return _selectedClass;
            }
            set
            {
                SetProperty(ref _selectedClass, value, "SelectedClass");
                Filter();
            }
        }

        public string FilterName
        {
            get
            {
                return _filterName;
            }
            set
            {
                SetProperty(ref _filterName, value, "FilterName");
                Filter();
            }
        }

        public string SelectedSource
        {
            get
            {
                return _selectedSource;
            }
            set
            {
                SetProperty(ref _selectedSource, value, "SelectedSource");
                Filter();
            }
        }

        public bool SupressFilter
        {
            get
            {
                return _supressFilter;
            }
            set
            {
                SetProperty(ref _supressFilter, value, "SupressFilter");
            }
        }

        public ICommand FilterCommand => new RelayCommand(Filter);

        public ICommand ResetCommand => new RelayCommand(Reset);

        public SpellCompendiumContentViewModel()
        {
            if (!base.IsInDesignMode)
            {
                SpellElements = new ObservableCollection<Spell>();
                FilteredSpellElements = new ObservableCollection<Spell>();
                Levels = new ObservableCollection<string>();
                Schools = new ObservableCollection<string>();
                Classes = new ObservableCollection<string>();
                Sources = new ObservableCollection<string>();
                base.EventAggregator.Subscribe(this);
                Populate();
            }
        }

        private void Reset()
        {
            SupressFilter = true;
            SelectedLevel = "--";
            SelectedSchool = "--";
            SelectedClass = "--";
            SelectedSource = "--";
            SupressFilter = false;
            FilterName = "";
        }

        private void Filter()
        {
            if (SupressFilter)
            {
                return;
            }
            FilteredSpellElements.Clear();
            foreach (Spell spellElement in SpellElements)
            {
                FilteredSpellElements.Add(spellElement);
            }
            if (!string.IsNullOrWhiteSpace(SelectedLevel) && SelectedLevel != "--")
            {
                List<Spell> list = FilteredSpellElements.Where((Spell x) => x.Level.ToString() == SelectedLevel).ToList();
                FilteredSpellElements.Clear();
                foreach (Spell item in list)
                {
                    FilteredSpellElements.Add(item);
                }
            }
            if (!string.IsNullOrWhiteSpace(SelectedSchool) && SelectedSchool != "--")
            {
                List<Spell> list2 = FilteredSpellElements.Where((Spell x) => x.MagicSchool == SelectedSchool).ToList();
                FilteredSpellElements.Clear();
                foreach (Spell item2 in list2)
                {
                    FilteredSpellElements.Add(item2);
                }
            }
            if (!string.IsNullOrWhiteSpace(SelectedClass) && SelectedClass != "--")
            {
                List<Spell> list3 = FilteredSpellElements.Where((Spell x) => x.Supports.Contains(SelectedClass)).ToList();
                FilteredSpellElements.Clear();
                foreach (Spell item3 in list3)
                {
                    FilteredSpellElements.Add(item3);
                }
            }
            if (!string.IsNullOrWhiteSpace(SelectedSource) && SelectedSource != "--")
            {
                List<Spell> list4 = FilteredSpellElements.Where((Spell x) => x.Source.Equals(SelectedSource)).ToList();
                FilteredSpellElements.Clear();
                foreach (Spell item4 in list4)
                {
                    FilteredSpellElements.Add(item4);
                }
            }
            if (string.IsNullOrWhiteSpace(FilterName))
            {
                return;
            }
            List<Spell> list5 = FilteredSpellElements.Where((Spell x) => x.Name.ToLower().Contains(FilterName.ToLower().Trim())).ToList();
            FilteredSpellElements.Clear();
            foreach (Spell item5 in list5)
            {
                FilteredSpellElements.Add(item5);
            }
        }

        private void Populate()
        {
            foreach (Spell item in (from x in DataManager.Current.ElementsCollection
                                    where x.Type == "Spell"
                                    orderby x.Name
                                    select x).Cast<Spell>())
            {
                SpellElements.Add(item);
            }
            Levels.Add("--");
            for (int i = 0; i < 10; i++)
            {
                Levels.Add(i.ToString());
            }
            Schools.Add("--");
            foreach (string item2 in from x in SpellElements
                                     group x by x.MagicSchool into x
                                     select x.Key into x
                                     orderby x
                                     select x)
            {
                Schools.Add(item2);
            }
            Classes.Add("--");
            foreach (string item3 in from x in DataManager.Current.ElementsCollection
                                     where x.Type == "Class"
                                     select x.Name into x
                                     orderby x
                                     select x)
            {
                Classes.Add(item3);
            }
            string[] array = new string[8] { "Wizard", "Warlock", "Bard", "Ranger", "Monk", "Paladin", "Sorcerer", "Druid" };
            foreach (string text in array)
            {
                if (!Classes.Contains(text))
                {
                    Classes.Add(text);
                    Logger.Debug("adding " + text + " to spell filter list");
                }
            }
            Sources.Add("--");
            foreach (string item4 in from x in SpellElements
                                     group x by x.Source into x
                                     select x.Key into x
                                     orderby x
                                     select x)
            {
                Sources.Add(item4);
            }
            Reset();
        }

        public void OnHandleEvent(ElementsCollectionPopulatedEvent args)
        {
            Populate();
        }

        public void OnHandleEvent(QuickSearchBarEventArgs args)
        {
            if (args.IsSearch)
            {
                SupressFilter = true;
                Reset();
                FilterName = args.SearchCriteria;
                SupressFilter = false;
                Filter();
                SelectedSpell = FilteredSpellElements.FirstOrDefault((Spell x) => x.Name.ToLower().Equals(args.SearchCriteria)) ?? FilteredSpellElements.FirstOrDefault();
            }
        }
    }
}
