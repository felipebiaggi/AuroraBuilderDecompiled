namespace Builder.Presentation.ViewModels.Base
{
    public class InitializationArguments
    {
        public object Argument { get; }

        public static InitializationArguments Empty => new InitializationArguments();

        public InitializationArguments(object argument = null)
        {
            Argument = argument;
        }
    }
}
