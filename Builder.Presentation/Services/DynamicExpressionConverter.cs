using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Builder.Presentation.Services
{
    public class DynamicExpressionConverter : IExpressionConverter
    {
        public const string SupportsIdMatchPattern = "(ID_[^+]\\w+)";

        public const string SupportsStringMatchPattern = "([/0-9-a-zA-Z_\\\\.\\\\$ \\\\w]+)";

        public const string RequirementsIdMatchPattern = "(ID_[^+]\\w+)";

        public const string BracketsExpression = "(\\[([^\\]])+])";

        public const string BracketsMatchExpression = "(\\[[^\\]]+])";

        public string SanitizeExpression(string expression)
        {
            if (expression.Contains(","))
            {
                expression = expression.Replace(",", "&&");
            }
            if (expression.Contains("&amp;"))
            {
                expression = expression.Replace("&amp;", "&");
            }
            expression = expression.Replace("&", "&&");
            while (expression.Contains("&&&"))
            {
                expression = expression.Replace("&&&", "&&");
            }
            expression = expression.Replace("|", "||");
            while (expression.Contains("|||"))
            {
                expression = expression.Replace("|||", "||");
            }
            return expression.Trim();
        }

        public string ConvertSupportsExpression(string expression, bool isRange = false)
        {
            if (isRange)
            {
                return ReplaceDistinctPattern(expression, "(ID_[^+]\\w+)", (string match) => "element.Id.Equals(\"" + match + "\")");
            }
            expression = SanitizeExpression(expression);
            if ((expression.Contains("$(spellcasting:list)") || expression.Contains("$(spellcasting:slots)")) && Debugger.IsAttached)
            {
                Debugger.Break();
            }
            expression = ReplacePattern(expression, "([/0-9-a-zA-Z_\\\\.\\\\$ \\\\w]+)", (string match) => "element.Supports.Contains(\"" + match + "\")");
            return expression;
        }

        public string ConvertRequirementsExpression(string expression)
        {
            return ConvertRequirementsExpression(expression, "ids");
        }

        public string ConvertRequirementsExpression(string expression, string listName)
        {
            expression = SanitizeExpression(expression);
            expression = ReplacePattern(expression, "(ID_[^+]\\w+)", (string match) => "evaluate.Contains(" + listName + ", \"" + match + "\")");
            expression = ReplacePattern(expression, "(\\[[^\\]]+])", delegate (string match)
            {
                KeyValuePair<string, string> keyValuePair = ParseBracketExpression(match);
                return "evaluate.Require(\"" + keyValuePair.Key + "\", \"" + keyValuePair.Value + "\")";
            });
            return expression;
        }

        public string ConvertEquippedExpression(string expression)
        {
            expression = SanitizeExpression(expression);
            expression = ReplacePattern(expression, "(\\[[^\\]]+])", delegate (string match)
            {
                KeyValuePair<string, string> keyValuePair = ParseBracketExpression(match);
                return "evaluate.Equipped(\"" + keyValuePair.Key + "\", \"" + keyValuePair.Value + "\")";
            });
            return expression;
        }

        public KeyValuePair<string, string> ParseBracketExpression(string input)
        {
            string text = input.Trim(' ', '[', ']');
            char c = (text.Contains("=") ? '=' : ':');
            string text2 = text.Split(c).LastOrDefault();
            return new KeyValuePair<string, string>(text.Replace($"{c}{text2}", ""), text2);
        }

        private string ReplacePattern(string expression, string pattern, Func<string, string> handleReplace)
        {
            List<string> list = (from Match x in Regex.Matches(expression, pattern)
                                 select x.Value.Trim()).ToList();
            string[] array = Regex.Split(expression, pattern);
            for (int i = 0; i < array.Length; i++)
            {
                string text = array[i].Trim();
                if (string.IsNullOrWhiteSpace(text))
                {
                    array[i] = text;
                }
                else if (list.Contains(text))
                {
                    array[i] = handleReplace(text);
                }
                else
                {
                    array[i] = text;
                }
            }
            return string.Join("", array).Trim();
        }

        private string ReplaceDistinctPattern(string expression, string pattern, Func<string, string> handleReplace)
        {
            List<string> list = (from Match x in Regex.Matches(expression, pattern)
                                 select x.Value.Trim()).Distinct().ToList();
            string[] array = Regex.Split(expression, pattern);
            for (int i = 0; i < array.Length; i++)
            {
                string text = array[i].Trim();
                if (string.IsNullOrWhiteSpace(text))
                {
                    array[i] = text;
                }
                else if (list.Contains(text))
                {
                    array[i] = handleReplace(text);
                }
                else
                {
                    array[i] = text;
                }
            }
            return string.Join("", array).Trim();
        }
    }
}
