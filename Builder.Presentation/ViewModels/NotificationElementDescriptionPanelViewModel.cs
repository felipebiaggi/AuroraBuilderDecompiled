using Builder.Core.Events;
using Builder.Core.Logging;
using Builder.Presentation.Events.Application;
using Builder.Presentation.Properties;
using System;
using System.Net.Http;

namespace Builder.Presentation.ViewModels
{
    public class NotificationElementDescriptionPanelViewModel : DescriptionPanelViewModelBase, ISubscriber<NotificationElementDescriptionDisplayRequestEvent>
    {
        public NotificationElementDescriptionPanelViewModel()
        {
            base.IsSpeechEnabled = false;
            base.IsDarkStyle = true;
            if (!base.IsInDesignMode)
            {
                DownloadUpdateNotificationContent(Resources.NotificationUrl);
            }
        }

        private async void DownloadUpdateNotificationContent(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HtmlDisplayRequestEvent args = new HtmlDisplayRequestEvent(await client.GetStringAsync(url));
                    base.HandleHtmlDisplayRequest(args);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "DownloadUpdateNotificationContent");
            }
        }

        public void OnHandleEvent(NotificationElementDescriptionDisplayRequestEvent args)
        {
            base.HandleHtmlDisplayRequest(args);
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
