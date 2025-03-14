using System;
using System.Drawing;
using System.IO;

namespace Builder.Presentation.Utilities
{
    public static class GalleryUtilities
    {
        public static string ConvertImageToBase64(string path)
        {
            return Convert.ToBase64String(File.ReadAllBytes(path));
        }

        public static Image ConvertToBase64(string base64)
        {
            using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(base64)))
            {
                return Image.FromStream(stream);
            }
        }

        public static bool SaveBase64AsImage(string base64, string outputPath)
        {
            using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(base64)))
            {
                Image.FromStream(stream).Save(outputPath);
                return true;
            }
        }
    }
}
