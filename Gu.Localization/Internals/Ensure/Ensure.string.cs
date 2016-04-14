#pragma warning disable SA1600 // Elements must be documented, reason: internal
#pragma warning disable SA1601 // Partial must be documented, reason: internal
namespace Gu.Localization
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.RegularExpressions;

    [SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "All this class does are precondition checks.")]
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
