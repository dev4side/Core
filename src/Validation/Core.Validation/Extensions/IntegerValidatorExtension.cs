
namespace Core.Validation.Extensions
{
    public static class IntValidatorExtension
    {
        public static bool IsInRange(this int value, int min, int max)
        {
            if ( value >= min && value <= max)
                return true;
            return false;
        }

        public static bool IsNotInRange(this int value, int min, int max)
        {
            return !value.IsInRange(min, max);
        }

        public static bool IsInRange(this int? value, int min, int max)
        {
            if (!value.HasValue)
                return false;
            if (value >= min && value <= max)
                return true;
            return false;
        }

        public static bool IsNotInRange(this int? value, int min, int max)
        {
            return !value.IsInRange(min, max);
        }

    }
}
