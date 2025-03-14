using Builder.Data.Strings;
using System.Collections.Generic;
using System.Linq;

namespace Builder.Presentation.Services.Calculator
{
    public class StatisticsCalculatedResult
    {
        private readonly Dictionary<string, int> _values;

        public StatisticsCalculatedResult(bool initializeDefaults = false)
        {
            _values = new Dictionary<string, int>();
            if (!initializeDefaults)
            {
                return;
            }
            AuroraStatisticStrings _names = new AuroraStatisticStrings();
            foreach (string item in from x in typeof(AuroraStatisticStrings).GetProperties()
                                    select x.GetValue(_names).ToString())
            {
                _values.Add(item, 0);
            }
            foreach (KeyValuePair<string, int> item2 in _values.ToList())
            {
                if (item2.Key.ToLower().Contains(":passive"))
                {
                    _values[item2.Key] = 10;
                }
            }
        }

        public void AddValue(string statisticsName, int value)
        {
            if (_values.ContainsKey(statisticsName))
            {
                _values[statisticsName] += value;
            }
            else
            {
                _values.Add(statisticsName, value);
            }
        }

        public bool ContainsValue(string statisticsName)
        {
            return _values.ContainsKey(statisticsName);
        }

        public int GetValue(string statisticsName)
        {
            if (ContainsValue(statisticsName))
            {
                return _values[statisticsName];
            }
            throw new KeyNotFoundException(statisticsName);
        }

        public Dictionary<string, int> GetValues()
        {
            return _values;
        }
    }
}
