namespace Gu.Localization
{
    using System.ComponentModel;
    using System.Globalization;
    using System.Resources;

    /// <summary>
    /// A translated key
    /// </summary>
    public interface ITranslation : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the key Translated to the <see cref="Translator.EffectiveCulture"/>
        /// This valus updates when <see cref="Translator.EffectiveCulture"/> changes
        /// </summary>
        string Translated { get; }

        /// <summary>Calls <see cref="Translator.Translate(ResourceManager, string, CultureInfo, ErrorHandling)"/> with the key.</summary>
        /// <param name="culture">The culture.</param>
        /// <param name="errorHandlingStrategy">Specifiec how errors are handled</param>
        /// <returns>The translated string.</returns>
        string Translate(CultureInfo culture, ErrorHandling errorHandlingStrategy = ErrorHandling.Inherit);
    }
}