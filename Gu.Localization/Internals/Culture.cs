namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary> Utility class for <see cref="CultureInfo"/> </summary>
    internal static class Culture
    {
        private static readonly HashSet<string> CultureNames = CreateCultureNames();

        /// <summary>Check if <paramref name="name"/> is the name of a culture</summary>
        /// <param name="name">The name</param>
        /// <returns>True if <paramref name="name"/> is a culture name.</returns>
        internal static bool Exists(string name)
        {
            return CultureNames.Contains(name);
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

        private static HashSet<string> CreateCultureNames()
        {
            var cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures)
                                          .Where(x => !string.IsNullOrEmpty(x.Name))
                                          .ToArray();
            var allNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            allNames.UnionWith(cultureInfos.Select(x => x.TwoLetterISOLanguageName));
            allNames.UnionWith(cultureInfos.Select(x => x.Name));
            return allNames;
        }
    }
}