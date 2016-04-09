namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using Gu.Localization;

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

        private bool disposed = false;

        static LanguageSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LanguageSelector), new FrameworkPropertyMetadata(typeof(LanguageSelector)));
        }

        public LanguageSelector()
        {
            Translator.LanguagesChanged += this.OnLanguagesChanged;
            Translator.LanguageChanged += this.OnLanguageChanged;
        }

        public Language CurrentLanguage
        {
            get { return (Language)this.GetValue(CurrentLanguageProperty); }
            set { this.SetValue(CurrentLanguageProperty, value); }
        }

        public Brush SelectedBrush
        {
            get { return (Brush)this.GetValue(SelectedBrushProperty); }
            set { this.SetValue(SelectedBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the cultures.
        /// </summary>
        public IEnumerable<Language> Languages
        {
            get { return (IEnumerable<Language>)this.GetValue(LanguagesProperty); }
            protected set { this.SetValue(LanguagesPropertyKey, value); }
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
        /// <param name="disposing">
        /// true: safe to free managed resources
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                Translator.LanguagesChanged -= this.OnLanguagesChanged;
                Translator.LanguageChanged -= this.OnLanguageChanged;
            }

            // Free any unmanaged objects here. 
            this.disposed = true;
        }

        private static void OnCurrentLanguageChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var language = e.NewValue as Language;
            Translator.CurrentCulture = language != null
                                            ? Translator.AllCultures.FirstOrDefault(x => x.TwoLetterISOLanguageName == language.Culture.TwoLetterISOLanguageName)
                                            : null;
        }

        private void OnLanguagesChanged(object sender, EventArgs eventArgs)
        {
            this.Dispatcher.BeginInvoke(
                () =>
                {
                    this.Languages = Translator.AllCultures.Select(x => new Language(x)).ToArray();
                    var currentCulture = Translator.CurrentCulture;
                    if (currentCulture != null)
                    {
                        this.CurrentLanguage = this.Languages.FirstOrDefault(x => x.Culture.TwoLetterISOLanguageName == currentCulture.TwoLetterISOLanguageName);
                    }
                });
        }

        private void OnLanguageChanged(object sender, CultureInfo e)
        {
            if (this.Languages == null)
            {
                return;
            }
            var currentCulture = Translator.CurrentCulture;
            var language = currentCulture != null
                               ? this.Languages.FirstOrDefault(
                                   x => x.Culture.TwoLetterISOLanguageName == currentCulture.TwoLetterISOLanguageName)
                               : null;
            this.Dispatcher.BeginInvoke(() => this.CurrentLanguage = language);
        }
    }
}
