namespace Builder.Presentation.ViewModels
{
    public static class DynamicSupportExpressions
    {
        public const string SpellcastingList = "$(spellcasting:list)";

        public const string SpellcastingSlots = "$(spellcasting:slots)";

        public static string[] All => new string[2] { "$(spellcasting:list)", "$(spellcasting:slots)" };
    }
}
