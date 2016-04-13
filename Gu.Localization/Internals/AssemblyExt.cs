namespace Gu.Localization
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;

    internal static class AssemblyExt
    {
        private static readonly ConcurrentDictionary<Assembly, IReadOnlyList<CultureInfo>> AllCulturesCache = new ConcurrentDictionary<Assembly, IReadOnlyList<CultureInfo>>();

        internal static IReadOnlyList<CultureInfo> AllCultures(this Assembly assembly)
        {
            return AllCulturesCache.GetOrAdd(assembly, GetAllCultures);
        }

        internal static IReadOnlyDictionary<CultureInfo, string> CulturesAndFileNames(this Assembly assembly)
        {
            var culturesAndFileNames = ResourceCultures.CulturesAndFileNames(assembly.Location);
            if (culturesAndFileNames != EmptyReadOnlyDictionary<CultureInfo, string>.Default)
            {
                return culturesAndFileNames;
            }

            return ResourceCultures.CulturesAndFileNames(assembly.CodeBase);
        }

        private static IReadOnlyList<CultureInfo> GetAllCultures(Assembly assembly)
        {
            var cultures = ResourceCultures.GetAllCultures(assembly.Location);
            if (cultures.Count > 0)
            {
                return cultures;
            }

            return ResourceCultures.GetAllCultures(assembly.CodeBase);
        }
    }
}
