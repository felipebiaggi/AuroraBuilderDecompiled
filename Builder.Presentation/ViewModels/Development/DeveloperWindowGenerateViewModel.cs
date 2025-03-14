using Builder.Presentation.Services.Data;
using Builder.Presentation.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Builder.Presentation.ViewModels.Development
{
    public class DeveloperWindowGenerateViewModel : ViewModelBase
    {
        private string _selectedType;

        private string _selectedSource;

        private string _name;

        private string _id;

        private bool _autoGenerateId;

        public ObservableCollection<string> ElementTypes { get; private set; }

        public ObservableCollection<string> ElementSources { get; private set; }

        public string SelectedType
        {
            get
            {
                return _selectedType;
            }
            set
            {
                SetProperty(ref _selectedType, value, "SelectedType");
                if (_autoGenerateId)
                {
                    Id = GenerateUniqueId(_name, _selectedType);
                }
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
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetProperty(ref _name, value, "Name");
                if (_autoGenerateId)
                {
                    Id = GenerateUniqueId(_name, _selectedType);
                }
            }
        }

        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                SetProperty(ref _id, value, "Id");
            }
        }

        public bool AutoGenerateId
        {
            get
            {
                return _autoGenerateId;
            }
            set
            {
                SetProperty(ref _autoGenerateId, value, "AutoGenerateId");
            }
        }

        public DeveloperWindowGenerateViewModel()
        {
            if (!base.IsInDesignMode)
            {
                InitializeElementCollections();
            }
        }

        private void InitializeElementCollections()
        {
            IEnumerable<string> collection = from e in DataManager.Current.ElementsCollection
                                             group e by e.Type into g
                                             select g.First().Type;
            IEnumerable<string> collection2 = from e in DataManager.Current.ElementsCollection
                                              group e by e.Source into g
                                              select g.First().Source;
            ElementTypes = new ObservableCollection<string>(collection);
            ElementSources = new ObservableCollection<string>(collection2);
            SelectedType = ElementTypes.First();
            SelectedSource = ElementSources.First();
            Name = string.Empty;
            Id = string.Empty;
            AutoGenerateId = true;
        }

        private string GenerateUniqueId(string elementName, string type)
        {
            string[] array = new string[3] { " ", "/", "'" };
            foreach (string oldValue in array)
            {
                elementName = elementName.Replace(oldValue, "");
            }
            return "ID_" + type.Replace(" ", "").ToUpper() + "_" + elementName.Trim().ToUpper();
        }
    }
}
