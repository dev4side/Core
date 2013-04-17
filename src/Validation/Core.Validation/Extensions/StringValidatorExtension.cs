using System.Linq;

namespace Core.Validation.Extensions
{
    public static class StringValidatorExtension
    {
        public static bool IsLenghtLessThan(this string value, int maxLengt)
        {
            return (value.Length <= maxLengt);
        }

        public static bool IsStringOneOfThisValues(this string value, params string[] values)
        {
            return values.Count(candidateValue => value == candidateValue) != 0;
        }
    }
}
