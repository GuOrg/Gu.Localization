namespace Gu.Localization.Errors
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>A key that does not have a translation to one or more languages.</summary>
    public class MissingTranslation : TranslationError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingTranslation"/> class.
        /// </summary>
        /// <param name="key">The resource key.</param>
        /// <param name="cultures">The cultures that does not have a translation.</param>
        public MissingTranslation(string key, IReadOnlyList<CultureInfo> cultures)
            : base(key)
        {
            this.Cultures = cultures;
        }

        /// <summary> Gets a list of <see cref="CultureInfo"/> for which there is no translation for the Key.</summary>
        public IReadOnlyList<CultureInfo> Cultures { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            var cultureNames = $" {string.Join(", ", this.Cultures.Select(CultureName))} ";
            return $"Missing for: {{{cultureNames}}}";
        }

        internal override void WriteTo(IndentedTextWriter writer)
        {
            writer.WriteLine(this.ToString());
        }

        private static string CultureName(CultureInfo culture)
        {
            return culture.IsInvariant()
                       ? "invariant"
                       : culture.TwoLetterISOLanguageName;
        }
    }
}
