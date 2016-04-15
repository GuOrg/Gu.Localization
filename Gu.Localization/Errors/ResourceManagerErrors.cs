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

    public class ResourceManagerErrors : IReadOnlyDictionary<string, IReadOnlyList<TranslationError>>
    {
        private static readonly IReadOnlyList<TranslationError> EmptyErrors = new TranslationError[0];
        public static readonly ResourceManagerErrors Empty = new ResourceManagerErrors(EmptyReadOnlyDictionary<string, IReadOnlyList<TranslationError>>.Default);

        private readonly IReadOnlyDictionary<string, IReadOnlyList<TranslationError>> errors;

        private ResourceManagerErrors(IReadOnlyDictionary<string, IReadOnlyList<TranslationError>> errors)
        {
            this.errors = errors;
        }

        public bool IsEmpty => this.errors.Count == 0;

        /// <inheritdoc />
        public int Count => this.errors.Count;

        /// <summary>
        /// Dumps the errors to a formatted string
        /// </summary>
        /// <param name="tabString">The string to use for indentation</param>
        /// <param name="newLine">The newline ex. <see cref="Environment.NewLine"/></param>
        /// <returns>A formatted string with all errors or empty if none.</returns>
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
        public IEnumerable<string> Keys => this.errors.Keys;

        /// <inheritdoc />
        public IEnumerable<IReadOnlyList<TranslationError>> Values => this.errors.Values;

        /// <inheritdoc />
        public IReadOnlyList<TranslationError> this[string key] => this.errors[key];

        public static ResourceManagerErrors For(ResourceManager resourceManager)
        {
            var resources = GetResources(resourceManager, Translator.AllCultures.Prepend(CultureInfo.InvariantCulture));
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
        /// This will probably mostly be used in tests
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

        public static IReadOnlyList<TranslationError> For(ResourceManager resourceManager, string key)
        {
            return For(resourceManager, key, Translator.AllCultures.Prepend(CultureInfo.InvariantCulture));
        }

        public static IReadOnlyList<TranslationError> For(ResourceManager resourceManager, string key, IEnumerable<CultureInfo> cultures)
        {
            var resources = GetResources(resourceManager, cultures);
            return For(resources, key);
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
