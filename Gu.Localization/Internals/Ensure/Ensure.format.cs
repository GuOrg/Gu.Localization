namespace Gu.Localization
{
    using System;

    internal static partial class Ensure
    {
        internal static void Format(string format, object[] args, string formatParameterName, string argsParameterName)
        {
            NotNullOrEmpty(format, "format");
            int count;
            bool? anyItemHasFormat;
            if (!FormatString.IsValidFormat(format, out count, out anyItemHasFormat) || count < 0)
            {
                var message = $"Expected the format items to be [0..1..n). They format was: {format}";
                throw new ArgumentException(message, $"{formatParameterName},{argsParameterName}");
            }

            if (count == 0)
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
                var message = $"The format string: {format} contains {count} arguments but: no arguments were passed.";
                throw new ArgumentException(message, $"{formatParameterName},{argsParameterName}");
            }

            if (args.Length != count)
            {
                var message = $"The format string: {format} contains {count} arguments but: {args.Length} arguments were provided";
                throw new ArgumentException(message, $"{formatParameterName},{argsParameterName}");
            }
        }

        internal static bool FormatMatches(string format, object[] args)
        {
            int count;
            bool? anyItemHasFormat;
            if (!FormatString.IsValidFormat(format, out count, out anyItemHasFormat) || count < 0)
            {
                return false;
            }

            return count == (args?.Length ?? 0);
        }
    }
}
