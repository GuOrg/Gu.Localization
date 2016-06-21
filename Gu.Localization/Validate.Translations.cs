// ReSharper disable PossibleMultipleEnumeration
namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Resources;

    using Gu.Localization.Errors;

    /// <summary>
    /// This class is meant to be used in unit tests.
    /// Contains helper methods for asserting properties on localization.
    /// </summary>
    public static partial class Validate
    {
        private static readonly IReadOnlyList<TranslationError> EmptyErrors = new TranslationError[0];

        /// <summary>
        /// This is meant to be used in unit tests.
        /// Performance is probably very poor and we load all resources into memory.
        /// Checks that:
        /// 1) All keys in <paramref name="resourceManager"/> have non null values for all cultures in <see cref="Translator.Cultures"/>
        /// 2) If the resource is a format string it checks that
        ///   - All formats have the same number of parameters.
        ///   - All formats have numbering 0..1..n for the parameters.
        /// </summary>
        /// <param name="resourceManager">The resource managerr to check</param>
        /// <returns>An <see cref="TranslationErrors"/> with all errors found in <paramref name="resourceManager"/></returns>
        public static TranslationErrors Translations(ResourceManager resourceManager)
        {
            return Translations(resourceManager, Translator.Cultures.Prepend(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// This is meant to be used in unit tests.
        /// Performance is probably very poor and we load all resources into memory.
        /// Checks that:
        /// 1) All keys in <paramref name="resourceManager"/> have non null values for all cultures in <paramref name="cultures"/>
        /// 2) If the resource is a format string it checks that
        ///   - All formats have the same number of parameters.
        ///   - All formats have numbering 0..1..n for the parameters.
        /// </summary>
        /// <param name="resourceManager">The resource managerr to check</param>
        /// <param name="cultures">The cultures to check resources for</param>
        /// <returns>An <see cref="TranslationErrors"/> with all errors found in <paramref name="resourceManager"/></returns>
        public static TranslationErrors Translations(ResourceManager resourceManager, IEnumerable<CultureInfo> cultures)
        {
            var culturesAndKeys = resourceManager.GetCulturesAndKeys(cultures);
            Dictionary<string, IReadOnlyList<TranslationError>> errors = null;
            foreach (var key in culturesAndKeys.AllKeys)
            {
                IReadOnlyList<TranslationError> keyErrors;
                if (TryGetTranslationErrors(culturesAndKeys, cultures, key, out keyErrors))
                {
                    if (errors == null)
                    {
                        errors = new Dictionary<string, IReadOnlyList<TranslationError>>();
                    }

                    errors.Add(key, keyErrors);
                }
            }

            resourceManager.ReleaseAllResources();
            return errors == null
                       ? TranslationErrors.Empty
                       : new TranslationErrors(errors);
        }

        /// <summary>
        /// This is meant to be used in unit tests.
        /// Performance is probably very poor and we load all resources into memory.
        /// Checks that all members of <typeparamref name="T"/> have corresponding key in <paramref name="resourceManager"/>
        /// and that the key has a non null value for all cultures in <see cref="Translator.Cultures"/>
        /// </summary>
        /// <typeparam name="T">An enum type</typeparam>
        /// <param name="resourceManager">The <see cref="ResourceManager"/> with translations for <typeparamref name="T"/></param>
        /// <returns>A list with all members that does not have </returns>
        public static TranslationErrors EnumTranslations<T>(ResourceManager resourceManager)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            return EnumTranslations<T>(resourceManager, Translator.Cultures.Prepend(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// This is meant to be used in unit tests.
        /// Performance is probably very poor and we load all resources into memory.
        /// Checks that all members of <typeparamref name="T"/> have corresponding key in <paramref name="resourceManager"/>
        /// and that the key has a non null value for all cultures in <see cref="Translator.Cultures"/>
        /// </summary>
        /// <typeparam name="T">An enum type</typeparam>
        /// <param name="resourceManager">The <see cref="ResourceManager"/> with translations for <typeparamref name="T"/></param>
        /// <param name="cultures">The cultures to check for.</param>
        /// <returns>A list with all members that does not have </returns>
        public static TranslationErrors EnumTranslations<T>(ResourceManager resourceManager, IEnumerable<CultureInfo> cultures)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            var culturesAndKeys = resourceManager.GetCulturesAndKeys(cultures);
            Dictionary<string, IReadOnlyList<TranslationError>> errors = null;
            foreach (var key in Enum.GetNames(typeof(T)))
            {
                IReadOnlyList<TranslationError> keyErrors;
                if (TryGetTranslationErrors(culturesAndKeys, cultures, key, out keyErrors))
                {
                    if (errors == null)
                    {
                        errors = new Dictionary<string, IReadOnlyList<TranslationError>>();
                    }

                    errors.Add(key, keyErrors);
                }
            }

            resourceManager.ReleaseAllResources();
            return errors == null
                       ? TranslationErrors.Empty
                       : new TranslationErrors(errors);
        }

        /// <summary>
        /// This is meant to be used in unit tests.
        /// Performance is probably very poor and we load all resources into memory.
        /// Checks that:
        /// 1) <paramref name="key"/> has non null values for all cultures in <see cref="Translator.Cultures"/>
        /// 2) If the resource is a format string it checks that
        ///   - All formats have the same number of parameters.
        ///   - All formats have numbering 0..1..n for the parameters.
        /// </summary>
        /// <param name="resourceManager">The <see cref="ResourceManager"/> with translations for <paramref name="key"/></param>
        /// <param name="key">The key</param>
        /// <returns>A list with all errors for the key or an empty list if no errors.</returns>
        public static IReadOnlyList<TranslationError> Translations(ResourceManager resourceManager, string key)
        {
            return Translations(resourceManager, key, Translator.Cultures.Prepend(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// This is meant to be used in unit tests.
        /// Performance is probably very poor and we load all resources into memory.
        /// Checks that:
        /// 1) <paramref name="key"/> has non null values for all cultures in <see cref="Translator.Cultures"/>
        /// 2) If the resource is a format string it checks that
        ///   - All formats have the same number of parameters.
        ///   - All formats have numbering 0..1..n for the parameters.
        /// </summary>
        /// <param name="resourceManager">The <see cref="ResourceManager"/> with translations for <paramref name="key"/></param>
        /// <param name="key">The key</param>
        /// <param name="cultures">The cultures to check</param>
        /// <returns>A list with all errors for the key or an empty list if no errors.</returns>
        public static IReadOnlyList<TranslationError> Translations(ResourceManager resourceManager, string key, IEnumerable<CultureInfo> cultures)
        {
            IReadOnlyList<TranslationError> errors;
            if (TryGetTranslationErrors(resourceManager, key, cultures, out errors))
            {
                return errors;
            }

            return EmptyErrors;
        }

        public static bool TryGetTranslationErrors(ResourceManager resourceManager, string key, IEnumerable<CultureInfo> cultures, out IReadOnlyList<TranslationError> errors)
        {
            var culturesAndKeys = resourceManager.GetCulturesAndKeys(cultures);
            var result = TryGetTranslationErrors(culturesAndKeys, cultures, key, out errors);
            resourceManager.ReleaseAllResources();
            return result;
        }

        private static bool TryGetTranslationErrors(ResourceManagerExt.CulturesAndKeys culturesAndKeys, IEnumerable<CultureInfo> cultures, string key, out IReadOnlyList<TranslationError> errors)
        {
            List<TranslationError> foundErrors = null;
            FormatError formatErrors;
            if (TryGetFormatErrors(key, culturesAndKeys, cultures, out formatErrors))
            {
                foundErrors = new List<TranslationError>(1) { formatErrors };
            }

            MissingTranslation missingTranslation;
            if (TryGetMissingTranslations(key, culturesAndKeys, cultures, out missingTranslation))
            {
                if (foundErrors == null)
                {
                    foundErrors = new List<TranslationError>(1) { missingTranslation };
                }
                else
                {
                    foundErrors.Add(missingTranslation);
                }
            }

            errors = foundErrors;
            return errors != null;
        }

        private static bool TryGetFormatErrors(
            string key,
            ResourceManagerExt.CulturesAndKeys culturesAndKeys,
            IEnumerable<CultureInfo> cultures,
            out FormatError formatErrors)
        {
            int? count = null;
            var translations = culturesAndKeys.GetTranslationsFor(key, cultures);
            foreach (var translation in translations)
            {
                int indexCount;
                bool? anyItemHasFormat;
                if (!FormatString.IsValidFormat(translation.Value, out indexCount, out anyItemHasFormat))
                {
                    formatErrors = new FormatError(key, translations);
                    return true;
                }

                if (count == null)
                {
                    count = indexCount;
                    continue;
                }

                if (count != indexCount)
                {
                    formatErrors = new FormatError(key, translations);
                    return true;
                }
            }

            formatErrors = null;
            return false;
        }

        private static bool TryGetMissingTranslations(
            string key,
            ResourceManagerExt.CulturesAndKeys culturesAndKeys,
            IEnumerable<CultureInfo> cultures,
            out MissingTranslation missingTranslations)
        {
            List<CultureInfo> missing = null;
            foreach (var culture in cultures)
            {
                if (!culturesAndKeys.HasKey(culture, key))
                {
                    if (missing == null)
                    {
                        missing = new List<CultureInfo>();
                    }

                    missing.Add(culture);
                }
            }

            if (missing == null)
            {
                missingTranslations = null;
                return false;
            }

            missingTranslations = new MissingTranslation(key, missing);
            return true;
        }
    }
}
