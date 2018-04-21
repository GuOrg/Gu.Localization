namespace Gu.Localization.Analyzers
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods that avoids allocations.
    /// </summary>
    internal static partial class EnumerableExt
    {
        internal static IEnumerable<T> Prepend<T>(this IEnumerable<T> items, T before)
        {
            yield return before;
            foreach (var item in items)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Try getting the element at <paramref name="index"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
        /// <param name="source">The source collection, can be null.</param>
        /// <param name="index">The index.</param>
        /// <param name="result">The element at index if found, can be null.</param>
        /// <returns>True if an element was found.</returns>
        internal static bool TryElementAt<T>(this IEnumerable<T> source, int index, out T result)
        {
            result = default(T);
            if (source == null)
            {
                return false;
            }

            var current = 0;
            using (var e = source.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    if (current == index)
                    {
                        result = e.Current;
                        return true;
                    }

                    current++;
                }
            }

            return false;
        }

        /// <summary>
        /// Try getting the single element in <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
        /// <param name="source">The source collection, can be null.</param>
        /// <param name="result">The single element, can be null.</param>
        /// <returns>True if an element was found.</returns>
        internal static bool TrySingle<T>(this IEnumerable<T> source, out T result)
        {
            result = default(T);
            if (source == null)
            {
                return false;
            }

            using (var e = source.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    result = e.Current;
                    if (!e.MoveNext())
                    {
                        return true;
                    }

                    return false;
                }

                result = default(T);
                return false;
            }
        }

        /// <summary>
        /// Try getting the single element in <paramref name="source"/> matching <paramref name="predicate"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
        /// <param name="source">The source collection, can be null.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="result">The single element matching the predicate, can be null.</param>
        /// <returns>True if an element was found.</returns>
        internal static bool TrySingle<T>(this IEnumerable<T> source, Func<T, bool> predicate, out T result)
        {
            result = default(T);
            if (source == null)
            {
                return false;
            }

            using (var e = source.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    result = e.Current;
                    if (predicate(result))
                    {
                        while (e.MoveNext())
                        {
                            if (predicate(e.Current))
                            {
                                result = default(T);
                                return false;
                            }
                        }

                        return true;
                    }
                }
            }

            result = default(T);
            return false;
        }

        /// <summary>
        /// Try getting the first element in <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
        /// <param name="source">The source collection, can be null.</param>
        /// <param name="result">The first element, can be null.</param>
        /// <returns>True if an element was found.</returns>
        internal static bool TryFirst<T>(this IEnumerable<T> source, out T result)
        {
            result = default(T);
            if (source == null)
            {
                return false;
            }

            using (var e = source.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    result = e.Current;
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Try getting the first element in <paramref name="source"/> matching <paramref name="predicate"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
        /// <param name="source">The source collection, can be null.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="result">The first element matching the predicate, can be null.</param>
        /// <returns>True if an element was found.</returns>
        internal static bool TryFirst<T>(this IEnumerable<T> source, Func<T, bool> predicate, out T result)
        {
            if (source == null)
            {
                result = default(T);
                return false;
            }

            using (var e = source.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    result = e.Current;
                    if (predicate(result))
                    {
                       return true;
                    }
                }
            }

            result = default(T);
            return false;
        }

        /// <summary>
        /// Try getting the first element in <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
        /// <param name="source">The source collection, can be null.</param>
        /// <param name="result">The first element, can be null.</param>
        /// <returns>True if an element was found.</returns>
        internal static bool TryLast<T>(this IEnumerable<T> source, out T result)
        {
            result = default(T);
            if (source == null)
            {
                return false;
            }

            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                {
                    return false;
                }

                while (e.MoveNext())
                {
                    result = e.Current;
                }

                return true;
            }
        }

        /// <summary>
        /// Try getting the first element in <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in <paramref name="source"/></typeparam>
        /// <typeparam name="TResult">The type to filter by.</typeparam>
        /// <param name="source">The source collection, can be null.</param>
        /// <param name="result">The first element, can be null.</param>
        /// <returns>True if an element was found.</returns>
        internal static bool TryLast<T, TResult>(this IEnumerable<T> source, out TResult result)
            where TResult : T
        {
            result = default(TResult);
            if (source == null)
            {
                return false;
            }

            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                {
                    return false;
                }

                var found = false;
                while (e.MoveNext())
                {
                    if (e.Current is TResult item)
                    {
                        result = item;
                        found = true;
                    }
                }

                return found;
            }
        }
    }
}
