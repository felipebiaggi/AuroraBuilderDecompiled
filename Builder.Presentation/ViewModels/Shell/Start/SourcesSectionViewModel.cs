using Builder.Core;
using Builder.Core.Events;
using Builder.Core.Logging;
using Builder.Data.Files;
using Builder.Data.Files.Updater;
using Builder.Presentation.Events.Application;
using Builder.Presentation.Events.Shell;
using Builder.Presentation.Properties;
using Builder.Presentation.Services;
using Builder.Presentation.Services.Data;
using Builder.Presentation.Services.QuickBar.Commands;
using Builder.Presentation.Telemetry;
using Builder.Presentation.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Builder.Presentation.ViewModels.Shell.Start
{
    public sealed class SourcesSectionViewModel : ViewModelBase, ISubscriber<IndexDownloadRequestEvent>, ISubscriber<BundleCommandEvent>
    {
        private readonly IndicesUpdateService _updateService;

        private IndexFile _selectedIndex;

        private string _remoteUrl;

        private bool _isCheckingForContentUpdates;

        private int _updateCount;

        private string _updateBadgeContent;

        private string _fileChangedStatusMessage;

        private int _fetchCount;

        private string _restartBadgeCount;

        private int _progress;

        private bool _awaitingRestart;

        private bool _containsIndices;

        private bool _bundlesEnabled;

        public ObservableCollection<IndexFile> Indices { get; } = new ObservableCollection<IndexFile>();

        public ObservableCollection<ElementsFile> ContentFiles { get; } = new ObservableCollection<ElementsFile>();

        public IndexFile SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                SetProperty(ref _selectedIndex, value, "SelectedIndex");
            }
        }

        public string RemoteUrl
        {
            get
            {
                return _remoteUrl;
            }
            set
            {
                SetProperty(ref _remoteUrl, value, "RemoteUrl");
                (FetchCustomFileCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand OpenCustomDocumentsFolderCommand => new RelayCommand(OpenCustomDocumentsFolder);

        public ICommand OpenUserCustomDocumentsFolderCommand => new RelayCommand(OpenUserCustomDocumentsFolder);

        public RelayCommand UpdateCustomFilesCommand { get; }

        public RelayCommand CleanCustomFilesCommand { get; }

        public ICommand RestartCommand => new RelayCommand(Restart);

        public ICommand ClearCommand => new RelayCommand(Clear);

        public bool IsCheckingForContentUpdates
        {
            get
            {
                return _isCheckingForContentUpdates;
            }
            set
            {
                SetProperty(ref _isCheckingForContentUpdates, value, "IsCheckingForContentUpdates");
            }
        }

        public int UpdateCount
        {
            get
            {
                return _updateCount;
            }
            set
            {
                SetProperty(ref _updateCount, value, "UpdateCount");
            }
        }

        public string UpdateBadgeContent
        {
            get
            {
                return _updateBadgeContent;
            }
            set
            {
                SetProperty(ref _updateBadgeContent, value, "UpdateBadgeContent");
            }
        }

        public string FileChangedStatusMessage
        {
            get
            {
                return _fileChangedStatusMessage;
            }
            set
            {
                SetProperty(ref _fileChangedStatusMessage, value, "FileChangedStatusMessage");
            }
        }

        public int FetchCount
        {
            get
            {
                return _fetchCount;
            }
            set
            {
                SetProperty(ref _fetchCount, value, "FetchCount");
            }
        }

        public string RestartBadgeCount
        {
            get
            {
                return _restartBadgeCount;
            }
            set
            {
                SetProperty(ref _restartBadgeCount, value, "RestartBadgeCount");
            }
        }

        public int Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                SetProperty(ref _progress, value, "Progress");
            }
        }

        public bool AwaitingRestart
        {
            get
            {
                return _awaitingRestart;
            }
            set
            {
                SetProperty(ref _awaitingRestart, value, "AwaitingRestart");
            }
        }

        public ICommand FetchCustomFileCommand { get; }

        public bool ContainsIndices
        {
            get
            {
                return _containsIndices;
            }
            set
            {
                SetProperty(ref _containsIndices, value, "ContainsIndices");
            }
        }

        public ICommand DownloadIndexCommand => new RelayCommand<string>(async delegate (string parameter)
        {
            if (!string.IsNullOrWhiteSpace(parameter))
            {
                string text = "";
                if (parameter != null)
                {
                    switch (parameter)
                    {
                        case "core":
                            text = "https://raw.githubusercontent.com/aurorabuilder/elements/master/core.index";
                            break;
                        case "supplements":
                            text = "https://raw.githubusercontent.com/aurorabuilder/elements/master/supplements.index";
                            break;
                        case "unearthed-arcana":
                            text = "https://raw.githubusercontent.com/aurorabuilder/elements/master/unearthed-arcana.index";
                            break;
                        case "third-party":
                            text = "https://raw.githubusercontent.com/aurorabuilder/elements/master/third-party.index";
                            break;
                        case "homebrew":
                            text = "https://raw.githubusercontent.com/aurorabuilder/elements/master/homebrew.index";
                            break;
                        case "reddit":
                            text = "https://raw.githubusercontent.com/community-elements/elements-reddit/master/reddit.index";
                            break;
                        case "dndwiki":
                            text = "https://raw.githubusercontent.com/community-elements/elements-dndwiki/master/dndwiki.index";
                            break;
                    }
                }
                if (!string.IsNullOrWhiteSpace(text))
                {
                    await DownloadIndexAsync(text);
                }
            }
        });

        public bool BundlesEnabled
        {
            get
            {
                return _bundlesEnabled;
            }
            set
            {
                SetProperty(ref _bundlesEnabled, value, "BundlesEnabled");
            }
        }

        public SourcesSectionViewModel()
        {
            Version appVersion = new Version(Resources.AppVersionCheck);
            _updateService = new IndicesUpdateService(appVersion);
            _updateService.StatusChanged += UpdateServiceStatusChanged;
            _updateService.FileUpdated += _updateService_FileUpdated;
            if (base.IsInDebugMode)
            {
                _remoteUrl = "https://raw.githubusercontent.com/aurorabuilder/elements/master/core.index";
            }
            if (base.IsInDesignMode)
            {
                InitializeDesignData();
                return;
            }
            _ = ApplicationManager.Current.IsInDeveloperMode;
            FetchCustomFileCommand = new RelayCommand(FetchCustomFile, CanFetchFile);
            UpdateCustomFilesCommand = new RelayCommand(UpdateCustomFiles, CanUpdateCustomFiles);
            CleanCustomFilesCommand = new RelayCommand(CleanCustomFiles, CanCleanCustomFiles);
            LoadIndices();
            _selectedIndex = Indices.FirstOrDefault();
            if (base.Settings.Settings.StartupCheckForContentUpdated)
            {
                PerformStartupContentUpdateCheck();
            }
            if (base.Settings.Settings.Bundle)
            {
                BundlesEnabled = true;
            }
            SubscribeWithEventAggregator();
        }

        private async Task PerformStartupContentUpdateCheck()
        {
            await Task.Delay(10000);
            if (ApplicationManager.Current.UpdateAvailable)
            {
                FileChangedStatusMessage = "An application update is available. The automatic updating of content files was halted to avoid possible incompatible updates to the files.";
            }
            else if (!IsCheckingForContentUpdates && !AwaitingRestart)
            {
                UpdateCustomFiles();
            }
        }

        private void _updateService_FileUpdated(object sender, IndicesUpdateStatusChangedEventArgs e)
        {
            Dispatcher.CurrentDispatcher.Invoke(delegate
            {
                Logger.Warning(e.StatusMessage ?? "");
                FileChangedStatusMessage = e.StatusMessage;
                int updateCount = UpdateCount;
                UpdateCount = updateCount + 1;
                RestartBadgeCount = UpdateCount.ToString();
            });
        }

        private void UpdateServiceStatusChanged(object sender, IndicesUpdateStatusChangedEventArgs e)
        {
            Dispatcher.CurrentDispatcher.Invoke(delegate
            {
                if (e.StatusMessage.ToLower().Contains("index"))
                {
                    FileChangedStatusMessage = e.StatusMessage;
                }
                Progress = e.ProgressPercentage;
            });
        }

        private void Restart()
        {
            if (MessageBox.Show("Your content files have been updated, do you want to restart the application to reload the content?", "Aurora", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private void Clear()
        {
            if (MessageBox.Show("Are you sure you want to clear the folder ", "Aurora Builder", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
            {
                AnalyticsEventHelper.ContentClear(confirmed: true);
            }
            else
            {
                AnalyticsEventHelper.ContentClear(confirmed: false);
            }
        }

        private async void FetchCustomFile()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(RemoteUrl))
                {
                    return;
                }
                RemoteUrl = RemoteUrl.Trim();
                if (!RemoteUrl.StartsWith("http") && !RemoteUrl.Contains("http"))
                {
                    RemoteUrl = "http://" + RemoteUrl;
                }
                if (RemoteUrl.ToLowerInvariant().EndsWith("/user.index"))
                {
                    MessageDialogService.Show("The name 'user' is a reserved name for index files. It's recommended you use 'user-yourname' instead for a personal index file.");
                }
                else if (RemoteUrl.EndsWith(".index"))
                {
                    base.EventAggregator.Send(new MainWindowStatusUpdateEvent("Fetching " + RemoteUrl));
                    IndexFile indexFile = await IndexFile.FromUrl(RemoteUrl);
                    if (!indexFile.MeetsMinimumAppVersionRequirements(_updateService.AppVersion))
                    {
                        MessageDialogService.Show($"You need to run Aurora v{indexFile.MinimumAppVersion} to include this file. It might contain updates that are not compatible with the version you're currently running.");
                        return;
                    }
                    indexFile.SaveContent(new FileInfo(Path.Combine(DataManager.Current.UserDocumentsCustomElementsDirectory, indexFile.Info.UpdateFilename)));
                    base.EventAggregator.Send(new MainWindowStatusUpdateEvent("The index file '" + indexFile.Info.UpdateFilename + "' has successfully been fetched. Run the 'update custom files' command to pull in the content."));
                    AnalyticsEventHelper.ContentDownloadIndex(RemoteUrl);
                    FetchCount++;
                    UpdateBadgeContent = FetchCount.ToString();
                    if (RemoteUrl == "https://raw.githubusercontent.com/aurorabuilder/elements/master/supplements.index")
                    {
                        IncludeCoreIndexIfMissing();
                    }
                }
                else
                {
                    MessageDialogService.Show("This field is used to fetch .index files only.");
                }
            }
            catch (HttpRequestException ex)
            {
                ex.Data["404"] = RemoteUrl;
                Logger.Exception(ex, "FetchCustomFile");
                MessageDialogService.ShowException(ex);
            }
            catch (Exception ex2)
            {
                Logger.Exception(ex2, "FetchCustomFile");
                MessageDialogService.ShowException(ex2);
            }
        }

        private bool CanFetchFile()
        {
            return !string.IsNullOrWhiteSpace(RemoteUrl);
        }

        private void OpenCustomDocumentsFolder()
        {
            Process.Start(DataManager.Current.UserDocumentsCustomElementsDirectory);
        }

        private void OpenUserCustomDocumentsFolder()
        {
            Process.Start(Path.Combine(DataManager.Current.UserDocumentsCustomElementsDirectory, "user"));
        }

        private async void UpdateCustomFiles()
        {
            try
            {
                base.EventAggregator.Send(new MainWindowStatusUpdateEvent("Checking for content updates..."));
                FileChangedStatusMessage = "Checking for content updates...";
                IsCheckingForContentUpdates = true;
                UpdateCustomFilesCommand.RaiseCanExecuteChanged();
                CleanCustomFilesCommand.RaiseCanExecuteChanged();
                bool num = await _updateService.UpdateIndexFiles(DataManager.Current.UserDocumentsCustomElementsDirectory);
                LoadIndices();
                if (num)
                {
                    AwaitingRestart = true;
                    FileChangedStatusMessage = "Your content files have been updated, restart the application to reload the content.";
                    base.EventAggregator.Send(new MainWindowStatusUpdateEvent("Your content files have been updated, restart the application to reload the content."));
                    AdditionalContentUpdatedEvent args = new AdditionalContentUpdatedEvent(RestartBadgeCount);
                    base.EventAggregator.Send(args);
                }
                else
                {
                    FileChangedStatusMessage = "Last checked for content updates at " + DateTime.Now.ToShortTimeString();
                    base.EventAggregator.Send(new MainWindowStatusUpdateEvent("Last checked for content updates at " + DateTime.Now.ToShortTimeString()));
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "UpdateCustomFiles");
                if (ex.Data.Contains("404"))
                {
                    string text = ex.Data["404"].ToString();
                    FileChangedStatusMessage = "The remote file with url '" + text + "' doesn't exist.";
                    MessageDialogService.Show("The remote file with url '" + text + "' doesn't exist.", "Aurora - 404 File Not Found");
                }
                else
                {
                    FileChangedStatusMessage = ex.Message ?? "";
                    MessageDialogService.ShowException(ex);
                }
            }
            finally
            {
                IsCheckingForContentUpdates = false;
                UpdateCustomFilesCommand.RaiseCanExecuteChanged();
                CleanCustomFilesCommand.RaiseCanExecuteChanged();
                FetchCount = 0;
                UpdateBadgeContent = "";
            }
        }

        private bool CanUpdateCustomFiles()
        {
            return !IsCheckingForContentUpdates;
        }

        private void CleanCustomFiles()
        {
            try
            {
                if (IsCheckingForContentUpdates)
                {
                    CleanCustomFilesCommand.RaiseCanExecuteChanged();
                    return;
                }
                string[] files = Directory.GetFiles(DataManager.Current.UserDocumentsCustomElementsDirectory, "*.index");
                if (!files.Any())
                {
                    return;
                }
                List<string> list = new List<string>();
                StringBuilder stringBuilder = new StringBuilder("This action will remove the content from the following folders that are created by the index files:");
                stringBuilder.AppendLine();
                string[] array = files;
                for (int i = 0; i < array.Length; i++)
                {
                    string contentDirectory = IndexFile.FromFile(new FileInfo(array[i])).GetContentDirectory();
                    stringBuilder.AppendLine(contentDirectory ?? "");
                    if (!contentDirectory.Equals(Path.Combine(DataManager.Current.UserDocumentsCustomElementsDirectory, "user"), StringComparison.OrdinalIgnoreCase) && Directory.Exists(contentDirectory))
                    {
                        list.Add(contentDirectory);
                    }
                }
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Once removed you can run the content updates to download the latest content.");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Continue?");
                if (MessageBox.Show(stringBuilder.ToString(), "Clear Custom Files", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                {
                    return;
                }
                List<string> list2 = new List<string>();
                int num = 0;
                foreach (string item in list)
                {
                    if (Directory.Exists(item))
                    {
                        Directory.Delete(item, recursive: true);
                        num++;
                        list2.Add(item);
                    }
                }
                StringBuilder stringBuilder2 = new StringBuilder("Your content folders have been cleared.");
                if (num == list.Count)
                {
                    FileChangedStatusMessage = stringBuilder2.ToString();
                }
                else
                {
                    stringBuilder2 = new StringBuilder("The following content folders have been cleared:");
                    foreach (string item2 in list2)
                    {
                        stringBuilder2.AppendLine(item2 ?? "");
                    }
                    MessageBox.Show(stringBuilder2.ToString(), "Content Cleared.");
                }
                UpdateCount = 0;
                RestartBadgeCount = "";
                UpdateBadgeContent = ((num > 0) ? $"{num}" : "");
            }
            catch (Exception ex)
            {
                MessageDialogService.ShowException(ex, "Error while trying to clear custom folders.");
            }
            finally
            {
                CleanCustomFilesCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanCleanCustomFiles()
        {
            if (IsCheckingForContentUpdates)
            {
                return false;
            }
            string[] files = Directory.GetFiles(DataManager.Current.UserDocumentsCustomElementsDirectory, "*.index");
            List<string> list = new List<string>();
            string[] array = files;
            for (int i = 0; i < array.Length; i++)
            {
                string contentDirectory = IndexFile.FromFile(new FileInfo(array[i])).GetContentDirectory();
                if (Directory.Exists(contentDirectory))
                {
                    list.Add(contentDirectory);
                }
            }
            return list.Any();
        }

        private void IncludeCoreIndexIfMissing()
        {
            if (!File.Exists(Path.Combine(DataManager.Current.UserDocumentsCustomElementsDirectory, "core.index")))
            {
                AnalyticsEventHelper.ApplicationEvent("auto_include_core");
                if (MessageBox.Show("You have added the supplements index file.\r\n\r\nIt is required to include at least the core index when adding index files. This will override the content from the bundled system reference document, will include all content from the core rulebooks, and will have the latest updates.\r\n\r\nDo you want to include the core index now?", "Aurora - Additional Content", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    RemoteUrl = "https://raw.githubusercontent.com/aurorabuilder/elements/master/core.index";
                    FetchCustomFile();
                }
            }
        }

        private void LoadIndices()
        {
            string[] files = Directory.GetFiles(DataManager.Current.UserDocumentsCustomElementsDirectory, "*.index", SearchOption.AllDirectories);
            Indices.Clear();
            foreach (IndexFile item in files.Select((string x) => new IndexFile(new FileInfo(x))))
            {
                item.Load();
                if (item.ContainsElementFiles())
                {
                    Indices.Add(item);
                }
            }
            ContainsIndices = Indices.Any();
        }

        protected override void InitializeDesignData()
        {
            _remoteUrl = "https://raw.githubusercontent.com/swdriessen/dndbuilder/master/elements/aurora.index";
            foreach (IndexFile item in from x in Directory.GetFiles("C:\\Users\\bas_d\\Documents\\5e Character Builder\\custom", "*.index", SearchOption.AllDirectories)
                                       select new IndexFile(new FileInfo(x)))
            {
                item.Load();
                Indices.Add(item);
            }
            _selectedIndex = Indices.FirstOrDefault();
        }

        public async void OnHandleEvent(IndexDownloadRequestEvent args)
        {
            if (!string.IsNullOrWhiteSpace(args.Url))
            {
                await DownloadIndexAsync(args.Url);
                if (CanUpdateCustomFiles())
                {
                    base.EventAggregator.Send(new SelectionRuleNavigationArgs(NavigationLocation.StartCustomContent));
                }
            }
        }

        private async Task DownloadIndexAsync(string url)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(url))
                {
                    url = url.Trim();
                    if (!url.StartsWith("http") && !url.Contains("http"))
                    {
                        url = "http://" + url;
                    }
                    if (url.EndsWith(".index"))
                    {
                        base.EventAggregator.Send(new MainWindowStatusUpdateEvent("Downloading " + url));
                        IndexFile obj = await IndexFile.FromUrl(url);
                        string updateFilename = obj.Info.UpdateFilename;
                        string fileName = Path.Combine(DataManager.Current.UserDocumentsCustomElementsDirectory, updateFilename);
                        obj.SaveContent(new FileInfo(fileName));
                        base.EventAggregator.Send(new MainWindowStatusUpdateEvent("The index file '" + updateFilename + "' has been downloaded successfully."));
                        FetchCount++;
                        UpdateBadgeContent = FetchCount.ToString();
                        AnalyticsEventHelper.ContentDownloadIndex(url, bundle: true);
                    }
                    else
                    {
                        MessageDialogService.Show("This field is used to fetch .index files only.");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                ex.Data["404"] = url;
                Logger.Exception(ex, "DownloadIndexAsync");
                MessageDialogService.ShowException(ex);
            }
            catch (Exception ex2)
            {
                Logger.Exception(ex2, "DownloadIndexAsync");
                MessageDialogService.ShowException(ex2);
            }
        }

        public void OnHandleEvent(BundleCommandEvent args)
        {
            switch (args.Command)
            {
                case "on":
                case "enable":
                    BundlesEnabled = true;
                    break;
            }
            if (BundlesEnabled)
            {
                base.Settings.Settings.Bundle = true;
                base.Settings.Save();
            }
        }
    }

}
