using Builder.Core.Logging;
using Builder.Data.Rules;
using Builder.Presentation.Interfaces;
using System.Diagnostics;
using System.Text;

namespace Builder.Presentation.Utilities
{
    public static class CharacterFileVerification
    {
        public static bool IsEqualCrC(string existing, ISelectionRuleExpander expander)
        {
            return existing.Equals(GenerateCrC(expander));
        }

        public static string GenerateCrC(SelectRule rule, int expanderNumber)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(rule.ElementHeader.Id);
            stringBuilder.Append(rule.Attributes.Name);
            stringBuilder.Append(rule.Attributes.Type);
            stringBuilder.Append(rule.Attributes.RequiredLevel);
            stringBuilder.Append(expanderNumber);
            string text = $"{rule.ElementHeader.Id}{rule.Attributes.Name}{rule.Attributes.Type}{rule.Attributes.RequiredLevel}{expanderNumber}";
            if (rule.Attributes.Type.Equals("Spell") && rule.Attributes.ContainsSupports())
            {
                stringBuilder.Append(rule.Attributes.Supports);
                text += rule.Attributes.Supports;
            }
            if (!stringBuilder.ToString().Equals(text) && Debugger.IsAttached)
            {
                Debugger.Break();
            }
            byte[] array = new Crc32().ComputeHash(Encoding.UTF8.GetBytes(text));
            string text2 = "";
            byte[] array2 = array;
            foreach (byte b in array2)
            {
                text2 += b.ToString("x2");
            }
            Logger.Debug("CRC: " + text + " => " + text2);
            return text2;
        }

        public static string GenerateCrC(ISelectionRuleExpander expander)
        {
            return GenerateCrC(expander.SelectionRule, expander.Number);
        }

        public static string GetCrC(this SelectRule rule, int number)
        {
            return GenerateCrC(rule, number);
        }
    }
}
