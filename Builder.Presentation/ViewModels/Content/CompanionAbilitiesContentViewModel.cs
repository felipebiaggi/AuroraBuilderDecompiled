using Builder.Presentation.Models.Collections;

namespace Builder.Presentation.ViewModels.Content
{
    public class CompanionAbilitiesContentViewModel : AbilitiesContentViewModel
    {
        public override AbilitiesCollection Abilities => CharacterManager.Current.Character.Companion.Abilities;

        public CompanionAbilitiesContentViewModel()
        {
            base.IsPointsGeneration = false;
            base.IsRandomizeGeneration = false;
        }
    }
}
