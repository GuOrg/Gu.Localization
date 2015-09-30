namespace Gu.Wpf.Localization.Internals
{
    using System.ComponentModel;

    using Gu.Localization;

    internal class TranslationInfo : ITranslation
    {
        public TranslationInfo(string translated)
        {
            Translated = translated;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Translated { get;  }
    }
}
