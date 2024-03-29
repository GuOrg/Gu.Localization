﻿namespace Gu.Localization.Errors
{
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;

    /// <summary>A collection of errors.</summary>
#pragma warning disable CA1710 // Identifiers should have correct suffix
    public sealed class TranslationErrors : IReadOnlyDictionary<string, IReadOnlyList<TranslationError>>
#pragma warning restore CA1710 // Identifiers should have correct suffix
    {
        /// <summary>
        /// An empty collection of <see cref="TranslationError"/>.
        /// </summary>
        public static readonly TranslationErrors Empty = new(EmptyReadOnlyDictionary<string, IReadOnlyList<TranslationError>>.Default);

        private readonly IReadOnlyDictionary<string, IReadOnlyList<TranslationError>> errors;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationErrors"/> class.
        /// </summary>
        /// <param name="errors">The map of keys and errors.</param>
        public TranslationErrors(IReadOnlyDictionary<string, IReadOnlyList<TranslationError>> errors)
        {
            this.errors = errors;
        }

        /// <summary>
        /// Gets a value indicating whether the collection is empty.
        /// </summary>
        public bool IsEmpty => this.errors.Count == 0;

        /// <inheritdoc />
        public int Count => this.errors.Count;

        /// <inheritdoc />
        public IEnumerable<string> Keys => this.errors.Keys.OrderBy(x => x);

        /// <inheritdoc />
        public IEnumerable<IReadOnlyList<TranslationError>> Values => this.errors.Values;

        /// <inheritdoc />
        public IReadOnlyList<TranslationError> this[string key] => this.errors[key];

        /// <summary>
        /// Dumps the errors to a formatted string.
        /// </summary>
        /// <param name="tabString">The string to use for indentation.</param>
        /// <param name="newLine">The newline ex. <see cref="System.Environment.NewLine"/>.</param>
        /// <returns>A formatted string with all errors or string.Empty if none.</returns>
        public string ToString(string tabString, string newLine)
        {
            if (this.errors.Count == 0)
            {
                return string.Empty;
            }

            using var writer = new StringWriter();
            using var indentedWriter = new IndentedTextWriter(writer, tabString) { NewLine = newLine };
            foreach (var keyAndErrors in this.errors.OrderBy(x => x.Key))
            {
                indentedWriter.Write("Key: ");
                indentedWriter.WriteLine(keyAndErrors.Key);
                indentedWriter.Indent++;
                foreach (var error in keyAndErrors.Value)
                {
                    error.WriteTo(indentedWriter);
                }

                indentedWriter.Indent--;
            }

            return writer.ToString();
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<string, IReadOnlyList<TranslationError>>> GetEnumerator() => this.errors.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc />
        public bool ContainsKey(string key) => this.errors.ContainsKey(key);

        /// <inheritdoc />
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        public bool TryGetValue(string key, [MaybeNullWhen(false)] out IReadOnlyList<TranslationError> value)
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
#pragma warning restore IDE0079 // Remove unnecessary suppression
        {
            return this.errors.TryGetValue(key, out value);
        }
    }
}
