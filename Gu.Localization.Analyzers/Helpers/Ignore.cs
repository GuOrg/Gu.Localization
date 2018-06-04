namespace Gu.Localization.Analyzers
{
    using System;

    [Obsolete("Don't use this")]
    internal static class Ignore
    {
        // ReSharper disable once UnusedParameter.Global
        internal static void IgnoreReturnValue<T>(this T _)
        {
        }
    }
}
