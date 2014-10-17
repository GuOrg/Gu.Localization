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

        public ResxTranslationProvider(ResourceManager resourceManager)
        {
            this.ResourceManager = resourceManager;
            this._languages = CultureInfo.GetCultures(CultureTypes.AllCultures)
                                    .Where(
                                        x => x.TwoLetterISOLanguageName != "iv" &&
                                             this.ResourceManager.GetResourceSet(x, true, false) != null)
                                    .ToList();
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

        public ResourceManager ResourceManager { get; private set; }


        /// <summary>
        /// See <see cref="ITranslationProvider.Languages" />
        /// </summary>
        public IEnumerable<CultureInfo> Languages
        {
            get
            {
                return this._languages;
            }
        }

        /// <summary>
        /// See <see cref="ITranslationProvider.Translate" />
        /// </summary>
        public string Translate(string key)
        {
            return this.ResourceManager.GetString(key);
        }

        public bool HasCulture(CultureInfo culture)
        {
            return _languages.Any(x => x.Name == culture.Name);
        }

        public bool HasKey(string key, CultureInfo culture)
        {
            if (this.ResourceManager == null)
            {
                return false;
            }
            if (culture != null)
            {
                var resourceSet = this.ResourceManager.GetResourceSet(culture, false, true);
                if (resourceSet == null)
                {
                    return false;
                }
                return !string.IsNullOrEmpty(resourceSet.GetString(key));
            }
            var value = this.ResourceManager.GetString(key, culture);
            return !string.IsNullOrEmpty(value);
        }
    }
}