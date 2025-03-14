using Builder.Core;
using Builder.Presentation.ViewModels.Shell.Items;
using System.Collections.ObjectModel;
using System.Linq;

namespace Builder.Presentation.Models.Equipment
{
    public class InventoryStorage : ObservableObject
    {
        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetProperty(ref _name, value, "Name");
            }
        }

        public ObservableCollection<RefactoredEquipmentItem> StoredItems { get; set; }

        public InventoryStorage()
        {
            StoredItems = new ObservableCollection<RefactoredEquipmentItem>();
        }

        public bool IsInUse()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return StoredItems.Any();
            }
            return true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
