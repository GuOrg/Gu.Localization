namespace Gu.Localization
{
    using System.ComponentModel;

    public interface ITranslation : INotifyPropertyChanged
    {
        /// <summary>
        /// The key Translated to the CurrentCulture
        /// </summary>
        string Translated { get; }
    }
}