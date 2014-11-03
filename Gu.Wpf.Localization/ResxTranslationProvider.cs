namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Resources;

    /// <summary>
    /// 
    /// </summary>
    public class ResxTranslationProvider : ITranslationProvider
    {
        private readonly List<CultureInfo> _languages = new List<CultureInfo>();
        private static readonly ConcurrentDictionary<Assembly, ResourceManager> Cache = new ConcurrentDictionary<Assembly, ResourceManager>();

        public ResxTranslationProvider(params ResourceManager[] resourceManagers)
        {
            ResourceManagers = resourceManagers.Select(r => new ResourceManagerWrapper(r)).ToArray();
            _languages = ResourceManagers.SelectMany(x => x.ResourceSets.Select(r => r.Culture))
                                         .Distinct()
                                         .ToList();
        }
        public ResxTranslationProvider(params Assembly[] assemblies)
            : this(assemblies.Where(a => a.GetManifestResourceNames().Any(x => x.Contains("Resources"))).Select(x => Cache.GetOrAdd(x, r => new ResourceManager(r.GetName().Name + ".Properties.Resources", r))))
        {
        }

        public ResxTranslationProvider(IEnumerable<ResourceManager> resourceManagers)
            : this(resourceManagers.ToArray())
        {
        }
        public ResxTranslationProvider(IEnumerable<Assembly> assemblies)
            : this(assemblies.ToArray())
        {
        }
        public ResxTranslationProvider(Type resourceSource)
            : this(new ResourceManager(resourceSource))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResxTranslationProvider"/> class.
        /// _resourceManager = new ResourceManager(baseName, assembly);
        /// </summary>
        /// <param name="baseName">Name of the base.</param>
        /// <param name="assembly">The assembly.</param>
        public ResxTranslationProvider(string baseName, Assembly assembly)
            : this(new ResourceManager(baseName, assembly))
        {
        }

        public ResourceManagerWrapper[] ResourceManagers { get; private set; }

        /// <summary>
        /// See <see cref="ITranslationProvider.Languages" />
        /// </summary>
        public IEnumerable<CultureInfo> Languages
        {
            get
            {
                return _languages;
            }
        }

        /// <summary>
        /// See <see cref="ITranslationProvider.Translate" />
        /// </summary>
        public string Translate(string key)
        {
            var values = this.ResourceManagers.Select(r => r.ResourceManager.GetString(key)).ToArray();
            if (values.All(string.IsNullOrEmpty))
            {
                return string.Format(Properties.Resources.MissingTranslationFormat, key);
            }
            return values.First(x => !string.IsNullOrEmpty(x));
        }

        public bool HasCulture(CultureInfo culture)
        {
            return _languages.Any(x => x.ThreeLetterISOLanguageName == culture.TwoLetterISOLanguageName);
        }

        public bool HasKey(string key, CultureInfo culture)
        {
            if (this.ResourceManagers == null)
            {
                return false;
            }
            if (culture != null)
            {
                var resourceSets = this.ResourceManagers.SelectMany(r => r.ResourceSets)
                                                        .Where(r => r.Culture.TwoLetterISOLanguageName == culture.TwoLetterISOLanguageName)
                                                        .ToArray();
                if (!resourceSets.Any())
                {
                    return false;
                }
                return resourceSets.Any(x => !string.IsNullOrEmpty(x.ResourceSet.GetString(key)));
            }
            var values = this.ResourceManagers.Select(r => r.ResourceManager.GetString(key, null));
            return values.All(x => !string.IsNullOrEmpty(x));
        }

        public class ResourceManagerWrapper
        {
            private static readonly ConcurrentDictionary<ResourceManager, ResourceSetAndCulture[]> Cache = new ConcurrentDictionary<ResourceManager, ResourceSetAndCulture[]>();

            public ResourceManagerWrapper(ResourceManager resourceManager)
            {
                this.ResourceManager = resourceManager;
                ResourceSets = Cache.GetOrAdd(
                    resourceManager,
                    r => GetCultures(r).ToArray());
            }

            public ResourceManager ResourceManager { get; private set; }

            public ResourceSetAndCulture[] ResourceSets { get; private set; }

            private static IEnumerable<ResourceSetAndCulture> GetCultures(ResourceManager manager)
            {
                var stopwatch = Stopwatch.StartNew();
                var cultureInfos = CultureInfo.GetCultures(CultureTypes.NeutralCultures).Where(x => x.Name != "");
                foreach (var culture in cultureInfos)
                {
                    var resourceSet = manager.GetResourceSet(culture, true, false);
                    if (resourceSet != null)
                    {
                        yield return new ResourceSetAndCulture(resourceSet, culture);
                    }
                }
                var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            }
        }

        public class ResourceSetAndCulture
        {
            public ResourceSetAndCulture(ResourceSet resourceSet, CultureInfo culture)
            {
                this.ResourceSet = resourceSet;
                this.Culture = culture;
            }
            public ResourceSet ResourceSet { get; private set; }
            public CultureInfo Culture { get; private set; }
        }
    }
}