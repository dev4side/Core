using System;
using System.ComponentModel;

namespace Core.Common.Extension
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Checks if the value contains the provided object.
        /// </summary>
        /// <typeparam name="T">The type in which applicate the comparison.</typeparam>
        /// <param name="type">The enumeration in which applicate the comparison.</param>
        /// <param name="value">The object for the comparison.</param>
        /// <returns>true if the <param name="value"/> contains the provided object, otherwise false.</returns>
        public static bool Has<T>(this System.Enum type, T value)
        {
            try
            {
                return (((int)(object)type & (int)(object)value) == (int)(object)value);
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Checks if the value is only the provided type.
        /// </summary>
        /// <typeparam name="T">The type in which applicate the comparison.</typeparam>
        /// <param name="type">The enumeration in which applicate the comparison.</param>
        /// <param name="value">The object for the comparison.</param>
        /// <returns>true if the <param name="value"/> is the provided object, otherwise false.</returns>
        public static bool Is<T>(this System.Enum type, T value)
        {
            try
            {
                return (int)(object)type == (int)(object)value;
            }
            catch
            {
                return false;
            }
        }

        public static string ToDescriptionString(this Enum val)
        {
            var attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}