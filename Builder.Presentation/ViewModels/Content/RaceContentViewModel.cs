using System.Collections.Generic;

namespace Builder.Presentation.ViewModels.Content
{
    public class RaceContentViewModel : SupportExpanderViewModel
    {
        public RaceContentViewModel()
            : base(new string[6] { "Race", "Race Variant", "Sub Race", "Racial Trait", "Dragonmark", "Dragonmark Feature" })
        {
            base.Listings = new List<string> { "Race", "Race Variant", "Sub Race", "Racial Trait", "Dragonmark", "Dragonmark Feature" };
        }
    }
}
