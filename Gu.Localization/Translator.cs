namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Resources;
    using System.Threading;

    using Gu.Localization.Properties;

    public class Translator
    {
        private static readonly List<CultureInfo> InnerAllCultures = new List<CultureInfo>();
        private static CultureInfo currentCulture = Thread.CurrentThread.CurrentUICulture;
        private readonly List<CultureInfo> cultures = new List<CultureInfo>();
        private readonly ResourceManagerWrapper manager;

        internal Translator(ResourceManagerWrapper manager)
        {
            this.manager = manager;
            foreach (var resourceSetAndCulture in manager.ResourceSets)
            {
                var cultureInfo = resourceSetAndCulture.Culture;
                if (AllCultures.All(c => cultureInfo != null && c.TwoLetterISOLanguageName != cultureInfo.TwoLetterISOLanguageName))
                {
                    InnerAllCultures.Add(cultureInfo);
                    OnLanguagesChanged();
                    OnLanguageChanged(cultureInfo);
                }

                this.cultures.Add(cultureInfo);
            }
        }

        /// <summary>
        /// Notifies when the current language changes.
        /// </summary>
        public static event EventHandler<CultureInfo> LanguageChanged;

        /// <summary>
        /// Notifies when languages are added or removed.
        /// </summary>
        public static event EventHandler<EventArgs> LanguagesChanged;

        /// <summary>
        /// Gets or sets the culture to translate to
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get
            {
                return currentCulture ?? AllCultures.FirstOrDefault();
            }

            set
            {
                if (Equals(currentCulture, value))
                {
                    return;
                }

                currentCulture = value;
                OnLanguageChanged(value);
            }
        }

        public static IReadOnlyList<CultureInfo> AllCultures => InnerAllCultures;

        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, () => Properties.Resources.SomeKey);
        /// </summary>
        /// <param name="resourceManager">
        /// The <see cref="ResourceManager"/> containing translations
        /// </param>
        /// <param name="key">() => Properties.Resources.AllLanguages</param>
        /// <returns>The key translated to the <see cref="CurrentCulture"/></returns>
        public static string Translate(ResourceManager resourceManager, Expression<Func<string>> key)
        {
            if (ExpressionHelper.IsResourceKey(key))
            {
                return Translate(resourceManager, ExpressionHelper.GetResourceKey(key));
            }

            return Translate(resourceManager, key.Compile().Invoke());
        }

        /// <summary>
        /// Translator.Translate{Properties.Resources}(nameof(Properties.Resources.SomeKey));
        /// </summary>
        /// <typeparam name="T">
        /// The <see cref="ResourceManager"/> containing translations
        /// </typeparam>
        /// <param name="key">() => Properties.Resources.AllLanguages</param>
        /// <returns>The key translated to the <see cref="CurrentCulture"/></returns>
        public static object Translate<T>(string key)
        {
            return Translate(ResourceManagerWrapper.FromType(typeof(T)), key);
        }

        /// <summary>
        /// Call like this () => Properties.Resources.SomeKey
        /// </summary>
        /// <param name="key">Path to the key. Must be include Resources.</param>
        /// <returns>The key translated to the <see cref="CurrentCulture"/></returns>
        public static string Translate(Expression<Func<string>> key)
        {
            if (ExpressionHelper.IsResourceKey(key))
            {
                return Translate(ExpressionHelper.GetResourceManager(key), ExpressionHelper.GetResourceKey(key));
            }

            return Translate(null, key.Compile().Invoke());
        }

        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey));
        /// </summary>
        /// <param name="resourceManager">
        /// The <see cref="ResourceManager"/> containing translations
        /// </param>
        /// <param name="key">The key in <paramref name="resourceManager"/></param>
        /// <returns>The key translated to the <see cref="CurrentCulture"/></returns>
        public static string Translate(ResourceManager resourceManager, string key)
        {
            if (resourceManager == null)
            {
                return string.Format(Resources.NullManagerFormat, key);
            }

            if (string.IsNullOrEmpty(key))
            {
                return "null";
            }

            return resourceManager.GetString(key, CurrentCulture);
        }

        public bool HasCulture(CultureInfo culture)
        {
            return this.cultures.Any(x => x.TwoLetterISOLanguageName == culture.TwoLetterISOLanguageName);
        }

        public bool HasKey(string key)
        {
            return this.manager.ResourceManager.GetString(key, CurrentCulture) != null;
        }

        public string Translate(string key)
        {
            var translated = this.manager.ResourceManager.GetString(key, CurrentCulture);
            if (translated == null)
            {
                return string.Format(Resources.MissingKeyFormat, key);
            }

            return translated;
        }

        private static void OnLanguageChanged(CultureInfo e)
        {
            LanguageChanged?.Invoke(null, e);
        }

        private static void OnLanguagesChanged()
        {
            LanguagesChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}
