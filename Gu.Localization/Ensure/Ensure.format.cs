namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal static partial class Ensure
    {
        internal static void Format(string format, object[] args, string formatParameterName, string argsParameterName)
        {
            NotNullOrEmpty(format, "format");
            var items = GetFormatItems(format);
            if (!AreItemsIntsZeroToN(items))
            {
                var joined = string.Join(", ", items.Select(x => $"{{{x}}}"));
                var message = $"Expected the format items to be [0..n). They were: {joined}";
                throw new ArgumentException(message, $"{formatParameterName},{argsParameterName}");
            }

            if (items.Count == 0)
            {
                if (args == null || args.Length == 0)
                {
                    return;
                }

                var message = $"The format string: {format} contains no arguments but: {string.Join(",", args)} was passed as args";
                throw new ArgumentException(message, $"{formatParameterName},{argsParameterName}");
            }

            if (args == null || args.Length == 0)
            {
                var message = $"The format string: {format} contains {items.Count} arguments but: no arguments were passed.";
                throw new ArgumentException(message, $"{formatParameterName},{argsParameterName}");
            }

            if (args.Length != items.Count)
            {
                var message = $"The format string: {format} contains {items.Count} arguments but: {args.Length} arguments were provided";
                throw new ArgumentException(message, $"{formatParameterName},{argsParameterName}");
            }
        }

        internal static bool FormatMatches(string format, object[] args)
        {
            var items = GetFormatItems(format);
            if (!AreItemsIntsZeroToN(items))
            {
                return false;
            }

            return items.Count == (args?.Length ?? 0);
        }

        internal static bool AreItemsIntsZeroToN(IReadOnlyCollection<string> items)
        {
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
            }

            return true;
        }

        internal static IReadOnlyCollection<string> GetFormatItems(string format)
        {
            var matches = Regex.Matches(format, @"{(?<index>\d+)}");
            var items = matches.Cast<Match>()
                               .Select(x => x.Groups["index"].Value)
                               .Distinct()
                               .ToList();
            return items;
        }
    }
}
