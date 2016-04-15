namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;

    internal static class FormatString
    {
        private static readonly IReadOnlyList<string> Empty = new string[0];
        private static readonly ThreadLocal<SortedSet<int>> Indexes = new ThreadLocal<SortedSet<int>>(() => new SortedSet<int>());

        /// <summary>
        /// Call with "first: {0}, second {1} returns new []{"0", "1"};
        /// </summary>
        /// <param name="format">The format string</param>
        /// <returns>An unordered list of format items found in <paramref name="format"/></returns>
        internal static IReadOnlyCollection<string> GetFormatItems(string format)
        {
            if (string.IsNullOrEmpty(format))
            {
                return Empty;
            }

            var matches = Regex.Matches(format, @"{(?<index>\d+)}");
            var items = matches.Cast<Match>()
                               .Select(x => x.Groups["index"].Value)
                               .ToList();
            return items;
        }

        internal static int CountUnique(IReadOnlyCollection<string> items)
        {
            if (items.Count == 0)
            {
                return 0;
            }

            var indexes = Indexes.Value;
            indexes.Clear();

            foreach (var item in items)
            {
                int index;
                if (!int.TryParse(item, out index))
                {
                    throw new InvalidOperationException($"Format item is not an int: {item}");
                }

                indexes.Add(index);
            }

            return indexes.Count;
        }

        /// <summary>Checks that <paramref name="items"/> are 0-n with no gaps.</summary>
        /// <param name="items">The format items</param>
        /// <returns>True if <paramref name="items"/> are 0-n with no gaps</returns>
        internal static bool AreItemsValid(IReadOnlyCollection<string> items)
        {
            if (items.Count == 0)
            {
                return true;
            }

            var indexes = Indexes.Value;
            indexes.Clear();

            foreach (var item in items)
            {
                int index;
                if (!int.TryParse(item, out index))
                {
                    return false;
                }

                if (index < 0 || index >= items.Count)
                {
                    return false;
                }

                indexes.Add(index);
            }

            return indexes.Min == 0 && indexes.Max == indexes.Count - 1;
        }
    }
}
