using System.Collections.Generic;
using Builder.Data.Elements;
using Builder.Presentation;
using Builder.Presentation.Models.Sheet;

namespace Builder.Presentation
{
    public class ContentGenerator
    {
        private readonly ElementsOrganizer _organizer;

        public ContentGenerator(ElementsOrganizer organizer)
        {
            _organizer = organizer;
        }

        public ContentField GetLanguagesField()
        {
            IEnumerable<Language> languages = _organizer.GetLanguages();
            ContentBuilder contentBuilder = new ContentBuilder("lang");
            contentBuilder.Append("Languages", string.Join(", ", languages), indent: false).AppendNewLine().Append("WP", "-", indent: false)
                .AppendNewLine()
                .Append("AP", "-", indent: false)
                .AppendNewLine()
                .Append("TP", "-", indent: false);
            return contentBuilder.GetContentField();
        }
    }
}
