using Builder.Core.Events;
using Builder.Presentation.Events.Application;

namespace Builder.Presentation.ViewModels
{
    public class CompendiumElementDescriptionPanelViewModel : DescriptionPanelViewModelBase, ISubscriber<CompendiumElementDescriptionDisplayRequestEvent>
    {
        public void OnHandleEvent(CompendiumElementDescriptionDisplayRequestEvent args)
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
