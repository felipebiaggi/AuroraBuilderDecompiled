using Builder.Core;

namespace Builder.Presentation.ViewModels.Shell.Manage
{
    public class ObservableSpell : ObservableObject
    {
        private string _name;

        private bool _isPrepared;

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

        public bool IsPrepared
        {
            get
            {
                return _isPrepared;
            }
            set
            {
                SetProperty(ref _isPrepared, value, "IsPrepared");
            }
        }

        public void Reset()
        {
            Name = "";
            IsPrepared = false;
        }
    }
}
