using System;
using Builder.Presentation.Models.CharacterSheet.Pages.Content;
using Builder.Presentation.Models.CharacterSheet.PDF;
using iTextSharp.text;

namespace Builder.Presentation.Models.CharacterSheet.Pages.Content
{
    public class ContentBuilder
    {
        private readonly Font _regular;

        private readonly Font _bold;

        private readonly Font _italic;

        private readonly Font _boldItalic;

        private readonly Paragraph _content;

        public ContentBuilder()
        {
            _regular = FontsHelper.GetRegular();
            _bold = FontsHelper.GetBold();
            _italic = FontsHelper.GetItalic();
            _boldItalic = FontsHelper.GetBoldItalic();
            _content = new Paragraph();
        }

        public ContentBuilder Append(string value, bool newLine = false)
        {
            return Append(value, _regular, newLine);
        }

        public ContentBuilder AppendBold(string value, bool newLine = false)
        {
            return Append(value, _bold, newLine);
        }

        public ContentBuilder AppendItalic(string value, bool newLine = false)
        {
            return Append(value, _italic, newLine);
        }

        public ContentBuilder AppendBoldItalic(string value, bool newLine = false)
        {
            return Append(value, _boldItalic, newLine);
        }

        public ContentBuilder AppendLine(string value)
        {
            return Append(value, _regular, newLine: true);
        }

        public Paragraph GetContent(int alignment = 0)
        {
            _content.Alignment = alignment;
            return _content;
        }

        public override string ToString()
        {
            return _content.Content;
        }

        private ContentBuilder Append(string value, Font font, bool newLine = false)
        {
            _content.Add(new Chunk(value + (newLine ? Environment.NewLine : ""), font));
            return this;
        }
    }
}
