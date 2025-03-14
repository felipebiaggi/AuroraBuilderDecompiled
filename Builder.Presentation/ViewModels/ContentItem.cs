using Builder.Core;
using System.Collections.ObjectModel;

namespace Builder.Presentation.ViewModels
{
    public class ContentItem : ObservableObject
    {
        private string _displayName;

        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                SetProperty(ref _displayName, value, "DisplayName");
            }
        }

        public ObservableCollection<ContentItem> Items { get; } = new ObservableCollection<ContentItem>();

        public ContentItem(string displayName)
        {
            _displayName = displayName;
        }
    }
}
