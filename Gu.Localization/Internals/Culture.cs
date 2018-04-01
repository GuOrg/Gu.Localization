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
                       .Where(x => !string.IsNullOrEmpty(x.Name))
                       .ToArray();

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

        private static readonly Dictionary<string, RegionInfo> RegionNameMap =
            AllCultures
                .Where(x => !x.IsNeutralCulture)
                .Select(x => new RegionInfo(x.Name))
                .ToDictionary(
                    x => x.Name,
                    x => x,
                    StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<CultureInfo, RegionInfo> PrimaryRegionMap =
            AllCultures
                .Where(x => x.IsNeutralCulture)
                .Select(PrimaryCulture)
                .Where(x => x?.Parent != null)
                .Distinct(CultureInfoComparer.ByTwoLetterIsoLanguageName)
                .ToDictionary(
                    x => x,
                    x => new RegionInfo(x.Name),
                    CultureInfoComparer.ByTwoLetterIsoLanguageName);

        /// <summary>Check if <paramref name="name"/> is the name of a culture</summary>
        /// <param name="name">The name</param>
        /// <returns>True if <paramref name="name"/> is a culture name.</returns>
        internal static bool Exists(string name)
        {
            return TwoLetterISOLanguageNameCultureMap.ContainsKey(name) ||
                   NameCultureMap.ContainsKey(name);
        }

        internal static bool TryGet(string name, out CultureInfo culture)
        {
            return NameCultureMap.TryGetValue(name, out culture) ||
                   TwoLetterISOLanguageNameCultureMap.TryGetValue(name, out culture);
        }

        internal static bool TryGetRegion(string name, out RegionInfo region)
        {
            return RegionNameMap.TryGetValue(name, out region) ||
                   (TwoLetterISOLanguageNameCultureMap.TryGetValue(name, out var culture) &&
                    PrimaryRegionMap.TryGetValue(culture, out region));
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

        private static CultureInfo PrimaryCulture(CultureInfo x)
        {
            return AllCultures.FirstOrDefault(c => !c.IsNeutralCulture &&
                                                   x.TwoLetterISOLanguageName == c.TwoLetterISOLanguageName);
        }
    }
}
