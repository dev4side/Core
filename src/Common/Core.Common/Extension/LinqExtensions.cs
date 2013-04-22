using System;
using System.Collections.Generic;

namespace Core.Common.Extension
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Retrieve the next item on a list with a specific condition.
        /// </summary>
        /// <typeparam name="T">The type of objects of list.</typeparam>
        /// <param name="source">The list.</param>
        /// <param name="predicate">The condition for retrieve the item.</param>
        /// <returns>the previous <typeparamref name="T"/> item, otherwise the <typeparamref name="T"/> default.</returns>
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

        /// <summary>
        /// Retrieve the previous item on a list with a specific condition.
        /// </summary>
        /// <typeparam name="T">The type of objects of list.</typeparam>
        /// <param name="source">The list.</param>
        /// <param name="predicate">The condition for retrieve the item.</param>
        /// <returns>the previous <typeparamref name="T"/> item, otherwise the <typeparamref name="T"/> default.</returns>
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
