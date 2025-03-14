using System;
using System.Collections.Generic;
using System.Linq;

namespace Builder.Presentation.Services.Calculator
{
    public class StatisticValuesGroup
    {
        private readonly Dictionary<string, int> _values;

        public string GroupName { get; }

        public bool IsFinalized { get; set; }

        public StatisticValuesGroup(string groupName)
        {
            GroupName = groupName;
            _values = new Dictionary<string, int>();
        }

        public void AddValue(string source, int value)
        {
            if (_values.ContainsKey(source))
            {
                _values[source] += value;
            }
            else
            {
                _values.Add(source, value);
            }
        }

        public bool ContainsValue(string source)
        {
            return _values.ContainsKey(source);
        }

        [Obsolete]
        public int GetValue(string source)
        {
            if (ContainsValue(source))
            {
                return _values[source];
            }
            throw new KeyNotFoundException(source);
        }

        public Dictionary<string, int> GetValues()
        {
            return _values;
        }

        public int Sum()
        {
            return _values.Sum((KeyValuePair<string, int> x) => x.Value);
        }

        public string GetSummery(bool includeValues = true)
        {
            if (_values.Count == 0)
            {
                return string.Empty;
            }
            if (includeValues)
            {
                return string.Join(", ", from x in GetValues()
                                         select $"{x.Key} ({x.Value})");
            }
            return string.Join(", ", from x in GetValues()
                                     select x.Key ?? "");
        }

        public override string ToString()
        {
            return $"{GroupName} [{Sum()}]";
        }

        public void Merge(StatisticValuesGroup group)
        {
            if (group == null || group.Sum() <= 0)
            {
                return;
            }
            foreach (KeyValuePair<string, int> value in group.GetValues())
            {
                AddValue(value.Key, value.Value);
            }
        }

        public void Finalized()
        {
            IsFinalized = true;
        }
    }
}
