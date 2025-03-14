using System;
using System.Windows.Input;
using Builder.Core.Events;
using Builder.Presentation.Services;

namespace Builder.Presentation.Commands
{
    public class ManageRestrictionsCommand : ICommand
    {
        private readonly IEventAggregator _eventAggregator;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public ManageRestrictionsCommand(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _eventAggregator.Send(new SelectionRuleNavigationArgs(NavigationLocation.StartSources));
        }
    }
}
