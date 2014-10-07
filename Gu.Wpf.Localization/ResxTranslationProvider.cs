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
        private readonly ResourceManager _resourceManager;
        private readonly List<CultureInfo> _languages = new List<CultureInfo>();

        /// <summary>
        /// Uses the default Properties.Resources.ResourceManager;
        /// </summary>
        ////public ResxTranslationProvider()
        ////    : this(Resources.ResourceManager)
        ////{
        ////}

        public ResxTranslationProvider(ResourceManager resourceManager)
        {
            this._resourceManager = resourceManager;
            this._languages = CultureInfo.GetCultures(CultureTypes.AllCultures)
                                    .Where(
                                        x => x.TwoLetterISOLanguageName != "iv" &&
                                             this._resourceManager.GetResourceSet(x, true, false) != null)
                                    .ToList();
            if (!this._languages.Any())
            {
                throw new ArgumentException("Resourcemanager contains no languages", "resourceManager");
            }
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
            return this._resourceManager.GetString(key);
        }

        public bool HasKey(string key, CultureInfo culture)
        {
            return !string.IsNullOrEmpty(this._resourceManager.GetString(key, culture));
        }
    }
}