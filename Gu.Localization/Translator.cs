namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Resources;

    /// <summary> Class for translating resources </summary>
    public static partial class Translator
    {
        private static DirectoryInfo resourceDirectory = ResourceCultures.DefaultResourceDirectory();

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

        /// <summary>Gets or sets a value indicating how errors are handled. The default is throw</summary>
        public static ErrorHandling ErrorHandling { get; set; } = ErrorHandling.ReturnErrorInfoPreserveNeutral;

        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey));
        /// </summary>
        /// <param name="resourceManager"> The <see cref="ResourceManager"/> containing translations.</param>
        /// <param name="key">The key in <paramref name="resourceManager"/></param>
        /// <returns>The key translated to the <see cref="EffectiveCulture"/></returns>
        public static string Translate(ResourceManager resourceManager, string key)
        {
            return Translate(resourceManager, key, EffectiveCulture);
        }

        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey));
        /// </summary>
        /// <param name="resourceManager"> The <see cref="ResourceManager"/> containing translations.</param>
        /// <param name="key">The key in <paramref name="resourceManager"/></param>
        /// <param name="errorHandling">Specifies how error handling is performed.</param>
        /// <returns>The key translated to the <see cref="EffectiveCulture()"/></returns>
        public static string Translate(ResourceManager resourceManager, string key, ErrorHandling errorHandling)
        {
            return Translate(resourceManager, key, CurrentCulture, errorHandling);
        }

        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey));
        /// </summary>
        /// <param name="resourceManager"> The <see cref="ResourceManager"/> containing translations.</param>
        /// <param name="key">The key in <paramref name="resourceManager"/></param>
        /// <param name="culture">The culture, if null CultureInfo.InvariantCulture is used</param>
        /// <returns>The key translated to the <paramref name="culture"/></returns>
        public static string Translate(ResourceManager resourceManager, string key, CultureInfo culture)
        {
            return Translate(resourceManager, key, culture, ErrorHandling);
        }

        /// <summary>
        /// Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeKey));
        /// </summary>
        /// <param name="resourceManager"> The <see cref="ResourceManager"/> containing translations.</param>
        /// <param name="key">The key in <paramref name="resourceManager"/></param>
        /// <param name="culture">The culture, if null CultureInfo.InvariantCulture is used</param>
        /// <param name="errorHandling">Specifies how to handle errors.</param>
        /// <returns>The key translated to the <paramref name="culture"/></returns>
        public static string Translate(
            ResourceManager resourceManager,
            string key,
            CultureInfo culture,
            ErrorHandling errorHandling)
        {
            string result;
            TryTranslateOrThrow(resourceManager, key, culture, errorHandling, out result);
            return result;
        }

        private static bool TryTranslateOrThrow(
            ResourceManager resourceManager,
            string key,
            CultureInfo culture,
            ErrorHandling errorHandling,
            out string result)
        {
            if (errorHandling == ErrorHandling.Inherit)
            {
                errorHandling = ErrorHandling == ErrorHandling.Inherit
                                    ? ErrorHandling.ReturnErrorInfoPreserveNeutral
                                    : ErrorHandling;
            }

            if (resourceManager == null)
            {
                if (errorHandling == ErrorHandling.Throw)
                {
                    throw new ArgumentNullException(nameof(resourceManager));
                }

                result = string.Format(Properties.Resources.NullManagerFormat, key);
                return false;
            }

            if (string.IsNullOrEmpty(key))
            {
                if (errorHandling == ErrorHandling.Throw)
                {
                    throw new ArgumentOutOfRangeException(nameof(key), "key == null");
                }

                result = "key == null";
                return false;
            }

            if (culture != null &&
                !culture.IsInvariant() &&
                allCultures.Contains(culture) == false)
            {
                if (resourceManager.HasCulture(culture))
                {
                    if (allCultures == null)
                    {
                        allCultures = new SortedSet<CultureInfo>(CultureInfoComparer.ByName);
                    }

                    allCultures.Add(culture);
                }
            }

            if (!resourceManager.HasCulture(culture))
            {
                if (errorHandling == ErrorHandling.Throw)
                {
                    var message = $"The resourcemanager {resourceManager.BaseName} does not have a translation for the culture: {culture?.Name ?? "null"}\r\n" +
                                   "Fix by either of:\r\n" +
                                  $"  - Add a resource file for the culture {culture?.Name ?? "null"}\r\n" +
                                  $"  - If falling back to neutral is desired specify {typeof(ErrorHandling).Name}.{nameof(ErrorHandling.ReturnErrorInfoPreserveNeutral)}";
                    throw new ArgumentOutOfRangeException(nameof(culture), message);
                }

                if (resourceManager.HasKey(key, CultureInfo.InvariantCulture))
                {
                    var neutral = resourceManager.GetString(key, CultureInfo.InvariantCulture);
                    if (errorHandling == ErrorHandling.ReturnErrorInfoPreserveNeutral)
                    {
                        result = neutral;
                        return true;
                    }

                    var arg = string.IsNullOrEmpty(neutral)
                                  ? key
                                  : neutral;
                    result = string.Format(Properties.Resources.MissingCultureFormat, arg);
                    return false;
                }

                result = string.Format(Properties.Resources.MissingCultureFormat, key);
                return false;
            }

            if (!resourceManager.HasKey(key, culture))
            {
                if (resourceManager.HasKey(key, CultureInfo.InvariantCulture))
                {
                    if (errorHandling == ErrorHandling.Throw)
                    {
                        var message = $"The resourcemanager {resourceManager.BaseName} does not have a translation for the key: {key} for the culture: {culture?.Name}\r\n" +
                                       "Fix by either of:\r\n" +
                                      $"  - Add a translation for the key '{key}' for the culture '{culture?.Name ?? "null"}'\r\n" +
                                      $"  - If falling back to neutral is desired specify {typeof(ErrorHandling).Name}.{nameof(ErrorHandling.ReturnErrorInfoPreserveNeutral)}";
                        throw new ArgumentOutOfRangeException(nameof(key), message);
                    }

                    var neutral = resourceManager.GetString(key, CultureInfo.InvariantCulture);
                    if (errorHandling == ErrorHandling.ReturnErrorInfoPreserveNeutral)
                    {
                        result = neutral;
                        return true;
                    }

                    var arg = string.IsNullOrEmpty(neutral)
                                  ? key
                                  : neutral;
                    result = string.Format(Properties.Resources.MissingTranslationFormat, arg);
                    return false;
                }

                if (errorHandling == ErrorHandling.Throw)
                {
                    var message = $"The resourcemanager {resourceManager.BaseName} does not have the key: {key}\r\n" +
                                  $"Fix the problem by adding a translation for the key '{key}'";
                    throw new ArgumentOutOfRangeException(nameof(key), message);
                }

                result = string.Format(Properties.Resources.MissingKeyFormat, key);
                return false;
            }

            result = resourceManager.GetString(key, culture);
            return true;
        }

        private static bool ShouldThrow(ErrorHandling errorHandling)
        {
            if (errorHandling == ErrorHandling.Inherit)
            {
                errorHandling = ErrorHandling;
            }

            var shouldThrow =
                errorHandling != ErrorHandling.ReturnErrorInfo &&
                errorHandling != ErrorHandling.ReturnErrorInfoPreserveNeutral;

            return shouldThrow;
        }
    }
}
