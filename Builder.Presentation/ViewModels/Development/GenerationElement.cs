using Builder.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Builder.Presentation.ViewModels.Development
{
    public class GenerationElement : ObservableObject
    {
        public class SetterItem
        {
            public string Name { get; set; }

            public string Value { get; set; }

            public KeyValuePair<string, string>[] Attributes { get; set; }

            public SetterItem(string name, string value, params KeyValuePair<string, string>[] attributes)
            {
                Name = name;
                Value = value;
                Attributes = attributes;
            }

            public string GetXmlNode()
            {
                string text = "";
                for (int i = 0; i < text.Length; i++)
                {
                    _ = text[i];
                }
                return "<set name=\"" + Name + "\">" + Value + "</set>";
            }
        }

        private string _name = "";

        private string _type = "";

        private string _id = "";

        private string _source = "";

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

        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                SetProperty(ref _type, value, "Type");
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

        public string Source
        {
            get
            {
                return _source;
            }
            set
            {
                SetProperty(ref _source, value, "Source");
            }
        }

        public ObservableCollection<string> Supports { get; } = new ObservableCollection<string>();

        public ObservableCollection<SetterItem> Setters { get; } = new ObservableCollection<SetterItem>();
    }
}
