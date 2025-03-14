using Builder.Core;

namespace Builder.Presentation.Models.Helpers
{
    public class Progress : ObservableObject
    {
        private string _message;

        private int _percentage;

        private bool _inProgress;

        private bool _completed;

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                SetProperty(ref _message, value, "Message");
            }
        }

        public int Percentage
        {
            get
            {
                return _percentage;
            }
            set
            {
                SetProperty(ref _percentage, value, "Percentage");
            }
        }

        public bool InProgress
        {
            get
            {
                return _inProgress;
            }
            set
            {
                SetProperty(ref _inProgress, value, "InProgress");
            }
        }

        public bool Completed
        {
            get
            {
                return _completed;
            }
            set
            {
                SetProperty(ref _completed, value, "Completed");
            }
        }
    }
}
