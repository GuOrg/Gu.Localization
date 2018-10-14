namespace Gu.Localization.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using BenchmarkDotNet.Attributes;

    [MemoryDiagnoser]
    public class CultureBenchmark
    {
        private static readonly IReadOnlyList<CultureInfo> AllCultures =
            CultureInfo.GetCultures(CultureTypes.AllCultures)
                .Where(x => x.Name != CultureInfo.InvariantCulture.Name)
                .ToArray();

        private static readonly IReadOnlyDictionary<CultureInfo, RegionInfo> CultureRegionMap =
            AllCultures
                .Where(x => !x.IsNeutralCulture)
                .ToDictionary(
                    x => x,
                    x => new RegionInfo(x.Name),
                    CultureInfoComparer.ByName);

        private static readonly Dictionary<string, CultureInfo> NameCultureMap =
            AllCultures.ToDictionary(
                x => x.Name,
                x => x,
                StringComparer.OrdinalIgnoreCase);

        [Benchmark]
        public object CreateNeutralCultureRegionMap()
        {
            return AllCultures
                .Where(x => x.IsNeutralCulture)
                .Select(x => CreateSpecificCultureOrDefault(x)?.Name)
                .Where(x => x != null && NameCultureMap.ContainsKey(x))
                .Select(x => NameCultureMap[x])
                .Distinct(CultureInfoComparer.ByTwoLetterIsoLanguageName)
                .ToDictionary(
                    x => x.Parent,
                    x => CultureRegionMap[x],
                    CultureInfoComparer.ByTwoLetterIsoLanguageName);
        }

        private static CultureInfo CreateSpecificCultureOrDefault(CultureInfo neutral)
        {
            if (neutral == null ||
                !neutral.IsNeutralCulture)
            {
                return null;
            }

            try
            {
                // try-catch swallow here as CreateSpecificCulture does parsing of the name.
                // don't know if there is a way to check if a specific culture can be created.
                return CultureInfo.CreateSpecificCulture(neutral.Name);
            }
            catch
            {
                return null;
            }
        }

        private class CultureInfoComparer : IEqualityComparer<CultureInfo>, IComparer<CultureInfo>
        {
            /// <summary> Gets a comparer that compares by <see cref="CultureInfo.TwoLetterISOLanguageName"/>. </summary>
            internal static readonly CultureInfoComparer ByTwoLetterIsoLanguageName = new CultureInfoComparer(x => x?.TwoLetterISOLanguageName);

            /// <summary> Gets a comparer that compares by <see cref="CultureInfo.Name"/>. </summary>
            internal static readonly CultureInfoComparer ByName = new CultureInfoComparer(x => x?.Name);

            private static readonly StringComparer StringComparer = StringComparer.OrdinalIgnoreCase;

            private readonly Func<CultureInfo, string> nameGetter;

            private CultureInfoComparer(Func<CultureInfo, string> nameGetter)
            {
                this.nameGetter = nameGetter;
            }

            /// <inheritdoc />
            public bool Equals(CultureInfo x, CultureInfo y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                return StringComparer.Equals(this.nameGetter(x), this.nameGetter(y));
            }

            /// <inheritdoc />
            public int GetHashCode(CultureInfo obj)
            {
                if (obj == null)
                {
                    return 0;
                }

                return StringComparer.GetHashCode(this.nameGetter(obj));
            }

            public int Compare(CultureInfo x, CultureInfo y)
            {
                return string.Compare(this.nameGetter(x), this.nameGetter(y), StringComparison.Ordinal);
            }
        }
    }
}
