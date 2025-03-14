using Builder.Presentation.Models.Collections;
using Builder.Presentation.ViewModels.Base;

namespace Builder.Presentation.ViewModels.Content
{
    public sealed class SavingThrowsContentViewModel : ViewModelBase
    {
        public SavingThrowCollection SavingThrows => CharacterManager.Current.Character.SavingThrows;

        public SavingThrowsContentViewModel()
        {
            if (base.IsInDesignMode)
            {
                InitializeDesignData();
            }
            else
            {
                base.EventAggregator.Subscribe(this);
            }
        }

        protected override void InitializeDesignData()
        {
            base.InitializeDesignData();
            SavingThrows.Dexterity.MiscBonus = 2;
            SavingThrows.Dexterity.ProficiencyBonus = 2;
            SavingThrows.Charisma.ProficiencyBonus = 2;
        }
    }
}
