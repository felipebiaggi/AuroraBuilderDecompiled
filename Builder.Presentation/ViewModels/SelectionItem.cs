using Builder.Core;

namespace Builder.Presentation.ViewModels
{
    public class SelectionItem : ObservableObject
    {
        private string _displayName;

        private int _value;

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

        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                SetProperty(ref _value, value, "Value");
            }
        }

        public SelectionItem(string displayName, int value)
        {
            _displayName = displayName;
            _value = value;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
