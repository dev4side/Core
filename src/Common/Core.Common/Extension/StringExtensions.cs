namespace Core.Common.Extension
{
    public static class StringExtensions
    {
        private const int DEFAULT_CHARACTERS_TO_TRUNCATE = 20;

        public static string TruncateWithDots(this string text)
        {
            return text.TruncateWithDots(DEFAULT_CHARACTERS_TO_TRUNCATE);
        }

        public static string TruncateWithDots(this string text, int charactersToTruncate)
        {
            var truncatedString = text.Substring(0, text.Length > charactersToTruncate ? charactersToTruncate : text.Length);
            truncatedString += "...";
            return truncatedString;
        }
    }
}
