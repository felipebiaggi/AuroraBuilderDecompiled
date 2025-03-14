using Builder.Presentation.Models;
using Builder.Presentation.Models.Equipment;
using Builder.Presentation.ViewModels.Base;

namespace Builder.Presentation.ViewModels.Shell.Items
{
    public sealed class ItemsSectionViewModel : ViewModelBase
    {
        public Character Character => CharacterManager.Current.Character;

        public CharacterInventory Inventory => Character.Inventory;

        public RefactoredEquipmentSectionViewModel RefactoredEquipmentSectionViewModel { get; } = new RefactoredEquipmentSectionViewModel();

        public ItemsSectionViewModel()
        {
            if (base.IsInDesignMode)
            {
                InitializeDesignData();
            }
            else
            {
                SubscribeWithEventAggregator();
            }
        }

        protected override void InitializeDesignData()
        {
            Inventory.Coins.Set(13L, 49L, 11L, 8L, 4L);
        }
    }
}
