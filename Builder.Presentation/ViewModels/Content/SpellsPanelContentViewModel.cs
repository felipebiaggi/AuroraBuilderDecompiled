using Builder.Core.Events;
using Builder.Data.Elements;
using Builder.Presentation.Events.Character;
using Builder.Presentation.ViewModels.Base;
using System.Collections.Generic;
using System.Linq;

namespace Builder.Presentation.ViewModels.Content
{
    public sealed class SpellsPanelContentViewModel : ViewModelBase, ISubscriber<CharacterManagerElementRegistered>, ISubscriber<CharacterManagerElementUnregistered>
    {
        private string _displayCantrips;

        private string _displaySpells;

        public List<Spell> Spells { get; } = new List<Spell>();

        public string DisplayCantrips
        {
            get
            {
                return _displayCantrips;
            }
            set
            {
                SetProperty(ref _displayCantrips, value, "DisplayCantrips");
            }
        }

        public string DisplaySpells
        {
            get
            {
                return _displaySpells;
            }
            set
            {
                SetProperty(ref _displaySpells, value, "DisplaySpells");
            }
        }

        public SpellsPanelContentViewModel()
        {
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
            IEnumerable<string> values = from Spell x in from x in CharacterManager.Current.GetElements()
                                                         where x.Type.Equals("Spell")
                                                         select x
                                         where x.Level == 0
                                         orderby x.Name
                                         select x.Name;
            DisplayCantrips = string.Join(", ", values);
            IEnumerable<string> values2 = from Spell x in from x in CharacterManager.Current.GetElements()
                                                          where x.Type.Equals("Spell")
                                                          select x
                                          where x.Level > 0
                                          orderby x.Level, x.Name
                                          select x.Name;
            DisplaySpells = string.Join(", ", values2);
        }

        protected override void InitializeDesignData()
        {
            base.InitializeDesignData();
            DisplayCantrips = "Eldritch Blast, Shocking Grasp, Firebolt";
            DisplaySpells = "Fireball, Lightning Bolt, Fly, Teleport";
        }
    }
}
