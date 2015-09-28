namespace Gu.Wpf.Localization
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Resources;

    internal class ResourceManagerWrapper
    {
        private static readonly ConcurrentDictionary<ResourceManager, ResourceSetAndCulture[]> Cache = new ConcurrentDictionary<ResourceManager, ResourceSetAndCulture[]>();

        internal ResourceManagerWrapper(ResourceManager resourceManager)
        {
            this.ResourceManager = resourceManager;
            ResourceSets = Cache.GetOrAdd(
                resourceManager,
                r => GetCultures(r).ToArray());
        }

        public ResourceManager ResourceManager { get; private set; }

        public IEnumerable<ResourceSetAndCulture> ResourceSets { get; private set; }
        
        public override string ToString()
        {
            var cultures = string.Join(", ", this.ResourceSets.Select(x => x.Culture.TwoLetterISOLanguageName));
            return string.Format("ResourceManager: {0}, ResourceSets: {1}", this.ResourceManager.BaseName, cultures);
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