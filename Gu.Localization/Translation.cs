namespace Gu.Localization
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Resources;
    using System.Runtime.CompilerServices;

    using Gu.Localization.Annotations;

    public class Translation : INotifyPropertyChanged, IDisposable
    {
        private readonly string _key;
        private readonly Translator _translator;
        private bool _disposed = false;
        public Translation(Expression<Func<string>> key)
        {
            if (ExpressionHelper.IsResourceKey(key))
            {
                _key = ExpressionHelper.GetResourceKey(key);
                _translator = new Translator(ResourceManagerWrapper.Create(key));
                Translator.LanguageChanged += OnLanguageChanged;
            }
            else
            {
                _key = key.Compile().Invoke();
            }
        }

        public Translation(ResourceManager resourceManager, string key)
            : this(new ResourceManagerWrapper(resourceManager), key)
        {
        }

        public Translation(ResourceManagerWrapper resourceManager, string key)
        {
            _translator = new Translator(resourceManager);
            _key = key;
            Translator.LanguageChanged += OnLanguageChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The Value for the key in CurrentCulture
        /// </summary>
        public string Value
        {
            get
            {
                if (_translator == null)
                {
                    return string.Format(Properties.Resources.NullManagerFormat, _key);
                }
                if (!_translator.HasKey(_key))
                {
                    return string.Format(Properties.Resources.MissingKeyFormat, _key);
                }
                if (!_translator.HasCulture(Translator.CurrentCulture))
                {
                    return string.Format(Properties.Resources.MissingTranslationFormat, _key);
                }
                return _translator.Translate(_key);
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
            OnPropertyChanged("Value");
        }
    }
}
