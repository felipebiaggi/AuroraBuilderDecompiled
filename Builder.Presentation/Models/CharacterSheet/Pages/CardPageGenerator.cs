using System;
using Builder.Presentation.Models.CharacterSheet;
using Builder.Presentation.Models.CharacterSheet.Pages;
using Builder.Presentation.Models.CharacterSheet.Pages.Content;
using Builder.Presentation.Models.CharacterSheet.PDF;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace Builder.Presentation.Models.CharacterSheet.Pages
{
    public class CardPageGenerator : PageGenerator
    {
        public int CardWidth { get; set; }

        public int CardHeight { get; set; }

        public int CardCount { get; protected set; }

        public int CurrentPosition { get; set; }

        public CardPageGenerator(int pageFollowNumber = 1)
            : base(pageFollowNumber)
        {
            CardWidth = 180;
            CardHeight = 240;
            CurrentPosition = -1;
        }

        public void AddGenericCard(GenericCardContent content)
        {
            IncrementCurrentPosition();
            PdfReader genericCardReader = GetGenericCardReader();
            PdfImportedPage importedPage = base.Writer.GetImportedPage(genericCardReader, 1);
            Rectangle cardRectangle = GetCardRectangle(CurrentPosition);
            base.Writer.DirectContentUnder.AddTemplate(importedPage, cardRectangle.Left, cardRectangle.Bottom);
            AddGenericFillableFields(CurrentPosition, content);
            CardCount++;
        }

        public void AddSpellCard(SpellCardContent content)
        {
            IncrementCurrentPosition();
            PdfReader spellCardReader = GetSpellCardReader();
            PdfImportedPage importedPage = base.Writer.GetImportedPage(spellCardReader, 1);
            Rectangle cardRectangle = GetCardRectangle(CurrentPosition);
            base.Writer.DirectContentUnder.AddTemplate(importedPage, cardRectangle.Left, cardRectangle.Bottom);
            AddSpellCardFillableFields(CurrentPosition, content);
            CardCount++;
        }

        public void AddAttackCard(AttackCardContent content)
        {
            IncrementCurrentPosition();
            PdfReader attackCardReader = GetAttackCardReader();
            PdfImportedPage importedPage = base.Writer.GetImportedPage(attackCardReader, 1);
            Rectangle cardRectangle = GetCardRectangle(CurrentPosition);
            base.Writer.DirectContentUnder.AddTemplate(importedPage, cardRectangle.Left, cardRectangle.Bottom);
            AddAttackCardFillableFields(CurrentPosition, content);
            CardCount++;
        }

        private Rectangle GetCardRectangle(int position)
        {
            int cardWidth = CardWidth;
            int cardHeight = CardHeight;
            int pageMargin = base.PageMargin;
            int num = base.PageHeight - base.PageMargin;
            switch (position)
            {
                case 0:
                    return new Rectangle(pageMargin, num - cardHeight, pageMargin + cardWidth, num);
                case 1:
                    pageMargin += cardWidth + base.PageGutter;
                    return new Rectangle(pageMargin, num - cardHeight, pageMargin + cardWidth, num);
                case 2:
                    pageMargin += (cardWidth + base.PageGutter) * 2;
                    return new Rectangle(pageMargin, num - cardHeight, pageMargin + cardWidth, num);
                case 3:
                    num -= cardHeight + base.PageGutter;
                    return new Rectangle(pageMargin, num - cardHeight, pageMargin + cardWidth, num);
                case 4:
                    pageMargin += cardWidth + base.PageGutter;
                    num -= cardHeight + base.PageGutter;
                    return new Rectangle(pageMargin, num - cardHeight, pageMargin + cardWidth, num);
                case 5:
                    pageMargin += (cardWidth + base.PageGutter) * 2;
                    num -= cardHeight + base.PageGutter;
                    return new Rectangle(pageMargin, num - cardHeight, pageMargin + cardWidth, num);
                case 6:
                    num -= (cardHeight + base.PageGutter) * 2;
                    return new Rectangle(pageMargin, num - cardHeight, pageMargin + cardWidth, num);
                case 7:
                    pageMargin += cardWidth + base.PageGutter;
                    num -= (cardHeight + base.PageGutter) * 2;
                    return new Rectangle(pageMargin, num - cardHeight, pageMargin + cardWidth, num);
                case 8:
                    pageMargin += (cardWidth + base.PageGutter) * 2;
                    num -= (cardHeight + base.PageGutter) * 2;
                    return new Rectangle(pageMargin, num - cardHeight, pageMargin + cardWidth, num);
                default:
                    throw new ArgumentException("card index based, between 0-8");
            }
        }

        private PdfReader GetGenericCardReader()
        {
            return GetResourceReader("Builder.Presentation.Resources.Sheets.Partial.card.pdf");
        }

        private PdfReader GetSpellCardReader()
        {
            return GetResourceReader("Builder.Presentation.Resources.Sheets.Partial.spellcard.pdf");
        }

        private PdfReader GetAttackCardReader()
        {
            return GetResourceReader("Builder.Presentation.Resources.Sheets.Partial.attackcard.pdf");
        }

        private void IncrementCurrentPosition()
        {
            if (CurrentPosition == 8)
            {
                base.Document.NewPage();
                CurrentPosition = 0;
                base.PageFollowNumber++;
            }
            else
            {
                CurrentPosition++;
            }
        }

        public override void StartNewPage()
        {
            CurrentPosition = -1;
            base.StartNewPage();
        }

        private string GetCurrentFieldSuffix()
        {
            return $"_{base.PageFollowNumber}:{CurrentPosition}";
        }

        private string GenerateCurrentFieldName(string name, bool flatten = false)
        {
            string text = name + GetCurrentFieldSuffix();
            if (flatten)
            {
                base.PartialFlatteningNames.Add(text);
            }
            return text;
        }

        private void AddGenericFillableFields(int position, GenericCardContent content)
        {
            FillableContentGenerator fillableContentGenerator = new FillableContentGenerator(base.Writer);
            Rectangle cardRectangle = GetCardRectangle(position);
            float width = (float)CardWidth - 6f;
            Rectangle rectangle = GetArea(width: (float)CardWidth - 13f, x: cardRectangle.Left + 6.5f, y: cardRectangle.Bottom + (float)CardHeight - 3.5f, height: 13.5f);
            Rectangle areaUnder = GetAreaUnder(rectangle, 9f, 3f);
            Rectangle area = GetArea(cardRectangle.Left + 3f, areaUnder.Bottom - 2f, width, 195f);
            Rectangle areaUnder2 = GetAreaUnder(area, 8.5f, 1.5f);
            fillableContentGenerator.AddText(fontsize: fillableContentGenerator.CalculateRequiredFontsize(content.Title, rectangle, 10f), area: rectangle, name: GenerateCurrentFieldName("card_title"), content: content.Title, alignment: 1);
            fillableContentGenerator.SetItalic();
            fillableContentGenerator.AddText(areaUnder, GenerateCurrentFieldName("card_subtitle"), content.Subtitle, 6f, 1);
            fillableContentGenerator.SetDefault();
            fillableContentGenerator.FillCardArea(requiredFontSize: fillableContentGenerator.CalculateRequiredFontsize(content.DescriptionHtml + Environment.NewLine + "<p></p>", new Rectangle(area.Left + 2f, area.Bottom + 2f, area.Right - 2f, area.Top - 2f), 7f), descriptionArea: new Rectangle(area.Left + 2f, area.Bottom + 2f, area.Right - 2f, area.Top - 2f), content: content.DescriptionHtml);
            fillableContentGenerator.AddText(areaUnder2, GenerateCurrentFieldName("card_footer_left"), content.LeftFooter, 6f);
            fillableContentGenerator.AddText(areaUnder2, GenerateCurrentFieldName("card_footer_right"), content.RightFooter, 6f, 2);
        }

        private void AddSpellCardFillableFields(int position, SpellCardContent content)
        {
            FillableContentGenerator fillableContentGenerator = new FillableContentGenerator(base.Writer);
            Rectangle cardRectangle = GetCardRectangle(position);
            float width = (float)CardWidth - 6f;
            float num = (float)CardWidth - 13f;
            Rectangle area = GetArea(cardRectangle.Left + 6.5f, cardRectangle.Bottom + (float)CardHeight - 3.5f, num, 13.5f);
            Rectangle areaUnder = GetAreaUnder(area, 9f, 3f);
            Rectangle area2 = GetArea(cardRectangle.Left + 6.5f + 45f, areaUnder.Bottom - 2.5f, num - 45f, 11f);
            Rectangle areaUnder2 = GetAreaUnder(area2, 11f, 0f);
            Rectangle areaUnder3 = GetAreaUnder(areaUnder2, 11f, 0f);
            Rectangle areaUnder4 = GetAreaUnder(areaUnder3, 11f, 0f);
            Rectangle area3 = GetArea(cardRectangle.Left + 3f, areaUnder.Bottom - 47f, width, 150f);
            Rectangle areaUnder5 = GetAreaUnder(area3, 8.5f, 1.5f);
            fillableContentGenerator.FillCardField(area, content.Title, FontsHelper.GetRegular(10f), 1);
            fillableContentGenerator.SetItalic();
            fillableContentGenerator.FillCardField(areaUnder, content.Subtitle, FontsHelper.GetItalic(6f), 1);
            fillableContentGenerator.SetDefault();
            fillableContentGenerator.FillCardField(new Rectangle(area2.Left + 2f, area2.Bottom + 1f, area2.Right - 1f, area2.Top - 1.5f), content.CastingTime, FontsHelper.GetRegular(6f));
            fillableContentGenerator.FillCardField(new Rectangle(areaUnder2.Left + 2f, areaUnder2.Bottom + 1f, areaUnder2.Right - 1f, areaUnder2.Top - 1.5f), content.Range, FontsHelper.GetRegular(6f));
            fillableContentGenerator.FillCardField(new Rectangle(areaUnder3.Left + 2f, areaUnder3.Bottom + 1f, areaUnder3.Right - 1f, areaUnder3.Top - 1.5f), content.Duration, FontsHelper.GetRegular(6f));
            fillableContentGenerator.FillCardField(new Rectangle(areaUnder4.Left + 2f, areaUnder4.Bottom + 1f, areaUnder4.Right - 1f, areaUnder4.Top - 1.5f), content.Components, FontsHelper.GetRegular(6f));
            fillableContentGenerator.FillCardArea(requiredFontSize: fillableContentGenerator.CalculateRequiredFontsize(content.DescriptionHtml + Environment.NewLine + "<p></p>", new Rectangle(area3.Left + 2f, area3.Bottom + 2f, area3.Right - 2f, area3.Top - 2f), 6f), descriptionArea: new Rectangle(area3.Left + 2f, area3.Bottom + 2f, area3.Right - 2f, area3.Top - 2f), content: content.DescriptionHtml);
            Rectangle area4 = new Rectangle(areaUnder5.Left + 3f, areaUnder5.Bottom, areaUnder5.Right - 3f, areaUnder5.Top);
            Font italic = FontsHelper.GetItalic(6f);
            fillableContentGenerator.FillCardField(area4, content.LeftFooter, italic);
            fillableContentGenerator.FillCardField(area4, content.RightFooter, italic, 2);
        }

        private void AddAttackCardFillableFields(int position, AttackCardContent content)
        {
            FillableContentGenerator fillableContentGenerator = new FillableContentGenerator(base.Writer);
            Rectangle cardRectangle = GetCardRectangle(position);
            float width = (float)CardWidth - 6f;
            float num = (float)CardWidth - 13f;
            Rectangle area = GetArea(cardRectangle.Left + 6.5f, cardRectangle.Bottom + (float)CardHeight - 3.5f, num, 13.5f);
            Rectangle areaUnder = GetAreaUnder(area, 9f, 3f);
            Rectangle area2 = GetArea(cardRectangle.Left + 6.5f + 45f, areaUnder.Bottom - 2.5f, num - 45f, 11f);
            Rectangle areaUnder2 = GetAreaUnder(area2, 11f, 0f);
            Rectangle areaUnder3 = GetAreaUnder(areaUnder2, 11f, 0f);
            Rectangle area3 = GetArea(cardRectangle.Left + 3f, areaUnder.Bottom - 36f, width, 161f);
            Rectangle areaUnder4 = GetAreaUnder(area3, 8.5f, 1.5f);
            fillableContentGenerator.AddText(area, GenerateCurrentFieldName("card_title"), content.Title, 10f, 1);
            fillableContentGenerator.SetItalic();
            fillableContentGenerator.AddText(areaUnder, GenerateCurrentFieldName("card_subtitle"), content.Subtitle, 6f, 1);
            fillableContentGenerator.SetDefault();
            fillableContentGenerator.AddText(area2, GenerateCurrentFieldName("card_range"), content.Range);
            fillableContentGenerator.AddText(areaUnder2, GenerateCurrentFieldName("card_attack"), content.Attack);
            fillableContentGenerator.AddText(areaUnder3, GenerateCurrentFieldName("card_damage"), content.Damage);
            fillableContentGenerator.AddText(area3, GenerateCurrentFieldName("card_description"), content.Description, 6f, 0, multiline: true);
            fillableContentGenerator.AddText(areaUnder4, GenerateCurrentFieldName("card_footer_left"), content.LeftFooter, 6f);
            fillableContentGenerator.AddText(areaUnder4, GenerateCurrentFieldName("card_footer_right"), content.RightFooter, 6f, 2);
        }

        private Rectangle GetArea(float x, float y, float width, float height)
        {
            return new Rectangle(x, y - height, x + width, y);
        }

        private Rectangle GetAreaUnder(Rectangle area, float height, float offset)
        {
            return new Rectangle(area.Left, area.Bottom - offset - height, area.Right, area.Bottom - offset);
        }
    }
}
