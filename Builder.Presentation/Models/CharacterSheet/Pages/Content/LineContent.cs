namespace Builder.Presentation.Models.CharacterSheet.Pages.Content
{
    public class LineContent : PageContentItem<string>
    {
        public float Fontsize { get; }

        public LineContent(string key, string content, float fontsize = 0f)
            : base(key, content)
        {
            Fontsize = fontsize;
        }
    }

}
