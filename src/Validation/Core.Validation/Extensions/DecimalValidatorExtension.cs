
namespace Core.Validation.Extensions
{
    public static class DecimalValidatorExtension
    {
        public static bool IsInRange(this decimal value, int min, int max)
        {
            if (value >= min && value <= max)
                return true;
            return false;
        }
    }
}
