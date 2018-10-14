namespace Gu.Localization.Errors
{
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>A collection of errors.</summary>
    public class TranslationErrors : IReadOnlyDictionary<string, IReadOnlyList<TranslationError>>
    {
        /// <summary>
        /// An empty collection of <see cref="TranslationError"/>.
        /// </summary>
        public static readonly TranslationErrors Empty = new TranslationErrors(EmptyReadOnlyDictionary<string, IReadOnlyList<TranslationError>>.Default);

        private readonly IReadOnlyDictionary<string, IReadOnlyList<TranslationError>> errors;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationErrors"/> class.
        /// </summary>
        public TranslationErrors(IReadOnlyDictionary<string, IReadOnlyList<TranslationError>> errors)
        {
            this.errors = errors;
        }

        /// <summary>
        /// Returns true if the collection is empty.
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
        /// <returns>A formatted string with all errors or srting.Empty if none.</returns>
        public string ToString(string tabString, string newLine)
        {
            if (this.errors.Count == 0)
            {
                return string.Empty;
            }

            using (var writer = new IndentedTextWriter(new StringWriter(), tabString) { NewLine = newLine })
            {
                foreach (var keyAndErrors in this.errors.OrderBy(x => x.Key))
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
    }
}
