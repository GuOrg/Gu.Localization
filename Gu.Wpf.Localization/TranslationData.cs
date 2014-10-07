namespace Gu.Wpf.Localization
{
    using System;
    using System.ComponentModel;
    using System.Windows;

    public class TranslationData : IWeakEventListener, INotifyPropertyChanged
    {
        private readonly string _key;

        private readonly WeakReference<TranslationManager> _translationManagerReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationData"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="translationManager"></param>
        public TranslationData(string key, TranslationManager translationManager)
        {
            this._key = key;
            LanguageChangedEventManager.AddListener(translationManager, this);
            this._translationManagerReference = new WeakReference<TranslationManager>(translationManager);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="TranslationData"/> is reclaimed by garbage collection.
        /// </summary>
        ~TranslationData()
        {
            TranslationManager mgr;
            if (this._translationManagerReference.TryGetTarget(out mgr))
            {
                LanguageChangedEventManager.RemoveListener(mgr, this);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsDesigntime
        {
            get
            {
                return DesignerProperties.GetIsInDesignMode( new DependencyObject());
            }
        }

        public string Value
        {
            get
            {
                if (!this.IsDesigntime)
                {
                    TranslationManager mgr;
                    if (this._translationManagerReference.TryGetTarget(out mgr))
                    {
                        return mgr.Translate(this._key);
                    }
                }
                return string.Format("?{0}?", this._key);
            }
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(LanguageChangedEventManager))
            {
                this.OnLanguageChanged(sender, e);
                return true;
            }

            return false;
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }
    }
}