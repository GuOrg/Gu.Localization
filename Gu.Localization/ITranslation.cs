namespace Gu.Localization
{
    using System.ComponentModel;

    /// <summary>
    /// A translated key
    /// </summary>
    public interface ITranslation : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the key Translated to the CurrentCulture
        /// This valus updates when <see cref="Translator.CurrentCulture"/> changes
        /// </summary>
        string Translated { get; }
    }
}