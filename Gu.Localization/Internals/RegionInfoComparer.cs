namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>A comparer for <see cref="RegionInfo"/>. </summary>
    internal sealed class RegionInfoComparer : IEqualityComparer<RegionInfo>, IComparer<RegionInfo>
    {
        /// <summary> Gets a comparer that compares by <see cref="RegionInfo.TwoLetterISORegionName"/>. </summary>
        internal static readonly RegionInfoComparer ByTwoLetterISORegionName = new(x => x?.TwoLetterISORegionName);

        /// <summary> Gets a comparer that compares by <see cref="RegionInfo.Name"/>. </summary>
        internal static readonly RegionInfoComparer ByName = new(x => x?.Name);

        private static readonly StringComparer StringComparer = StringComparer.OrdinalIgnoreCase;

        private readonly Func<RegionInfo?, string?> nameGetter;

        private RegionInfoComparer(Func<RegionInfo?, string?> nameGetter)
        {
            this.nameGetter = nameGetter;
        }

        /// <inheritdoc />
        public bool Equals(RegionInfo? x, RegionInfo? y)
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
        public int GetHashCode(RegionInfo obj)
        {
            return this.nameGetter(obj) is { } name
                ? StringComparer.GetHashCode(name)
                : 0;
        }

        public int Compare(RegionInfo? x, RegionInfo? y)
        {
            return string.Compare(this.nameGetter(x), this.nameGetter(y), StringComparison.Ordinal);
        }
    }
}
