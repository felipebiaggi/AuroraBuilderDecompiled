using Builder.Presentation;
using Builder.Presentation.Models;
using Builder.Presentation.Models.Collections;
using Builder.Presentation.ViewModels.Base;

namespace Builder.Presentation.ViewModels
{
    public class CharacterOverviewViewModel : ViewModelBase
    {
        public Character Character => CharacterManager.Current.Character;

        public SkillsCollection Skills => CharacterManager.Current.Character.Skills;

        public CharacterOverviewViewModel()
        {
            SubscribeWithEventAggregator();
        }
    }
}
