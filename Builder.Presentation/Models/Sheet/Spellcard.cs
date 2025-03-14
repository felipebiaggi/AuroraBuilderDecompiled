using System;
using System.IO;
using System.Linq;
using System.Text;
using Builder.Core.Logging;
using Builder.Data.Elements;
using Builder.Presentation.Models.Sheet;
using Builder.Presentation.Services.Data;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;

namespace Builder.Presentation.Models.Sheet
{
    public class Spellcard
    {
        private readonly string _fieldname;

        private readonly Spell _spell;

        public Spellcard(string fieldname, Spell spell)
        {
            _fieldname = fieldname;
            _spell = spell;
        }

        public void Stamp(PdfStamper stamper, Rectangle pageSize, int page, CardPosition position)
        {
            Rectangle cardRectangle = GetCardRectangle(pageSize, position);
            try
            {
                Image instance = Image.GetInstance(Path.Combine(DataManager.Current.LocalAppDataRootDirectory, "spellcard-background.jpg"));
                instance.SetAbsolutePosition(cardRectangle.Left, cardRectangle.Top - cardRectangle.Height);
                instance.ScaleToFit(cardRectangle.Width, cardRectangle.Height);
                stamper.GetUnderContent(page).AddImage(instance);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Stamp");
            }
            Font font = FontFactory.GetFont("Helvetica", 6f);
            Font font2 = FontFactory.GetFont("Helvetica-Bold", 6f);
            Font font3 = FontFactory.GetFont("Helvetica-BoldOblique", 6f);
            Font font4 = FontFactory.GetFont("Helvetica-Bold", 9f);
            Font font5 = FontFactory.GetFont("Helvetica-Oblique", 6f);
            Phrase phrase = new Phrase();
            phrase.Font = font;
            Chunk chunk = new Chunk(_spell.Name + Environment.NewLine);
            chunk.Font = font4;
            chunk.setLineHeight(10f);
            phrase.Add(chunk);
            phrase.Add(new Chunk(_spell.Underline + Environment.NewLine + Environment.NewLine)
            {
                Font = font5
            });
            phrase.Add(new Chunk("Casting Time: ")
            {
                Font = font2
            });
            phrase.Add(new Chunk(_spell.CastingTime + Environment.NewLine));
            phrase.Add(new Chunk("Range: ")
            {
                Font = font2
            });
            phrase.Add(new Chunk(_spell.Range + Environment.NewLine));
            phrase.Add(new Chunk("Components: ")
            {
                Font = font2
            });
            phrase.Add(new Chunk(_spell.GetComponentsString() + Environment.NewLine));
            phrase.Add(new Chunk("Duration: ")
            {
                Font = font2
            });
            phrase.Add(new Chunk(_spell.Duration + Environment.NewLine + Environment.NewLine));
            new MemoryStream(Encoding.UTF8.GetBytes(_spell.Description));
            foreach (IElement item in HTMLWorker.ParseToList(new StringReader(_spell.Description), null))
            {
                _spell.Name.Contains("Enhance Ability");
                foreach (Chunk chunk2 in item.Chunks)
                {
                    chunk2.Font = font;
                    if (chunk2.Content.ToLower().Contains("at higher level"))
                    {
                        chunk2.Font = font3;
                    }
                    phrase.Add(chunk2);
                    if (chunk2.Content != "\n")
                    {
                        phrase.Add(new Chunk(Environment.NewLine));
                    }
                }
                phrase.Add(new Chunk(Environment.NewLine));
            }
            foreach (Chunk chunk3 in phrase.Chunks)
            {
                if (phrase.Chunks.First() != chunk3)
                {
                    chunk3.setLineHeight(8f);
                }
            }
            ColumnText columnText = new ColumnText(stamper.GetOverContent(page));
            columnText.SetSimpleColumn(GetCardRectangle(pageSize, position, 2));
            columnText.AddText(phrase);
            columnText.Go();
        }

        private Rectangle GetCardRectangle(Rectangle pageSize, CardPosition position, int padding = 0)
        {
            int num = (int)((pageSize.Width - 80f) / 3f);
            int num2 = (int)((pageSize.Height - 80f) / 3f);
            int num3 = 20;
            int num4 = 20;
            switch (position)
            {
                case CardPosition.UpperLeft:
                    num4 += 20 + num2;
                    num4 += 20 + num2;
                    break;
                case CardPosition.UpperCenter:
                    num3 += 20 + num;
                    num4 += 20 + num2;
                    num4 += 20 + num2;
                    break;
                case CardPosition.UpperRight:
                    num3 += 20 + num;
                    num3 += 20 + num;
                    num4 += 20 + num2;
                    num4 += 20 + num2;
                    break;
                case CardPosition.CenterLeft:
                    num4 += 20 + num2;
                    break;
                case CardPosition.CenterCenter:
                    num3 += 20 + num;
                    num4 += 20 + num2;
                    break;
                case CardPosition.CenterRight:
                    num3 += 20 + num;
                    num3 += 20 + num;
                    num4 += 20 + num2;
                    break;
                case CardPosition.BottomCenter:
                    num3 += 20 + num;
                    break;
                case CardPosition.BottomRight:
                    num3 += 20 + num;
                    num3 += 20 + num;
                    break;
            }
            return new Rectangle(num3 + padding, num4 + padding, num3 + num - padding * 2, num4 + num2 - padding * 2);
        }
    }
}
