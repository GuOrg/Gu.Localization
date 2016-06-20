namespace Gu.Localization
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Resources;

    internal static class ResourceManagerExt
    {
        private static readonly ConcurrentDictionary<ResourceManager, CulturesAndKeys> Cache = new ConcurrentDictionary<ResourceManager, CulturesAndKeys>();

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
            var culturesAndKeys = Cache.GetOrAdd(resourceManager, r => new CulturesAndKeys(r));
            return culturesAndKeys.HasKey(culture, key);
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
            var culturesAndKeys = Cache.GetOrAdd(resourceManager, r => new CulturesAndKeys(r));
            return culturesAndKeys.HasCulture(culture);
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

        internal sealed class CulturesAndKeys
        {
            private readonly ConcurrentDictionary<CultureInfo, HashSet<string>> culturesAndKeys = new ConcurrentDictionary<CultureInfo, HashSet<string>>(CultureInfoComparer.Default);
            private readonly ResourceManager resourceManager;

            public CulturesAndKeys(ResourceManager resourceManager)
            {
                this.resourceManager = resourceManager;
            }

            public bool HasKey(CultureInfo culture, string key)
            {
                var keys = this.culturesAndKeys.GetOrAdd(culture ?? CultureInfo.InvariantCulture, this.CreateKeysForCulture);
                return keys?.Contains(key) == true;
            }

            public bool HasCulture(CultureInfo culture)
            {
                var keys = this.culturesAndKeys.GetOrAdd(culture ?? CultureInfo.InvariantCulture, this.CreateKeysForCulture);
                return keys != null;
            }

            private HashSet<string> CreateKeysForCulture(CultureInfo culture)
            {
                using (var clone = this.resourceManager.Clone())
                {
                    if (clone?.ResourceManager == null)
                    {
                        return null;
                    }

                    using (var resourceSet = clone.ResourceManager.GetResourceSet(culture, true, false))
                    {
                        if (resourceSet == null)
                        {
                            return null;
                        }

                        return new HashSet<string>(resourceSet.OfType<DictionaryEntry>().Select(x => x.Key).OfType<string>());
                    }
                }
            }
        }
    }
}