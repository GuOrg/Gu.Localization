namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>A comparer for <see cref="CultureInfo"/> </summary>
    internal class CultureInfoComparer : IEqualityComparer<CultureInfo>, IComparer<CultureInfo>
    {
        /// <summary> Gets a comparer that compares by <see cref="CultureInfo.TwoLetterISOLanguageName"/> </summary>
        internal static readonly CultureInfoComparer ByTwoLetterIsoLanguageName = new CultureInfoComparer(x => x?.TwoLetterISOLanguageName);

        /// <summary> Gets a comparer that compares by <see cref="CultureInfo.Name"/> </summary>
        internal static readonly CultureInfoComparer ByName = new CultureInfoComparer(x => x?.Name);

        private readonly Func<CultureInfo, string> nameGetter;

        private CultureInfoComparer(Func<CultureInfo, string> nameGetter)
        {
            this.nameGetter = nameGetter;
        }

        /// <inheritdoc />
        public bool Equals(CultureInfo x, CultureInfo y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return this.nameGetter(x) == this.nameGetter(y);
        }

        /// <inheritdoc />
        public int GetHashCode(CultureInfo obj)
        {
            Ensure.NotNull(obj, nameof(obj));
            return this.nameGetter(obj).GetHashCode();
        }

        public int Compare(CultureInfo x, CultureInfo y)
        {
            return string.Compare(this.nameGetter(x), this.nameGetter(y), StringComparison.Ordinal);
        }
    }
}