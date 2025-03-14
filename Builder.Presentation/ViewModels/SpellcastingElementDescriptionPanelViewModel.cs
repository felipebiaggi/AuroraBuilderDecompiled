using Builder.Core.Events;
using Builder.Presentation.Events.Application;

namespace Builder.Presentation.ViewModels
{
    public class SpellcastingElementDescriptionPanelViewModel : DescriptionPanelViewModelBase, ISubscriber<SpellcastingElementDescriptionDisplayRequestEvent>
    {
        public SpellcastingElementDescriptionPanelViewModel()
        {
            base.SupportedTypes.Add("Spell");
        }

        public void OnHandleEvent(SpellcastingElementDescriptionDisplayRequestEvent args)
        {
            base.HandleDisplayRequest(args);
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
