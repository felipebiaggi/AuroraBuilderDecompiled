using Builder.Data;
using Builder.Data.Elements;
using Builder.Presentation.Services.Data;
using Builder.Presentation.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Builder.Presentation.ViewModels.Shell.Manage
{
    public class SpellContentViewModel : ViewModelBase
    {
        private List<Spell> _spells;

        private string _spellcastingClass;

        private string _spellcastingAbility;

        private string _spellcastingAttackModifier;

        private string _spellcastingDifficultyClass;

        public string SpellcastingClass
        {
            get
            {
                return _spellcastingClass;
            }
            set
            {
                SetProperty(ref _spellcastingClass, value, "SpellcastingClass");
            }
        }

        public string SpellcastingAbility
        {
            get
            {
                return _spellcastingAbility;
            }
            set
            {
                SetProperty(ref _spellcastingAbility, value, "SpellcastingAbility");
            }
        }

        public string SpellcastingAttackModifier
        {
            get
            {
                return _spellcastingAttackModifier;
            }
            set
            {
                SetProperty(ref _spellcastingAttackModifier, value, "SpellcastingAttackModifier");
            }
        }

        public string SpellcastingDifficultyClass
        {
            get
            {
                return _spellcastingDifficultyClass;
            }
            set
            {
                SetProperty(ref _spellcastingDifficultyClass, value, "SpellcastingDifficultyClass");
            }
        }

        public SpellcastingCollection SpellcastingCollection => CharacterManager.Current.Character.SpellcastingCollection;

        public ObservableCollection<string> Cantrips { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> Spells1 { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> Spells2 { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> Spells3 { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> Spells4 { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> Spells5 { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> Spells6 { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> Spells7 { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> Spells8 { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> Spells9 { get; } = new ObservableCollection<string>();

        public SpellContentViewModel()
        {
            if (DataManager.Current.IsElementsCollectionPopulated)
            {
                Populate();
            }
        }

        private void SpellcastingCollection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SpellcastingClass")
            {
                Populate();
            }
        }

        private void Populate()
        {
            _spells = DataManager.Current.ElementsCollection.Where((ElementBase x) => x.Type == "Spell").Cast<Spell>().ToList();
            if (!string.IsNullOrWhiteSpace(SpellcastingCollection.SpellcastingClass))
            {
                _spells = _spells.Where((Spell x) => x.Supports.Contains(SpellcastingCollection.SpellcastingClass)).ToList();
            }
            Cantrips.Clear();
            Spells1.Clear();
            Spells2.Clear();
            Spells3.Clear();
            Spells4.Clear();
            Spells5.Clear();
            Spells6.Clear();
            Spells7.Clear();
            Spells8.Clear();
            Spells9.Clear();
            Cantrips.Add(string.Empty);
            Spells1.Add(string.Empty);
            Spells2.Add(string.Empty);
            Spells3.Add(string.Empty);
            Spells4.Add(string.Empty);
            Spells5.Add(string.Empty);
            Spells6.Add(string.Empty);
            Spells7.Add(string.Empty);
            Spells8.Add(string.Empty);
            Spells9.Add(string.Empty);
            foreach (Spell item in _spells.Where((Spell x) => x.Level == 0))
            {
                Cantrips.Add(item.Name);
            }
            foreach (Spell item2 in _spells.Where((Spell x) => x.Level == 1))
            {
                Spells1.Add(item2.Name);
            }
            foreach (Spell item3 in _spells.Where((Spell x) => x.Level == 2))
            {
                Spells2.Add(item3.Name);
            }
            foreach (Spell item4 in _spells.Where((Spell x) => x.Level == 3))
            {
                Spells3.Add(item4.Name);
            }
            foreach (Spell item5 in _spells.Where((Spell x) => x.Level == 4))
            {
                Spells4.Add(item5.Name);
            }
            foreach (Spell item6 in _spells.Where((Spell x) => x.Level == 5))
            {
                Spells5.Add(item6.Name);
            }
            foreach (Spell item7 in _spells.Where((Spell x) => x.Level == 6))
            {
                Spells6.Add(item7.Name);
            }
            foreach (Spell item8 in _spells.Where((Spell x) => x.Level == 7))
            {
                Spells7.Add(item8.Name);
            }
            foreach (Spell item9 in _spells.Where((Spell x) => x.Level == 8))
            {
                Spells8.Add(item9.Name);
            }
            foreach (Spell item10 in _spells.Where((Spell x) => x.Level == 9))
            {
                Spells9.Add(item10.Name);
            }
        }
    }
}
