namespace Gu.Localization.Errors
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public class MissingTranslation : TranslationError
    {
        public MissingTranslation(string key, IReadOnlyList<CultureInfo> missingCultures)
            : base(key)
        {
            this.MissingCultures = missingCultures;
        }

        public IReadOnlyList<CultureInfo> MissingCultures { get; }

        public override string ToString()
        {
            var cultureNames = $" {string.Join(", ", this.MissingCultures.Select(CultureName))} ";
            return $"Missing for: {{{cultureNames}}}";
        }

        internal override void WriteTo(IndentedTextWriter writer)
        {
            writer.WriteLine(this.ToString());
        }

        private static string CultureName(CultureInfo culture)
        {
            return CultureInfoComparer.Equals(culture, CultureInfo.InvariantCulture)
                       ? "invariant"
                       : culture.TwoLetterISOLanguageName;
        }
    }
}
