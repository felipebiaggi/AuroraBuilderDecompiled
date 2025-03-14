using Builder.Data;

namespace Builder.Presentation.ViewModels
{
    public sealed class EquipmentElementDescriptionDisplayRequestEvent : ElementDescriptionDisplayRequestEvent
    {
        public ElementBase Parent { get; }

        public EquipmentElementDescriptionDisplayRequestEvent(ElementBase element, ElementBase parent = null, string stylesheet = null)
            : base(element, stylesheet)
        {
            if (parent != null && element != null && !parent.Id.Equals(element.Id))
            {
                Parent = parent;
            }
        }
    }
}
