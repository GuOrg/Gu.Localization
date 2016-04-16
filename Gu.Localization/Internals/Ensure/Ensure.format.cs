#pragma warning disable SA1600 // Elements must be documented, reason: internal
#pragma warning disable SA1601 // Partial must be documented, reason: internal
namespace Gu.Localization
{
    using System;
    using System.Linq;

    internal static partial class Ensure
    {
        internal static void Format(string format, object[] args, string formatParameterName, string argsParameterName)
        {
            NotNullOrEmpty(format, "format");
            var items = FormatString.GetFormatIndices(format);
            if (!FormatString.AreItemsValid(items))
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

            if (args.Length != FormatString.CountUnique(items))
            {
                var message = $"The format string: {format} contains {items.Count} arguments but: {args.Length} arguments were provided";
                throw new ArgumentException(message, $"{formatParameterName},{argsParameterName}");
            }
        }

        internal static bool FormatMatches(string format, object[] args)
        {
            var items = FormatString.GetFormatIndices(format);
            if (!FormatString.AreItemsValid(items))
            {
                return false;
            }

            return FormatString.CountUnique(items) == (args?.Length ?? 0);
        }
    }
}
