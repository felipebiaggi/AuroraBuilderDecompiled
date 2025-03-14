namespace Builder.Presentation.Models.CharacterSheet.Pages.Content
{
    public interface IPageContentWriter
    {
        void Write(string key, string content);

        void Write<T>(T item) where T : IPageContentItem;
    }

}
