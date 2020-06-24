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
        private static readonly ConcurrentDictionary<ResourceManager, CulturesAndKeys> Cache = new ConcurrentDictionary<ResourceManager, CulturesAndKeys>(ResourceManagerComparer.ByBaseName);

        /// <summary>
        /// Check if the <paramref name="resourceManager"/> has a translation for <paramref name="key"/>.
        /// </summary>
        /// <param name="resourceManager">The <see cref="ResourceManager"/>.</param>
        /// <param name="key">The key.</param>
        /// <param name="culture">The <see cref="CultureInfo"/>.</param>
        /// <returns>True if a translation exists.</returns>
        internal static bool HasKey(this ResourceManager resourceManager, string key, CultureInfo culture)
        {
            var culturesAndKeys = Cache.GetOrAdd(resourceManager, r => new CulturesAndKeys(r));
            return culturesAndKeys.HasKey(culture, key);
        }

        /// <summary>
        /// Check if the <paramref name="resourceManager"/> has a translation for <paramref name="key"/>.
        /// </summary>
        /// <param name="resourceManager">The <see cref="ResourceManager"/>.</param>
        /// <param name="key">The key.</param>
        /// <returns>True if a translation exists.</returns>
        internal static bool HasKey(this ResourceManager resourceManager, string key)
        {
            return resourceManager.HasKey(key, Translator.CurrentCulture) ||
                   resourceManager.HasKey(key, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Check if the <paramref name="resourceManager"/> has translations for <paramref name="culture"/>.
        /// </summary>
        /// <param name="resourceManager">The <see cref="ResourceManager"/>.</param>
        /// <param name="culture">The <see cref="CultureInfo"/>.</param>
        /// <returns>True if a translation exists.</returns>
        internal static bool HasCulture(this ResourceManager resourceManager, CultureInfo culture)
        {
            var culturesAndKeys = Cache.GetOrAdd(resourceManager, r => new CulturesAndKeys(r));
            return culturesAndKeys.HasCulture(culture);
        }

        internal static CulturesAndKeys GetCulturesAndKeys(this ResourceManager resourceManager, IEnumerable<CultureInfo> cultures)
        {
            var culturesAndKeys = Cache.GetOrAdd(resourceManager, r => new CulturesAndKeys(r));
            culturesAndKeys.CreateKeysForCultures(cultures);
            return culturesAndKeys;
        }

        internal static Type ContainingType(this ResourceManager resourceManager)
        {
            return ResourceManagers.TypeManagerCache.GetOrAdd(resourceManager);
        }

        internal sealed class CulturesAndKeys
        {
            private readonly ConcurrentDictionary<CultureInfo, ReadOnlySet<string>> culturesAndKeys = new ConcurrentDictionary<CultureInfo, ReadOnlySet<string>>(CultureInfoComparer.ByName);
            private readonly ResourceManager resourceManager;
            private readonly HashSet<string> allKeys = new HashSet<string>();

            internal CulturesAndKeys(ResourceManager resourceManager)
            {
                this.resourceManager = resourceManager;
            }

            internal IEnumerable<string> AllKeys => this.allKeys;

            internal IEnumerable<CultureInfo> Cultures => this.culturesAndKeys.Keys;

            internal bool HasKey(CultureInfo culture, string key)
            {
                var keys = this.culturesAndKeys.GetOrAdd(culture ?? CultureInfo.InvariantCulture, this.CreateKeysForCulture);
                return keys?.Contains(key) == true;
            }

            internal bool HasCulture(CultureInfo culture)
            {
                var keys = this.culturesAndKeys.GetOrAdd(culture ?? CultureInfo.InvariantCulture, this.CreateKeysForCulture);
                return keys != null;
            }

            internal IReadOnlyDictionary<CultureInfo, string> GetTranslationsFor(string key, IEnumerable<CultureInfo> cultures)
            {
                return new Translations(this, key, cultures);
            }

            internal void CreateKeysForCultures(IEnumerable<CultureInfo> cultures)
            {
                if ((cultures?.Any() != true || cultures.All(this.culturesAndKeys.ContainsKey)) && this.culturesAndKeys.ContainsKey(CultureInfo.InvariantCulture))
                {
                    return;
                }

                // I don't remember if this cloning solves a problem or if it is some old thing.
                using (var clone = new ResourceManagerClone(this.resourceManager))
                {
                    if (clone.ResourceManager is null)
                    {
                        return;
                    }

                    var hasAddedNeutral = false;
                    lock (this.culturesAndKeys)
                    {
                        foreach (var culture in cultures)
                        {
                            hasAddedNeutral |= culture.IsInvariant();
                            using (var resourceSet = clone.ResourceManager.GetResourceSet(culture, createIfNotExists: true, tryParents: false))
                            {
                                if (resourceSet is null)
                                {
                                    this.culturesAndKeys.TryAdd(culture, null);
                                }
                                else
                                {
                                    var keys = ReadOnlySet.Create(resourceSet.OfType<DictionaryEntry>().Select(x => x.Key).OfType<string>());
                                    this.allKeys.UnionWith(keys);
                                    this.culturesAndKeys.TryAdd(culture, keys);
                                }
                            }
                        }

                        if (!hasAddedNeutral)
                        {
                            using (var resourceSet = clone.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, createIfNotExists: true, tryParents: false))
                            {
                                if (resourceSet is null)
                                {
                                    this.culturesAndKeys.TryAdd(CultureInfo.InvariantCulture, null);
                                }
                                else
                                {
                                    var keys = ReadOnlySet.Create(resourceSet.OfType<DictionaryEntry>().Select(x => x.Key).OfType<string>());
                                    this.allKeys.UnionWith(keys);
                                    this.culturesAndKeys.TryAdd(CultureInfo.InvariantCulture, keys);
                                }
                            }
                        }
                    }
                }
            }

            private string? GetString(string key, CultureInfo cultureInfo)
            {
                return this.HasKey(cultureInfo, key)
                           ? this.resourceManager.GetString(key, cultureInfo)
                           : null;
            }

            private ReadOnlySet<string>? CreateKeysForCulture(CultureInfo culture)
            {
                // I don't remember if this cloning solves a problem or if it is some old thing.
                using (var clone = new ResourceManagerClone(this.resourceManager))
                {
                    if (clone?.ResourceManager is null)
                    {
                        return null;
                    }

                    using (var resourceSet = clone.ResourceManager.GetResourceSet(culture, createIfNotExists: true, tryParents: false))
                    {
                        if (resourceSet is null)
                        {
                            return null;
                        }

                        var keys = ReadOnlySet.Create(resourceSet.OfType<DictionaryEntry>().Select(x => x.Key).OfType<string>());
                        this.allKeys.UnionWith(keys);
                        return keys;
                    }
                }
            }

            /// <summary>Creates a clone of the <see cref="ResourceManager"/> passed in. Releases all resources on dispose.</summary>
            private sealed class ResourceManagerClone : IDisposable
            {
                internal readonly ResourceManager ResourceManager;

                internal ResourceManagerClone(ResourceManager source)
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

            private class Translations : IReadOnlyDictionary<CultureInfo, string>
            {
                private readonly IReadOnlyList<KeyValuePair<CultureInfo, string>> translations;

                internal Translations(CulturesAndKeys culturesAndKeys, string key, IEnumerable<CultureInfo> cultures)
                {
                    this.translations = cultures.Select(c => new KeyValuePair<CultureInfo, string>(c, culturesAndKeys.GetString(key, c)))
                                                .ToList();
                }

                public int Count => this.translations.Count;

                IEnumerable<CultureInfo> IReadOnlyDictionary<CultureInfo, string>.Keys => this.translations.Select(x => x.Key);

                IEnumerable<string> IReadOnlyDictionary<CultureInfo, string>.Values => this.translations.Select(x => x.Value);

                public string this[CultureInfo key]
                {
                    get
                    {
                        if (this.TryGetValue(key, out var value))
                        {
                            return value;
                        }

                        throw new ArgumentOutOfRangeException(nameof(key));
                    }
                }

                public IEnumerator<KeyValuePair<CultureInfo, string>> GetEnumerator() => this.translations.GetEnumerator();

                IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

                public bool ContainsKey(CultureInfo key) => this.translations.Any(x => Culture.NameEquals(x.Key, key));

                public bool TryGetValue(CultureInfo key, out string value)
                {
                    foreach (var keyValuePair in this.translations)
                    {
                        if (Culture.NameEquals(keyValuePair.Key, key))
                        {
                            value = keyValuePair.Value;
                            return true;
                        }
                    }

                    value = null;
                    return false;
                }
            }
        }
    }
}
