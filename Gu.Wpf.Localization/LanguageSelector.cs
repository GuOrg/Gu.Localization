namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    using Gu.Localization;
    using Gu.Localization.Internals;

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
            Languages.CollectionChanged += (_, __) => Update();
            Update();
        }

        /// <summary>
        /// Gets or sets the cultures.
        /// </summary>
        public ObservableCollection<Language> Languages { get; }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Translator.CurrentLanguageChanged -= OnCurrentLanguageChanged;
            }
        }

        private void OnCurrentLanguageChanged(object sender, CultureInfo e)
        {
            Update();
        }

        private void Update()
        {
            if (Languages == null)
            {
                return;
            }

            foreach (var language in Languages)
            {
                language.IsSelected = CultureInfoComparer.Default.Equals(language.Culture, Translator.CurrentCulture);
            }
        }
    }
}
