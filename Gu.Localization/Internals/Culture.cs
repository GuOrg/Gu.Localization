namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary> Utility class for <see cref="CultureInfo"/> </summary>
    internal static class Culture
    {
        internal static IReadOnlyList<CultureInfo> AllCultures =
            CultureInfo.GetCultures(CultureTypes.AllCultures)
                .Where(x => !IsInvariant(x))
                .ToArray();

        internal static IReadOnlyList<RegionInfo> AllRegions
        {
            get
            {
                return AllCultures.Select(x => TryGetRegion(x, out var region) ? region : null).Where(x => x != null).ToList();
            }
        }

        internal static bool TryGet(string name, out CultureInfo culture)
        {
            if (name == null)
            {
                culture = null;
                return false;
            }

            try
            {
                culture = CultureInfo.GetCultureInfo(name);
                return true;
            }
            catch (CultureNotFoundException)
            {
                // CultureInfo.GetCultureInfo throws a CultureNotFoundException if
                // the a culture with the given name is not installed on the system.
                // https://referencesource.microsoft.com/#mscorlib/system/globalization/cultureinfo.cs,589
                culture = null;
                return false;
            }
        }

        internal static bool TryGetRegion(CultureInfo culture, out RegionInfo region)
        {
            if (culture == null || culture.IsInvariant())
            {
                region = null;
                return false;
            }

            if (culture.IsNeutralCulture)
            {
                try
                {
                    culture = CultureInfo.CreateSpecificCulture(culture.Name);
                    if (culture.IsNeutralCulture)
                    {
                        // See https://github.com/GuOrg/Gu.Localization/issues/82
                        region = null;
                        return false;
                    }
                }
                catch (CultureNotFoundException)
                {
                    // This is only possible if the given culture is a mock culture.
                    region = null;
                    return false;
                }
            }

            try
            {
                region = new RegionInfo(culture.Name);
                return true;
            }
            catch (ArgumentException)
            {
                // Odd that this exception is not a CultureNotFoundException. But that's what Microsoft decided to throw
                // https://referencesource.microsoft.com/#mscorlib/system/globalization/regioninfo.cs,86
                region = null;
                return false;
            }
        }

        internal static bool NameEquals(CultureInfo first, CultureInfo other)
        {
            return CultureInfoComparer.ByName.Equals(first, other);
        }

        internal static bool TwoLetterIsoLanguageNameEquals(CultureInfo first, CultureInfo other)
        {
            return CultureInfoComparer.ByTwoLetterIsoLanguageName.Equals(first, other);
        }

        internal static bool IsInvariant(this CultureInfo culture)
        {
            return NameEquals(culture, CultureInfo.InvariantCulture);
        }
    }
}
