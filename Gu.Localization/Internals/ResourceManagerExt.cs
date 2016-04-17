namespace Gu.Localization
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Resources;

    internal static class ResourceManagerExt
    {
        /// <summary>
        /// Check if the <paramref name="resourceManager"/> has a translation for <paramref name="key"/>
        /// This is a pretty expensive call but should only happen in the error path.
        /// No memoization is done.
        /// </summary>
        /// <param name="resourceManager">The <see cref="ResourceManager"/></param>
        /// <param name="key">The key</param>
        /// <param name="culture">The <see cref="CultureInfo"/></param>
        /// <returns>True if a translation exists</returns>
        internal static bool HasKey(this ResourceManager resourceManager, string key, CultureInfo culture)
        {
            using (var resourceSet = resourceManager.GetTempResourceSet(culture))
            {
                return resourceSet?.ResourceSet.OfType<DictionaryEntry>()
                                   .Any(x => Equals(x.Key, key)) == true;
            }
        }

        /// <summary>
        /// Check if the <paramref name="resourceManager"/> has translations for <paramref name="culture"/>
        /// This is a pretty expensive call but should only happen in the error path.
        /// No memoization is done.
        /// </summary>
        /// <param name="resourceManager">The <see cref="ResourceManager"/></param>
        /// <param name="culture">The <see cref="CultureInfo"/></param>
        /// <returns>True if a translation exists</returns>
        internal static bool HasCulture(this ResourceManager resourceManager, CultureInfo culture)
        {
            using (var resourceSet = resourceManager.GetTempResourceSet(culture))
            {
                return resourceSet != null;
            }
        }

        // Clones the resourcemanager and gets the resource set for the culture
        // This is slow and backwards but can't think of another way that does not load a the resourceset into memory.
        // Also calling resourceManager.ReleaseAllResources() feels really nasty in a lib like this.
        // Keeping it slow and dumb until something better.
        private static Disposer GetTempResourceSet(this ResourceManager resourceManager, CultureInfo culture)
        {
            var type = AppDomain.CurrentDomain.GetAssemblies()
                                .Select(x => x.GetType(resourceManager.BaseName))
                                .SingleOrDefault(x => x != null &&
                                                        x.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                                                        .Any(p => p.PropertyType == typeof(ResourceManager)));
            if (type == null)
            {
                return null;
            }

            var clone = new ResourceManager(resourceManager.BaseName, type.Assembly);
            var resourceSet = clone.GetResourceSet(culture, true, false);
            if (resourceSet == null)
            {
                resourceManager.ReleaseAllResources();
                return null;
            }

            return new Disposer(resourceManager, resourceSet);
        }

        private class Disposer : IDisposable
        {
            internal readonly ResourceSet ResourceSet;
            private readonly ResourceManager resourceManager;

            public Disposer(ResourceManager resourceManager, ResourceSet resourceSet)
            {
                Debug.Assert(resourceManager != null, "resourceManager == null");
                Debug.Assert(resourceSet != null, "resourceSet == null");
                this.resourceManager = resourceManager;
                this.ResourceSet = resourceSet;
            }

            public void Dispose()
            {
                this.resourceManager.ReleaseAllResources();
                this.ResourceSet.Dispose();
            }
        }
    }
}
