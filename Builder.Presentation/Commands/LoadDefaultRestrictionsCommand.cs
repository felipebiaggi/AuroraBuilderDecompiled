using System;
using System.Windows.Input;
using Builder.Presentation;
using Builder.Presentation.Services.Sources;

namespace Builder.Presentation.Commands
{
    public class LoadDefaultRestrictionsCommand : ICommand
    {
        private readonly CharacterManager _manager;

        private readonly SourcesManager _sourcesManager;

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

        public LoadDefaultRestrictionsCommand(CharacterManager manager)
        {
            _manager = manager;
            _sourcesManager = manager.SourcesManager;
        }

        public bool CanExecute(object parameter)
        {
            return _manager.Status.IsLoaded;
        }

        public void Execute(object parameter)
        {
            _sourcesManager.LoadDefaults();
            _sourcesManager.ApplyRestrictions(reprocess: true);
        }
    }
}
