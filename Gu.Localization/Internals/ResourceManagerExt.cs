namespace Gu.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Resources;

    internal static class ResourceManagerExt
    {
        private static readonly ConcurrentDictionary<ResourceManager, IReadOnlyList<CultureInfo>> ManagerCulturesMap = new ConcurrentDictionary<ResourceManager, IReadOnlyList<CultureInfo>>(ResourceManagerComparer.Default);
        private static readonly IReadOnlyList<CultureInfo> AllCultures = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(x => x.Name != CultureInfo.InvariantCulture.Name && x.IsValidCultureName()).ToArray();

        internal static IReadOnlyList<CultureInfo> GetCultures(this ResourceManager manager)
        {
            return ManagerCulturesMap.GetOrAdd(manager, GetCulturesFromManager);
        }

        private static IReadOnlyList<CultureInfo> GetCulturesFromManager(ResourceManager manager)
        {
            var cultures = new List<CultureInfo>();
            foreach (var culture in AllCultures)
            {
                using (var resourceSet = manager.GetResourceSet(culture, true, false))
                {
                    if (resourceSet != null)
                    {
                        cultures.Add(culture);
                    }
                }
            }

            return cultures;
        }

        private static bool IsValidCultureName(this CultureInfo culture)
        {
            for (var i = 0; i < culture.Name.Length; i++)
            {
                var c = culture.Name[i];
                if (char.IsLetterOrDigit(c) || c == '-' || c == '_')
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        private class ResourceManagerComparer : IEqualityComparer<ResourceManager>
        {
            private static readonly StringComparer StringComparer = StringComparer.OrdinalIgnoreCase;

            public static readonly ResourceManagerComparer Default = new ResourceManagerComparer();

            private ResourceManagerComparer()
            {
            }

            public bool Equals(ResourceManager x, ResourceManager y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                return StringComparer.Equals(x.BaseName, y.BaseName);
            }

            public int GetHashCode(ResourceManager obj)
            {
                return StringComparer.GetHashCode(obj.BaseName);
            }
        }
    }
}
