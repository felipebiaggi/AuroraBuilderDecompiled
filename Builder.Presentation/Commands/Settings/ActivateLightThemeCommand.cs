using MahApps.Metro;
using System.Windows;

namespace Builder.Presentation.Commands.Settings
{
    internal class ActivateLightThemeCommand : SettingsCommand
    {
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            base.Settings.Theme = "Aurora Light";
            base.Settings.Save();
            foreach (Window window in Application.Current.Windows)
            {
                ThemeManager.ChangeAppTheme(window, base.Settings.Theme);
            }
        }
    }
}
