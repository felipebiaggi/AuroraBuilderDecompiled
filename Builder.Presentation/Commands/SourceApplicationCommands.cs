using System.Windows.Input;
using Builder.Core.Events;
using Builder.Presentation;
using Builder.Presentation.Commands;

namespace Builder.Presentation.Commands
{
    internal class SourceApplicationCommands : Builder.Presentation.Commands.ApplicationCommands
    {
        public ICommand ManageRestrictionsCommand { get; }

        public ICommand SaveRestrictionsCommand { get; }

        public ICommand LoadRestrictionsCommand { get; }

        public ICommand ApplyRestrictionsCommand { get; }

        public SourceApplicationCommands(IEventAggregator eventAggregator, CharacterManager manager)
            : base(eventAggregator, manager)
        {
            ManageRestrictionsCommand = new ManageRestrictionsCommand(base.EventAggregator);
            SaveRestrictionsCommand = new SaveDefaultRestrictionsCommand(base.Manager);
            LoadRestrictionsCommand = new LoadDefaultRestrictionsCommand(base.Manager);
            ApplyRestrictionsCommand = new ApplyRestrictionsCommand(base.Manager);
        }
    }
}
