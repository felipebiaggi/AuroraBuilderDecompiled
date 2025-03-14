using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Builder.Core.Logging;
using Builder.Data.Elements;
using Builder.Presentation.Models.Sheet;
using Builder.Presentation.Services.Data;
using Builder.Presentation.ViewModels;
using Builder.Presentation.ViewModels.Shell.Items;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;

namespace Builder.Presentation.Models.Sheet
{
    public class Itemcard
    {
        private readonly string _fieldname;

        private readonly RefactoredEquipmentItem _item;

        public Itemcard(string fieldname, RefactoredEquipmentItem item)
        {
            _fieldname = fieldname;
            _item = item;
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
            FontFactory.GetFont("Helvetica-Bold", 6f);
            FontFactory.GetFont("Helvetica-BoldOblique", 6f);
            Font font2 = FontFactory.GetFont("Helvetica-Bold", 9f);
            Font font3 = FontFactory.GetFont("Helvetica-Oblique", 6f);
            Phrase phrase = new Phrase();
            phrase.Font = font;
            Chunk chunk = new Chunk(string.IsNullOrWhiteSpace(_item.AlternativeName) ? (_item.DisplayName + Environment.NewLine) : (_item.AlternativeName + Environment.NewLine));
            chunk.Font = font2;
            chunk.setLineHeight(10f);
            phrase.Add(chunk);
            if (_item.IsAdorned || _item.Item.Type.Equals("Magic Item"))
            {
                StringBuilder stringBuilder = new StringBuilder();
                Item item = (_item.IsAdorned ? _item.AdornerItem : _item.Item);
                if (!string.IsNullOrWhiteSpace(item.ItemType))
                {
                    string setterAdditionAttribute = item.GetSetterAdditionAttribute("type");
                    stringBuilder.Append((setterAdditionAttribute != null) ? (item.ItemType + " (" + setterAdditionAttribute + "), ") : (item.ItemType + ", "));
                }
                else
                {
                    stringBuilder.Append("Magic item, ");
                }
                if (!string.IsNullOrWhiteSpace(item.Rarity))
                {
                    stringBuilder.Append(item.Rarity.ToLower() + " ");
                }
                if (item.RequiresAttunement)
                {
                    string setterAdditionAttribute2 = item.GetSetterAdditionAttribute("attunement");
                    stringBuilder.Append((setterAdditionAttribute2 != null) ? ("(requires attunement " + setterAdditionAttribute2 + ")") : "(requires attunement)");
                }
                phrase.Add(new Chunk(stringBuilder.ToString() + Environment.NewLine)
                {
                    Font = font3
                });
                phrase.Add(new Chunk(Environment.NewLine));
            }
            else if (_item.IsAdorned || (!_item.Item.Type.Equals("Weapon") && !_item.Item.Type.Equals("Armor")))
            {
                phrase.Add(new Chunk(_item.Item.Category + Environment.NewLine)
                {
                    Font = font3
                });
                phrase.Add(new Chunk(Environment.NewLine));
            }
            List<IElement> list = HTMLWorker.ParseToList(new StringReader(_item.IsAdorned ? _item.AdornerItem.Description : _item.Item.Description), null);
            if (!_item.IsAdorned && (_item.Item.Type.Equals("Weapon") || _item.Item.Type.Equals("Armor")))
            {
                list = HTMLWorker.ParseToList(new StringReader(DescriptionPanelViewModelBase.GenerateHeaderForCard(_item.Item)), null);
            }
            foreach (IElement item2 in list)
            {
                StringBuilder stringBuilder2 = new StringBuilder();
                foreach (Chunk chunk2 in item2.Chunks)
                {
                    if (item2.GetType() == typeof(List))
                    {
                        stringBuilder2.Append((item2 as List).Symbol);
                        stringBuilder2.AppendLine(chunk2.Content);
                    }
                    else
                    {
                        stringBuilder2.Append(chunk2.Content);
                    }
                }
                Chunk element = new Chunk(stringBuilder2.ToString() + Environment.NewLine)
                {
                    Font = font
                };
                phrase.Add(element);
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
