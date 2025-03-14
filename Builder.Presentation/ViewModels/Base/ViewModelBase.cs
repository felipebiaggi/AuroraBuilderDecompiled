using Builder.Core;
using Builder.Core.Events;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace Builder.Presentation.ViewModels.Base
{
    public class ViewModelBase : ObservableObject
    {
        protected IEventAggregator EventAggregator => ApplicationManager.Current.EventAggregator;

        public ApplicationSettings Settings => ApplicationManager.Current.Settings;

        public bool IsInDebugMode => Debugger.IsAttached;

        public bool IsInDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());

        public bool IsInDeveloperMode => ApplicationManager.Current.IsInDeveloperMode;

        protected ViewModelBase()
        {
        }

        protected void SubscribeWithEventAggregator()
        {
            EventAggregator.Subscribe(this);
        }

        public virtual Task InitializeAsync()
        {
            return InitializeAsync(null);
        }

        public virtual async Task InitializeAsync(InitializationArguments args)
        {
            await Task.FromResult(result: true);
        }

        protected virtual void InitializeDesignData()
        {
        }
    }
}
