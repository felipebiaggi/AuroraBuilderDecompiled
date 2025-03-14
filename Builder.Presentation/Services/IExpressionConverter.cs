namespace Builder.Presentation.Services
{
    public interface IExpressionConverter
    {
        string SanitizeExpression(string expression);

        string ConvertSupportsExpression(string expression, bool isRange = false);

        string ConvertRequirementsExpression(string expression);

        string ConvertRequirementsExpression(string expression, string listName);

        string ConvertEquippedExpression(string expression);
    }
}
