using Builder.Core.Events;
using Builder.Core.Logging;
using Builder.Data.Files;
using Builder.Data.Files.Updater;
using Builder.Presentation.Events.Shell;
using Builder.Presentation.Properties;
using Builder.Presentation.Services.Data;
using Builder.Presentation.Services.QuickBar.Commands.Base;
using Builder.Presentation.ViewModels.Shell.Start;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Builder.Presentation.Services.QuickBar.Commands
{
    public class QuickBarBundleCommand : QuickBarCommand
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly IndicesUpdateService _updater;

        private readonly string[] _parameters;

        public QuickBarBundleCommand()
            : base("bundle")
        {
            _eventAggregator = ApplicationManager.Current.EventAggregator;
            Version appVersion = new Version(Resources.AppVersionCheck);
            _updater = new IndicesUpdateService(appVersion);
            _updater.StatusChanged += _updater_StatusChanged;
            _parameters = new string[6] { "core", "supplements", "unearthed-arcana", "third-party", "reddit", "clear" };
        }

        private void _updater_StatusChanged(object sender, IndicesUpdateStatusChangedEventArgs e)
        {
            MainWindowStatusUpdateEvent args = new MainWindowStatusUpdateEvent(e.StatusMessage)
            {
                IsSuccess = true,
                ProgressPercentage = e.ProgressPercentage
            };
            _eventAggregator.Send(args);
        }

        public override void Execute(string parameter)
        {
            if (parameter == "?" || parameter == "help")
            {
                MessageDialogService.Show("@" + base.CommandName + " parameters are: " + string.Join(", ", _parameters), "@" + base.CommandName);
                return;
            }
            MainWindowStatusUpdateEvent mainWindowStatusUpdateEvent = new MainWindowStatusUpdateEvent("executing @" + base.CommandName + " parameter: " + parameter);
            switch (parameter)
            {
                case "ui":
                case "enable":
                case "on":
                    _eventAggregator.Send(new BundleCommandEvent(parameter));
                    break;
                case "":
                case "?":
                case "help":
                    {
                        string text = "@" + base.CommandName + "parameters are:" + Environment.NewLine;
                        string[] indexFiles = _parameters;
                        foreach (string text2 in indexFiles)
                        {
                            text = text + text2 + Environment.NewLine;
                        }
                        MessageDialogService.Show(text, "@" + base.CommandName);
                        return;
                    }
                case "core":
                    SendDownloadRequest("https://raw.githubusercontent.com/aurorabuilder/elements/master/core.index");
                    break;
                case "supplements":
                    SendDownloadRequest("https://raw.githubusercontent.com/aurorabuilder/elements/master/supplements.index");
                    break;
                case "unearthed-arcana":
                    SendDownloadRequest("https://raw.githubusercontent.com/aurorabuilder/elements/master/unearthed-arcana.index");
                    break;
                case "third-party":
                    SendDownloadRequest("https://raw.githubusercontent.com/aurorabuilder/elements/master/third-party.index");
                    break;
                case "homebrew":
                    SendDownloadRequest("https://raw.githubusercontent.com/aurorabuilder/elements/master/homebrew.index");
                    break;
                case "reddit":
                case "community-reddit":
                    SendDownloadRequest("https://raw.githubusercontent.com/community-elements/elements-reddit/master/reddit.index");
                    break;
                case "clear":
                    {
                        if (MessageBox.Show("This will remove all content from folders created by index files. Proceed?", "Clear Bundles", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                        {
                            break;
                        }
                        string[] indexFiles = _updater.GetIndexFiles(DataManager.Current.UserDocumentsCustomElementsDirectory);
                        for (int i = 0; i < indexFiles.Length; i++)
                        {
                            IndexFile indexFile = IndexFile.FromFile(new FileInfo(indexFiles[i]));
                            if (Directory.Exists(indexFile.GetContentDirectory()))
                            {
                                Directory.Delete(indexFile.GetContentDirectory(), recursive: true);
                            }
                        }
                        mainWindowStatusUpdateEvent.StatusMessage = "Content cleared.";
                        break;
                    }
                default:
                    mainWindowStatusUpdateEvent.StatusMessage = "Invalid @bundle command (" + parameter + ")";
                    mainWindowStatusUpdateEvent.IsDanger = true;
                    MessageDialogService.Show(mainWindowStatusUpdateEvent.StatusMessage);
                    break;
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
                _eventAggregator.Send(new MainWindowStatusUpdateEvent("The index file '" + file.Info.UpdateFilename + "' has successfully been downloaded. Processing the file now."));
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

        private async void SendDownloadRequest(params string[] urls)
        {
            foreach (string url in urls)
            {
                _eventAggregator.Send(new IndexDownloadRequestEvent(url));
                await Task.Delay(1000);
            }
        }
    }
}
