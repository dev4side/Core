using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static string JoinFormat<T>(this IEnumerable<T> list, string separator, string formatString)
        {
            formatString = string.IsNullOrWhiteSpace(formatString) ? "{0}" : formatString;
            return string.Join(separator, list.Select(item => string.Format(formatString, item)));
        }
    }
}
