using Builder.Core.Events;
using Builder.Data;
using Builder.Presentation.Events.Character;
using Builder.Presentation.Models;
using Builder.Presentation.ViewModels.Base;
using System.Linq;

namespace Builder.Presentation.ViewModels.Development
{
    public class DeveloperWindowCharacterDataViewModel : ViewModelBase, ISubscriber<CharacterManagerElementRegistered>
    {
        private ElementBase _selectedElement;

        public Character Character => CharacterManager.Current.Character;

        public ElementBaseCollection Elements { get; set; } = new ElementBaseCollection();

        public ElementBase SelectedElement
        {
            get
            {
                return _selectedElement;
            }
            set
            {
                SetProperty(ref _selectedElement, value, "SelectedElement");
            }
        }

        public DeveloperWindowCharacterDataViewModel()
        {
            if (!base.IsInDesignMode)
            {
                base.EventAggregator.Subscribe(this);
            }
        }

        public void OnHandleEvent(CharacterManagerElementRegistered args)
        {
            Elements.Clear();
            Elements.AddRange(CharacterManager.Current.GetElements().ToList());
        }
    }
}
