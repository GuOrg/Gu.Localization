﻿namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;

    /// <summary> Utility class for <see cref="CultureInfo"/>. </summary>
    internal static class Culture
    {
#pragma warning disable SA1401 // Fields should be private
        internal static readonly IReadOnlyList<CultureInfo> AllCultures =
            CultureInfo.GetCultures(CultureTypes.AllCultures)
                       .Where(x => !IsInvariant(x))
                       .ToArray();

        internal static IReadOnlyList<RegionInfo> AllRegions =
            AllCultures.Select(x => TryGetRegion(x, out var region) ? region : null)
                       .Where(x => x != null)
                       .ToList()!;
#pragma warning restore SA1401 // Fields should be private

        internal static bool TryGet(string? name, [NotNullWhen(true)] out CultureInfo? culture)
        {
            if (name is null)
            {
                culture = null;
                return false;
            }

            culture = AllCultures.SingleOrDefault(c => NameEquals(c, name));
            return culture != null;
        }

        internal static bool TryGetRegion(CultureInfo culture, [NotNullWhen(true)] out RegionInfo? region)
        {
            if (culture is null || culture.IsInvariant())
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

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1307 // Specify StringComparison for clarity
            if (!culture.Name.Contains('-'))
            {
                region = null;
                return false;
            }
#pragma warning restore CA1307 // Specify StringComparison for clarity
#pragma warning restore IDE0079 // Remove unnecessary suppression

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

        internal static bool NameEquals(CultureInfo? first, CultureInfo? other)
        {
            return CultureInfoComparer.ByName.Equals(first, other);
        }

        internal static bool NameEquals(CultureInfo culture, string name)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(culture.Name, name);
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
