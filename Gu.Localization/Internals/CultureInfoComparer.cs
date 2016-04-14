namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>A comparer for <see cref="CultureInfo"/> </summary>
    internal class CultureInfoComparer : IEqualityComparer<CultureInfo>
    {
        /// <summary> Gets a comparer that compares by <see cref="CultureInfo.TwoLetterISOLanguageName"/> </summary>
        internal static readonly IEqualityComparer<CultureInfo> Default = new CultureInfoComparer(x => x.TwoLetterISOLanguageName);

        /// <summary> Gets a comparer that compares by <see cref="CultureInfo.TwoLetterISOLanguageName"/> </summary>
        internal static readonly IEqualityComparer<CultureInfo> ByTwoLetterIsoLanguageName = new CultureInfoComparer(x => x.TwoLetterISOLanguageName);

        /// <summary> Gets a comparer that compares by <see cref="CultureInfo.Name"/> </summary>
        internal static readonly IEqualityComparer<CultureInfo> ByName = new CultureInfoComparer(x => x.Name);

        private readonly Func<CultureInfo, string> nameGetter;

        private CultureInfoComparer(Func<CultureInfo, string> nameGetter)
        {
            this.nameGetter = nameGetter;
        }

        /// <summary> Calls Default.Equals(x, y) </summary>
        /// <param name="x">The x</param>
        /// <param name="y">The y</param>
        /// <returns>True if <paramref name="x"/> and <paramref name="y"/> are equal.</returns>
        public static bool Equals(CultureInfo x, CultureInfo y)
        {
            return Default.Equals(x, y);
        }

        /// <inheritdoc />
        bool IEqualityComparer<CultureInfo>.Equals(CultureInfo x, CultureInfo y)
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
        int IEqualityComparer<CultureInfo>.GetHashCode(CultureInfo obj)
        {
            Ensure.NotNull(obj, nameof(obj));
            return this.nameGetter(obj).GetHashCode();
        }
    }
}