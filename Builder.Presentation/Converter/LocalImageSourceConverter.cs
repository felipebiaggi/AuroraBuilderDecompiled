using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Builder.Core.Logging;
using Builder.Presentation.Services.Data;

namespace Builder.Presentation.Converter
{
    public class LocalImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return "pack://application:,,,/Resources/default-portrait.png";
            }
            try
            {
                string text = value.ToString();
                BitmapImage bitmapImage = new BitmapImage();
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
            try
            {
                BitmapImage bitmapImage2 = new BitmapImage();
                bitmapImage2.BeginInit();
                bitmapImage2.UriSource = new Uri(Path.Combine(DataManager.Current.UserDocumentsPortraitsDirectory, "default-portrait.png"), UriKind.RelativeOrAbsolute);
                bitmapImage2.EndInit();
                return bitmapImage2;
            }
            catch (Exception ex2)
            {
                Logger.Exception(ex2, "Convert");
            }
            try
            {
                BitmapImage bitmapImage3 = new BitmapImage();
                bitmapImage3.BeginInit();
                bitmapImage3.UriSource = new Uri(Path.Combine(DataManager.Current.UserDocumentsCompanionGalleryDirectory, "default-companion.png"), UriKind.RelativeOrAbsolute);
                bitmapImage3.EndInit();
                return bitmapImage3;
            }
            catch (Exception ex3)
            {
                Logger.Exception(ex3, "Convert");
            }
            try
            {
                BitmapImage bitmapImage4 = new BitmapImage();
                bitmapImage4.BeginInit();
                bitmapImage4.UriSource = new Uri(Path.Combine(DataManager.Current.UserDocumentsSymbolsGalleryDirectory, "default-companion.png"), UriKind.RelativeOrAbsolute);
                bitmapImage4.EndInit();
                return bitmapImage4;
            }
            catch (Exception ex4)
            {
                Logger.Exception(ex4, "Convert");
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
