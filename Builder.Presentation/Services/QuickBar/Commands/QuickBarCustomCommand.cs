using Builder.Presentation.Services.Data;
using Builder.Presentation.Services.QuickBar.Commands.Base;
using System.Diagnostics;

namespace Builder.Presentation.Services.QuickBar.Commands
{
    public sealed class QuickBarCustomCommand : QuickBarCommand
    {
        public QuickBarCustomCommand()
            : base("custom")
        {
        }

        public override void Execute(string parameter)
        {
            Process.Start(DataManager.Current.UserDocumentsCustomElementsDirectory);
        }
    }
}
