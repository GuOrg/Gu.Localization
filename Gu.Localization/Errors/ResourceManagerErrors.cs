namespace Gu.Localization.Errors
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Resources;

    /// <summary>A collection of errors.</summary>
    public class ResourceManagerErrors : IReadOnlyDictionary<string, IReadOnlyList<TranslationError>>
    {
        public static readonly ResourceManagerErrors Empty = new ResourceManagerErrors(EmptyReadOnlyDictionary<string, IReadOnlyList<TranslationError>>.Default);

        private static readonly IReadOnlyList<TranslationError> EmptyErrors = new TranslationError[0];

        private readonly IReadOnlyDictionary<string, IReadOnlyList<TranslationError>> errors;

        private ResourceManagerErrors(IReadOnlyDictionary<string, IReadOnlyList<TranslationError>> errors)
        {
            this.errors = errors;
        }

        public bool IsEmpty => this.errors.Count == 0;

        /// <inheritdoc />
        public int Count => this.errors.Count;

        /// <inheritdoc />
        public IEnumerable<string> Keys => this.errors.Keys;

        /// <inheritdoc />
        public IEnumerable<IReadOnlyList<TranslationError>> Values => this.errors.Values;

        /// <inheritdoc />
        public IReadOnlyList<TranslationError> this[string key] => this.errors[key];

        /// <summary>
        /// This is meant to be used in unit tests.
        /// Performance is probably very poor and we load all resources into memory.
        /// Checks that:
        /// 1) All keys in <paramref name="resourceManager"/> have non null values for all cultures in <see cref="Translator.AllCultures"/>
        /// 2) If the resource is a format string it checks that
        ///   - All formats have the same number of parameters.
        ///   - All formats have numbering 0..1..n for the parameters.
        /// </summary>
        /// <param name="resourceManager">The resource managerr to check</param>
        /// <returns>An <see cref="ResourceManagerErrors"/> with all errors found in <paramref name="resourceManager"/></returns>
        public static ResourceManagerErrors For(ResourceManager resourceManager)
        {
            return For(resourceManager, Translator.AllCultures.Prepend(CultureInfo.InvariantCulture));
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
        /// <returns>An <see cref="ResourceManagerErrors"/> with all errors found in <paramref name="resourceManager"/></returns>
        public static ResourceManagerErrors For(ResourceManager resourceManager, IEnumerable<CultureInfo> cultures)
        {
            var resources = GetResources(resourceManager, cultures);
            var keys = GetKeys(resourceManager);
            Dictionary<string, IReadOnlyList<TranslationError>> errors = null;
            foreach (var key in keys)
            {
                var keyErrors = For(resources, key);
                if (keyErrors.Count == 0)
                {
                    continue;
                }

                if (errors == null)
                {
                    errors = new Dictionary<string, IReadOnlyList<TranslationError>>();
                }

                errors.Add(key, keyErrors);
            }

            return errors == null
                       ? Empty
                       : new ResourceManagerErrors(errors);
        }

        /// <summary>
        /// This is meant to be used in unit tests.
        /// Performance is probably very poor and we load all resources into memory.
        /// Checks that all members of <typeparamref name="T"/> have corresponding key in <paramref name="resourceManager"/>
        /// and that the key has a non null value for all cultures in <see cref="Translator.AllCultures"/>
        /// </summary>
        /// <typeparam name="T">An enum type</typeparam>
        /// <param name="resourceManager">The <see cref="ResourceManager"/> with translations for <typeparamref name="T"/></param>
        /// <returns>A list with all members that does not have </returns>
        public static ResourceManagerErrors ForEnum<T>(ResourceManager resourceManager)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            var resources = GetResources(resourceManager, Translator.AllCultures.Prepend(CultureInfo.InvariantCulture));
            Dictionary<string, IReadOnlyList<TranslationError>> errors = null;
            foreach (var key in Enum.GetNames(typeof(T)))
            {
                var keyErrors = For(resources, key);
                if (keyErrors.Count == 0)
                {
                    continue;
                }

                if (errors == null)
                {
                    errors = new Dictionary<string, IReadOnlyList<TranslationError>>();
                }

                errors.Add(key, keyErrors);
            }

            return errors == null
                       ? Empty
                       : new ResourceManagerErrors(errors);
        }

        /// <summary>
        /// This is meant to be used in unit tests.
        /// Performance is probably very poor and we load all resources into memory.
        /// Checks that:
        /// 1) <paramref name="key"/> has non null values for all cultures in <see cref="Translator.AllCultures"/>
        /// 2) If the resource is a format string it checks that
        ///   - All formats have the same number of parameters.
        ///   - All formats have numbering 0..1..n for the parameters.
        /// </summary>
        /// <param name="resourceManager">The <see cref="ResourceManager"/> with translations for <paramref name="key"/></param>
        /// <param name="key">The key</param>
        /// <returns>A list with all errors for the key or an empty list if no errors.</returns>
        public static IReadOnlyList<TranslationError> For(ResourceManager resourceManager, string key)
        {
            return For(resourceManager, key, Translator.AllCultures.Prepend(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// This is meant to be used in unit tests.
        /// Performance is probably very poor and we load all resources into memory.
        /// Checks that:
        /// 1) <paramref name="key"/> has non null values for all cultures in <see cref="Translator.AllCultures"/>
        /// 2) If the resource is a format string it checks that
        ///   - All formats have the same number of parameters.
        ///   - All formats have numbering 0..1..n for the parameters.
        /// </summary>
        /// <param name="resourceManager">The <see cref="ResourceManager"/> with translations for <paramref name="key"/></param>
        /// <param name="key">The key</param>
        /// <param name="cultures">The cultures to check</param>
        /// <returns>A list with all errors for the key or an empty list if no errors.</returns>
        public static IReadOnlyList<TranslationError> For(ResourceManager resourceManager, string key, IEnumerable<CultureInfo> cultures)
        {
            var resources = GetResources(resourceManager, cultures);
            return For(resources, key);
        }

        /// <summary>
        /// Dumps the errors to a formatted string
        /// </summary>
        /// <param name="tabString">The string to use for indentation</param>
        /// <param name="newLine">The newline ex. <see cref="Environment.NewLine"/></param>
        /// <returns>A formatted string with all errors or srting.Empty if none.</returns>
        public string ToString(string tabString, string newLine)
        {
            if (this.errors.Count == 0)
            {
                return string.Empty;
            }

            using (var writer = new IndentedTextWriter(new StringWriter(), tabString) { NewLine = newLine })
            {
                foreach (var keyAndErrors in this.errors)
                {
                    writer.Write("Key: ");
                    writer.WriteLine(keyAndErrors.Key);
                    writer.Indent++;
                    foreach (var error in keyAndErrors.Value)
                    {
                        error.WriteTo(writer);
                    }

                    writer.Indent--;
                }

                return writer.InnerWriter.ToString();
            }
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<string, IReadOnlyList<TranslationError>>> GetEnumerator() => this.errors.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc />
        public bool ContainsKey(string key) => this.errors.ContainsKey(key);

        /// <inheritdoc />
        public bool TryGetValue(string key, out IReadOnlyList<TranslationError> value)
        {
            return this.errors.TryGetValue(key, out value);
        }

        private static IReadOnlyList<TranslationError> For(IReadOnlyDictionary<CultureInfo, ResourceSet> resources, string key)
        {
            // not optimized at all here, only expecting this to be called in tests.
            List<TranslationError> errors = new List<TranslationError>();
            var translations = resources.ToDictionary(x => x.Key, x => x.Value.GetString(key));
            if (translations.Any(x => FormatString.GetFormatItems(x.Value).Count > 0))
            {
                if (translations.Any(x => !FormatString.AreItemsValid(FormatString.GetFormatItems(x.Value))))
                {
                    errors.Add(new FormatError(key, translations));
                }
                else if (translations.Select(x => FormatString.CountUnique(FormatString.GetFormatItems(x.Value)))
                                .Distinct()
                                .Count() > 1)
                {
                    errors.Add(new FormatError(key, translations));
                }
            }

            if (translations.Any(x => x.Value == null))
            {
                errors.Add(new MissingTranslation(key, translations.Where(x => x.Value == null).Select(x => x.Key).ToArray()));
            }

            return errors;
        }

        private static IReadOnlyList<string> GetKeys(ResourceManager resourceManager)
        {
            return resourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, false)
                             .OfType<DictionaryEntry>()
                             .Select(x => x.Key)
                             .OfType<string>()
                             .ToArray();
        }

        private static IReadOnlyDictionary<CultureInfo, ResourceSet> GetResources(ResourceManager resourceManager, IEnumerable<CultureInfo> cultures)
        {
            return cultures.ToDictionary(c => c, c => resourceManager.GetResourceSet(c, true, false), CultureInfoComparer.Default);
        }
    }
}
