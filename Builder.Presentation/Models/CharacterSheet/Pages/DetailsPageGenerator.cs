using Builder.Presentation.Models.CharacterSheet;
using Builder.Presentation.Models.CharacterSheet.Content;
using Builder.Presentation.Models.CharacterSheet.Pages;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Builder.Presentation.Models.CharacterSheet.Pages
{
    public class DetailsPageGenerator : PageGenerator
    {
        private readonly PdfReader _detailsPageReader;

        private readonly PdfReader _backgroundPageReader;

        public DetailsPageGenerator()
        {
            _detailsPageReader = CharacterSheetResources.GetDetailsPage().CreateReader();
            _backgroundPageReader = CharacterSheetResources.GetBackgroundPage().CreateReader();
        }

        public void AddDetails(CharacterSheetExportContent content)
        {
            PlacePage(_detailsPageReader, new Rectangle(0f, 0f, base.PageWidth, base.PageHeight));
        }

        public void AddBackground(CharacterSheetExportContent content)
        {
            PlacePage(_backgroundPageReader, new Rectangle(0f, 0f, base.PageWidth, base.PageHeight));
        }

        public override void Dispose()
        {
            base.Dispose();
            _detailsPageReader?.Dispose();
        }
    }
}
