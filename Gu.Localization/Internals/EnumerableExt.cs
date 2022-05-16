namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;

    internal static class EnumerableExt
    {
        internal static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource element)
        {
            return PrependIterator(source ?? throw new ArgumentNullException(nameof(source)), element);

            static IEnumerable<TSource> PrependIterator(IEnumerable<TSource> source, TSource element)
            {
                yield return element;

                foreach (var item in source)
                {
                    yield return item;
                }
            }
        }
    }
}
