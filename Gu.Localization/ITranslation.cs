namespace Gu.Localization
{
    using System;
    using System.ComponentModel;

    public interface ITranslation : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// The key Translated to the CurrentCulture
        /// </summary>
        string Translated { get; }
    }
}