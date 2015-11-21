namespace Gu.Localization.Internals
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;

    public class CultureInfoComparer : IEqualityComparer<CultureInfo>
    {
        public static readonly IEqualityComparer<CultureInfo> Default = new CultureInfoComparer(x => x.TwoLetterISOLanguageName);
        public static readonly IEqualityComparer<CultureInfo> TwoLetterISOLanguageName = new CultureInfoComparer(x => x.TwoLetterISOLanguageName);
        public static readonly IEqualityComparer<CultureInfo> FullName = new CultureInfoComparer(x => x.Name);

        private readonly Func<CultureInfo, string> _nameGetter;

        private CultureInfoComparer(Func<CultureInfo, string> nameGetter)
        {
            _nameGetter = nameGetter;
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

            return _nameGetter(x) == _nameGetter(y);
        }

        public int GetHashCode(CultureInfo obj)
        {
            Ensure.NotNull(obj, nameof(obj));
            return _nameGetter(obj).GetHashCode();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private new static bool Equals(object x, object y)
        {
            throw new NotSupportedException("This is meant to be hidden");
        }
    }
}
