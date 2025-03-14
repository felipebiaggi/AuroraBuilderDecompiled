using Builder.Core.Logging;
using Builder.Presentation.Services.Data;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Builder.Presentation.Converter
{
    public class PortraitImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                try
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    string text = value.ToString();
                    if (!File.Exists(text))
                    {
                        string fileName = Path.GetFileName(text);
                        text = Path.Combine(DataManager.Current.UserDocumentsPortraitsDirectory, fileName);
                    }
                    if (File.Exists(text))
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri(text, UriKind.RelativeOrAbsolute);
                        bitmapImage.EndInit();
                    }
                    return bitmapImage;
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, "Convert");
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
