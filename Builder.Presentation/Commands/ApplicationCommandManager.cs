using System.Windows.Input;
using Builder.Core.Events;
using Builder.Presentation;
using Builder.Presentation.Commands;

namespace Builder.Presentation.Commands
{
    internal class ApplicationCommandManager
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly CharacterManager _manager;

        public SourceApplicationCommands SourceCommands { get; set; }

        public ApplicationCommandManager(IEventAggregator eventAggregator, CharacterManager manager)
        {
            _eventAggregator = eventAggregator;
            _manager = manager;
            SourceCommands = new SourceApplicationCommands(_eventAggregator, _manager);
        }

        public void InvalidateRequerySuggested()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

}
