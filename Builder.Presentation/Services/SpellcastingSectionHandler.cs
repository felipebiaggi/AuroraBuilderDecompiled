using System.Collections.ObjectModel;
using System.Linq;
using Builder.Core.Logging;
using Builder.Data.Elements;
using Builder.Data.Extensions;
using Builder.Presentation.Extensions;
using Builder.Presentation.Services;
// using Builder.Presentation.UserControls.Spellcasting;
using Builder.Presentation.ViewModels;

namespace Builder.Presentation.Services
{
    public class SpellcastingSectionHandler
    {
        private static SpellcastingSectionHandler _instance;

        //private readonly ObservableCollection<SpellcasterSelectionControl> _spellcastingSections;

        public static SpellcastingSectionHandler Current => _instance ?? (_instance = new SpellcastingSectionHandler());

        private SpellcastingSectionHandler()
        {
            Logger.Initializing("SpellcastingSectionHandler");
            //_spellcastingSections = new ObservableCollection<SpellcasterSelectionControl>();
        }

        //public void Add(SpellcasterSelectionControl section)
        //{
        //    _spellcastingSections.Add(section);
        //}

        //public bool Remove(string identifier)
        //{
            //SpellcasterSelectionControl spellcasterSection = GetSpellcasterSection(identifier);
            //return _spellcastingSections.Remove(spellcasterSection);
        //}

        //public SpellcasterSelectionControl GetSpellcasterSection(string identifier)
        //{
        //    return _spellcastingSections.FirstOrDefault((SpellcasterSelectionControl x) => x.GetViewModel<SpellcasterSelectionControlViewModel>().Information.UniqueIdentifier.Equals(identifier));
        //}

        //public SpellcasterSelectionControlViewModel GetSpellcasterSectionViewModel(string identifier)
        //{
        //    return GetSpellcasterSection(identifier)?.GetViewModel<SpellcasterSelectionControlViewModel>();
        //}

        public bool SetPrepareSpell(SpellcastingInformation information, string elementId)
        {
            //SpellcasterSelectionControlViewModel spellcasterSelectionControlViewModel = GetSpellcasterSection(information.UniqueIdentifier)?.GetViewModel<SpellcasterSelectionControlViewModel>();
            //SelectionElement selectionElement = spellcasterSelectionControlViewModel?.KnownSpells.FirstOrDefault((SelectionElement x) => x.Element.Id.Equals(elementId));
            //if (selectionElement == null || selectionElement.IsChosen)
            //{
            //    return false;
            //}
            //spellcasterSelectionControlViewModel.SelectedKnownSpell = selectionElement;
            //spellcasterSelectionControlViewModel.TogglePrepareSpellCommand.Execute(null);
            return false;
        }

        //public IOrderedEnumerable<SelectionElement> GetSpells(SpellcastingInformation information)
        //{
        //    return (from x in (GetSpellcasterSection(information.UniqueIdentifier)?.GetViewModel<SpellcasterSelectionControlViewModel>())?.KnownSpells
        //            orderby x.IsChosen descending, x.Element.AsElement<Spell>().Level, x.Element.Name
        //            select x);
        //}
    }

}
