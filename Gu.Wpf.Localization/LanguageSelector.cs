namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    using Gu.Localization;
    [ContentProperty("Languages")]
    public class LanguageSelector : Control, IDisposable
    {
        public static readonly DependencyProperty CurrentLanguageProperty = DependencyProperty.Register(
            "CurrentLanguage",
            typeof(Language),
            typeof(LanguageSelector),
            new PropertyMetadata(
                new Language(new CultureInfo("en")),
                OnCurrentLanguageChanged));

        private bool _disposed = false;

        static LanguageSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LanguageSelector), new FrameworkPropertyMetadata(typeof(LanguageSelector)));
        }

        public LanguageSelector()
        {
            Translator.CurrentLanguageChanged += OnCurrentLanguageChanged;
            Languages = new ObservableCollection<Language>();
        }

        public Language CurrentLanguage
        {
            get { return (Language)GetValue(CurrentLanguageProperty); }
            set { SetValue(CurrentLanguageProperty, value); }
        }

        /// <summary>
        /// Gets or sets the cultures.
        /// </summary>
        public ObservableCollection<Language> Languages { get; }

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
        /// <param name="disposing">
        /// true: safe to free managed resources
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Translator.CurrentLanguageChanged -= OnCurrentLanguageChanged;
            }

            // Free any unmanaged objects here. 
            _disposed = true;
        }

        private static void OnCurrentLanguageChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var language = e.NewValue as Language;
            if (language != null)
            {
                Translator.CurrentCulture = Translator.AllCultures.FirstOrDefault(x => x.TwoLetterISOLanguageName == language.Culture.TwoLetterISOLanguageName);
            }
            else
            {
                Translator.CurrentCulture = (CultureInfo)e.NewValue;
            }
        }

        private void OnCurrentLanguageChanged(object sender, CultureInfo e)
        {
            if (Languages == null)
            {
                return;
            }
            var currentCulture = Translator.CurrentCulture;
            var language = currentCulture != null
                               ? Languages.FirstOrDefault(
                                   x => x.Culture.TwoLetterISOLanguageName == currentCulture.TwoLetterISOLanguageName)
                               : null;
            Dispatcher.BeginInvoke(() => CurrentLanguage = language);
        }
    }
}
