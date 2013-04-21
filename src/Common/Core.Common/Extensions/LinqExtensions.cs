using System;
using System.Collections.Generic;

namespace Core.Common.Extensions
{
    public static class LinqExtensions
    {
        public static T Next<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            bool flag = false;

            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (flag)
                    {
                        return enumerator.Current;
                    }
                    
                    if (predicate(enumerator.Current))
                    {
                        flag = true;
                    }
                }
            }

            return default(T);
        }

        public static T Previous<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            bool flag = false;

            using (var enumerator = source.GetEnumerator())
            {
                T previuos = default(T);

                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        flag = true;
                    }

                    if (flag)
                    {
                        return previuos;
                    }
                    
                    previuos = enumerator.Current;
                }
            }
            return default(T);
        }
    }
}
