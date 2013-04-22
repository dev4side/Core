namespace Core.Common.Extension
{
    public static class StringExtensions
    {
        private const int DefaultCharectersToTruncate = 20;

        public static string TruncateWithDots(this string text)
        {
            return text.TruncateWithDots(DefaultCharectersToTruncate);
        }

        public static string TruncateWithDots(this string text, int charactersToTruncate)
        {
            var truncatedString = text.Substring(0, text.Length > charactersToTruncate ? charactersToTruncate : text.Length);
            truncatedString += "...";
            return truncatedString;
        }
    }
}
