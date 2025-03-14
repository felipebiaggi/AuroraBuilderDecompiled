using System.Diagnostics;
using Builder.Presentation.Services.Data;
using Builder.Presentation.Services.QuickBar.Commands.Base;

namespace Builder.Presentation.Services.QuickBar.Commands
{
    public sealed class QuickSearchDataCommand : QuickBarCommand
    {
        public QuickSearchDataCommand()
            : base("data")
        {
        }

        public override void Execute(string parameter)
        {
            switch (parameter)
            {
                case "custom":
                    Process.Start(DataManager.Current.UserDocumentsCustomElementsDirectory);
                    break;
                case "portraits":
                    Process.Start(DataManager.Current.UserDocumentsPortraitsDirectory);
                    break;
                case "local":
                    Process.Start(DataManager.Current.LocalAppDataRootDirectory);
                    break;
                case "logs":
                    Process.Start(DataManager.Current.LocalAppDataLogsDirectory);
                    break;
            }
        }
    }
}
