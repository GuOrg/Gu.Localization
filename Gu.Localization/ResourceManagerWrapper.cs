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
        private static readonly ConcurrentDictionary<ResourceManager, ResourceSetAndCulture[]> Cache = new ConcurrentDictionary<ResourceManager, ResourceSetAndCulture[]>();
        private static readonly ConcurrentDictionary<Type, ResourceManager> TypeResourceManagerMap = new ConcurrentDictionary<Type, ResourceManager>();

        internal ResourceManagerWrapper(ResourceManager resourceManager)
        {
            if (resourceManager == null)
            {
                throw new ArgumentNullException(nameof(resourceManager));
            }

            this.ResourceManager = resourceManager;
            this.ResourceSets = Cache.GetOrAdd(
                resourceManager,
                r => GetCultures(r).ToArray());
        }

        public ResourceManager ResourceManager { get; private set; }

        public IEnumerable<ResourceSetAndCulture> ResourceSets { get; private set; }

        public static ResourceManagerWrapper Create(Expression<Func<string>> key)
        {
            return new ResourceManagerWrapper(ExpressionHelper.GetResourceManager(key));
        }

        public static ResourceManager FromType(Type type)
        {
            ResourceManager manager;
            if (!TypeResourceManagerMap.TryGetValue(type, out manager))
            {
                var propertyInfo = type.GetProperty("ResourceManager", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (propertyInfo == null)
                {
                    return null;
                }

                manager = propertyInfo.GetValue(null) as ResourceManager;
                if (manager != null)
                {
                    TypeResourceManagerMap.TryAdd(type, manager);
                }
            }

            return manager;
        }

        public override string ToString()
        {
            var cultures = string.Join(", ", this.ResourceSets.Select(x => x.Culture.TwoLetterISOLanguageName));
            return $"ResourceManager: {this.ResourceManager.BaseName}, ResourceSets: {cultures}";
        }

        private static IEnumerable<ResourceSetAndCulture> GetCultures(ResourceManager manager)
        {
            var cultureInfos = CultureInfo.GetCultures(CultureTypes.NeutralCultures).Where(x => x.Name != "");
            foreach (var culture in cultureInfos)
            {
                var resourceSet = manager.GetResourceSet(culture, true, false);
                if (resourceSet != null)
                {
                    yield return new ResourceSetAndCulture(resourceSet, culture);
                }
            }
        }
    }
}