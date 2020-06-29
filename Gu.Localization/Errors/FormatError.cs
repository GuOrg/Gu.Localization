namespace Gu.Localization.Errors
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// A key that has errors in format parameters.
    /// Errors can be:
    /// 'First: {1}' (does not start at zero)
    /// If different languages have different number of parameters.
    /// </summary>
    public class FormatError : TranslationError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatError"/> class.
        /// </summary>
        /// <param name="key">The resource key.</param>
        /// <param name="formats">The formats found in current cultures.</param>
        public FormatError(string key, IReadOnlyDictionary<CultureInfo, string?> formats)
            : base(key)
        {
            this.Formats = formats;
        }

        /// <summary>Gets all formats for all cultures.</summary>
        public IReadOnlyDictionary<CultureInfo, string?> Formats { get; }

        internal override void WriteTo(IndentedTextWriter writer)
        {
            writer.WriteLine("Has format errors, the formats are:");
            writer.Indent++;
            foreach (var format in this.Formats)
            {
                writer.WriteLine(format.Value ?? "null");
            }

            writer.Indent--;
        }
    }
}
