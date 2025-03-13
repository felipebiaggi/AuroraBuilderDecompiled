using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Media;
using Builder.Core.Events;
using Builder.Core.Logging;
using Builder.Presentation;
using Builder.Presentation.Events.Shell;
using Builder.Presentation.Properties;
using Builder.Presentation.Services;
using Builder.Presentation.Telemetry;
using Builder.Presentation.Views.Development;
using MahApps.Metro;

namespace Builder.Presentation
{
    public sealed class ApplicationManager
    {
        private DiagnosticsWindow _diagnosticsWindow;

        public static ApplicationManager Current { get; } = new ApplicationManager();

        public IEventAggregator EventAggregator { get; }

        public ApplicationSettings Settings { get; }

        public bool IsInDeveloperMode { get; set; }

        public bool EnableDiagnostics { get; set; }

        public string LoadedCharacterFilePath { get; set; }

        public bool HasCharacterFileRequest => !string.IsNullOrWhiteSpace(LoadedCharacterFilePath);

        public ObservableCollection<string> CharacterGroups { get; } = new ObservableCollection<string>();

        public bool UpdateAvailable { get; set; }

        private ApplicationManager()
        {
            Logger.Initializing(this);
            ThemeManager.IsThemeChanged += ThemeManagerIsThemeChanged;
            EventAggregator = new EventAggregator();
            Settings = new ApplicationSettings(EventAggregator);
        }

        public void SendStatusMessage(string statusMessage)
        {
            EventAggregator.Send(new MainWindowStatusUpdateEvent(statusMessage));
        }

        public void RestartApplication(bool killCurrentProcess = true)
        {
            Process.Start(Application.ResourceAssembly.Location);
            if (killCurrentProcess)
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        public void SetSecurityProtocol()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        private void ThemeManagerIsThemeChanged(object sender, OnThemeChangedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                SendStatusMessage("ThemeManagerIsThemeChanged " + e.Accent.Name + "/" + e.AppTheme.Name);
            }
        }

        public IEnumerable<string> GetAccentNames()
        {
            return new string[10] { "Default", "Black", "Brown", "Purple", "Green", "Aqua", "Blue", "Mauve", "Pink", "Red" };
        }

        public void SetDefaultAccent(bool saveSettings)
        {
            Accent accent = ThemeManager.GetAccent("Aurora Default");
            foreach (Window window in Application.Current.Windows)
            {
                ThemeManager.ChangeAppStyle(window, accent, ThemeManager.DetectAppStyle(Application.Current.MainWindow).Item1);
            }
            if (saveSettings)
            {
                Settings.Settings.Accent = "Aurora Default";
                Settings.Save();
            }
        }

        public void SetLightTheme(bool saveSettings)
        {
            foreach (Window window in Application.Current.Windows)
            {
                ThemeManager.ChangeAppTheme(window, "Aurora Light");
            }
            Settings.Settings.Theme = "Aurora Light";
            Settings.Save(saveSettings);
        }

        public void SetDarkTheme(bool saveSettings)
        {
            foreach (Window window in Application.Current.Windows)
            {
                ThemeManager.ChangeAppTheme(window, "Aurora Dark");
            }
            Settings.Settings.Theme = "Aurora Dark";
            Settings.Save(saveSettings);
        }

        public void LoadThemes()
        {
            try
            {
                foreach (string accentName in GetAccentNames())
                {
                    ThemeManager.AddAccent("Aurora " + accentName, new Uri("pack://application:,,,/Aurora.Presentation;component/Styles/Accents/" + accentName + ".xaml"));
                }
                ThemeManager.AddAppTheme("Aurora Light", new Uri("pack://application:,,,/Aurora.Presentation;component/Styles/Theme/AuroraLight.xaml"));
                ThemeManager.AddAppTheme("Aurora Dark", new Uri("pack://application:,,,/Aurora.Presentation;component/Styles/Theme/AuroraDark.xaml"));
            }
            catch (Exception ex)
            {
                ex.Data["warning"] = "Unable to load themes.";
                AnalyticsErrorHelper.Exception(ex, null, "unable to load themes", "LoadThemes", 131);
                MessageDialogService.ShowException(ex);
                Application.Current.Shutdown();
            }
        }

        public void SetAccent(string accentName)
        {
            try
            {
                Accent accent = ThemeManager.GetAccent(accentName);
                AppTheme item = ThemeManager.DetectAppStyle(Application.Current.MainWindow).Item1;
                foreach (Window window in Application.Current.Windows)
                {
                    ThemeManager.ChangeAppStyle(window, accent, item);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "SetAccent");
                MessageDialogService.ShowException(ex);
            }
        }

