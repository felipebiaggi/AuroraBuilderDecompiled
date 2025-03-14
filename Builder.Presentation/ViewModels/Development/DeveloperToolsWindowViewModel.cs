using Builder.Core;
using Builder.Core.Events;
using Builder.Data;
using Builder.Presentation.Services.Data;
using Builder.Presentation.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace Builder.Presentation.ViewModels.Development
{
    public sealed class DeveloperToolsWindowViewModel : ViewModelBase, ISubscriber<ElementDescriptionDisplayRequestEvent>
    {
        private string _styleSheet;

        private string _input;

        private string _output;

        private bool _isSelectionEnabled;

        private bool _isContextMenuEnabled;

        private ElementBase _selectedElement;

        private bool _autoGenerateId;

        private ElementBase _createElement;

        private string _createElementOutput;

        private string _createElementName;

        private string _createElementType;

        private string _createElementSource;

        private string _createElementID;

        public string StyleSheet
        {
            get
            {
                return _styleSheet;
            }
            set
            {
                SetProperty(ref _styleSheet, value, "StyleSheet");
                Generate();
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
                Generate();
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

        public bool IsSelectionEnabled
        {
            get
            {
                return _isSelectionEnabled;
            }
            set
            {
                SetProperty(ref _isSelectionEnabled, value, "IsSelectionEnabled");
            }
        }

        public bool IsContextMenuEnabled
        {
            get
            {
                return _isContextMenuEnabled;
            }
            set
            {
                SetProperty(ref _isContextMenuEnabled, value, "IsContextMenuEnabled");
            }
        }

        public ElementBaseCollection Elements { get; }

        public ElementBase SelectedElement
        {
            get
            {
                return _selectedElement;
            }
            set
            {
                SetProperty(ref _selectedElement, value, "SelectedElement");
                Input = ((_selectedElement == null) ? "" : _selectedElement.Description);
            }
        }

        public ICommand GenerateDescriptionNodeCommand { get; }

        public ObservableCollection<string> ElementTypes { get; private set; }

        public ObservableCollection<string> ElementSources { get; private set; }

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

        public ElementBase CreateElement
        {
            get
            {
                return _createElement;
            }
            set
            {
                SetProperty(ref _createElement, value, "CreateElement");
            }
        }

        public string CreateElementOutput
        {
            get
            {
                return _createElementOutput;
            }
            set
            {
                SetProperty(ref _createElementOutput, value, "CreateElementOutput");
            }
        }

        public string CreateElementName
        {
            get
            {
                return _createElementName;
            }
            set
            {
                SetProperty(ref _createElementName, value, "CreateElementName");
                if (AutoGenerateId)
                {
                    CreateElementID = GenerateUniqueId(CreateElementName, CreateElementType);
                }
                GenerateElementOutput();
            }
        }

        public string CreateElementType
        {
            get
            {
                return _createElementType;
            }
            set
            {
                SetProperty(ref _createElementType, value, "CreateElementType");
                if (AutoGenerateId)
                {
                    CreateElementID = GenerateUniqueId(CreateElementName, CreateElementType);
                }
                GenerateElementOutput();
            }
        }

        public string CreateElementSource
        {
            get
            {
                return _createElementSource;
            }
            set
            {
                SetProperty(ref _createElementSource, value, "CreateElementSource");
                GenerateElementOutput();
            }
        }

        public string CreateElementID
        {
            get
            {
                return _createElementID;
            }
            set
            {
                SetProperty(ref _createElementID, value, "CreateElementID");
                GenerateElementOutput();
            }
        }

        public DeveloperToolsWindowViewModel()
        {
            _isSelectionEnabled = true;
            _isContextMenuEnabled = true;
            _autoGenerateId = true;
            _styleSheet = DataManager.Current.GetResourceWebDocument("stylesheet.css");
            if (base.IsInDesignMode)
            {
                string resourceWebDocument = DataManager.Current.GetResourceWebDocument("design-data.html");
                _output = "<style>" + _styleSheet + "</style><body><h3>DWARF</h3>" + resourceWebDocument + "</body>";
                InitializeDesignData();
                return;
            }
            Elements = DataManager.Current.ElementsCollection;
            _selectedElement = Elements.First();
            IEnumerable<string> collection = from e in DataManager.Current.ElementsCollection
                                             group e by e.Type into g
                                             select g.First().Type;
            IEnumerable<string> collection2 = from e in DataManager.Current.ElementsCollection
                                              group e by e.Source into g
                                              select g.First().Source;
            ElementTypes = new ObservableCollection<string>(collection);
            ElementSources = new ObservableCollection<string>(collection2);
            _createElementName = "element";
            _createElementType = ElementTypes.First();
            _createElementSource = ElementSources.First();
            GenerateDescriptionNodeCommand = new RelayCommand(GenerateDescriptionNode);
            base.EventAggregator.Subscribe(this);
            _createElement = new ElementBase
            {
                ElementHeader = new ElementHeader("", "", "", "")
            };
        }

        private void GenerateDescriptionNode()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.AppendChild(xmlDocument.CreateElement("description")).AppendChild(xmlDocument.CreateCDataSection(Input));
            Clipboard.SetText(xmlDocument.OuterXml);
        }

        private void Generate()
        {
            if (SelectedElement == null)
            {
                Output = "";
                return;
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<html>");
            stringBuilder.AppendLine("<body>");
            stringBuilder.AppendLine("<h2>" + SelectedElement.Name.ToUpper() + "</h2>");
            stringBuilder.AppendLine(Input);
            stringBuilder.AppendLine("<h6>SOURCE</h6>");
            stringBuilder.AppendLine("<p class=\"flavor\">" + SelectedElement.Source + "</p>");
            stringBuilder.AppendLine("</body>");
            stringBuilder.AppendLine("</html>");
            Output = stringBuilder.ToString();
        }

        public void OnHandleEvent(ElementDescriptionDisplayRequestEvent args)
        {
            SelectedElement = args.Element;
        }

        protected override void InitializeDesignData()
        {
            CreateElementName = "grewgwrewg";
            CreateElementType = "rewgre";
            CreateElementSource = "5435432";
            CreateElementID = "regreg";
            CreateElementOutput = "CreateElementOutput";
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

        private void GenerateElementOutput()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (CreateElementName.Contains(";"))
            {
                string[] array = CreateElementName.Split(';');
                foreach (string text in array)
                {
                    stringBuilder.AppendLine("<element name=\"" + text + "\" type=\"" + CreateElementType + "\" source=\"" + CreateElementSource + "\" id=\"" + GenerateUniqueId(text, CreateElementType) + "\">");
                    stringBuilder.AppendLine("\t<description>");
                    stringBuilder.AppendLine("\t\t" + Input);
                    stringBuilder.AppendLine("\t</description>");
                    if (CreateElementType != "Item")
                    {
                        stringBuilder.AppendLine("\t<rules />");
                    }
                    else
                    {
                        stringBuilder.AppendLine("\t<setters>");
                        stringBuilder.AppendLine("\t\t<set name=\"category\">Adventuring Gear</set>");
                        stringBuilder.AppendLine("\t\t<set name=\"cost\" currency=\"gp\">1</set>");
                        stringBuilder.AppendLine("\t\t<set name=\"weight\" lb=\"1\">1 lbs.</set>");
                        stringBuilder.AppendLine("\t\t<set name=\"container\"></set>");
                        stringBuilder.AppendLine("\t</setters>");
                    }
                    stringBuilder.AppendLine("</element>");
                }
            }
            else
            {
                stringBuilder.AppendLine("<element name=\"" + CreateElementName + "\" type=\"" + CreateElementType + "\" source=\"" + CreateElementSource + "\" id=\"" + CreateElementID + "\">");
                stringBuilder.AppendLine("\t<description>");
                stringBuilder.AppendLine("\t\t" + Input);
                stringBuilder.AppendLine("\t</description>");
                if (CreateElementType != "Item")
                {
                    stringBuilder.AppendLine("\t<rules />");
                }
                else
                {
                    stringBuilder.AppendLine("\t<setters>");
                    stringBuilder.AppendLine("\t\t<set name=\"category\">Adventuring Gear</set>");
                    stringBuilder.AppendLine("\t\t<set name=\"cost\" currency=\"gp\">1</set>");
                    stringBuilder.AppendLine("\t\t<set name=\"weight\" lb=\"1\">1 lbs.</set>");
                    stringBuilder.AppendLine("\t\t<set name=\"container\"></set>");
                    stringBuilder.AppendLine("\t</setters>");
                }
                stringBuilder.AppendLine("</element>");
            }
            CreateElementOutput = stringBuilder.ToString();
        }
    }
}
