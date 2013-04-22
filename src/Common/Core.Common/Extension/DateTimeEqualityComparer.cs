using System;
using System.Collections;

namespace Core.Common.Extension
{
    public class DateTimeEqualityComparer : IEqualityComparer
    {
        private readonly TimeSpan _maxDifference;

        public DateTimeEqualityComparer(TimeSpan maxDifference)
        {
            this._maxDifference = maxDifference;
        }

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

        public int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }
    }
}