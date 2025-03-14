using Builder.Core.Events;
using Builder.Data;
using Builder.Presentation.Events.Application;
using System.Text;

namespace Builder.Presentation.ViewModels
{
    public class EquipmentElementDescriptionPanelViewModel : DescriptionPanelViewModelBase, ISubscriber<EquipmentElementDescriptionDisplayRequestEvent>
    {
        private ElementBase _parent;

        public EquipmentElementDescriptionPanelViewModel()
        {
            base.SupportedTypes.Add("Item");
            base.SupportedTypes.Add("Item Pack");
            base.SupportedTypes.Add("Magic Item");
            base.SupportedTypes.Add("Armor");
            base.SupportedTypes.Add("Weapon");
        }

        public void OnHandleEvent(EquipmentElementDescriptionDisplayRequestEvent args)
        {
            _parent = args.Parent;
            base.HandleDisplayRequest(args);
        }

        protected override void AppendBeforeSource(StringBuilder descriptionBuilder, ElementBase currentElement)
        {
            string.IsNullOrWhiteSpace(_parent?.Description);
        }

        public override void OnHandleEvent(ElementDescriptionDisplayRequestEvent args)
        {
        }

        public override void OnHandleEvent(HtmlDisplayRequestEvent args)
        {
        }

        public override void OnHandleEvent(ResourceDocumentDisplayRequestEvent args)
        {
        }
    }
}
