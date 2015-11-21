namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    using Gu.Localization;
    [ContentProperty("Languages")]
    public class LanguageSelector : Control, IDisposable
    {
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
            _disposed = true;

            if (disposing)
            {
                Translator.CurrentLanguageChanged -= OnCurrentLanguageChanged;
            }
        }

        private void OnCurrentLanguageChanged(object sender, CultureInfo e)
        {
            if (Languages == null)
            {
                return;
            }
            foreach (var language in Languages)
            {
                language.IsSelected = Equals(language.Culture, Translator.CurrentCulture);
            }
        }
    }
}
