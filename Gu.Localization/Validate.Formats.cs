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
        /// <typeparam name="T0">The type of <paramref name="arg0"/> generic to avoid boxing.</typeparam>
        /// <param name="format">The format string ex: 'First: {0:N}.</param>
        /// <param name="arg0">The argument.</param>
        public static void Format<T0>(string format, T0 arg0)
        {
            if (!FormatString.IsValidFormat(format, out var count, out _))
            {
                throw new FormatException($"Invalid format string: \"{format}\".");
            }

            // not sure if we should bother with checking individual format items here
            if (count != 1)
            {
                throw new FormatException($"Invalid format string: \"{format}\" for the single argument: {arg0}.");
            }
        }

        /// <summary>
        /// Call with Validate.IsValidFormat("First: {0:N}", 1.2);
        /// Throws a <see cref="FormatException"/> if error(s) are found.
        /// </summary>
        /// <typeparam name="T0">The type of <paramref name="arg0"/> generic to avoid boxing.</typeparam>
        /// <typeparam name="T1">The type of <paramref name="arg1"/> generic to avoid boxing.</typeparam>
        /// <param name="format">The format string ex: 'First: {0:N}.</param>
        /// <param name="arg0">The first argument.</param>
        /// <param name="arg1">The second argument.</param>
        public static void Format<T0, T1>(string format, T0 arg0, T1 arg1)
        {
            if (!FormatString.IsValidFormat(format, out var count, out _))
            {
                throw new FormatException($"Invalid format string: \"{format}\".");
            }

            // not sure if we should bother with checking individual format items here
            if (count != 2)
            {
                throw new FormatException($"Invalid format string: \"{format}\" for the two arguments: {arg0}, {arg1}.");
            }
        }

        /// <summary>
        /// Call with Validate.IsValidFormat("First: {0:N}", 1, 2, 3..);
        /// Throws a <see cref="FormatException"/> if error(s) are found.
        /// </summary>
        /// <param name="format">The format string ex: 'First: {0:N}.</param>
        /// <param name="args">The arguments.</param>
        public static void Format(string format, params object[] args)
        {
            if (!FormatString.IsValidFormat(format, out var count, out _))
            {
                throw new FormatException($"Invalid format string: \"{format}\".");
            }

            if (args is null || args.Length == 0)
            {
                if (count == 0)
                {
                    return;
                }

                throw new FormatException($"Invalid format string: \"{format}\" when no arguments.");
            }

            if (count != args.Length)
            {
                throw new FormatException($"Invalid format string: \"{format}\" for the arguments: {string.Join(", ", args)}.");
            }
        }

        /// <summary>Call with Validate.IsValidFormat("First: {0:N}", 1.2);. </summary>
        /// <typeparam name="T">The type of <paramref name="arg0"/> generic to avoid boxing.</typeparam>
        /// <param name="format">The format string ex: 'First: {0:N}.</param>
        /// <param name="arg0">The argument.</param>
        /// <returns>True if <paramref name="format"/> is valid for the argument <paramref name="arg0"/>.</returns>
#pragma warning disable CA1801 // Review unused parameters
        public static bool IsValidFormat<T>(string format, T arg0)
#pragma warning restore CA1801 // Review unused parameters
        {
            return IsValidFormat(format, 1);
        }

        /// <summary>Call with Validate.IsValidFormat("First: {0:N}, Second: {1}", 1, 2);. </summary>
        /// <typeparam name="T0">The type of <paramref name="arg0"/> generic to avoid boxing.</typeparam>
        /// <typeparam name="T1">The type of <paramref name="arg1"/> generic to avoid boxing.</typeparam>
        /// <param name="format">The format string ex: 'First: {0:N}.</param>
        /// <param name="arg0">The first argument.</param>
        /// <param name="arg1">The second argument.</param>
        /// <returns>True if <paramref name="format"/> is valid for the two arguments <paramref name="arg0"/> and <paramref name="arg1"/>.</returns>
#pragma warning disable CA1801 // Review unused parameters
        public static bool IsValidFormat<T0, T1>(string format, T0 arg0, T1 arg1)
#pragma warning restore CA1801 // Review unused parameters
        {
            return IsValidFormat(format, 2);
        }

        /// <summary>Call with Validate.IsValidFormat("First: {0:N}, Second: {1}", 2);. </summary>
        /// <param name="format">The format string ex: 'First: {0:N}.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>True if <paramref name="format"/> is valid for <paramref name="args"/>.</returns>
        public static bool IsValidFormat(string format, params object[] args)
        {
            return IsValidFormat(format, args?.Length ?? 0);
        }

        private static bool IsValidFormat(string format, int argumentCount)
        {
            if (!FormatString.IsValidFormat(format, out var count, out _))
            {
                return false;
            }

            return count == argumentCount;
        }
    }
}
