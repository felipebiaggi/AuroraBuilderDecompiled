using System.Text.RegularExpressions;

public class RegexReplaceService
{
    public const string InlineReplacePattern = "\\$\\((.*?)\\)";

    public bool ContainsInlineReplacement(string input)
    {
        return Regex.IsMatch(input, InlineReplacePattern, RegexOptions.IgnoreCase);
    }
}
