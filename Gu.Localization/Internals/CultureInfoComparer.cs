namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;

    public class CultureInfoComparer : IEqualityComparer<CultureInfo>
    {
        public static readonly IEqualityComparer<CultureInfo> Default = new CultureInfoComparer(x => x.TwoLetterISOLanguageName);
        public static readonly IEqualityComparer<CultureInfo> ByTwoLetterIsoLanguageName = new CultureInfoComparer(x => x.TwoLetterISOLanguageName);
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

        int IEqualityComparer<CultureInfo>.GetHashCode(CultureInfo obj)
        {
            Ensure.NotNull(obj, nameof(obj));
            return this.nameGetter(obj).GetHashCode();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private new static bool Equals(object x, object y)
        {
            throw new NotSupportedException("This is meant to be hidden");
        }
    }
}