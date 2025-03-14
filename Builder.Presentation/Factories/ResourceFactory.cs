using Builder.Presentation.Models.CharacterSheet;

namespace Builder.Presentation.Factories
{
    public static class ResourceFactory
    {
        public static CharacterSheetResourcePage CreateCharacterSheetResourcePage(string resourcePath)
        {
            return new CharacterSheetResourcePage(resourcePath);
        }
    }
}
