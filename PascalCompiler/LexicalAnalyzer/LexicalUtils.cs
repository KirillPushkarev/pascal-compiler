using System.Text.RegularExpressions;

namespace PascalCompiler
{
    public static class LexicalUtils
    {
        public static bool IsDigit(char ch)
        {
            return Regex.IsMatch(ch.ToString(), @"^[0-9]+$");
        }

        public static bool IsDecimalSeparator(char ch)
        {
            return Regex.IsMatch(ch.ToString(), @"^[\.]+$");
        }

        public static bool IsIdentifierStart(char ch)
        {
            return Regex.IsMatch(ch.ToString(), @"[a-z_]$", RegexOptions.IgnoreCase);
        }

        public static bool IsIdentifierChar(char ch)
        {
            return Regex.IsMatch(ch.ToString(), @"[a-z0-9_]$", RegexOptions.IgnoreCase);
        }

        public static bool IsStringConstantStart(char ch)
        {
            return Regex.IsMatch(ch.ToString(), @"[']$", RegexOptions.IgnoreCase);
        }
    }
}
