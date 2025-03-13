using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Builder.Presentation.Models.Sources;

namespace Builder.Presentation.Converter
{
    public class CampaignSourceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            List<string> list = (from f in ((parameter as string) ?? "").Split(',')
                                 select f.Trim()).ToList();
            while (values.Length > list.Count)
            {
                list.Add(string.Empty);
            }
            List<object> list2 = new List<object>();
            for (int i = 0; i < values.Length; i++)
            {
                IEnumerable enumerable = (values[i] as IEnumerable) ?? new List<object> { values[i] };
                string text = list[i];
                if (text != string.Empty)
                {
                    SourcesGroup sourcesGroup = new SourcesGroup(text);
                    foreach (object item in enumerable)
                    {
                        sourcesGroup.Sources.Add(item as SourceItem);
                    }
                    list2.Add(sourcesGroup);
                    continue;
                }
                foreach (object item2 in enumerable)
                {
                    list2.Add(item2);
                }
            }
            return list2;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot perform reverse-conversion");
        }
    }
}
