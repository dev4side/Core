using System;
using System.Collections.Generic;

namespace Core.Validation.Extensions
{
    public static class GenericListExtension
    {
        public static bool Validate<T>(this IList<T> list, Func<T, bool> function)
        {
            var validationResult = true;

            foreach (var item in list)
            {
                var functionResult = function.Invoke(item);
                validationResult = validationResult & functionResult;
            }
            return validationResult;
        }

        public static bool ValidateCountGraterThanZero<T>(this IList<T> list)
        {
            return list.Count > 0;
        }
    }
}
