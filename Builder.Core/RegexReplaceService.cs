using System.Text.RegularExpressions;

namespace Builder.Core
{
    public class RegexReplaceService
    {
        public const string InlineReplacePattern = "\\$\\((.*?)\\)";

        public bool ContainsInlineReplacement(string input)
        {
            return Regex.IsMatch(input, "\\$\\((.*?)\\)", RegexOptions.IgnoreCase);
        }
    }
}
