using Builder.Data;
using Builder.Presentation.Services.Data;
using Builder.Presentation.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Builder.Presentation.ViewModels.Development
{
    public sealed class DeveloperWindowDataViewModel : ViewModelBase
    {
        private string _selectedType;

        private string _name;

        private ElementBase _selectedFilterElement;

        private bool _copyGrant;

        private bool _copySelect;

        private string _grant;

        private string _select;

        public ObservableCollection<string> ElementTypes { get; private set; }

        public ElementBaseCollection FilteredElements { get; set; }

        public ElementBase SelectedFilterElement
        {
            get
            {
                return _selectedFilterElement;
            }
            set
            {
                SetProperty(ref _selectedFilterElement, value, "SelectedFilterElement");
                if (_selectedFilterElement == null)
                {
                    Select = "";
                    Grant = "";
                    return;
                }
                Grant = "<grant type=\"" + _selectedFilterElement.Type + "\" name=\"" + _selectedFilterElement.Id + "\" />";
                Select = "<select type=\"" + _selectedFilterElement.Type + "\" />";
                if (_copyGrant)
                {
                    Clipboard.SetText(Grant);
                }
                if (_copySelect)
                {
                    Clipboard.SetText(Select);
                }
            }
        }

        public string SelectedType
        {
            get
            {
                return _selectedType;
            }
            set
            {
                SetProperty(ref _selectedType, value, "SelectedType");
                Filter();
            }
        }

        public string NameFilter
        {
            get
            {
                return _name;
            }
            set
            {
                SetProperty(ref _name, value, "NameFilter");
                Filter();
            }
        }

        public bool CopyGrant
        {
            get
            {
                return _copyGrant;
            }
            set
            {
                SetProperty(ref _copyGrant, value, "CopyGrant");
                _copySelect = !_copyGrant;
                OnPropertyChanged("CopySelect");
            }
        }

        public bool CopySelect
        {
            get
            {
                return _copySelect;
            }
            set
            {
                SetProperty(ref _copySelect, value, "CopySelect");
                _copyGrant = !_copySelect;
                OnPropertyChanged("CopyGrant");
            }
        }

        public string Grant
        {
            get
            {
                return _grant;
            }
            set
            {
                SetProperty(ref _grant, value, "Grant");
            }
        }

        public string Select
        {
            get
            {
                return _select;
            }
            set
            {
                SetProperty(ref _select, value, "Select");
            }
        }

        public DeveloperWindowDataViewModel()
        {
            if (!base.IsInDesignMode)
            {
                IEnumerable<string> collection = from e in DataManager.Current.ElementsCollection
                                                 group e by e.Type into g
                                                 select g.First().Type;
                ElementTypes = new ObservableCollection<string>(collection);
                FilteredElements = new ElementBaseCollection();
            }
        }

        public void Filter()
        {
            List<ElementBase> list = DataManager.Current.ElementsCollection.Where((ElementBase x) => x.Type == _selectedType).ToList();
            if (!string.IsNullOrWhiteSpace(NameFilter))
            {
                list = list.Where((ElementBase x) => x.Name.ToLower().Contains(NameFilter.ToLower())).ToList();
            }
            FilteredElements.Clear();
            FilteredElements.AddRange(list);
        }
    }
}
