using System.Text;
using Builder.Presentation.Models.CharacterSheet.Pages.Content;
using iTextSharp.text.pdf;

namespace Builder.Presentation.Models.CharacterSheet.Pages.Content
{
    public abstract class PageContentItem<T> : IPageContentItem
    {
        public string Key { get; }

        public T Content { get; }

        protected PageContentItem(string key, T content)
        {
            Key = key;
            Content = content;
        }

        public override string ToString()
        {
            return Key;
        }
    }

    public class PageContentWriter : IPageContentWriter
    {
        private readonly PdfStamper _stamper;

        public PageContentWriter(PdfStamper stamper)
        {
            _stamper = stamper;
        }

        public void Write(string key, string content)
        {
            _stamper.AcroFields.SetField(key, content);
        }

        public void Write<T>(T item) where T : IPageContentItem
        {
            if (item is LineContent)
            {
                LineContent lineContent = item as LineContent;
                if (lineContent.Fontsize > 0f)
                {
                    SetFontSize(lineContent.Key, lineContent.Fontsize);
                }
                _stamper.AcroFields.SetField(lineContent.Key, lineContent.Content);
            }
            if (!(item is AreaContent))
            {
                return;
            }
            AreaContent areaContent = item as AreaContent;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string item2 in areaContent.Content)
            {
                stringBuilder.AppendLine(item2);
            }
            _stamper.AcroFields.SetField(areaContent.Key, stringBuilder.ToString());
        }

        public void SetFontSize(string key, float size)
        {
            _stamper.AcroFields.SetFieldProperty(key, "textsize", size, null);
        }
    }
}
