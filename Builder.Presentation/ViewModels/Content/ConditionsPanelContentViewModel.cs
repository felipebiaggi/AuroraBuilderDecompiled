using Builder.Core.Events;
using Builder.Data;
using Builder.Presentation.Events.Character;
using Builder.Presentation.ViewModels.Base;
using System.Collections.Generic;
using System.Linq;

namespace Builder.Presentation.ViewModels.Content
{
    public sealed class ConditionsPanelContentViewModel : ViewModelBase, ISubscriber<CharacterManagerElementRegistered>, ISubscriber<CharacterManagerElementUnregistered>
    {
        private string _displayResistances;

        public ElementBaseCollection Resistances { get; }

        public bool HasConditions => Resistances.Any();

        public string DisplayResistances
        {
            get
            {
                return _displayResistances;
            }
            set
            {
                SetProperty(ref _displayResistances, value, "DisplayResistances");
            }
        }

        public ConditionsPanelContentViewModel()
        {
            Resistances = new ElementBaseCollection();
            if (base.IsInDesignMode)
            {
                InitializeDesignData();
            }
            else
            {
                base.EventAggregator.Subscribe(this);
            }
        }

        public void OnHandleEvent(CharacterManagerElementRegistered args)
        {
            Handle();
        }

        public void OnHandleEvent(CharacterManagerElementUnregistered args)
        {
            Handle();
        }

        private void Handle()
        {
            List<ElementBase> source = (from x in CharacterManager.Current.GetElements()
                                        where x.Type.Equals("Condition")
                                        orderby x.Name
                                        select x).ToList();
            IEnumerable<ElementBase> elements = source.Where((ElementBase x) => x.Supports.Contains("Resistance"));
            IEnumerable<ElementBase> elements2 = source.Where((ElementBase x) => x.Supports.Contains("Vulnerability"));
            IEnumerable<ElementBase> elements3 = source.Where((ElementBase x) => x.Supports.Contains("Immunity"));
            Resistances.Clear();
            Resistances.AddRange(elements);
            Resistances.AddRange(elements2);
            Resistances.AddRange(elements3);
            DisplayResistances = string.Join(", ", source.Select((ElementBase x) => x.Name));
            OnPropertyChanged("HasConditions");
        }

        protected override void InitializeDesignData()
        {
            base.InitializeDesignData();
            DisplayResistances = "Acid, Fire, Slashing";
        }
    }
}
