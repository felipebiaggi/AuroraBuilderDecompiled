namespace Builder.Presentation.Models.CharacterSheet.Pages.Content
{
    public class GenericCardContent
    {
        public string Title { get; set; }

        public string Subtitle { get; set; }

        public string Description { get; set; }

        public string LeftFooter { get; set; }

        public string RightFooter { get; set; }

        public string DescriptionHtml { get; set; }

        public GenericCardContent(string title, string subtitle, string description = "", string left = "", string right = "")
        {
            Title = title;
            Subtitle = subtitle;
            Description = description;
            LeftFooter = left;
            RightFooter = right;
            DescriptionHtml = "";
        }
    }
}
