namespace Gu.Localization
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Resources;
    using System.Runtime.CompilerServices;
    using Gu.Localization.Properties;
    using JetBrains.Annotations;

    public class Translation : ITranslation
    {
        internal readonly Translator Translator;
        private readonly string key;
        private readonly Func<string> keyGetter;
        private readonly IDisposable subscription;
        private bool disposed = false;

        public Translation(Expression<Func<string>> key)
        {
            if (ExpressionHelper.IsResourceKey(key))
            {
                this.key = ExpressionHelper.GetResourceKey(key);
                this.Translator = new Translator(ResourceManagerWrapper.Create(key));
                Translator.LanguageChanged += this.OnLanguageChanged;
            }
            else
            {
                this.keyGetter = key.Compile();
            }
        }

        public Translation(ResourceManager resourceManager, string key)
            : this(new ResourceManagerWrapper(resourceManager), key)
        {
        }

        public Translation(ResourceManager resourceManager, Func<string> key, IObservable<object> trigger)
            : this(new ResourceManagerWrapper(resourceManager), null)
        {
            this.keyGetter = key;
            var propertyName = ExpressionHelper.PropertyName(() => this.Translated);
            this.subscription = trigger.Subscribe(new Observer(() => this.OnPropertyChanged(propertyName)));
        }

        internal Translation(ResourceManagerWrapper resourceManager, string key)
        {
            this.Translator = new Translator(resourceManager);
            this.key = key;
            Translator.LanguageChanged += this.OnLanguageChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The key Translated to the CurrentCulture
        /// </summary>
        public string Translated
        {
            get
            {
                var currentKey = this.key ?? this.keyGetter();
                if (this.Translator == null)
                {
                    return string.Format(Resources.NullManagerFormat, currentKey);
                }

                if (!this.Translator.HasKey(currentKey))
                {
                    return string.Format(Resources.MissingKeyFormat, currentKey);
                }

                if (!this.Translator.HasCulture(Translator.CurrentCulture))
                {
                    return string.Format(Resources.MissingTranslationFormat, currentKey);
                }

                return this.Translator.Translate(currentKey);
            }
        }

        /// <summary>
        /// Dispose(true); //I am calling you from Dispose, it's safe
        /// GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                Translator.LanguageChanged -= this.OnLanguageChanged;
                this.subscription?.Dispose();
            }

            this.disposed = true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnLanguageChanged(object sender, CultureInfo e)
        {
            this.OnPropertyChanged(nameof(this.Translated));
        }
    }
}
