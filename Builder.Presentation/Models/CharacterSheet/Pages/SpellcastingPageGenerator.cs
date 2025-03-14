using System;
using System.Linq;
using Builder.Presentation.Models.CharacterSheet;
using Builder.Presentation.Models.CharacterSheet.Content;
using Builder.Presentation.Models.CharacterSheet.Pages;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Builder.Presentation.Models.CharacterSheet.Pages
{
    public class SpellcastingPageGenerator : PageGenerator
    {
        private PdfReader _headerReader;

        private PdfReader _topReader;

        private PdfReader _middleReader;

        private PdfReader _bottomReader;

        public int HeaderHeight { get; set; }

        public int HeaderOffset { get; set; }

        public int SectionTopHeight { get; set; }

        public int SectionMiddleHeight { get; set; }

        public int SectionBottomHeight { get; set; }

        public int SectionMargin { get; set; }

        public Rectangle LatestArea { get; set; }

        public SpellcastingPageGenerator(int pageFollowNumber = 1)
            : base(pageFollowNumber)
        {
            HeaderHeight = 100;
            HeaderOffset = 10;
            SectionTopHeight = 24;
            SectionMiddleHeight = 12;
            SectionBottomHeight = 24;
            SectionMargin = 0;
            _headerReader = CharacterSheetResources.PartialSpellcastingHeader().CreateReader();
            _topReader = CharacterSheetResources.PartialSpellcastingTop().CreateReader();
            _middleReader = CharacterSheetResources.PartialSpellcastingMiddle().CreateReader();
            _bottomReader = CharacterSheetResources.PartialSpellcastingBottom().CreateReader();
        }

        public void Add(CharacterSheetSpellcastingPageExportContent spellcastingContent, Rectangle newLatestArea = null)
        {
            if (LatestArea == null || newLatestArea != null)
            {
                Rectangle rectangle = GetArea(0f, base.PageHeight - HeaderOffset, base.PageWidth, HeaderHeight);
                if (newLatestArea != null)
                {
                    rectangle = newLatestArea;
                }
                PlacePage(_headerReader, rectangle);
                AddHeaderFields(rectangle, spellcastingContent);
                Rectangle previousRectangle = AddSection(rectangle, spellcastingContent.Cantrips, spellcastingContent.SpellcastingClass);
                if (RequiresNewPage(previousRectangle, spellcastingContent.Spells1))
                {
                    previousRectangle = AppendNewPage(spellcastingContent);
                }
                previousRectangle = AddSection(previousRectangle, spellcastingContent.Spells1, spellcastingContent.SpellcastingClass, spellcastingContent.IsMulticlassSpellcaster);
                if (RequiresNewPage(previousRectangle, spellcastingContent.Spells2))
                {
                    previousRectangle = AppendNewPage(spellcastingContent);
                }
                previousRectangle = AddSection(previousRectangle, spellcastingContent.Spells2, spellcastingContent.SpellcastingClass, spellcastingContent.IsMulticlassSpellcaster);
                if (RequiresNewPage(previousRectangle, spellcastingContent.Spells3))
                {
                    previousRectangle = AppendNewPage(spellcastingContent);
                }
                previousRectangle = AddSection(previousRectangle, spellcastingContent.Spells3, spellcastingContent.SpellcastingClass, spellcastingContent.IsMulticlassSpellcaster);
                if (RequiresNewPage(previousRectangle, spellcastingContent.Spells4))
                {
                    previousRectangle = AppendNewPage(spellcastingContent);
                }
                previousRectangle = AddSection(previousRectangle, spellcastingContent.Spells4, spellcastingContent.SpellcastingClass, spellcastingContent.IsMulticlassSpellcaster);
                if (RequiresNewPage(previousRectangle, spellcastingContent.Spells5))
                {
                    previousRectangle = AppendNewPage(spellcastingContent);
                }
                previousRectangle = AddSection(previousRectangle, spellcastingContent.Spells5, spellcastingContent.SpellcastingClass, spellcastingContent.IsMulticlassSpellcaster);
                if (RequiresNewPage(previousRectangle, spellcastingContent.Spells6))
                {
                    previousRectangle = AppendNewPage(spellcastingContent);
                }
                previousRectangle = AddSection(previousRectangle, spellcastingContent.Spells6, spellcastingContent.SpellcastingClass, spellcastingContent.IsMulticlassSpellcaster);
                if (RequiresNewPage(previousRectangle, spellcastingContent.Spells7))
                {
                    previousRectangle = AppendNewPage(spellcastingContent);
                }
                previousRectangle = AddSection(previousRectangle, spellcastingContent.Spells7, spellcastingContent.SpellcastingClass, spellcastingContent.IsMulticlassSpellcaster);
                if (RequiresNewPage(previousRectangle, spellcastingContent.Spells8))
                {
                    previousRectangle = AppendNewPage(spellcastingContent);
                }
                previousRectangle = AddSection(previousRectangle, spellcastingContent.Spells8, spellcastingContent.SpellcastingClass, spellcastingContent.IsMulticlassSpellcaster);
                if (RequiresNewPage(previousRectangle, spellcastingContent.Spells9))
                {
                    previousRectangle = AppendNewPage(spellcastingContent);
                }
                previousRectangle = AddSection(previousRectangle, spellcastingContent.Spells9, spellcastingContent.SpellcastingClass, spellcastingContent.IsMulticlassSpellcaster);
                LatestArea = previousRectangle;
                return;
            }
            int num = 0;
            num += HeaderHeight + HeaderOffset;
            if (spellcastingContent.Cantrips.Spells.Any())
            {
                num += SectionMargin;
                num += SectionTopHeight;
                int num2 = 0;
                if (spellcastingContent.Cantrips.Spells.Count > 5)
                {
                    num2 = (spellcastingContent.Cantrips.Spells.Count - 5) / 3;
                    if ((spellcastingContent.Cantrips.Spells.Count - 5) % 3 > 0)
                    {
                        num2++;
                    }
                }
                for (int i = 0; i < num2; i++)
                {
                    num += SectionMiddleHeight;
                }
                num += SectionBottomHeight;
            }
            if ((float)num <= LatestArea.Bottom)
            {
                int num3 = (((float)num <= LatestArea.Bottom - 20f) ? 30 : HeaderOffset);
                Rectangle area = GetArea(0f, LatestArea.Bottom - (float)num3, base.PageWidth, HeaderHeight);
                Add(spellcastingContent, area);
            }
            else
            {
                NewPage();
                LatestArea = null;
                Add(spellcastingContent);
            }
        }

        private Rectangle AddSection(Rectangle previousRectangle, CharacterSheetSpellcastingPageExportContent.SpellcastingLevelExportContent spells, string className, bool isMulticlass = false)
        {
            if (!spells.Spells.Any() && spells.AvailableSlots <= 0)
            {
                return previousRectangle;
            }
            int num = Math.Max(5, spells.Spells.Count);
            int num2 = 0;
            if (num > 5)
            {
                num2 = (num - 5) / 3;
                if ((num - 5) % 3 > 0)
                {
                    num2++;
                }
            }
            FillableContentGenerator fillableContentGenerator = new FillableContentGenerator(base.Writer);
            Rectangle areaUnder = GetAreaUnder(previousRectangle, 24f, (Math.Abs(previousRectangle.Height - 100f) < 1f) ? 10 : SectionMargin);
            _topReader = CharacterSheetResources.PartialSpellcastingTop(spells.Level).CreateReader();
            PlacePage(_topReader, areaUnder);
            int num3 = 0;
            Rectangle area = GetArea(areaUnder.Left + (float)base.PageMargin + 20f, areaUnder.Top - 13f, 154f, 10f);
            Rectangle area2 = GetArea(area.Right + (float)base.PageGutter + 26f, area.Top, 154f, 10f);
            Rectangle area3 = GetArea(area2.Right + (float)base.PageGutter + 26f, area.Top, 154f, 10f);
            float num4 = area.Right - 3.2f;
            float num5 = area.Bottom + 5f;
            Rectangle area4 = GetArea(num4 - 4f, num5 + 5f, 8f, 10f);
            for (int i = 0; i < spells.AvailableSlots; i++)
            {
                base.Writer.DirectContentUnder.SetColorFill(BaseColor.WHITE);
                base.Writer.DirectContentUnder.Circle(num4, num5, 3f);
                base.Writer.DirectContentUnder.Fill();
                area4 = GetArea(num4 - 4f, num5 + 5f, 8f, 10f);
                fillableContentGenerator.AddCheck(area4, GenerateCurrentFieldName($"slot{i}_{spells.Level}:{num3}_", isMulticlass ? "multiclass" : className));
                num4 -= 10f;
            }
            if (spells.AvailableSlots > 0)
            {
                fillableContentGenerator.SetBold();
                fillableContentGenerator.SetColor(BaseColor.WHITE);
                string text = GenerateCurrentFieldName($"slots_{spells.Level}:{num3}_", className);
                fillableContentGenerator.AddText(GetArea(area.Left + 35f, area.Top, 40f, area.Height), text, $"{spells.AvailableSlots} SPELL SLOTS", 5f);
                base.PartialFlatteningNames.Add(text);
                fillableContentGenerator.ResetColor();
                fillableContentGenerator.SetDefault();
            }
            fillableContentGenerator.AddText(area2, GenerateCurrentFieldName($"spells_{spells.Level}:{num3}_", className), spells.Get(num3)?.ToString(), 7f);
            if (spells.Level != 0)
            {
                fillableContentGenerator.AddCheck(GetAreaLeftOf(area2, 8f, 5f), GenerateCurrentFieldName($"prepare_{spells.Level}:{num3}_", className), spells.Get(num3));
            }
            num3++;
            fillableContentGenerator.AddText(area3, GenerateCurrentFieldName($"spells_{spells.Level}:{num3}_", className), spells.Get(num3)?.ToString(), 7f);
            if (spells.Level != 0)
            {
                fillableContentGenerator.AddCheck(GetAreaLeftOf(area3, 8f, 5f), GenerateCurrentFieldName($"prepare_{spells.Level}:{num3}_", className), spells.Get(num3));
            }
            num3++;
            Rectangle rectangle = areaUnder;
            for (int j = 0; j < num2; j++)
            {
                rectangle = GetAreaUnder(rectangle, 12f, 0f);
                PlacePage(_middleReader, rectangle);
                area = GetArea(rectangle.Left + (float)base.PageMargin + 20f, rectangle.Top - 1f, 154f, 10f);
                area2 = GetArea(area.Right + (float)base.PageGutter + 26f, area.Top, 154f, 10f);
                area3 = GetArea(area2.Right + (float)base.PageGutter + 26f, area.Top, 154f, 10f);
                fillableContentGenerator.AddText(area, GenerateCurrentFieldName($"spells_{spells.Level}:{num3}_", className), spells.Get(num3)?.ToString(), 7f);
                if (spells.Level != 0)
                {
                    fillableContentGenerator.AddCheck(GetAreaLeftOf(area, 8f, 4.5f), GenerateCurrentFieldName($"prepare_{spells.Level}:{num3}_", className), spells.Get(num3));
                }
                num3++;
                fillableContentGenerator.AddText(area2, GenerateCurrentFieldName($"spells_{spells.Level}:{num3}_", className), spells.Get(num3)?.ToString(), 7f);
                if (spells.Level != 0)
                {
                    fillableContentGenerator.AddCheck(GetAreaLeftOf(area2, 8f, 5f), GenerateCurrentFieldName($"prepare_{spells.Level}:{num3}_", className), spells.Get(num3));
                }
                num3++;
                fillableContentGenerator.AddText(area3, GenerateCurrentFieldName($"spells_{spells.Level}:{num3}_", className), spells.Get(num3)?.ToString(), 7f);
                if (spells.Level != 0)
                {
                    fillableContentGenerator.AddCheck(GetAreaLeftOf(area3, 8f, 5f), GenerateCurrentFieldName($"prepare_{spells.Level}:{num3}_", className), spells.Get(num3));
                }
                num3++;
            }
            Rectangle areaUnder2 = GetAreaUnder(rectangle, 24f, 0f);
            PlacePage(_bottomReader, areaUnder2);
            area = GetArea(areaUnder2.Left + (float)base.PageMargin + 20f, areaUnder2.Top - 1f, 154f, 10f);
            area2 = GetArea(area.Right + (float)base.PageGutter + 26f, area.Top, 154f, 10f);
            area3 = GetArea(area2.Right + (float)base.PageGutter + 26f, area.Top, 154f, 10f);
            fillableContentGenerator.AddText(area, GenerateCurrentFieldName($"spells_{spells.Level}:{num3}_", className), spells.Get(num3)?.ToString(), 7f);
            if (spells.Level != 0)
            {
                fillableContentGenerator.AddCheck(GetAreaLeftOf(area, 8f, 4.5f), GenerateCurrentFieldName($"prepare_{spells.Level}:{num3}_", className), spells.Get(num3));
            }
            num3++;
            fillableContentGenerator.AddText(area2, GenerateCurrentFieldName($"spells_{spells.Level}:{num3}_", className), spells.Get(num3)?.ToString(), 7f);
            if (spells.Level != 0)
            {
                fillableContentGenerator.AddCheck(GetAreaLeftOf(area2, 8f, 5f), GenerateCurrentFieldName($"prepare_{spells.Level}:{num3}_", className), spells.Get(num3));
            }
            num3++;
            fillableContentGenerator.AddText(area3, GenerateCurrentFieldName($"spells_{spells.Level}:{num3}_", className), spells.Get(num3)?.ToString(), 7f);
            if (spells.Level != 0)
            {
                fillableContentGenerator.AddCheck(GetAreaLeftOf(area3, 8f, 5f), GenerateCurrentFieldName($"prepare_{spells.Level}:{num3}_", className), spells.Get(num3));
            }
            return areaUnder2;
        }

        private void AddHeaderFields(Rectangle area, CharacterSheetSpellcastingPageExportContent content)
        {
            content.SpellcastingClass.ToLower().Replace(" ", "_");
            FillableContentGenerator fillableContentGenerator = new FillableContentGenerator(base.Writer);
            Rectangle area2 = GetArea(area.Left + (float)base.PageMargin + 40f, area.Top - 59f, 170f, 20f);
            fillableContentGenerator.AddText(area2, GenerateCurrentFieldName("spellcasting_class", content.SpellcastingClass), content.ToString(), 10f, 1);
            Rectangle area3 = GetArea(area.Left + (float)base.PageMargin + 267f, area.Top - 32f, 49f, 16f);
            Rectangle area4 = GetArea(area3.Left + 75f, area3.Top, area3.Width, area3.Height);
            Rectangle area5 = GetArea(area4.Left + 75f, area3.Top, area3.Width, area3.Height);
            Rectangle area6 = GetArea(area5.Left + 75f, area3.Top, area3.Width, area3.Height);
            fillableContentGenerator.AddText(area3, GenerateCurrentFieldName("spellcasting_ability", content.SpellcastingClass), content.Ability, 0f, 1);
            fillableContentGenerator.AddText(area4, GenerateCurrentFieldName("spellcasting_bonus", content.SpellcastingClass), content.AttackBonus, 0f, 1);
            fillableContentGenerator.AddText(area5, GenerateCurrentFieldName("spellcasting_save", content.SpellcastingClass), content.Save, 0f, 1);
            fillableContentGenerator.AddText(area6, GenerateCurrentFieldName("spellcasting_prepare", content.SpellcastingClass), content.PrepareCount, 0f, 1);
        }

        private void NewPage()
        {
            base.Document.NewPage();
            base.PageFollowNumber++;
            LatestArea = null;
        }

        private Rectangle AppendNewPage(CharacterSheetSpellcastingPageExportContent content)
        {
            base.Document.NewPage();
            base.PageFollowNumber++;
            LatestArea = GetArea(0f, base.PageHeight - HeaderOffset, base.PageWidth, HeaderHeight);
            PlacePage(_headerReader, LatestArea);
            AddHeaderFields(LatestArea, content);
            return LatestArea;
        }

        private float CalculateRequireSectionHeight(Rectangle previousRectangle, CharacterSheetSpellcastingPageExportContent.SpellcastingLevelExportContent spells)
        {
            if (!spells.Spells.Any())
            {
                return 0f;
            }
            float bottom = previousRectangle.Bottom;
            int num = Math.Max(5, spells.Spells.Count);
            int num2 = 0;
            if (num > 5)
            {
                num2 = (num - 5) / 3;
                if ((num - 5) % 3 > 0)
                {
                    num2++;
                }
            }
            new FillableContentGenerator(base.Writer);
            Rectangle areaUnder = GetAreaUnder(previousRectangle, 24f, (Math.Abs(previousRectangle.Height - 100f) < 1f) ? 10 : SectionMargin);
            for (int i = 0; i < num2; i++)
            {
                areaUnder = GetAreaUnder(areaUnder, 12f, 0f);
            }
            float bottom2 = GetAreaUnder(areaUnder, 24f, 0f).Bottom;
            return bottom - bottom2;
        }

        private bool RequiresNewPage(Rectangle previousRectangle, CharacterSheetSpellcastingPageExportContent.SpellcastingLevelExportContent spells)
        {
            float bottom = previousRectangle.Bottom;
            float num = CalculateRequireSectionHeight(previousRectangle, spells);
            if (bottom - num <= (float)base.PageMargin)
            {
                return true;
            }
            return false;
        }

        private string GenerateCurrentFieldName(string name, string className)
        {
            return $"{name}:{className}:{base.PageFollowNumber}";
        }

        private Rectangle GetArea(float x, float y, float width, float height)
        {
            return new Rectangle(x, y - height, x + width, y);
        }

        private Rectangle GetAreaUnder(Rectangle area, float height, float offset)
        {
            return new Rectangle(area.Left, area.Bottom - offset - height, area.Right, area.Bottom - offset);
        }

        private Rectangle GetAreaLeftOf(Rectangle area, float width, float offset)
        {
            return new Rectangle(area.Left - offset - width, area.Bottom, area.Left - offset, area.Top);
        }

        public override void Dispose()
        {
            base.Dispose();
            _headerReader?.Dispose();
            _topReader?.Dispose();
            _middleReader?.Dispose();
            _bottomReader?.Dispose();
        }
    }
}
