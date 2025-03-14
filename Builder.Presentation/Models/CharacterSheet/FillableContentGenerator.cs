using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Builder.Presentation.Models.CharacterSheet.Content;
using Builder.Presentation.Models.CharacterSheet.PDF;
using Builder.Presentation.Models.Sheet;
using Builder.Presentation.Utilities;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;



namespace Builder.Presentation.Models.CharacterSheet
{
    public class FillableContentGenerator
    {
        private readonly PdfWriter _writer;

        private readonly BaseFont _defaultFont;

        private readonly BaseFont _italicFont;

        private readonly BaseFont _boldFont;

        private readonly BaseFont _boldItalicFont;

        private BaseFont _currentFont;

        private BaseColor _currentFontColor;

        private const string ListIndentation = "    ";

        public FillableContentGenerator(PdfWriter writer)
        {
            _writer = writer;
            _defaultFont = FontsHelper.GetRegular().BaseFont;
            _italicFont = FontsHelper.GetItalic().BaseFont;
            _boldFont = FontsHelper.GetBold().BaseFont;
            _boldItalicFont = FontsHelper.GetBoldItalic().BaseFont;
            _currentFont = _defaultFont;
            _currentFontColor = BaseColor.BLACK;
        }

        public void SetDefault()
        {
            _currentFont = _defaultFont;
        }

        public void SetBold()
        {
            _currentFont = _boldFont;
        }

        public void SetItalic()
        {
            _currentFont = _italicFont;
        }

        public void SetBoldItalic()
        {
            _currentFont = _boldItalicFont;
        }

        public void SetColor(BaseColor color)
        {
            _currentFontColor = color;
        }

        public void ResetColor()
        {
            _currentFontColor = BaseColor.BLACK;
        }

        [Obsolete]
        public BaseFont GetCurrentFont()
        {
            return _currentFont;
        }

        public static Font GetFont(string fontName, string filename, float size = 0f)
        {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            if (!FontFactory.IsRegistered(filename))
            {
                FontFactory.Register(Path.Combine(folderPath, filename));
            }
            return FontFactory.GetFont(fontName, "Identity-H", embedded: true, size);
        }

        public Font GetCurrentAsFont(float size)
        {
            return new Font(_currentFont, size);
        }

        public TextField CreateText(Rectangle area, string name, string content = "", float fontsize = 0f, int alignment = 0)
        {
            TextField textField = new TextField(_writer, area, name)
            {
                Font = _currentFont,
                Alignment = alignment,
                FontSize = fontsize,
                Options = 12582912,
                TextColor = _currentFontColor
            };
            if (!string.IsNullOrWhiteSpace(content))
            {
                textField.Text = content;
            }
            return textField;
        }

        public TextField CreateArea(Rectangle area, string name, string content = "", float fontsize = 0f, int alignment = 0)
        {
            TextField textField = CreateText(area, name, content, fontsize, alignment);
            textField.Options = 46141440;
            return textField;
        }

        public TextField AddText(Rectangle area, string name, string content = "", float fontsize = 0f, int alignment = 0, bool multiline = false)
        {
            TextField textField = (multiline ? CreateArea(area, name, content, fontsize, alignment) : CreateText(area, name, content, fontsize, alignment));
            _writer.AddAnnotation(textField.GetTextField());
            return textField;
        }

        public RadioCheckField AddCheck(Rectangle area, string name, CharacterSheetSpellcastingPageExportContent.SpellExportContent spell = null)
        {
            RadioCheckField radioCheckField = new RadioCheckField(_writer, area, name, "Yes");
            radioCheckField.Checked = spell?.IsPrepared ?? false;
            _writer.AddAnnotation(radioCheckField.CheckField);
            return radioCheckField;
        }

        public float CalculateRequiredFontsize(string content, Rectangle contentArea, float maximumFontSize)
        {
            return ColumnText.FitText(FontsHelper.GetRegular(), content, contentArea, maximumFontSize, 0);
        }

        public void FillArea(Rectangle descriptionArea, string content, float requiredFontSize, int alignment = 0)
        {
            ColumnText columnText = new ColumnText(_writer.DirectContent);
            columnText.Alignment = alignment;
            columnText.SetSimpleColumn(descriptionArea);
            ElementDescriptionGenerator.FillColumn(columnText, content, requiredFontSize);
            columnText.Go();
        }

        public void FillCardArea(Rectangle descriptionArea, string content, float requiredFontSize, int alignment = 0)
        {
            ColumnText columnText = new ColumnText(_writer.DirectContent);
            columnText.Alignment = alignment;
            columnText.SetSimpleColumn(descriptionArea);
            FillCardDescription(columnText, content, requiredFontSize);
            columnText.Go();
        }

        public void FillCardField(Rectangle area, string description, Font font, int alignment = 0)
        {
            float num = CalculateRequiredFontsize(description.Trim(), area, font.Size);
            ColumnText columnText = new ColumnText(_writer.DirectContent);
            columnText.Alignment = alignment;
            columnText.SetSimpleColumn(area);
            columnText.SetLeading(num, 1f);
            Chunk chunk = new Chunk(description, font);
            chunk.Font.Size = num;
            chunk.setLineHeight(num);
            new Paragraph(chunk);
            columnText.AddText(chunk);
            columnText.Go();
        }

