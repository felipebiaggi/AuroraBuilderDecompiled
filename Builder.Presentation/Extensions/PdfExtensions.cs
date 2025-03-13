using iTextSharp.text.pdf;

namespace Builder.Presentation.Extensions
{
    public static class PdfExtensions
    {
        public static void SetFontSize(this PdfStamper stamper, float fontsize, params string[] fields)
        {
            foreach (string field in fields)
            {
                stamper.AcroFields.SetFieldProperty(field, "textsize", fontsize, null);
            }
        }
    }
}
