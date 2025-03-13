using Builder.Core;
using Builder.Data.Elements;
using Builder.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Models.Sources
{
    public class SourceItem : ObservableObject
    {
        private bool? _isChecked;

        public Source Source { get; }

        public bool AllowUnchecking { get; set; }

        public List<ElementHeader> Elements { get; set; }

        public bool? IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                SetIsChecked(value, updateChildren: true, updateParent: true);
            }
        }

        public SourcesGroup Parent { get; private set; }

        public bool HasElements
        {
            get
            {
                if (Elements != null)
                {
                    return Elements.Any();
                }
                return false;
            }
        }

        public SourceItem(Source source)
        {
            Source = source;
            Elements = new List<ElementHeader>();
            AllowUnchecking = true;
        }

        public void SetParent(SourcesGroup parent)
        {
            Parent = parent;
            if (AllowUnchecking)
            {
                AllowUnchecking = parent.AllowUnchecking;
            }
            OnPropertyChanged("AllowUnchecking");
        }

        public void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value != _isChecked)
            {
                _isChecked = value;
                if (updateChildren)
                {
                    _ = _isChecked.HasValue;
                }
                if (updateParent)
                {
                    Parent?.VerifyCheckState();
                }
                OnPropertyChanged("IsChecked");
            }
        }

        public override string ToString()
        {
            return Source.Name ?? "";
        }
    }
}
