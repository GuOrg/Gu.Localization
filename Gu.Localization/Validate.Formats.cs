// ReSharper disable UnusedParameter.Global
namespace Gu.Localization
{
    using System;

    /// <summary>Methods for validating format resources.</summary>
    public partial class Validate
    {
        /// <summary>
        /// Call with Validate.IsValidFormat("First: {0:N}", 1.2);
        /// Throws a <see cref="FormatException"/> if error(s) are found.
        /// </summary>
        /// <typeparam name="T">Using generic to avoid boxing</typeparam>
        /// <param name="format">The format string ex: 'First: {0:N}</param>
        /// <param name="arg">The argument</param>
        public static void Format<T>(string format, T arg)
        {
            int count;
            bool? anyItemHasFormat;
            if (!FormatString.IsValidFormat(format, out count, out anyItemHasFormat))
            {
                throw new FormatException($"Invalid format string: {format}.");
            }

            // not sure if we should bother with checking individual format items here
            if (count != 1)
            {
                throw new FormatException($"Invalid format string: {format} for the single argument: {arg}.");
            }
        }

        /// <summary>Call with Validate.IsValidFormat("First: {0:N}", 1.2); </summary>
        /// <typeparam name="T">Using generic to avoid boxing</typeparam>
        /// <param name="format">The format string ex: 'First: {0:N}</param>
        /// <param name="arg">The argument</param>
        /// <returns>True if <paramref name="format"/> is valid for one argument</returns>
        public static bool IsValidFormat<T>(string format, T arg)
        {
            int count;
            bool? anyItemHasFormat;
            if (!FormatString.IsValidFormat(format, out count, out anyItemHasFormat))
            {
                return false;
            }

            return count == 1;
        }
    }
}
