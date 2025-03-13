using System.Globalization;
using System.IO;


namespace Builder.Presentation.Extensions
{
    public static class StringExtentions
    {
        public static string ToValueString(this int value)
        {
            if (value < 0)
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }
            return "+" + value;
        }

        public static string ToLevelString(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
            int num = int.Parse(value);
            switch (num)
            {
                case 0:
                    return value;
                case 1:
                    return "1st";
                case 2:
                    return "2nd";
                case 3:
                    return "3rd";
                default:
                    return $"{num}th";
            }
        }

        public static string ToSafeFilename(this string value, bool lowercase = true)
        {
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidFileNameChars)
            {
                value = value.Replace(c.ToString(), "");
            }
            if (!lowercase)
            {
                return value.Trim();
            }
            return value.ToLower().Trim();
        }
    }
}
