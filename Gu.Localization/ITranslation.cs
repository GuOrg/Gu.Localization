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
        /// Gets the <see cref="Key"/> Translated to the <see cref="Translator.CurrentCulture"/>
        /// This valus updates when <see cref="Translator.CurrentCulture"/> changes
        /// </summary>
        string Translated { get; }

        /// <summary>Gets the key for the resource in the resourcemanager.</summary>
        string Key { get; }

        /// <summary>Gets the <see cref="ErrorHandling"/> used by this translation.</summary>
        ErrorHandling ErrorHandling { get; }

        /// <summary>Calls <see cref="Translator.Translate(ResourceManager, string, CultureInfo, ErrorHandling)"/> with the key.</summary>
        /// <param name="culture">The culture.</param>
        /// <param name="errorHandlingStrategy">Specifiec how errors are handled</param>
        /// <returns>The translated string.</returns>
        string Translate(CultureInfo culture, ErrorHandling errorHandlingStrategy = ErrorHandling.Inherit);
    }
}