using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Validation.Extensions
{
    public static class GenericListExtension
    {
        public static bool Validate<T>(this IList<T> list, Func<T, bool> function)
        {
            return list.Select(function.Invoke).Aggregate(true, (current, functionResult) => current & functionResult);
        }

        public static bool ValidateCountGraterThanZero<T>(this IList<T> list)
        {
            return list.Count > 0;
        }
    }
}
