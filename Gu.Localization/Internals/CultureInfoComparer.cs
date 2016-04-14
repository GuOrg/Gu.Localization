namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>A comparer for <see cref="CultureInfo"/> </summary>
    public class CultureInfoComparer : IEqualityComparer<CultureInfo>
    {
        /// <summary> Gets a comparer that compares by <see cref="CultureInfo.TwoLetterISOLanguageName"/> </summary>
        public static readonly IEqualityComparer<CultureInfo> Default = new CultureInfoComparer(x => x.TwoLetterISOLanguageName);

        /// <summary> Gets a comparer that compares by <see cref="CultureInfo.TwoLetterISOLanguageName"/> </summary>
        public static readonly IEqualityComparer<CultureInfo> ByTwoLetterIsoLanguageName = new CultureInfoComparer(x => x.TwoLetterISOLanguageName);

        /// <summary> Gets a comparer that compares by <see cref="CultureInfo.Name"/> </summary>
        public static readonly IEqualityComparer<CultureInfo> ByName = new CultureInfoComparer(x => x.Name);

        private readonly Func<CultureInfo, string> nameGetter;

        private CultureInfoComparer(Func<CultureInfo, string> nameGetter)
        {
            this.nameGetter = nameGetter;
        }

        /// <summary>
        /// Calls Default.Equals(x, y)
        /// </summary>
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

        [EditorBrowsable(EditorBrowsableState.Never)]
        //// ReSharper disable UnusedParameter.Local
        private new static bool Equals(object x, object y)
        //// ReSharper restore UnusedParameter.Local
        {
            throw new NotSupportedException("This is meant to be hidden");
        }
    }
}