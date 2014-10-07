namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Threading;

    public class TranslationManager
    {
        private static readonly ConcurrentDictionary<Assembly, TranslationManager> Cache = new ConcurrentDictionary<Assembly, TranslationManager>();

        private TranslationManager()
        {
        }

        public event EventHandler LanguageChanged;

        public static TranslationManager Instance
        {
            get
            {
                var assembly = Assembly.GetCallingAssembly();
                return GetInstance(assembly);
            }
        }

        public ITranslationProvider TranslationProvider { get; set; }

        public CultureInfo CurrentLanguage
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }

            set
            {
                if (!Equals(value, Thread.CurrentThread.CurrentUICulture))
                {
                    Thread.CurrentThread.CurrentUICulture = value;
                    this.OnLanguageChanged();
                }
            }
        }

        public IEnumerable<CultureInfo> Languages
        {
            get
            {
                if (this.TranslationProvider != null)
                {
                    return this.TranslationProvider.Languages;
                }

                return Enumerable.Empty<CultureInfo>();
            }
        }

        public string Translate(string key)
        {
            if (this.TranslationProvider != null)
            {
                try
                {
                    string translatedValue = this.TranslationProvider.Translate(key);
                    if (translatedValue != null)
                    {
                        return translatedValue;
                    }
                }
                catch (Exception)
                {
                    return string.Format("!{0}!", key);
                }
            }

            return string.Format("!{0}!", key);
        }

        public bool HasKey(string key, CultureInfo culture)
        {
            return this.TranslationProvider.HasKey(key, culture);
        }

        /// <summary>
        /// Use this to get the translationmanager for another assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static TranslationManager GetInstance(Assembly assembly)
        {
            var manager = Cache.GetOrAdd(assembly, a => CreateManager(a));
            return manager;
        }

        private static TranslationManager CreateManager(Assembly assembly)
        {
            var resourceManager = new ResourceManager(assembly.GetName().Name + ".Properties.Resources", assembly);
            return new TranslationManager
                              {
                                  TranslationProvider = new ResxTranslationProvider(resourceManager)
                              };
        }

        private void OnLanguageChanged()
        {
            if (this.LanguageChanged != null)
            {
                this.LanguageChanged(this, EventArgs.Empty);
            }
        }
    }
}