using System.IO;
using System.Reflection;
using Builder.Presentation.Models.CharacterSheet;
using iTextSharp.text.pdf;

namespace Builder.Presentation.Models.CharacterSheet
{
    public class CharacterSheetResourcePage
    {
        private readonly string _resourcePath;

        public CharacterSheetResourcePage(string resourcePath)
        {
            _resourcePath = resourcePath;
        }

        public Stream GetResourceStream()
        {
            return Assembly.GetAssembly(typeof(CharacterSheetResourcePage)).GetManifestResourceStream(_resourcePath);
        }

        public PdfReader CreateReader()
        {
            return new PdfReader(GetResourceStream());
        }
    }
}
