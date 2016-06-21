namespace Gu.Localization
{
    using System.ComponentModel;

    /// <summary>
    /// A translated key
    /// </summary>
    public interface ITranslation : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the key Translated to the <see cref="Translator.CurrentCultureOrDefault()"/>
        /// This valus updates when <see cref="Translator.CurrentCultureOrDefault()"/> changes
        /// </summary>
        string Translated { get; }
    }
}