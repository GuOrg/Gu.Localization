namespace Gu.Localization
{
    using System.Collections.Generic;
    using System.Diagnostics;

    internal static class EnumerableExt
    {
        internal static IEnumerable<TSource> Append<TSource>(this IEnumerable<TSource> source, TSource element)
        {
            Debug.Assert(source != null, "source == null");
            return AppendIterator(source, element);
        }

        internal static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource element)
        {
            Debug.Assert(source != null, "source == null");
            return PrependIterator(source, element);
        }

        private static IEnumerable<TSource> AppendIterator<TSource>(IEnumerable<TSource> source, TSource element)
        {
            foreach (var e1 in source)
            {
                yield return e1;
            }

            yield return element;
        }

        private static IEnumerable<TSource> PrependIterator<TSource>(IEnumerable<TSource> source, TSource element)
        {
            yield return element;

            foreach (var e1 in source)
            {
                yield return e1;
            }
        }
    }
}