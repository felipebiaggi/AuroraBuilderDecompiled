using Builder.Core;
using Builder.Data;
using Builder.Data.Rules;
using Builder.Presentation.Services.Data;
using Builder.Presentation.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml;

namespace Builder.Presentation.ViewModels.Development
{
    public class DeveloperToolsCreateViewModel : ViewModelBase
    {
        private readonly ElementBaseCollection _elements = new ElementBaseCollection();

        private GenerationElement _element;

        private string _generatedXmlNode;

        private bool _isAutoGenerateIdEnabled;

        private bool _isCustomElement;

        private string _customAbbreviation;

        private ElementBase _selectedElement;

        private string _selectedSupport;

        public List<string> ElementTypes { get; set; }

        public List<string> ElementSources { get; set; }

        public List<string> ExistingStatNames { get; set; }

        public GenerationElement Element
        {
            get
            {
                return _element;
            }
            set
            {
                SetProperty(ref _element, value, "Element");
            }
        }

        public string GeneratedXmlNode
        {
            get
            {
                return _generatedXmlNode;
            }
            set
            {
                SetProperty(ref _generatedXmlNode, value, "GeneratedXmlNode");
            }
        }

        public bool IsAutoGenerateIdEnabled
        {
            get
            {
                return _isAutoGenerateIdEnabled;
            }
            set
            {
                SetProperty(ref _isAutoGenerateIdEnabled, value, "IsAutoGenerateIdEnabled");
                if (_isAutoGenerateIdEnabled)
                {
                    _element.Id = GenerateUniqueId(_element.Name, _element.Type, IsCustomElement);
                }
            }
        }

        public bool IsCustomElement
        {
            get
            {
                return _isCustomElement;
            }
            set
            {
                SetProperty(ref _isCustomElement, value, "IsCustomElement");
                _element.Id = GenerateUniqueId(_element.Name, _element.Type, IsCustomElement);
            }
        }

        public string CustomAbbreviation
        {
            get
            {
                return _customAbbreviation;
            }
            set
            {
                SetProperty(ref _customAbbreviation, value, "CustomAbbreviation");
                if (string.IsNullOrWhiteSpace(_customAbbreviation))
                {
                    IsCustomElement = false;
                    return;
                }
                IsCustomElement = true;
                if (_isAutoGenerateIdEnabled)
                {
                    _element.Id = GenerateUniqueId(_element.Name, _element.Type, IsCustomElement);
                }
            }
        }

        public ElementBaseCollection Elements { get; } = new ElementBaseCollection();

        public ElementBase SelectedElement
        {
            get
            {
                return _selectedElement;
            }
            set
            {
                SetProperty(ref _selectedElement, value, "SelectedElement");
            }
        }

        public ICommand LoadSelectedElementCommand => new RelayCommand(LoadSelectedElement);

        public List<string> ExistingSupports { get; set; }

        public string SelectedSupport
        {
            get
            {
                return _selectedSupport;
            }
            set
            {
                SetProperty(ref _selectedSupport, value, "SelectedSupport");
            }
        }

        public ICommand AddSupportCommand => new RelayCommand(AddSupport);

        public DeveloperToolsCreateViewModel()
        {
            _isAutoGenerateIdEnabled = true;
            _isCustomElement = true;
            _customAbbreviation = "WOTC";
            if (base.IsInDesignMode)
            {
                GeneratedXmlNode = "<element name=\"design generated\"";
                return;
            }
            _elements.AddRange(DataManager.Current.ElementsCollection);
            Elements.AddRange(_elements);
            SelectedElement = Elements.First();
            _element = new GenerationElement();
            _element.PropertyChanged += ElementPropertyChanged;
            ElementTypes = new List<string>(from e in _elements
                                            group e by e.Type into g
                                            select g.First().Type);
            ElementSources = new List<string>(from e in _elements
                                              group e by e.Source into g
                                              select g.First().Source);
            List<string> collection = (from x in _elements.Select((ElementBase x) => x.Supports).SelectMany((List<string> x) => x)
                                       group x by x into x
                                       select x.First() into x
                                       where x.Length > 0
                                       orderby x
                                       select x).ToList();
            List<string> collection2 = (from x in _elements.Select((ElementBase x) => x.GetStatisticRules()).SelectMany((IEnumerable<StatisticRule> x) => x)
                                        select x.Attributes.Name into x
                                        group x by x into x
                                        select x.First() into x
                                        orderby x
                                        select x).ToList();
            ExistingSupports = new List<string>(collection);
            ExistingStatNames = new List<string>(collection2);
            _element.Name = "Weapon Proficiency (Tail)";
            _element.Type = ElementTypes.First((string x) => x.StartsWith("Proficiency"));
            _element.Source = ElementSources.First((string x) => x.StartsWith("Player"));
        }

        private void LoadSelectedElement()
        {
            if (SelectedElement != null)
            {
                _element.Name = SelectedElement.Name;
                _element.Type = SelectedElement.Type;
                _element.Source = SelectedElement.Source;
                _element.Id = SelectedElement.Id;
            }
        }

        private void AddSupport()
        {
            _element.Supports.Add(SelectedSupport);
        }

        private void GenerateNode()
        {
            ElementBase elementBase = new ElementBase(_element.Name, _element.Type, _element.Source, _element.Id);
            foreach (string support in _element.Supports)
            {
                elementBase.Supports.Add(support);
            }
            GeneratedXmlNode = elementBase.GenerateElementNode().GenerateCleanOutput();
        }

        private void ElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "Name" || e.PropertyName == "Type") && _isAutoGenerateIdEnabled)
            {
                _element.Id = GenerateUniqueId(_element.Name, _element.Type, IsCustomElement);
            }
            GenerateNode();
        }

        private string GenerateUniqueId(string elementName, string type, bool isCustom)
        {
            string[] array = new string[3] { "/", "'", "’" };
            foreach (string oldValue in array)
            {
                elementName = elementName.Replace(oldValue, "");
            }
            string text = type.Replace(" ", "").ToUpper();
            string text2 = elementName.Replace(" ", "_").ToUpper().Trim();
            return ("ID" + (isCustom ? ("_" + _customAbbreviation) : "") + "_" + text + "_" + text2).ToUpper();
        }

        private static string Beautify(XmlDocument doc)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = Environment.NewLine,
                NewLineHandling = NewLineHandling.Replace
            };
            using (XmlWriter w = XmlWriter.Create(stringBuilder, settings))
            {
                doc.Save(w);
            }
            return stringBuilder.ToString();
        }
    }
}
