using Builder.Core;
using Builder.Data;
using Builder.Presentation.Models.NewFolder1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation
{
    public class ElementContainer : ObservableObject
    {
        private bool _isEnabled;

        private bool _isNested;

        public ElementBase Element { get; }

        public FillableField Name { get; set; } = new FillableField();

        public FillableField Description { get; set; } = new FillableField();

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                SetProperty(ref _isEnabled, value, "IsEnabled");
            }
        }

        public bool IsNested
        {
            get
            {
                return _isNested;
            }
            set
            {
                SetProperty(ref _isNested, value, "IsNested");
            }
        }

        public ElementContainer(ElementBase element)
        {
            if (element != null)
            {
                Element = element;
                Name.OriginalContent = Element.Name;
                if (Element.SheetDescription.HasAlternateName)
                {
                    Name.OriginalContent = Element.SheetDescription.AlternateName;
                }
                Description.OriginalContent = element.SheetDescription.FirstOrDefault()?.Description ?? "n/a";
                IsEnabled = element.SheetDescription.DisplayOnSheet;
            }
        }

        public override string ToString()
        {
            return Name.Content;
        }
    }
}
