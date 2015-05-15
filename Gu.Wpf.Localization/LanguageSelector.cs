namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class LanguageSelector : Control, IDisposable
    {
        public static readonly DependencyProperty CurrentLanguageProperty = DependencyProperty.Register(
            "CurrentLanguage",
            typeof(Language),
            typeof(LanguageSelector),
            new PropertyMetadata(
                new Language(new CultureInfo("en")),
                OnCurrentLanguageChanged));

        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register(
            "SelectedBrush",
            typeof(Brush),
            typeof(LanguageSelector),
            new PropertyMetadata(default(Brush)));

        private static readonly DependencyPropertyKey LanguagesPropertyKey = DependencyProperty.RegisterReadOnly(
                "Languages",
                typeof(IEnumerable<Language>),
                typeof(LanguageSelector),
                new FrameworkPropertyMetadata(
                    default(IEnumerable<Language>),
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        public static readonly DependencyProperty LanguagesProperty = LanguagesPropertyKey.DependencyProperty;

        private bool _disposed = false;

        static LanguageSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LanguageSelector), new FrameworkPropertyMetadata(typeof(LanguageSelector)));
        }

        public LanguageSelector()
        {
            Gu.Localization.Translator.LanguagesChanged += OnLanguagesChanged;
            Gu.Localization.Translator.LanguageChanged += OnLanguageChanged;
        }

        public Language CurrentLanguage
        {
            get { return (Language)GetValue(CurrentLanguageProperty); }
            set { SetValue(CurrentLanguageProperty, value); }
        }

        public Brush SelectedBrush
        {
            get { return (Brush)GetValue(SelectedBrushProperty); }
            set { SetValue(SelectedBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the cultures.
        /// </summary>
        public IEnumerable<Language> Languages
        {
            get { return (IEnumerable<Language>)GetValue(LanguagesProperty); }
            protected set { SetValue(LanguagesPropertyKey, value); }
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
                Gu.Localization.Translator.LanguagesChanged -= OnLanguagesChanged;
                Gu.Localization.Translator.LanguageChanged -= OnLanguageChanged;
            }

            // Free any unmanaged objects here. 
            _disposed = true;
        }

        private static void OnCurrentLanguageChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var language = e.NewValue as Language;
            if (language != null)
            {
                var match = Gu.Localization.Translator.AllCultures.FirstOrDefault(x => x.TwoLetterISOLanguageName == language.Culture.TwoLetterISOLanguageName);
                Gu.Localization.Translator.CurrentCulture = match;
            }
            else
            {
                Gu.Localization.Translator.CurrentCulture = null;
            }
        }

        private void OnLanguagesChanged(object sender, EventArgs eventArgs)
        {
            Dispatcher.BeginInvoke(
                () =>
                {
                    Languages = Gu.Localization.Translator.AllCultures.Select(x => new Language(x)).ToArray();
                    var currentCulture = Gu.Localization.Translator.CurrentCulture;
                    if (currentCulture != null)
                    {
                        CurrentLanguage = Languages.FirstOrDefault(x => x.Culture.TwoLetterISOLanguageName == currentCulture.TwoLetterISOLanguageName);
                    }
                });
        }

        private void OnLanguageChanged(object sender, CultureInfo e)
        {
            if (Languages == null)
            {
                return;
            }
            var currentCulture = Gu.Localization.Translator.CurrentCulture;
            var language = currentCulture != null
                               ? Languages.FirstOrDefault(
                                   x => x.Culture.TwoLetterISOLanguageName == currentCulture.TwoLetterISOLanguageName)
                               : null;
            Dispatcher.BeginInvoke(() => CurrentLanguage = language);
        }
    }
}
