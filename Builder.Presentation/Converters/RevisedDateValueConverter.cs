using Builder.Core.Logging;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Builder.Presentation.Converters
{
    public class RevisedDateValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return DateTime.Parse(value.ToString()).ToShortDateString();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Convert");
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
