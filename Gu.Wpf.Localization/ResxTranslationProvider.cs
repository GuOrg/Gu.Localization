namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections.Generic;
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

        public ResxTranslationProvider(params ResourceManager[] resourceManagers)
        {
            ResourceManagers = resourceManagers;
            _languages = CultureInfo.GetCultures(CultureTypes.AllCultures)
                                    .Where(x => x.TwoLetterISOLanguageName != "iv" &&
                                                this.ResourceManagers.Any(r => r.GetResourceSet(x, true, false) != null))
                                    .ToList();
        }

        public ResxTranslationProvider(IEnumerable<ResourceManager> resourceManagers)
            : this(resourceManagers.ToArray())
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

        public ResourceManager[] ResourceManagers { get; private set; }


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
            var manager = this.ResourceManagers.FirstOrDefault(x => !string.IsNullOrEmpty(x.GetString(key)));
            if (manager == null)
            {
                return string.Format(Properties.Resources.MissingTranslationFormat, key);
            }
            return manager.GetString(key);
        }

        public bool HasCulture(CultureInfo culture)
        {
            return _languages.Any(x => x.Name == culture.Name);
        }

        public bool HasKey(string key, CultureInfo culture)
        {
            if (this.ResourceManagers == null)
            {
                return false;
            }
            if (culture != null)
            {
                var resourceSet = this.ResourceManagers.FirstOrDefault(r => r.GetResourceSet(culture, false, true) != null);
                if (resourceSet == null)
                {
                    return false;
                }
                return !string.IsNullOrEmpty(resourceSet.GetString(key));
            }
            var values = this.ResourceManagers.Select(r => r.GetString(key, culture));
            return values.All(x => !string.IsNullOrEmpty(x));
        }
    }
}