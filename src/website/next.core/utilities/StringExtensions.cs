using System.Globalization;

namespace next.core.utilities
{
    internal static class StringExtensions
    {
        public static string ToTitleCase(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;
            return textInfo.ToTitleCase(s.ToLower());
        }

        public static string TrimSlash(this string s)
        {
            const char slash = '/';
            if (string.IsNullOrWhiteSpace(s)) return s;
            if (!s.Contains(slash)) return s;
            var slashes = new[] { slash };
            var trimmed = s.Trim();
            return trimmed.TrimEnd(slashes);
        }

        private static readonly TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
    }
}