        public void SetTheme(string name, bool save)
        {
            try
            {
                if (name.Contains("Dark"))
                {
                    SetDarkTheme(save);
                    return;
                }
                if (name.Contains("Light"))
                {
                    SetLightTheme(save);
                    return;
                }
                throw new ArgumentNullException("name");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "SetTheme");
                MessageDialogService.ShowException(ex);
            }
        }

        public void SetWindowTheme(Window window)
        {
            try
            {
                string accent = Builder.Presentation.Properties.Settings.Default.Accent;
                string theme = Builder.Presentation.Properties.Settings.Default.Theme;
                Accent newAccent = ThemeManager.GetAccent(accent) ?? ThemeManager.GetAccent("Aurora Default");
                AppTheme newTheme = ThemeManager.GetAppTheme(theme) ?? ThemeManager.GetAppTheme("Aurora Light");
                ThemeManager.ChangeAppStyle(window, newAccent, newTheme);
            }
            catch (Exception ex)
            {
                MessageDialogService.ShowException(ex);
            }
        }

        public void UpgradeConfigurationCheck()
        {
            Settings @default = Builder.Presentation.Properties.Settings.Default;
            if (@default.ConfigurationUpgradeRequired)
            {
                try
                {
                    @default.Upgrade();
                    @default.ConfigurationUpgradeRequired = false;
                    @default.Save();
                }
                catch (Exception ex)
                {
                    AnalyticsErrorHelper.Exception(ex, null, null, "UpgradeConfigurationCheck", 227);
                }
            }
        }

        public void ValidateConfiguration(bool openDirectory)
        {
            try
            {
                Builder.Presentation.Properties.Settings.Default.Reload();
                _ = Builder.Presentation.Properties.Settings.Default.Accent;
            }
            catch (ConfigurationException ex)
            {
                AnalyticsErrorHelper.Exception(ex, null, null, "ValidateConfiguration", 241);
                if (MessageBox.Show("Aurora has detected that your user settings file has become corrupted. This may be due to a previous crash." + Environment.NewLine + Environment.NewLine + "Do you want to reset your user settings? Click no to try to open the folder containing your user settings to manually fix or remove it.", "Corrupted Configuration", MessageBoxButton.YesNo, MessageBoxImage.Hand) == MessageBoxResult.Yes)
                {
                    if (ex.InnerException is ConfigurationException ex2)
                    {
                        if (File.Exists(ex2.Filename))
                        {
                            File.Delete(ex2.Filename);
                            RestartApplication();
                        }
                    }
                    else if (ex.InnerException is ConfigurationException ex3)
                    {
                        if (File.Exists(ex3.Filename))
                        {
                            File.Delete(ex3.Filename);
                            RestartApplication();
                        }
                    }
                    else if (ex is ConfigurationErrorsException ex4)
                    {
                        if (File.Exists(ex4.Filename))
                        {
                            File.Delete(ex4.Filename);
                            RestartApplication();
                        }
                    }
                    else if (ex.InnerException is ConfigurationErrorsException ex5)
                    {
                        if (File.Exists(ex5.Filename))
                        {
                            File.Delete(ex5.Filename);
                            RestartApplication();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Unable to detect user settings file." + Environment.NewLine + Environment.NewLine + ex.Message, ex.ToString());
                    }
                    return;
                }
                if (openDirectory)
                {
                    if (ex.InnerException is ConfigurationException ex6)
                    {
                        FileInfo fileInfo = new FileInfo(ex6.Filename);
                        if (fileInfo.DirectoryName != null)
                        {
                            Process.Start(fileInfo.DirectoryName);
                        }
                    }
                    if (ex != null)
                    {
                        ConfigurationException ex7 = ex;
                        FileInfo fileInfo2 = new FileInfo(ex7.Filename);
                        if (fileInfo2.DirectoryName != null)
                        {
                            Process.Start(fileInfo2.DirectoryName);
                        }
                    }
                }
                Process.GetCurrentProcess().Kill();
            }
        }

        public void InitializeDiagnosticsWindow()
        {
            if (_diagnosticsWindow == null)
            {
                _diagnosticsWindow = new DiagnosticsWindow();
            }
        }

        public void ShowDiagnosticsWindow()
        {
            if (_diagnosticsWindow == null)
            {
                _diagnosticsWindow = new DiagnosticsWindow();
            }
            _diagnosticsWindow.Show();
        }

        [Obsolete("legacy - not used")]
        public Color GetHighlightColor()
        {
            return (Color)ThemeManager.GetAccent(Builder.Presentation.Properties.Settings.Default.Accent).Resources["HighlightColor"];
        }

        [Obsolete("legacy - not used")]
        public Color GetAccentColor()
        {
            return (Color)ThemeManager.GetAccent(Builder.Presentation.Properties.Settings.Default.Accent).Resources["AccentColor"];
        }
    }
}
