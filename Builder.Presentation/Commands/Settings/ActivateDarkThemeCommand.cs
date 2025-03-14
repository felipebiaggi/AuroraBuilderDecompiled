using Builder.Presentation.Events.Application;
using MahApps.Metro;
using System.Windows;

namespace Builder.Presentation.Commands.Settings
{
    internal class ActivateDarkThemeCommand : SettingsCommand
    {
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            base.Settings.Theme = "Aurora Dark";
            base.Settings.Save();
            foreach (Window window in Application.Current.Windows)
            {
                ThemeManager.ChangeAppTheme(window, base.Settings.Theme);
            }
            ApplicationManager.Current.EventAggregator.Send(new SettingsChangedEvent());
        }
    }
}
