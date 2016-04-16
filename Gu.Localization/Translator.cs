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

    /// <summary> Class for translating resources </summary>
    public static class Translator
    {
        private static CultureInfo currentCulture = Thread.CurrentThread.CurrentUICulture;
        private static IReadOnlyList<CultureInfo> allCultures;

        private static DirectoryInfo resourceDirectory = ResourceCultures.DefaultResourceDirectory();

        /// <summary>
        /// Notifies when the current language changes.
        /// </summary>
        public static event EventHandler<CultureInfo> CurrentCultureChanged;

        /// <summary>
        /// Gets or sets set the current directory where resources are found.
        /// Default is Directory.GetCurrentDirectory()
        /// Changing the default is perhaps useful in tests.
        /// </summary>
        public static DirectoryInfo ResourceDirectory
        {
            get
            {
                return resourceDirectory;
            }

            set
            {
                resourceDirectory = value;
                allCultures = GetAllCultures();
            }
        }

        /// <summary>
        /// Gets or sets the culture to translate to
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get
            {
                return currentCulture ??
                       Cultures.FirstOrDefault() ??
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

        /// <summary> Gets a list with all cultures found for the application </summary>
        public static IReadOnlyList<CultureInfo> Cultures => allCultures ?? (allCultures = GetAllCultures());

        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey));
        /// </summary>
        /// <param name="resourceManager"> The <see cref="ResourceManager"/> containing translations.</param>
        /// <param name="key">The key in <paramref name="resourceManager"/></param>
        /// <returns>The key translated to the <see cref="CurrentCulture"/></returns>
        public static string Translate(ResourceManager resourceManager, string key)
        {
            return Translate(resourceManager, key, CurrentCulture);
        }

        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey));
        /// </summary>
        /// <param name="resourceManager"> The <see cref="ResourceManager"/> containing translations.</param>
        /// <param name="key">The key in <paramref name="resourceManager"/></param>
        /// <param name="culture">The culture.</param>
        /// <returns>The key translated to the <see cref="CurrentCulture"/></returns>
        public static string Translate(ResourceManager resourceManager, string key, CultureInfo culture)
        {
            if (resourceManager == null)
            {
                return string.Format(Properties.Resources.NullManagerFormat, key);
            }

            if (string.IsNullOrEmpty(key))
            {
                return "null";
            }

            var translated = resourceManager.GetString(key, culture);
            if (translated == null)
            {
                return string.Format(Properties.Resources.MissingKeyFormat, key);
            }

            if (translated == string.Empty)
            {
                if (!Cultures.Contains(culture, CultureInfoComparer.Default))
                {
                    return string.Format(Properties.Resources.MissingCultureFormat, key);
                }

                if (resourceManager.GetResourceSet(culture, false, false)
                                   .OfType<DictionaryEntry>()
                                   .All(x => !Equals(x.Key, key)))
                {
                    return string.Format(Properties.Resources.MissingTranslationFormat, key);
                }
            }

            return translated;
        }

        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey));
        /// This assumes that the resource is something like 'Value: {0}' i.e. having one format parameter.
        /// </summary>
        /// <param name="resourceManager"> The <see cref="ResourceManager"/> containing translations.</param>
        /// <param name="key">The key in <paramref name="resourceManager"/></param>
        /// <param name="arg">The argument will be used as string.Format(format, <paramref name="arg"/>)</param>
        /// <returns>The key translated to the <see cref="CurrentCulture"/></returns>
        public static string Translate(ResourceManager resourceManager, string key, object arg)
        {
            return Translate(resourceManager, key, CurrentCulture, arg);
        }

        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey));
        /// This assumes that the resource is something like 'Value: {0}' i.e. having one format parameter.
        /// </summary>
        /// <param name="resourceManager"> The <see cref="ResourceManager"/> containing translations.</param>
        /// <param name="key">The key in <paramref name="resourceManager"/></param>
        /// <param name="culture">The culture.</param>
        /// <param name="arg">The argument will be used as string.Format(format, <paramref name="arg"/>)</param>
        /// <returns>The key translated to the <see cref="CurrentCulture"/></returns>
        public static string Translate(ResourceManager resourceManager, string key, CultureInfo culture, object arg)
        {
            var format = Translate(resourceManager, key, culture);
            return string.Format(format, arg);
        }

        /// <summary>
        /// Check if the <paramref name="resourceManager"/> has a translation for <paramref name="key"/>
        /// </summary>
        /// <param name="resourceManager">The <see cref="ResourceManager"/></param>
        /// <param name="key">The key</param>
        /// <param name="culture">The <see cref="CultureInfo"/></param>
        /// <returns>True if a translation exists</returns>
        internal static bool HasKey(ResourceManager resourceManager, string key, CultureInfo culture)
        {
            return resourceManager.GetString(key, culture) != null;
        }

        private static void OnCurrentCultureChanged(CultureInfo e)
        {
            CurrentCultureChanged?.Invoke(null, e);
        }

        private static IReadOnlyList<CultureInfo> GetAllCultures()
        {
            Debug.WriteLine(resourceDirectory);
            return resourceDirectory?.Exists == true
                       ? ResourceCultures.GetAllCultures(resourceDirectory)
                       : new CultureInfo[0];
        }
    }
}
