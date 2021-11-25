namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>A comparer for <see cref="CultureInfo"/>. </summary>
    internal sealed class CultureInfoComparer : IEqualityComparer<CultureInfo>, IComparer<CultureInfo>
    {
        /// <summary> Gets a comparer that compares by <see cref="CultureInfo.TwoLetterISOLanguageName"/>. </summary>
        internal static readonly CultureInfoComparer ByTwoLetterIsoLanguageName = new(x => x?.TwoLetterISOLanguageName);

        /// <summary> Gets a comparer that compares by <see cref="CultureInfo.Name"/>. </summary>
        internal static readonly CultureInfoComparer ByName = new(x => x?.Name);

        private static readonly StringComparer StringComparer = StringComparer.OrdinalIgnoreCase;

        private readonly Func<CultureInfo?, string?> nameGetter;

        private CultureInfoComparer(Func<CultureInfo?, string?> nameGetter)
        {
            this.nameGetter = nameGetter;
        }

        /// <inheritdoc />
        public bool Equals(CultureInfo? x, CultureInfo? y)
        {
            if (x is null && y is null)
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return StringComparer.Equals(this.nameGetter(x), this.nameGetter(y));
        }

        /// <inheritdoc />
        public int GetHashCode(CultureInfo? obj)
        {
            return this.nameGetter(obj) is { } name
                ? StringComparer.GetHashCode(name)
                : 0;
        }

        public int Compare(CultureInfo? x, CultureInfo? y)
        {
            return string.Compare(this.nameGetter(x), this.nameGetter(y), StringComparison.Ordinal);
        }
    }
}
