using Builder.Core.Events;
using Builder.Data;
using Builder.Data.Elements;
using Builder.Presentation.Events.Application;
using System.Text;

namespace Builder.Presentation.ViewModels
{
    public class SourceElementDescriptionPanelViewModel : DescriptionPanelViewModelBase, ISubscriber<SourceElementDescriptionDisplayRequestEvent>
    {
        public SourceElementDescriptionPanelViewModel()
        {
            base.IncludeSource = false;
            base.SupportedTypes.Add("Source");
        }

        public void OnHandleEvent(SourceElementDescriptionDisplayRequestEvent args)
        {
            args.IgnoreGeneratedDescription = true;
            base.HandleDisplayRequest(args);
        }

        protected override string GenerateHeader(ElementBase element)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder stringBuilder2 = new StringBuilder();
            if (element is Source source && source.HasInformationMessage)
            {
                stringBuilder.Append("<p class=\"underline\"><em class=\"info\">" + source.InformationMessage + "</em>");
                if (source.HasInformationUrl)
                {
                    stringBuilder.Append(" - <a href=\"" + source.InformationUrl + "\" style=\"color:#3aa6ae\">Learn More...</a>");
                }
                stringBuilder.Append("</p>");
            }
            if (element is Source source2)
            {
                if (source2.IsIncomplete)
                {
                    if (!string.IsNullOrWhiteSpace(stringBuilder2.ToString()))
                    {
                        stringBuilder2.Append(" - ");
                    }
                    stringBuilder2.Append("<strong><em class=\"danger\">Incomplete Source</em></strong>");
                }
                if (source2.IsWorkInProgress)
                {
                    if (!string.IsNullOrWhiteSpace(stringBuilder2.ToString()))
                    {
                        stringBuilder2.Append(" - ");
                    }
                    stringBuilder2.Append("<strong><em class=\"danger\">Work in Progress</em></strong>");
                }
                if (source2.IsPlaytestContent)
                {
                    if (!string.IsNullOrWhiteSpace(stringBuilder2.ToString()))
                    {
                        stringBuilder2.Append(" - ");
                    }
                    stringBuilder2.Append("<strong><em class=\"warning\">Playtest Material</em></strong>");
                }
            }
            if (string.IsNullOrWhiteSpace(stringBuilder.ToString()) && string.IsNullOrWhiteSpace(stringBuilder2.ToString()))
            {
                return string.Empty;
            }
            string text = "";
            if (!string.IsNullOrWhiteSpace(stringBuilder.ToString()))
            {
                text += stringBuilder.ToString();
            }
            if (!string.IsNullOrWhiteSpace(stringBuilder2.ToString()))
            {
                text += $"<p class=\"underline\">{stringBuilder2}</p>";
            }
            return text;
        }

        protected override void AppendBeforeSource(StringBuilder descriptionBuilder, ElementBase currentElement)
        {
            if (currentElement is Source source)
            {
                descriptionBuilder.Append(source.HasSourceUrl ? ("<h6>SOURCE</h6><p><em><a href=\"" + source.Url + "\">" + source.Name + "</a></em></p>") : ("<h6>SOURCE</h6><p><em>" + source.Name + "</em></p>"));
                if (source.HasErrataUrl)
                {
                    descriptionBuilder.Append("<h6>ERRATA</h6><p><em><a href=\"" + source.ErrataUrl + "\">" + source.Name + " (Errata)</a></em></p>");
                }
                descriptionBuilder.Append(source.HasAuthorUrl ? ("<h6>AUTHOR</h6><p><em><a href=\"" + source.AuthorUrl + "\">" + source.Author + "</a></em></p>") : ("<h6>AUTHOR</h6><p><em>" + source.Author + "</em></p>"));
            }
        }

        protected override void AppendAfterSource(StringBuilder descriptionBuilder, ElementBase currentElement)
        {
            if (currentElement is Source source && source.HasImageUrl)
            {
                descriptionBuilder.Append("<br><br><center style=\"height: 300px;max-height: 300px;max-width: 350px;\"><img src=\"" + source.ImageUrl + "\" style=\"height: 300px;max-height: 300px;max-width: 350px;\"></center><br><br>");
            }
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
