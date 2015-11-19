namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal static partial class Ensure
    {
        internal static void Format(string format, object[] args)
        {
            Ensure.NotNullOrEmpty(format, "format");
            var matches = Regex.Matches(format, @"{(?<index>\d+)}");
            if (matches.Count == 0)
            {
                if (args != null && args.Any())
                {
                    var message = string.Format("The format string: {0} contains no arguments but: {1} was passed as args", format, string.Join(",", args));
                    throw new InvalidOperationException(message);
                }
                return;
            }
            var indexes = matches.AsEnumerable()
                                 .Select(x => int.Parse(x.Groups["index"].Value))
                                 .Distinct()
                                 .OrderBy(x => x)
                                 .ToArray();
            if (indexes[0] != 0)
            {
                throw new InvalidOperationException(string.Format("Indexes must start at zero. String was: {0}", format));
            }
            if (indexes.Last() != indexes.Length - 1)
            {
                throw new InvalidOperationException(string.Format("Invalid indexes. String was: {0}", format));
            }

            if (args == null || !args.Any())
            {
                var message = string.Format("The format string: {0} contains {1} arguments but: no arguments were passed.", format, indexes.Length);
                throw new InvalidOperationException(message);
            }

            if (args.Length != indexes.Length)
            {
                var message = string.Format("The format string: {0} contains {1} arguments but: {2} arguments were provided", format, indexes.Length, args.Length);
                throw new InvalidOperationException(message);
            }
        }

        private static IEnumerable<Match> AsEnumerable(this MatchCollection col)
        {
            foreach (Match item in col)
            {
                yield return item;
            }
        }

        private static IEnumerable<Group> AsEnumerable(this GroupCollection col)
        {
            foreach (Group item in col)
            {
                yield return item;
            }
        }
    }
}
