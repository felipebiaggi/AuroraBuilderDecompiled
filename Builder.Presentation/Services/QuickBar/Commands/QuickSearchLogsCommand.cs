using Builder.Presentation.Services.Data;
using Builder.Presentation.Services.QuickBar.Commands.Base;
using System.IO;


namespace Builder.Presentation.Services.QuickBar.Commands
{
    public sealed class QuickSearchLogsCommand : QuickBarCommand
    {
        public QuickSearchLogsCommand()
            : base("logs")
        {
        }

        public override void Execute(string parameter)
        {
            if (parameter != null && parameter == "clear")
            {
                ExecuteClearCommand();
            }
        }

        private void ExecuteClearCommand()
        {
            if (!Directory.Exists(DataManager.Current.LocalAppDataLogsDirectory))
            {
                return;
            }
            string[] files = Directory.GetFiles(DataManager.Current.LocalAppDataLogsDirectory);
            foreach (string path in files)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
