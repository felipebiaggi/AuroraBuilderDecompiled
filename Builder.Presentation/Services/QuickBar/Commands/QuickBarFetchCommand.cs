using Builder.Core.Events;
using Builder.Core.Logging;
using Builder.Data.Files;
using Builder.Data.Files.Updater;
using Builder.Presentation.Events.Shell;
using Builder.Presentation.Properties;
using Builder.Presentation.Services.Data;
using Builder.Presentation.Services.QuickBar.Commands.Base;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;


namespace Builder.Presentation.Services.QuickBar.Commands
{
    public class QuickBarFetchCommand : QuickBarCommand
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly IndicesUpdateService _updater;

        public QuickBarFetchCommand()
            : base("fetch")
        {
            _eventAggregator = ApplicationManager.Current.EventAggregator;
            Version appVersion = new Version(Resources.AppVersionCheck);
            _updater = new IndicesUpdateService(appVersion);
            _updater.StatusChanged += _updater_StatusChanged;
        }

        private void _updater_StatusChanged(object sender, IndicesUpdateStatusChangedEventArgs e)
        {
            MainWindowStatusUpdateEvent args = new MainWindowStatusUpdateEvent(e.StatusMessage)
            {
                ProgressPercentage = e.ProgressPercentage
            };
            _eventAggregator.Send(args);
        }

        public override void Execute(string parameter)
        {
            if (parameter == "?" || parameter == "help" || !parameter.StartsWith("http") || !parameter.EndsWith(".index"))
            {
                MessageDialogService.Show("@" + base.CommandName + " accepts a valid (starting with http:// or https://) url for an index file.", "@" + base.CommandName);
                return;
            }
            MainWindowStatusUpdateEvent mainWindowStatusUpdateEvent = new MainWindowStatusUpdateEvent("executing @" + base.CommandName + " " + parameter);
            try
            {
                Fetch(parameter);
            }
            catch (Exception ex)
            {
                mainWindowStatusUpdateEvent.IsDanger = true;
                mainWindowStatusUpdateEvent.StatusMessage = ex.Message;
            }
            _eventAggregator.Send(mainWindowStatusUpdateEvent);
        }

        private async void Fetch(string url)
        {
            _ = 2;
            try
            {
                IndexFile file = await IndexFile.FromUrl(url);
                file.SaveContent(new FileInfo(Path.Combine(DataManager.Current.UserDocumentsCustomElementsDirectory, file.Info.UpdateFilename)));
                _eventAggregator.Send(new MainWindowStatusUpdateEvent("The index file '" + file.Info.UpdateFilename + "' has successfully been written to the custom folder. Run the 'update custom files' command to pull in the content."));
                await Task.Delay(1000);
                if (await _updater.UpdateIndexFiles(DataManager.Current.UserDocumentsCustomElementsDirectory, file.FileInfo.FullName) && MessageBox.Show(Application.Current.MainWindow, "Your custom files have been updated, do you want to restart the applicaton to reload the content?", Resources.ApplicationName, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                }
                _eventAggregator.Send(new MainWindowStatusUpdateEvent("The '" + file.Info.DisplayName + "' bundle has been added."));
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "Fetch");
                _eventAggregator.Send(new MainWindowStatusUpdateEvent(ex.Message)
                {
                    IsDanger = true
                });
                MessageDialogService.ShowException(ex);
            }
        }
    }
}
