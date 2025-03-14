namespace Builder.Presentation.Models.CharacterSheet.Pages.Content
{
    public class SpellCardContent : GenericCardContent
    {
        public string CastingTime { get; set; }

        public string Range { get; set; }

        public string Duration { get; set; }

        public string Components { get; set; }

        public SpellCardContent(string title, string subtitle, string description = "", string left = "", string right = "")
            : base(title, subtitle, description, left, right)
        {
        }
    }
}
