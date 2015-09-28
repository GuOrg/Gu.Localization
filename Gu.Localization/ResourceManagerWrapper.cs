namespace Gu.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Resources;

    public class ResourceManagerWrapper
    {
        private static readonly ConcurrentDictionary<ResourceManager, IReadOnlyList<ResourceSetAndCulture>> Cache = new ConcurrentDictionary<ResourceManager, IReadOnlyList<ResourceSetAndCulture>>();
        private static readonly ConcurrentDictionary<Type, ResourceManager> TypeResourceManagerMap = new ConcurrentDictionary<Type, ResourceManager>();
        internal ResourceManagerWrapper(ResourceManager resourceManager)
        {
            if (resourceManager == null)
            {
                throw new ArgumentNullException(nameof(resourceManager));
            }
            ResourceManager = resourceManager;
            ResourceSets = Cache.GetOrAdd(resourceManager, GetCultures);
        }

        public ResourceManager ResourceManager { get; }

        public IEnumerable<ResourceSetAndCulture> ResourceSets { get; }

        public static ResourceManagerWrapper Create(Expression<Func<string>> key)
        {
            var resourceManager = ExpressionHelper.GetResourceManager(key);
            if (resourceManager == null)
            {
                throw new ArgumentException($"Could not find a resourcemanager for {key}", nameof(key));
            }
            return new ResourceManagerWrapper(resourceManager);
        }

        public static bool HasResourceManager(Type type)
        {
            return FromType(type) != null;
        }

        public static ResourceManager FromType(Type type)
        {
            ResourceManager manager;
            if (!TypeResourceManagerMap.TryGetValue(type, out manager))
            {
                var propertyInfo = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                                       .SingleOrDefault(x => typeof(ResourceManager).IsAssignableFrom(x.PropertyType));
                if (propertyInfo == null)
                {
                    return null;
                }
                manager = (ResourceManager)propertyInfo.GetValue(null);
                TypeResourceManagerMap.TryAdd(type, manager);
            }
            return manager;
        }

        public override string ToString()
        {
            var cultures = string.Join(", ", ResourceSets.Select(x => x.Culture.TwoLetterISOLanguageName));
            return $"ResourceManager: {ResourceManager.BaseName}, ResourceSets: {cultures}";
        }

        private static IReadOnlyList<ResourceSetAndCulture> GetCultures(ResourceManager manager)
        {
            var cultureInfos = CultureInfo.GetCultures(CultureTypes.NeutralCultures)
                                          .Where(x => x.Name != "")
                                          .ToArray();
            var result = new List<ResourceSetAndCulture>();
            foreach (var culture in cultureInfos)
            {
                var resourceSet = manager.GetResourceSet(culture, true, false);
                if (resourceSet != null)
                {
                    result.Add(new ResourceSetAndCulture(resourceSet, culture));
                }
            }
            return result;
        }
    }
}