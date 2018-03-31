namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary> Utility class for <see cref="CultureInfo"/> </summary>
    internal static class Culture
    {
        private static readonly Dictionary<string, CultureInfo> TwoLetterISOLanguageNameCultureMap;
        private static readonly Dictionary<string, CultureInfo> NameCultureMap;
        private static readonly Dictionary<RegionInfo, CultureInfo> RegionCultureMap;
        private static readonly Dictionary<string, RegionInfo> NameRegioneMap;

        static Culture()
        {
            var cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .Where(x => !string.IsNullOrEmpty(x.Name))
                .ToArray();
            TwoLetterISOLanguageNameCultureMap = cultureInfos
                .Where(x => x.Name == x.TwoLetterISOLanguageName)
                .ToDictionary(
                    x => x.TwoLetterISOLanguageName,
                    x => x,
                    StringComparer.OrdinalIgnoreCase);
            NameCultureMap = cultureInfos.ToDictionary(
                x => x.Name,
                x => x,
                StringComparer.OrdinalIgnoreCase);

            RegionCultureMap = cultureInfos
                .Where(x => !x.IsNeutralCulture)
                .ToDictionary(
                    x => new RegionInfo(x.Name),
                    x => x);

            NameRegioneMap = cultureInfos
                .Where(x => !x.IsNeutralCulture)
                .Select(x => new RegionInfo(x.Name))
                .ToDictionary(
                    x => x.Name,
                    x => x,
                    StringComparer.OrdinalIgnoreCase);
        }

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
            return TwoLetterISOLanguageNameCultureMap.TryGetValue(name, out culture) ||
                   NameCultureMap.TryGetValue(name, out culture);
        }

        internal static bool TryGet(RegionInfo region, out CultureInfo culture)
        {
            return RegionCultureMap.TryGetValue(region, out culture);
        }

        internal static bool TryGetRegion(string name, out RegionInfo region)
        {
            return NameRegioneMap.TryGetValue(name, out region);
        }

        internal static bool NameEquals(CultureInfo first, CultureInfo other)
        {
            return CultureInfoComparer.ByName.Equals(first, other);
        }

        internal static bool TwoLetterIsoLanguageNameEquals(CultureInfo first, CultureInfo other)
        {
            return CultureInfoComparer.ByTwoLetterIsoLanguageName.Equals(first, other);
        }

        ////internal static bool ThreeLetterIsoLanguageNameEquals(CultureInfo first, CultureInfo other)
        ////{
        ////    return CultureInfoComparer.ByThreeLetterISOLanguageName.Equals(first, other);
        ////}

        internal static bool IsInvariant(this CultureInfo culture)
        {
            return NameEquals(culture, CultureInfo.InvariantCulture);
        }
    }
}
