using Builder.Data;
using Builder.Presentation.Services.Data;

namespace Builder.Presentation.ViewModels.Base
{
    public class ElementsViewModelBase : ViewModelBase
    {
        private ElementBase _selectedElement;

        public ElementBaseCollection Elements { get; set; }

        public ElementBase SelectedElement
        {
            get
            {
                return _selectedElement;
            }
            set
            {
                bool isChanged = SetProperty(ref _selectedElement, value, "SelectedElement");
                OnSelectedElementChanged(isChanged);
            }
        }

        public ElementsViewModelBase()
        {
            if (!base.IsInDesignMode)
            {
                Elements = new ElementBaseCollection(DataManager.Current.ElementsCollection);
            }
        }

        public virtual void OnSelectedElementChanged(bool isChanged)
        {
        }
    }
}
