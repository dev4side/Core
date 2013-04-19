
namespace Core.Validation.Extensions
{
    public static class IntValidatorExtension
    {
        public static bool IsInRange(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        public static bool IsNotInRange(this int value, int min, int max)
        {
            return !value.IsInRange(min, max);
        }

        public static bool IsInRange(this int? value, int min, int max)
        {
            if (!value.HasValue)
            {
                return false;
            }
                
            return value >= min && value <= max;
        }

        public static bool IsNotInRange(this int? value, int min, int max)
        {
            return !value.IsInRange(min, max);
        }
    }
}
