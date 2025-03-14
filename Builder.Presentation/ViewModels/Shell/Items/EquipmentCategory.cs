using Builder.Data;

namespace Builder.Presentation.ViewModels.Shell.Items
{
    public class EquipmentCategory
    {
        public string DisplayName { get; }

        public bool IsEnabled { get; set; }

        public ElementBaseCollection Items { get; } = new ElementBaseCollection();

        public int EnabledItemCount { get; set; }

        public EquipmentCategory(string displayName)
        {
            DisplayName = displayName;
            IsEnabled = true;
        }

        public override string ToString()
        {
            return DisplayName ?? "";
        }
    }
}
