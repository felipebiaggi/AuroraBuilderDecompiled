using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Builder.Presentation.Telemetry
{
    public static class AnalyticsErrorHelper
    {
        private static ErrorAttachmentLog CreateAttachment(string content, string filename = "attachment.txt")
        {
            return ErrorAttachmentLog.AttachmentWithText(content, filename);
        }

        public static Dictionary<string, string> CreateProperties(string key = null, string value = null)
        {
            if (key == null || value == null)
            {
                return new Dictionary<string, string>();
            }
            return new Dictionary<string, string> { { key, value } };
        }

        public static void Exception(Exception ex, Dictionary<string, string> additionalProperties = null, string attachmentContent = null, [CallerMemberName] string method = "", [CallerLineNumber] int line = 0)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>
        {
            { "method", method },
            {
                "line",
                line.ToString()
            }
        };
            if (additionalProperties != null)
            {
                foreach (KeyValuePair<string, string> additionalProperty in additionalProperties)
                {
                    dictionary.Add(additionalProperty.Key, additionalProperty.Value);
                }
            }
            if (string.IsNullOrWhiteSpace(attachmentContent))
            {
                Crashes.TrackError(ex, dictionary);
                return;
            }
            ErrorAttachmentLog errorAttachmentLog = CreateAttachment(attachmentContent);
            Crashes.TrackError(ex, dictionary, errorAttachmentLog);
        }
    }

}
