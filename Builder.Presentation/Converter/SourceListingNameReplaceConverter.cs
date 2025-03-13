using System;
using System.Globalization;
using System.Windows.Data;
using Builder.Presentation.Models.Sources;

namespace Builder.Presentation.Converter
{
    public class SourceListingNameReplaceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            if (value is SourceItem sourceItem)
            {
                if (sourceItem.Source.IsOfficialContent && sourceItem.Source.IsPlaytestContent && sourceItem.Source.HasReleaseDate)
                {
                    return sourceItem.ToString().Replace("Unearthed Arcana:", "UA:").Trim();
                }
                if (sourceItem.Source.IsOfficialContent && sourceItem.Source.IsAdventureLeagueContent)
                {
                    return sourceItem.ToString().Replace("Adventurers League:", "AL:").Trim();
                }
                return sourceItem.ToString();
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

}
