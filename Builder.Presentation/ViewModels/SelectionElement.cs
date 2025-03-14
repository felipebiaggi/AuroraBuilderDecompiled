using Builder.Core;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Data.Extensions;

namespace Builder.Presentation.ViewModels
{
    public class SelectionElement : ObservableObject
    {
        private string _shortDescription = string.Empty;

        private bool _isEnabled;

        private bool _isRecommended;

        private bool _isDefault;

        private bool _isHighlighted;

        private bool _isChosen;

        public ElementBase Element { get; }

        public string DisplayName => Element.Name;

        public string DisplayShortDescription
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_shortDescription) && Element.ElementSetters.ContainsSetter("short"))
                {
                    return Element.ElementSetters.GetSetter("short").Value;
                }
                switch (Element.Type)
                {
                    case "Deity":
                        return Element.AsElement<Deity>().Domains;
                    case "Spell":
                        return Element.AsElement<Spell>().GetShortDescription();
                    default:
                        return _shortDescription;
                }
            }
            set
            {
                _shortDescription = value;
            }
        }

        public string DisplayPrerequisites => Element.Prerequisite;

        public string DisplaySource
        {
            get
            {
                if (Element.Source.StartsWith("Unearthed Arcana: "))
                {
                    return Element.Source.Replace("Unearthed Arcana: ", "UA: ");
                }
                if (Element.Source.StartsWith("Adventurers League: "))
                {
                    return Element.Source.Replace("Adventurers League: ", "AL: ");
                }
                return Element.Source;
            }
        }

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

        public bool IsRecommended
        {
            get
            {
                return _isRecommended;
            }
            set
            {
                SetProperty(ref _isRecommended, value, "IsRecommended");
            }
        }

        public bool IsDefault
        {
            get
            {
                return _isDefault;
            }
            set
            {
                SetProperty(ref _isDefault, value, "IsDefault");
            }
        }

        public bool IsHighlighted
        {
            get
            {
                return _isHighlighted;
            }
            set
            {
                SetProperty(ref _isHighlighted, value, "IsHighlighted");
            }
        }

        public bool IsChosen
        {
            get
            {
                return _isChosen;
            }
            set
            {
                SetProperty(ref _isChosen, value, "IsChosen");
            }
        }

        public SelectionElement(ElementBase element, bool isEnabled = true)
        {
            Element = element;
            IsEnabled = isEnabled;
        }

        public override string ToString()
        {
            return Element.Name + (IsEnabled ? "" : " (disabled)");
        }
    }
}
