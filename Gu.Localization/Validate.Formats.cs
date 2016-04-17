namespace Gu.Localization
{
    using System;

    public partial class Validate
    {
        public static void Format<T>(string format, T arg)
        {
            int count;
            bool? anyItemHasFormat;
            if (!FormatString.IsValidFormat(format, out count, out anyItemHasFormat))
            {
                throw new FormatException($"Invalid format string: {format}.");
            }

            throw new NotImplementedException("check if T is IFormattable if(anyItemHasFormat == true) ");

            if (count != 1)
            {
                throw new FormatException($"Invalid format string: {format} for the single argument: {arg}.");
            }
        }

        public static bool IsValidFormat<T>(string format, T arg)
        {
            int count;
            bool? anyItemHasFormat;
            if (!FormatString.IsValidFormat(format, out count, out anyItemHasFormat))
            {
                return false;
            }

            throw new NotImplementedException("check if T is IFormattable if(anyItemHasFormat == true) ");

            return count == 1;
        }
    }
}
