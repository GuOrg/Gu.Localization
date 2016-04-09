namespace Gu.Localization
{
    using System;
    using System.Diagnostics;
    using System.Text.RegularExpressions;

    internal static partial class Ensure
    {
        internal static void NotNullOrEmpty(string value, string parameterName)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), $"{nameof(parameterName)} cannot be null");
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        internal static void IsMatch(string text, string pattern, string parameterName)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), $"{nameof(parameterName)} cannot be null");
            if (!Regex.IsMatch(text, pattern))
            {
                throw new ArgumentException(parameterName);
            }
        }
    }
}
