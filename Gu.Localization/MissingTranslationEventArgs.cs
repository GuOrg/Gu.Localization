namespace Gu.Localization
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Reported by <see cref="Translator.MissingTranslation"/>.
    /// </summary>
    public class MissingTranslationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingTranslationEventArgs"/> class.
        /// </summary>
        /// <param name="language">The <see cref="CultureInfo"/> that a translation was sought for.</param>
        /// <param name="key">The resource key a translation was sought for.</param>
        public MissingTranslationEventArgs(CultureInfo? language, string key)
        {
            this.Language = language;
            this.Key = key;
        }

        /// <summary>
        /// Gets the <see cref="CultureInfo"/> that a translation was sought for.
        /// </summary>
        public CultureInfo? Language { get; }

        /// <summary>
        /// Gets the resource key a translation was sought for.
        /// </summary>
        public string Key { get; }
    }
}
