using System;
using System.Collections;

namespace Core.Common.Extension
{
    /// <summary>
    /// Defines methods to support the comparison of DateTime objects for equality.
    /// </summary>
    public class DateTimeEqualityComparer : IEqualityComparer
    {
        private readonly TimeSpan _maxDifference;

        public DateTimeEqualityComparer(TimeSpan maxDifference)
        {
            this._maxDifference = maxDifference;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns></returns>
        public new bool Equals(object x, object y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            
            if (x is DateTime && y is DateTime)
            {
                var dt1 = (DateTime)x;
                var dt2 = (DateTime)y;
                var duration = (dt1 - dt2).Duration();
                return duration < _maxDifference;
            }

            return x.Equals(y);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The System.Object for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }
    }
}