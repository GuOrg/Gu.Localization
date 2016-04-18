namespace Gu.Localization
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
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
            using (var clone = resourceManager.Clone())
            {
                if (clone?.ResourceManager == null)
                {
                    return false;
                }

                using (var resourceSet = clone.ResourceManager.GetResourceSet(culture, true, false))
                {
                    return resourceSet?.OfType<DictionaryEntry>()
                                       .Any(x => Equals(x.Key, key)) == true;
                }
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
            using (var clone = resourceManager.Clone())
            {
                return clone?.ResourceManager?.GetResourceSet(culture, true, false) != null;
            }
        }

        // Clones the resourcemanager
        // This is slow and backwards but can't think of another way that does not load a the resourceset into memory.
        // Also calling resourceManager.ReleaseAllResources() feels really nasty in a lib like this.
        // Keeping it slow and dumb until something better.
        internal static ResourceManagerClone Clone(this ResourceManager resourceManager)
        {
            return new ResourceManagerClone(resourceManager);
        }

        internal static Type ContainingType(this ResourceManager resourceManager)
        {
            return ResourceManagers.TypeManagerCache.GetOrAdd(resourceManager);
        }

        /// <summary>Creates a clone of the <see cref="ResourceManager"/> passed in. Releases all resources on dispose.</summary>
        internal sealed class ResourceManagerClone : IDisposable
        {
            internal readonly ResourceManager ResourceManager;

            public ResourceManagerClone(ResourceManager source)
            {
                Debug.Assert(source != null, "resourceManager == null");
                var containingType = source.ContainingType();
                Debug.Assert(containingType != null, "containingType == null");

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse want this check in release build
                if (containingType != null)
                {
                    this.ResourceManager = new ResourceManager(source.BaseName, containingType.Assembly);
                }
            }

            public void Dispose()
            {
                this.ResourceManager?.ReleaseAllResources();
            }
        }
    }
}
