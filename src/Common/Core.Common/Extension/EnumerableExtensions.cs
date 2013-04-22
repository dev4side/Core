using System.Collections.Generic;
using System.Linq;

namespace Core.Common.Extension
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Join a list into a provided format with separato.
        /// </summary>
        /// <typeparam name="T">The type in which applicate the method.</typeparam>
        /// <param name="list">The list in which applicate the method.</param>
        /// <param name="separator">The separator for the join.</param>
        /// <param name="formatString">The format in which join the list.</param>
        /// <returns></returns>
        public static string JoinFormat<T>(this IEnumerable<T> list, string separator, string formatString)
        {
            formatString = string.IsNullOrWhiteSpace(formatString) ? "{0}" : formatString;
            return string.Join(separator, list.Select(item => string.Format(formatString, item)));
        }
    }
}
