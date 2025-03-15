using System;
using System.Diagnostics;
using System.Windows;
using Builder.Presentation.Properties;
//using Builder.Presentation.Views.Dialogs;

namespace Builder.Presentation.Services
{

    public static class MessageDialogService
    {
        public static void Show(string message, string title = null)
        {
            Show((object)message, title ?? (Resources.ApplicationName + " (" + Resources.ApplicationVersion + ")"));
        }

        private static void Show(object content, string title)
        {
            MessageBox.Show(content.ToString(), title);
        }

        public static void ShowException(Exception ex)
        {
            ShowException(ex, ex.GetType().ToString());
        }

        public static void ShowException(Exception ex, string title)
        {
            ShowException(ex, title, null);
        }

        public static void ShowException(Exception ex, string title, string introMessage)
        {
            if (Debugger.IsAttached)
            {
                string text = ex.GetType().Name + " " + ((ex.InnerException != null) ? "has inner exception" : "with no inner exception") + ": " + ex.Message;
                text = text + Environment.NewLine + "Source: " + ex.Source;
                text = text + Environment.NewLine + "Trace: " + ex.StackTrace;
                if (ex.InnerException != null)
                {
                    text = text + Environment.NewLine + $"Inner Exception: {ex.InnerException}";
                }
                //new ExceptionMessageWindow(title, introMessage, text).ShowDialog();
                return;
            }
            string text2 = "";
            string text3 = "";
            string message = ex.Message;
            if (ex.Data.Contains("filename"))
            {
                string text4 = ex.Data["filename"].ToString();
                text2 = "An error occurred trying to parse a file.";
                text3 = (ex.Data.Contains("warning") ? ex.Data["warning"].ToString() : text4);
                message = text4 + "\r\n\r\n" + ex.Message;
            }
            else if (ex.Data.Contains("warning"))
            {
                text2 = "An error occurred trying to parse internal file.";
                text3 = ex.Data["warning"].ToString();
            }
            else if (ex.Data.Contains("404"))
            {
                text2 = "An error occurred while trying to perform a web request.";
                text3 = ex.Data["404"].ToString();
            }
            else
            {
                text2 = "An error occurred";
                text3 = ex.Message;
                message = ex.Source + "\r\n\r\n" + ex.StackTrace;
            }
            //new ExceptionWindow(title, text2, text3, message).ShowDialog();
        }
    }
}
