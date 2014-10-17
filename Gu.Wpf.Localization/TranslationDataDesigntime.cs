namespace Gu.Wpf.Localization
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Gu.Wpf.Localization.Annotations;

    /// <summary>
    /// The object that the translation extension binds to during designtime
    /// </summary>
    public class TranslationDataDesigntime : ITranslationData
    {
        private readonly string _key;
        private readonly TranslationManager _translationManager;

        private int count = 0;
        public TranslationDataDesigntime(string key, TranslationManager translationManager)
        {
            _key = key;
            if (key == null)
            {
                throw new ArgumentException("TranslationDataDesigntime.ctor key == null");
                
            }
            _translationManager = translationManager;
            _translationManager.LanguageChanged += (_, e) => this.OnPropertyChanged("Value");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The value bound to, this will contain the translated text
        /// </summary>
        public string Value
        {
            get
            {
                if (count == 0)
                {
                    var translationData = new TranslationData(_key, _translationManager);
                    return translationData.Value;
                }
                count++;
                if (_translationManager != null)
                {
                    var translationInfo = _translationManager.GetInfo(_key);
                    string failedLanguages = "";
                    var missing = _translationManager.Languages.Where(x => !_translationManager.HasKey(_key, x))
                                     .ToArray();
                    if (missing.Any())
                    {
                        failedLanguages = string.Format(
                            "No translation for {1} to the following languages: {{{0}}}",
                            _key,
                            string.Join(", ", missing.Select(x => x.TwoLetterISOLanguageName)));
                    }
                    switch (translationInfo)
                    {
                        case TranslationInfo.CanTranslate:
                            var translate = _translationManager.Translate(_key);
                            return translate;
                        case TranslationInfo.MissingKey:
                            throw new InvalidOperationException(
                                "The key: {0} is missing in resources" + Environment.NewLine + failedLanguages);
                        case TranslationInfo.NoLanguages:
                            throw new InvalidOperationException(
                                "No languages in resources" + Environment.NewLine + failedLanguages);
                        case TranslationInfo.NoResources:
                            throw new InvalidOperationException("No resources" + Environment.NewLine + failedLanguages);
                        case TranslationInfo.NoProvider:
                            throw new InvalidOperationException(
                                "No translation provider" + Environment.NewLine + failedLanguages);
                        case TranslationInfo.NoTranslation:
                            throw new InvalidOperationException(
                                string.Format("No translation provider for {0} in {1}", _key, _translationManager.CurrentLanguage)
                                + Environment.NewLine + failedLanguages);
                        default:
                            throw new InvalidOperationException("Unecpected translation error");
                    }
                }
                throw new InvalidOperationException(string.Format("{0}.Value: TranslationManager mgr == null", this.GetType().Name));
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
