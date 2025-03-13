using System.Windows;
using Builder.Presentation.Commands.Settings;
using MahApps.Metro;

namespace Builder.Presentation.Commands.Settings
{
    internal class ActivateDefaultAccentCommand : SettingsCommand
    {
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            base.Settings.Accent = "Aurora Default";
            base.Settings.Save();
            Accent accent = ThemeManager.GetAccent(base.Settings.Accent);
            AppTheme appTheme = ThemeManager.GetAppTheme(base.Settings.Theme);
            foreach (Window window in Application.Current.Windows)
            {
                ThemeManager.ChangeAppStyle(window, accent, appTheme);
            }
        }
    }
}
