using Builder.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Models.Sources
{
    public class SourcesGroup : ObservableObject
    {
        private bool? _isChecked;

        public string Name { get; set; }

        public bool AllowUnchecking { get; }

        public ObservableCollection<SourceItem> Sources { get; }

        public bool? IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                SetIsChecked(value, updateChildren: true);
            }
        }

        public string Underline
        {
            get
            {
                int num = Sources.Count((SourceItem x) => x.IsChecked == true);
                int count = Sources.Count;
                if (num == 0)
                {
                    return $"All {count} Sources Excluded";
                }
                if (num == count)
                {
                    return $"All {count} Sources Included";
                }
                return $"{num}/{count} Sources Included";
            }
        }

        public SourcesGroup(string name, bool allowUnchecking = true)
        {
            Name = name;
            AllowUnchecking = allowUnchecking;
            Sources = new ObservableCollection<SourceItem>();
        }

        public void SetIsChecked(bool? value, bool updateChildren)
        {
            if (value == _isChecked)
            {
                return;
            }
            _isChecked = value;
            if (updateChildren && _isChecked.HasValue)
            {
                foreach (SourceItem source in Sources)
                {
                    if (_isChecked != false || source.AllowUnchecking)
                    {
                        source.SetIsChecked(_isChecked, updateChildren: true, updateParent: false);
                    }
                }
            }
            OnPropertyChanged("IsChecked", "Underline");
        }

        public void VerifyCheckState()
        {
            bool? flag = null;
            for (int i = 0; i < Sources.Count; i++)
            {
                bool? isChecked = Sources[i].IsChecked;
                if (i == 0)
                {
                    flag = isChecked;
                }
                else if (flag != isChecked)
                {
                    flag = null;
                    break;
                }
            }
            SetIsChecked(flag, updateChildren: false);
            OnPropertyChanged("Underline");
        }

        public override string ToString()
        {
            return $"{Name} ({Sources.Count})";
        }
    }
}
