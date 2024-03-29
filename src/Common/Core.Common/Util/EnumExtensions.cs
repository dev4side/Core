﻿using System;

namespace Core.Common.Util
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

        //appends a value
        public static T Add<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type | (int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not append value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }

        //completely removes the value
        public static T Remove<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type & ~(int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not remove value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }
    }
}
