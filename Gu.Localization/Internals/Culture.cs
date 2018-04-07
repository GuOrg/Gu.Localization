namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary> Utility class for <see cref="CultureInfo"/> </summary>
    internal static class Culture
    {
        internal static readonly IReadOnlyList<CultureInfo> AllCultures =
            CultureInfo.GetCultures(CultureTypes.AllCultures)
                       .Where(x => !IsInvariant(x))
                       .ToArray();

        internal static readonly IReadOnlyDictionary<CultureInfo, RegionInfo> CultureRegionMap =
            AllCultures
                .Where(x => !x.IsNeutralCulture)
                .ToDictionary(
                    x => x,
                    x => new RegionInfo(x.Name),
                    CultureInfoComparer.ByName);

        internal static readonly IEnumerable<RegionInfo> AllRegions = CultureRegionMap.Values;

        private static readonly Dictionary<string, CultureInfo> TwoLetterISOLanguageNameCultureMap =
            AllCultures.Where(x => x.Name == x.TwoLetterISOLanguageName)
                       .ToDictionary(
                           x => x.TwoLetterISOLanguageName,
                           x => x,
                           StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<string, CultureInfo> NameCultureMap =
            AllCultures.ToDictionary(
                x => x.Name,
                x => x,
                StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<string, RegionInfo> NameRegionMap =
            AllRegions
                .ToDictionary(
                    x => x.Name,
                    x => x,
                    StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<CultureInfo, RegionInfo> NeutralCultureRegionMap =
            AllCultures
                .Where(x => x.IsNeutralCulture)
                .Select(x => NameCultureMap[CultureInfo.CreateSpecificCulture(x.Name).Name])
                .Distinct(CultureInfoComparer.ByTwoLetterIsoLanguageName)
                .ToDictionary(
                    x => x.Parent,
                    x => CultureRegionMap[x],
                    CultureInfoComparer.ByTwoLetterIsoLanguageName);

        internal static bool TryGet(string name, out CultureInfo culture)
        {
            if (name == null)
            {
                culture = null;
                return false;
            }

            return NameCultureMap.TryGetValue(name, out culture) ||
                   TwoLetterISOLanguageNameCultureMap.TryGetValue(name, out culture);
        }

        internal static bool TryGetRegion(CultureInfo culture, out RegionInfo region)
        {
            if (culture == null)
            {
                region = null;
                return false;
            }

            if (culture.IsNeutralCulture)
            {
                return NeutralCultureRegionMap.TryGetValue(culture, out region);
            }

            return NameRegionMap.TryGetValue(culture.Name, out region);
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
