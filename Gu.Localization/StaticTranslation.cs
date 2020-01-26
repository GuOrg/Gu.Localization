namespace Gu.Localization
{
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.CompilerServices;

    /// <summary>A fake translation that can be used fo nonlocalized strings.</summary>
    public class StaticTranslation : ITranslation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticTranslation"/> class.
        /// Note that <see cref="Translated"/> does not change when <see cref="Translator.CurrentCulture"/> changed.
        /// </summary>
        /// <param name="translated">
        /// The text will be used as <see cref="Translated"/> and returned for every culture.
        /// </param>
        public StaticTranslation(string translated)
            : this(translated, $"{typeof(StaticTranslation).FullName}: {translated}")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticTranslation"/> class.
        /// Note that <see cref="Translated"/> does not change when <see cref="Translator.CurrentCulture"/> changed.
        /// </summary>
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
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>Gets the ~translated~ value. Note that this value does not change when <see cref="Translator.CurrentCulture"/> changed.</summary>
        public string Translated { get; }

        /// <inheritdoc />
        public string Key { get; }

        /// <inheritdoc />
        public ErrorHandling ErrorHandling { get; }

        /// <summary>Returns <see cref="Translated"/> for all inputs.</summary>
        /// <param name="culture">The culture is ignored.</param>
        /// <param name="errorHandlingStrategy">The error handling is ignored.</param>
        /// <returns>Returns <see cref="Translated"/>.</returns>
        public string Translate(CultureInfo culture, ErrorHandling errorHandlingStrategy = ErrorHandling.Inherit)
        {
            return this.Translated;
        }

        /// <summary>Use this to notify about a change of the value for the property named <paramref name="propertyName"/>.</summary>
        /// <param name="propertyName">The name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
