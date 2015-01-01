namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Resources;

    public class Translator
    {
        /// <summary>
        /// This is exposed for binding current language see demo.
        /// </summary>
        public static readonly CultureProxy CultureProxy = new CultureProxy();
        private static readonly List<CultureInfo> InnerAllCultures = new List<CultureInfo>();
        private readonly List<CultureInfo> _cultures = new List<CultureInfo>();
        private static CultureInfo _currentCulture;
        private readonly ResourceManagerWrapper _manager;

        internal Translator(ResourceManagerWrapper manager)
        {
            _manager = manager;
            foreach (var resourceSetAndCulture in manager.ResourceSets)
            {
                var cultureInfo = resourceSetAndCulture.Culture;
                if (AllCultures.All(c => cultureInfo != null && c.TwoLetterISOLanguageName != cultureInfo.TwoLetterISOLanguageName))
                {
                    InnerAllCultures.Add(cultureInfo);
                    OnLanguagesChanged();
                    OnLanguageChanged(cultureInfo);
                }
                _cultures.Add(cultureInfo);
            }
        }

        public static event EventHandler<CultureInfo> LanguageChanged;

        public static event EventHandler<EventArgs> LanguagesChanged;

        /// <summary>
        /// The culture to translate to
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get
            {
                return _currentCulture ?? AllCultures.FirstOrDefault();
            }
            set
            {
                if (Equals(_currentCulture, value))
                {
                    return;
                }
                _currentCulture = value;
                OnLanguageChanged(value);
            }
        }

        public static IEnumerable<CultureInfo> AllCultures
        {
            get
            {
                return InnerAllCultures;
            }
        }

        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, () => Properties.Resources.AllLanguages);
        /// </summary>
        /// <param name="resourceManager"></param>
        /// <param name="key">() => Properties.Resources.AllLanguages</param>
        /// <returns>The key translated to the CurrentCulture</returns>
        public static string Translate(ResourceManager resourceManager, Expression<Func<string>> key)
        {
            if (ExpressionHelper.IsResourceKey(key))
            {
                return Translate(resourceManager, ExpressionHelper.GetResourceKey(key));
            }
            return Translate(resourceManager, key.Compile().Invoke());
        }

        public static string Translate(Expression<Func<string>> key)
        {
            if (ExpressionHelper.IsResourceKey(key))
            {
                return Translate(ExpressionHelper.GetResourceManager(key), ExpressionHelper.GetResourceKey(key));
            }
            return Translate(null, key.Compile().Invoke());
        }

        public static string Translate(ResourceManager resourceManager, string key)
        {
            if (resourceManager == null)
            {
                return string.Format(Properties.Resources.NullManagerFormat, key);
            }
            if (string.IsNullOrEmpty(key))
            {
                return "null";
            }
            return resourceManager.GetString(key, CurrentCulture);
        }

        public bool HasCulture(CultureInfo culture)
        {
            return _cultures.Any(x => x.TwoLetterISOLanguageName == culture.TwoLetterISOLanguageName);
        }

        public bool HasKey(string key)
        {
            return _manager.ResourceManager.GetString(key, CurrentCulture) != null;
        }

        public string Translate(string key)
        {
            var translated  = _manager.ResourceManager.GetString(key, CurrentCulture);
            if (translated == null)
            {
                return string.Format(Properties.Resources.MissingKeyFormat, key);
            }
            return translated;
        }

        private static void OnLanguageChanged(CultureInfo e)
        {
            var handler = LanguageChanged;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        private static void OnLanguagesChanged()
        {
            var handler = LanguagesChanged;
            if (handler != null)
            {
                handler(null, EventArgs.Empty);
            }
        }
    }
}