        public void Fill(Rectangle area, ContentArea content, int alignment = 0)
        {
            float num = CalculateRequiredFontsize(content.ToString(), area, 8f);
            ColumnText columnText = new ColumnText(_writer.DirectContent)
            {
                Alignment = alignment
            };
            columnText.SetSimpleColumn(area);
            columnText.SetLeading(num, 1f);
            foreach (ContentLine item in content)
            {
                Paragraph paragraph = new Paragraph(num);
                if (item.HasName())
                {
                    paragraph.Add(new Chunk(item.Name + ". ", FontsHelper.GetBoldItalic(num)));
                }
                if (item.HasContent())
                {
                    GetElements(item.Content);
                    paragraph.Add(new Chunk(" " + item.Content, FontsHelper.GetRegular(num)));
                }
            }
            columnText.Go();
        }

        private void FillCardDescription(ColumnText column, string description, float fontsize)
        {
            column.SetLeading(fontsize, 1f);
            List<IElement> elements = GetElements(description);
            bool flag = false;
            foreach (IElement item in elements)
            {
                Paragraph paragraph = new Paragraph(fontsize);
                if (item is PdfPTable pdfPTable)
                {
                    List<PdfPRow> rows = pdfPTable.Rows;
                    foreach (PdfPRow item2 in rows)
                    {
                        Chunk chunk = new Chunk();
                        chunk.Append("    ");
                        paragraph.Add(chunk);
                        bool flag2 = item2 == rows.FirstOrDefault();
                        PdfPCell[] cells = item2.GetCells();
                        for (int i = 0; i < cells.Length; i++)
                        {
                            foreach (IElement compositeElement in cells[i].CompositeElements)
                            {
                                foreach (Chunk chunk2 in compositeElement.Chunks)
                                {
                                    if (flag2)
                                    {
                                        chunk2.Font.SetStyle(1);
                                    }
                                    paragraph.Add(chunk2);
                                }
                                paragraph.Add(CreateIndentationChunk());
                            }
                        }
                        paragraph.Add(Environment.NewLine);
                    }
                    paragraph.Add(Environment.NewLine);
                }
                else if (item is List listElement)
                {
                    paragraph.Add(CreateListChunk(listElement));
                    paragraph.Add(Environment.NewLine);
                }
                else
                {
                    foreach (Chunk chunk3 in item.Chunks)
                    {
                        if (flag)
                        {
                            paragraph.Add(CreateIndentationChunk());
                            flag = false;
                        }
                        else if (item.Chunks.Count > 1 && chunk3 == item.Chunks.First() && elements.First() != item)
                        {
                            paragraph.Add(CreateIndentationChunk());
                        }
                        else if (chunk3.Content.ToLower().Contains("at higher level"))
                        {
                            paragraph.Add(CreateIndentationChunk());
                        }
                        paragraph.Add(chunk3);
                    }
                    if (!item.Equals(elements.Last()))
                    {
                        paragraph.Add(Environment.NewLine);
                    }
                }
                flag = !(item is List);
                FormatCardParagraphChunks(paragraph, fontsize);
                column.AddText(paragraph);
            }
        }

        private void FillCardField(ColumnText column, string description, Font font)
        {
            column.SetLeading(font.Size, 1f);
            Paragraph phrase = new Paragraph(font.Size, description, font);
            column.AddText(phrase);
        }

        private List<IElement> GetElements(string description)
        {
            if (description == null)
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                return null;
            }
            return HTMLWorker.ParseToList(new StringReader(description), null);
        }

        private Chunk CreateListChunk(List listElement)
        {
            Chunk chunk = new Chunk();
            Chunk symbol = listElement.Symbol;
            foreach (Chunk chunk2 in listElement.Chunks)
            {
                chunk.Append("    " + $"{symbol} {chunk2.Content}" + Environment.NewLine);
            }
            return chunk;
        }

        private Chunk CreateIndentationChunk()
        {
            return new Chunk("    ");
        }

        private void FormatCardParagraphChunks(Paragraph paragraph, float fontsize)
        {
            bool flag = false;
            foreach (Chunk chunk in paragraph.Chunks)
            {
                chunk.setLineHeight(fontsize);
                switch (chunk.Font.Style)
                {
                    case -1:
                    case 0:
                        chunk.Font = FontsHelper.GetRegular(fontsize);
                        break;
                    case 1:
                        chunk.Font = FontsHelper.GetBold(fontsize);
                        break;
                    case 2:
                        chunk.Font = FontsHelper.GetItalic(fontsize);
                        break;
                    case 3:
                        chunk.Font = FontsHelper.GetBoldItalic(fontsize);
                        break;
                    default:
                        chunk.Font = FontsHelper.GetRegular(fontsize);
                        break;
                }
            }
        }

        private void FormatParagraphWithFont(Paragraph paragraph, Font font)
        {
            foreach (Chunk chunk in paragraph.Chunks)
            {
                chunk.setLineHeight(font.Size);
                chunk.Font = font;
            }
        }
    }
}
