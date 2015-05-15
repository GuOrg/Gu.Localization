namespace Gu.Localization
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Resources;
    using System.Runtime.CompilerServices;

    using Gu.Localization.Annotations;
    using Gu.Localization.Properties;

    public class Translation : ITranslation
    {
        private readonly string _key;
        private readonly Func<string> _keyGetter;
        internal readonly Translator Translator;
        private bool _disposed = false;
        private readonly IDisposable _subscription;
        public Translation(Expression<Func<string>> key)
        {
            if (ExpressionHelper.IsResourceKey(key))
            {
                _key = ExpressionHelper.GetResourceKey(key);
                Translator = new Translator(ResourceManagerWrapper.Create(key));
                Translator.LanguageChanged += OnLanguageChanged;
            }
            else
            {
                _keyGetter = key.Compile();
            }
        }

        public Translation(ResourceManager resourceManager, string key)
            : this(new ResourceManagerWrapper(resourceManager), key)
        {
        }

        public Translation(ResourceManager resourceManager, Func<string> key, IObservable<object> trigger)
            : this(new ResourceManagerWrapper(resourceManager), null)
        {
            _keyGetter = key;
            var propertyName = ExpressionHelper.PropertyName(() => Translated);
            _subscription = trigger.Subscribe(new Observer(() => OnPropertyChanged(propertyName)));
        }

        internal Translation(ResourceManagerWrapper resourceManager, string key)
        {
            Translator = new Translator(resourceManager);
            _key = key;
            Translator.LanguageChanged += OnLanguageChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The key Translated to the CurrentCulture
        /// </summary>
        public string Translated
        {
            get
            {
                var key = _key ?? _keyGetter();
                if (Translator == null)
                {
                    return string.Format(Resources.NullManagerFormat, key);
                }
                if (!Translator.HasKey(key))
                {
                    return string.Format(Resources.MissingKeyFormat, key);
                }
                if (!Translator.HasCulture(Translator.CurrentCulture))
                {
                    return string.Format(Resources.MissingTranslationFormat, key);
                }
                return Translator.Translate(key);
            }
        }

        /// <summary>
        /// Dispose(true); //I am calling you from Dispose, it's safe
        /// GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern. 
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Translator.LanguageChanged -= OnLanguageChanged;
                if (_subscription != null)
                {
                    _subscription.Dispose();
                }
            }
            _disposed = true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnLanguageChanged(object sender, CultureInfo e)
        {
            OnPropertyChanged(ExpressionHelper.PropertyName(() => Translated));
        }
    }
}
