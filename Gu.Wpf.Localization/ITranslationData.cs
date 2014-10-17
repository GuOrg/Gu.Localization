namespace Gu.Wpf.Localization
{
    using System.ComponentModel;
    /// <summary>
    /// The object that the translation extension binds to
    /// </summary>
    public interface ITranslationData : INotifyPropertyChanged
    {
        /// <summary>
        /// The value bound to, this will contain the translated text
        /// </summary>
        string Value { get; }
    }
}