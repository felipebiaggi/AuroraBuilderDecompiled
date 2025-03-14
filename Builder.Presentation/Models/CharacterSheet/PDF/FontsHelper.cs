using System;
using System.IO;
using iTextSharp.text;


namespace Builder.Presentation.Models.CharacterSheet.PDF
{
    public static class FontsHelper
    {
        public static Font GetRegular(float size = 12f)
        {
            return GetFont("Calibri", "calibri.ttf", size) ?? FontFactory.GetFont("Helvetica", size);
        }

        public static Font GetBold(float size = 12f)
        {
            return GetFont("Calibri Bold", "calibrib.ttf", size) ?? FontFactory.GetFont("Helvetica-Bold", size);
        }

        public static Font GetItalic(float size = 12f)
        {
            return GetFont("Calibri Italic", "calibrii.ttf", size) ?? FontFactory.GetFont("Helvetica-Oblique", size);
        }

        public static Font GetBoldItalic(float size = 12f)
        {
            return GetFont("Calibri Bold Italic", "calibriz.ttf", size) ?? FontFactory.GetFont("Helvetica-BoldOblique", size);
        }

        private static Font GetFont(string fontName, string filename, float size = 0f)
        {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            if (!FontFactory.IsRegistered(filename))
            {
                FontFactory.Register(Path.Combine(folderPath, filename));
            }
            return FontFactory.GetFont(fontName, "Identity-H", embedded: true, size);
        }
    }
}
