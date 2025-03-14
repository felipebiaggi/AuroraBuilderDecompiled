namespace Builder.Presentation.ViewModels
{
    public class BuildElementDescriptionPanelViewModel : DescriptionPanelViewModelBase
    {
        public override void OnHandleEvent(ElementDescriptionDisplayRequestEvent args)
        {
            base.CurrentElement = args.Element;
            if (base.CurrentElement != null)
            {
                switch (args.Element.Type)
                {
                    case "Item":
                        return;
                    case "Item Pack":
                        return;
                    case "Magic Item":
                        return;
                    case "Spell":
                        return;
                }
                base.OnHandleEvent(args);
            }
        }
    }
}
