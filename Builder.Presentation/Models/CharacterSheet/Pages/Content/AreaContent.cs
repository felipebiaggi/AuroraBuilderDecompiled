using System.Collections.Generic;

namespace Builder.Presentation.Models.CharacterSheet.Pages.Content
{
    public class AreaContent : PageContentItem<List<string>>
    {
        public AreaContent(string key, List<string> content)
            : base(key, content)
        {
        }
    }

}
