namespace Builder.Presentation.Models.CharacterSheet.Pages.Content
{
    public class AttackCardContent : GenericCardContent
    {
        public string Range { get; set; }

        public string Attack { get; set; }

        public string Damage { get; set; }

        public AttackCardContent(string title, string subtitle, string description = "", string left = "", string right = "")
            : base(title, subtitle, description, left, right)
        {
        }
    }
}
