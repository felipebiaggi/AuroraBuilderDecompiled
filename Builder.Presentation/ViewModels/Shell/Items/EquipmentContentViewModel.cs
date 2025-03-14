using Builder.Presentation.ViewModels.Base;

namespace Builder.Presentation.ViewModels.Shell.Items
{
    public sealed class EquipmentContentViewModel : ViewModelBase
    {
        public EquipmentContentViewModel()
        {
            if (!base.IsInDesignMode)
            {
                base.EventAggregator.Subscribe(this);
            }
        }
    }
}
