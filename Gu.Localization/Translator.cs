namespace Gu.Localization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Resources;
    using System.Threading;
    using Gu.Localization.Properties;

    public static class Translator
    {
        private static CultureInfo currentCulture = Thread.CurrentThread.CurrentUICulture;

        /// <summary>
        /// Notifies when the current language changes.
        /// </summary>
        public static event EventHandler<CultureInfo> CurrentCultureChanged;

        /// <summary>
        /// Gets or sets the culture to translate to
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get
            {
                return currentCulture ??
                       AllCultures.FirstOrDefault() ??
                       CultureInfo.InvariantCulture;
            }

            set
            {
                if (CultureInfoComparer.Equals(currentCulture, value))
                {
                    return;
                }

                currentCulture = value;
                OnCurrentCultureChanged(value);
            }
        }

        public static IReadOnlyList<CultureInfo> AllCultures => GetAllCultures();

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

            var translated = resourceManager.GetString(key, CurrentCulture);
            if (translated == null)
            {
                return string.Format(Properties.Resources.MissingKeyFormat, key);
            }

            if (translated == string.Empty)
            {
                if (!AllCultures.Contains(CurrentCulture, CultureInfoComparer.Default))
                {
                    return string.Format(Properties.Resources.MissingCultureFormat, key);
                }

                if (resourceManager.GetResourceSet(CurrentCulture, false, false)
                                   .OfType<DictionaryEntry>()
                                   .All(x => !Equals(x.Key, key)))
                {
                    return string.Format(Properties.Resources.MissingTranslationFormat, key);
                }
            }

            return translated;
        }

        public static bool HasCulture(CultureInfo culture)
        {
            return AllCultures.Contains(culture, CultureInfoComparer.Default);
        }

        public static bool HasKey(ResourceManager resourceManager, string key, CultureInfo culture)
        {
            return resourceManager.GetString(key, culture) != null;
        }

        private static void OnCurrentCultureChanged(CultureInfo e)
        {
            CurrentCultureChanged?.Invoke(null, e);
        }

        private static IReadOnlyList<CultureInfo> GetAllCultures()
        {
            var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            Debug.WriteLine(currentDirectory);
            return ResourceCultures.GetAllCultures(currentDirectory);
        }
    }
}
