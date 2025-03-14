using System.Collections.Generic;
using System.Linq;

namespace Builder.Presentation.Services.Calculator
{
    public class StatisticValuesGroupCollection : List<StatisticValuesGroup>
    {
        public bool ContainsGroup(string groupName)
        {
            if (groupName == null)
            {
                return false;
            }
            if (groupName.StartsWith("-"))
            {
                return this.Any((StatisticValuesGroup x) => x.GroupName.Equals(groupName.Substring(1, groupName.Length - 1)));
            }
            return this.Any((StatisticValuesGroup x) => x.GroupName.Equals(groupName));
        }

        public void AddGroup(StatisticValuesGroup group)
        {
            if (ContainsGroup(group.GroupName))
            {
                StatisticValuesGroup group2 = GetGroup(group.GroupName);
                {
                    foreach (KeyValuePair<string, int> value in group.GetValues())
                    {
                        group2.AddValue(value.Key, value.Value);
                    }
                    return;
                }
            }
            Add(group);
        }

        public StatisticValuesGroup GetGroup(string groupName, bool createNonExisting = true)
        {
            if (groupName.StartsWith("-"))
            {
                groupName = groupName.Substring(1, groupName.Length - 1);
            }
            if (ContainsGroup(groupName))
            {
                return this.Single((StatisticValuesGroup x) => x.GroupName.Equals(groupName));
            }
            if (createNonExisting)
            {
                StatisticValuesGroup statisticValuesGroup = new StatisticValuesGroup(groupName);
                Add(statisticValuesGroup);
                return statisticValuesGroup;
            }
            return null;
        }

        public int GetValue(string groupName)
        {
            if (ContainsGroup(groupName))
            {
                return GetGroup(groupName).Sum();
            }
            return 0;
        }
    }
}
