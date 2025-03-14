using Builder.Core;
using Builder.Presentation.Services.Calculator;
using System.Collections.Generic;
using System.Linq;

namespace Builder.Presentation.ViewModels.Content
{
    public class StatisticsPanelItem : ObservableObject
    {
        private bool _exists;

        private bool _updated;

        private string _displayName;

        private string _displayValue;

        private int _value;

        private string _summery;

        public bool Exists
        {
            get
            {
                return _exists;
            }
            set
            {
                SetProperty(ref _exists, value, "Exists");
            }
        }

        public bool IsUpdated
        {
            get
            {
                return _updated;
            }
            set
            {
                SetProperty(ref _updated, value, "IsUpdated");
            }
        }

        public string DisplayName
        {
            get
            {
                return _displayName.ToUpper();
            }
            set
            {
                SetProperty(ref _displayName, value, "DisplayName");
            }
        }

        public string DisplayValue
        {
            get
            {
                return _displayValue;
            }
            set
            {
                SetProperty(ref _displayValue, value, "DisplayValue");
            }
        }

        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                SetProperty(ref _value, value, "Value");
            }
        }

        public string Summery
        {
            get
            {
                return _summery;
            }
            set
            {
                SetProperty(ref _summery, value, "Summery");
            }
        }

        public StatisticsPanelItem(string displayName, string displayValue = "")
        {
            _displayName = displayName;
            _displayValue = displayValue;
        }

        public void Update(StatisticValuesGroup group)
        {
            Exists = group != null;
            if (group != null)
            {
                Value = group.Sum();
                Dictionary<string, int> values = group.GetValues();
                string summery = string.Join(", ", from x in values
                                                   where x.Value > 0
                                                   select $"{x.Key} ({x.Value})");
                Summery = summery;
                if (Value != 0)
                {
                    IsUpdated = true;
                }
            }
        }
    }
}
