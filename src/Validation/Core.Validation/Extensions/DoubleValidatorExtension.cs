
namespace Core.Validation.Extensions
{
    public static class DoubleValidatorExtension
    {
        public static bool IsInRange(this double value, int min, int max)
        {
            if (value >= min && value <= max)
                return true;
            return false;
        }

        public static bool IsNotInRange(this double value, int min, int max)
        {
            return !IsInRange(value, min, max);
        }

        public static bool IsInRange(this double value, double min, double max)
        {
            if (value >= min && value <= max)
                return true;
            return false;
        }

        public static bool IsNotInRange(this double value, double min, double max)
        {
            return !IsInRange(value, min, max);
        }
    }
}
