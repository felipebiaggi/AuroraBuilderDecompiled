using Builder.Core;
using Builder.Core.Events;
using Builder.Presentation.Events.Developer;
using Builder.Presentation.ViewModels.Base;
using System.Text;
using System.Windows.Input;

namespace Builder.Presentation.ViewModels.Development
{
    public class DeveloperWindowViewModel : ViewModelBase, ISubscriber<DeveloperWindowStatusUpdateEvent>
    {
        private string _statusMessage;

        private string _progressPercentage;

        private string _input;

        private string _output;

        private string _elementName;

        private string _elementType;

        private string _elementSource;

        private string _elementId;

        public string StatusMessage
        {
            get
            {
                return _statusMessage;
            }
            set
            {
                SetProperty(ref _statusMessage, value, "StatusMessage");
            }
        }

        public string ProgressPercentage
        {
            get
            {
                return _progressPercentage;
            }
            set
            {
                SetProperty(ref _progressPercentage, value, "ProgressPercentage");
            }
        }

        public string ElementName
        {
            get
            {
                return _elementName;
            }
            set
            {
                SetProperty(ref _elementName, value, "ElementName");
                ElementId = "ID_" + _elementName.ToUpper();
            }
        }

        public string ElementType
        {
            get
            {
                return _elementType;
            }
            set
            {
                SetProperty(ref _elementType, value, "ElementType");
            }
        }

        public string ElementSource
        {
            get
            {
                return _elementSource;
            }
            set
            {
                SetProperty(ref _elementSource, value, "ElementSource");
            }
        }

        public string ElementId
        {
            get
            {
                return _elementId;
            }
            set
            {
                SetProperty(ref _elementId, value, "ElementId");
            }
        }

        public string Input
        {
            get
            {
                return _input;
            }
            set
            {
                SetProperty(ref _input, value, "Input");
            }
        }

        public string Output
        {
            get
            {
                return _output;
            }
            set
            {
                SetProperty(ref _output, value, "Output");
            }
        }

        public ICommand GenerateCommand => new RelayCommand(Generate);

        public DeveloperWindowViewModel()
        {
            _statusMessage = "Developer Console";
            if (!base.IsInDesignMode)
            {
                base.EventAggregator.Subscribe(this);
            }
        }

        private void Generate()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(GenerateIndentations(0) + "<element name=\"" + ElementName + "\" type=\"" + ElementType + "\" source=\"" + ElementSource + "\" id=\"" + ElementId.ToUpper() + "\" />");
            Output = stringBuilder.ToString();
        }

        private static string GenerateIndentations(int amount = 1)
        {
            string text = "";
            for (int i = 0; i < amount; i++)
            {
                text += "\t";
            }
            return text;
        }

        public void OnHandleEvent(DeveloperWindowStatusUpdateEvent args)
        {
            StatusMessage = args.StatusMessage;
        }
    }
}
