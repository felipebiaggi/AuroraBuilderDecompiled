using Builder.Core.Events;

namespace Builder.Presentation.Commands
{
    internal abstract class ApplicationCommands
    {
        protected IEventAggregator EventAggregator { get; }

        protected CharacterManager Manager { get; }

        protected ApplicationCommands(IEventAggregator eventAggregator, CharacterManager manager)
        {
            EventAggregator = eventAggregator;
            Manager = manager;
        }
    }
}
