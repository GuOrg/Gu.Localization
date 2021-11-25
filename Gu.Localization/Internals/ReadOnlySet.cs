namespace Gu.Localization
{
    using System.Collections.Generic;
    using System.Linq;

    internal static class ReadOnlySet
    {
        internal static ReadOnlySet<T> Create<T>(IEnumerable<T> items)
        {
            if (items.Any())
            {
                return new ReadOnlySet<T>(items);
            }

            return ReadOnlySet<T>.Empty;
        }
    }
}
