using Builder.Core.Events;
using Builder.Core.Logging;
using Builder.Presentation.Events.Data;
using Builder.Presentation.Properties;
using Builder.Presentation.Services;
using Builder.Presentation.Services.Data;
using Builder.Presentation.ViewModels.Base;
using System;
using System.Threading.Tasks;

namespace Builder.Presentation.ViewModels
{
    public class LauncherWindowViewModel : ViewModelBase, ISubscriber<DataManagerProgressChanged>
    {
        private string _progressMessage;

        private int _progressPercentage;

        private bool _progressCompleted;

        private string _version;

        public string ProgressMessage
        {
            get
            {
                return _progressMessage;
            }
            set
            {
                SetProperty(ref _progressMessage, value, "ProgressMessage");
            }
        }

        public int ProgressPercentage
        {
            get
            {
                return _progressPercentage;
            }
            set
            {
                SetProperty(ref _progressPercentage, value, "ProgressPercentage");
            }
        }

        public bool ProgressCompleted
        {
            get
            {
                return _progressCompleted;
            }
            set
            {
                SetProperty(ref _progressCompleted, value, "ProgressCompleted");
            }
        }

        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                SetProperty(ref _version, value, "Version");
            }
        }

        public LauncherWindowViewModel()
        {
            _progressMessage = "Initializing...";
            _version = Resources.ApplicationVersion;
            if (base.IsInDesignMode)
            {
                ProgressPercentage = 67;
            }
            else
            {
                SubscribeWithEventAggregator();
            }
        }

        public override async Task InitializeAsync()
        {
            ProgressCompleted = false;
            try
            {
                Logger.Initializing("directories");
                DataManager.Current.InitializeDirectories();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "InitializeAsync");
                ex.Data["warning"] = "Failed to initialize directories";
                MessageDialogService.ShowException(ex);
            }
            if (ApplicationManager.Current.IsInDeveloperMode)
            {
                try
                {
                    Logger.Initializing("file logger");
                    DataManager.Current.InitializeFileLogger();
                }
                catch (Exception ex2)
                {
                    Logger.Exception(ex2, "InitializeAsync");
                    MessageDialogService.ShowException(ex2);
                }
            }
            try
            {
                Logger.Initializing("image resources");
                DataManager.Current.CopyPortraitsFromResources();
                DataManager.Current.CopyCompanionPortraitsFromResources();
                DataManager.Current.CopySymbolsFromResources();
                DataManager.Current.CopyDragonmarksFromResources();
            }
            catch (Exception ex3)
            {
                Logger.Exception(ex3, "InitializeAsync");
                ex3.Data["warning"] = "Failed to initialize resources.";
                MessageDialogService.ShowException(ex3);
            }
            await DataManager.Current.InitializeElementDataAsync();
            ProgressMessage = "Starting Aurora Builder";
            ProgressCompleted = true;
        }

        public void OnHandleEvent(DataManagerProgressChanged args)
        {
            ProgressMessage = args.ProgressMessage;
            ProgressPercentage = args.ProgressPercentage;
        }
    }
}
