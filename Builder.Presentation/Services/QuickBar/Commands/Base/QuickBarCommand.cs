namespace Builder.Presentation.Services.QuickBar.Commands.Base
{
    public abstract class QuickBarCommand
    {
        public string CommandName { get; }

        protected QuickBarCommand(string commandName)
        {
            CommandName = commandName;
        }

        public abstract void Execute(string parameter);
    }
}
