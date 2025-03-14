using Builder.Core.Logging;
using Builder.Presentation.Services.Data;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Builder.Presentation.Converter
{
    public class SymbolGalleryImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return Binding.DoNothing;
            }
            string uriString = value.ToString();
            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(uriString, UriKind.RelativeOrAbsolute);
                bitmapImage.EndInit();
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
                bitmapImage2.UriSource = new Uri(Path.Combine(DataManager.Current.UserDocumentsSymbolsGalleryDirectory, "default-symbol.jpg"), UriKind.RelativeOrAbsolute);
                bitmapImage2.EndInit();
                return bitmapImage2;
            }
            catch (Exception ex2)
            {
                Logger.Exception(ex2, "Convert");
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
