namespace Gu.Localization
{
    using System;

    public partial class Validate
    {
        internal static void Format(string format, object arg)
        {
            int count;
            bool? anyItemHasFormat;
            if (!FormatString.IsValidFormat(format, out count, out anyItemHasFormat))
            {
                throw new FormatException($"Invalid format string: {format}.");
            }

            if (count != 1)
            {
                throw new FormatException($"Invalid format string: {format} for the single argument: {arg}.");
            }
        }
    }
}
