using Builder.Presentation.Services.QuickBar.Commands.Base;

namespace Builder.Presentation.Services.QuickBar.Commands
{
    public class QuickBarDiagnosticsCommand : QuickBarCommand
    {
        public QuickBarDiagnosticsCommand()
            : base("diag")
        {
        }

        public override void Execute(string parameter)
        {
            //new DiagnosticsWindow().Show();
        }
    }
}
