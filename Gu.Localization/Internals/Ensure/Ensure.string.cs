namespace Gu.Localization
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

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
    }
}
