using System;
using System.Diagnostics;
using Builder.Presentation;
using Builder.Presentation.Events.Shell;
using Builder.Presentation.Services.Data;
using Builder.Presentation.Services.QuickBar.Commands.Base;

namespace Builder.Presentation.Services.QuickBar.Commands
{
    public sealed class QuickBarPortraitsCommand : QuickBarCommand
    {
        private readonly string[] _parameters;

        public QuickBarPortraitsCommand()
            : base("portraits")
        {
            _parameters = new string[1] { "open" };
        }

        public override void Execute(string parameter)
        {
            MainWindowStatusUpdateEvent mainWindowStatusUpdateEvent = new MainWindowStatusUpdateEvent("");
            try
            {
                if (parameter != null)
                {
                    switch (parameter)
                    {
                        default:
                            if (parameter.Length != 0)
                            {
                                goto case null;
                            }
                            goto IL_0072;
                        case null:
                            if (!(parameter == "open"))
                            {
                                break;
                            }
                            goto IL_0072;
                        case "?":
                        case "help":
                            {
                                mainWindowStatusUpdateEvent.StatusMessage = "@" + base.CommandName + " parameters are: " + string.Join(", ", _parameters);
                                goto end_IL_000b;
                            }
                        IL_0072:
                            Process.Start(DataManager.Current.UserDocumentsPortraitsDirectory);
                            mainWindowStatusUpdateEvent.StatusMessage = "opening " + DataManager.Current.UserDocumentsPortraitsDirectory;
                            goto end_IL_000b;
                    }
                }
                mainWindowStatusUpdateEvent.StatusMessage = "invalid @" + base.CommandName + " command (" + parameter + ")";
                mainWindowStatusUpdateEvent.IsDanger = true;
            end_IL_000b:;
            }
            catch (Exception ex)
            {
                mainWindowStatusUpdateEvent.IsDanger = true;
                mainWindowStatusUpdateEvent.StatusMessage = ex.Message;
            }
            ApplicationManager.Current.EventAggregator.Send(mainWindowStatusUpdateEvent);
        }
    }
}
