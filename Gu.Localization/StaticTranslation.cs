namespace Gu.Localization
{
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>A fake translation that can be used fo nonlocalized strings.</summary>
    public class StaticTranslation : ITranslation
    {
        /// <summary>Initializes a new instance of the <see cref="StaticTranslation"/> class.</summary>
        /// <param name="translated">
        /// The text will be used as <see cref="Translated"/> and returned for every culture.
        /// </param>
        public StaticTranslation(string translated)
            : this(translated, $"{typeof(StaticTranslation).FullName}: {translated}")
        {
        }

        /// <summary>Initializes a new instance of the <see cref="StaticTranslation"/> class.</summary>
        /// <param name="translated">
        /// The text will be used as <see cref="Translated"/> and returned for every culture.
        /// </param>
        /// <param name="key">Dunno if there will ever be a use case for setting key on this.</param>
        /// <param name="errorHandling">Dunno if there will ever be a use case for setting <see cref="ErrorHandling"/> on this.</param>
        public StaticTranslation(string translated, string key, ErrorHandling errorHandling = ErrorHandling.Inherit)
        {
            this.Translated = translated;
            this.Key = key;
            this.ErrorHandling = errorHandling;
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        public string Translated { get; }

        /// <inheritdoc />
        public string Key { get; }

        /// <inheritdoc />
        public ErrorHandling ErrorHandling { get; }

        /// <inheritdoc />
        public string Translate(CultureInfo culture, ErrorHandling errorHandlingStrategy = ErrorHandling.Inherit)
        {
            return this.Translated;
        }
    }
}