using System.Collections.Generic;

namespace Builder.Presentation.Models.CharacterSheet.Pages
{
    public class GenericCardsPage
    {
        public class GenericCardContent
        {
            public string Title { get; set; } = string.Empty;

            public string Subtitle { get; set; } = string.Empty;

            public string Description { get; set; } = string.Empty;

            public string LeftFooter { get; set; } = string.Empty;

            public string RightFooter { get; set; } = string.Empty;
        }

        public Dictionary<int, GenericCardContent> Cards { get; } = new Dictionary<int, GenericCardContent>();

        public void PopulateCard(int index, GenericCardContent content)
        {
            if (Cards.ContainsKey(index))
            {
                Cards[index] = content;
            }
            else
            {
                Cards.Add(index, content);
            }
        }
    }
}
