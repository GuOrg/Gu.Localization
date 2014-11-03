namespace Gu.Wpf.Localization
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;

    /// <summary>
    /// The object that the translation extension binds to
    /// </summary>
    public class TranslationData : IWeakEventListener, ITranslationData
    {
        protected readonly TranslationManager TranslationManager;

        protected readonly string Key;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationData"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="translationManager"></param>
        public TranslationData(string key, TranslationManager translationManager)
        {
            this.Key = key;
            this.TranslationManager = translationManager;
            if (this.TranslationManager != null)
            {
                PropertyChangedEventManager.AddListener(translationManager, this,"CurrentLanguage");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The value bound to, this will contain the translated text
        /// </summary>
        public virtual string Value
        {
            get
            {
                if (this.TranslationManager != null)
                {
                    var translationInfo = this.TranslationManager.GetInfo(this.Key);
                    switch (translationInfo)
                    {
                        case TranslationInfo.CanTranslate:
                            var translate = this.TranslationManager.Translate(this.Key);
                            return translate;
                        case TranslationInfo.MissingKey:
                            return string.Format(Properties.Resources.MissingKeyFormat, this.Key);
                        case TranslationInfo.NoLanguages:
                            return string.Format(Properties.Resources.NoLanguagesFormat, this.Key);
                        case TranslationInfo.NoResources:
                            return string.Format(Properties.Resources.MissingResourcesFormat, this.Key);
                        case TranslationInfo.NoProvider:
                            return string.Format(Properties.Resources.NullManagerFormat, this.Key);
                        case TranslationInfo.NoTranslation:
                            return string.Format(Properties.Resources.MissingTranslationFormat, this.Key);
                        default:
                            break;
                    }
                }
                return string.Format(Properties.Resources.NullManagerFormat, this.Key);
            }
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(PropertyChangedEventManager))
            {
                var handler = this.PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs("Value"));
                }
                return true;
            }
            return false;
        }
    }
}