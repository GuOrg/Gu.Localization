// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageSelector.cs" company="">
//   
// </copyright>
// <summary>
//   The language selector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Gu.Localization;

    /// <summary>
    /// The language selector.
    /// </summary>
    public class LanguageSelector : Control, IDisposable
    {
        /// <summary>
        /// The current culture property.
        /// </summary>
        public static readonly DependencyProperty CurrentLanguageProperty = DependencyProperty.Register(
            "CurrentLanguage",
            typeof(Language),
            typeof(LanguageSelector),
            new PropertyMetadata(
                new Language(new CultureInfo("en")),
                OnCurrentLanguageChanged));

        /// <summary>
        /// The cultures property key.
        /// </summary>
        private static readonly DependencyPropertyKey LanguagesPropertyKey = DependencyProperty.RegisterReadOnly(
            "Languages",
            typeof(IEnumerable<Language>),
            typeof(LanguageSelector),
            new PropertyMetadata(default(IEnumerable<Language>)));

        /// <summary>
        /// The cultures property.
        /// </summary>
        public static readonly DependencyProperty LanguagesProperty = LanguagesPropertyKey.DependencyProperty;

        /// <summary>
        /// The _disposed.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Initializes static members of the <see cref="LanguageSelector"/> class.
        /// </summary>
        static LanguageSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LanguageSelector), new FrameworkPropertyMetadata(typeof(LanguageSelector)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageSelector"/> class.
        /// </summary>
        public LanguageSelector()
        {
            Translator.LanguagesChanged += OnLanguagesChanged;
            Translator.LanguageChanged += OnLanguageChanged;
        }

        /// <summary>
        /// Gets or sets the current culture.
        /// </summary>
        public Language CurrentLanguage
        {
            get
            {
                return (Language)GetValue(CurrentLanguageProperty);
            }

            set
            {
                SetValue(CurrentLanguageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the cultures.
        /// </summary>
        public IEnumerable<Language> Languages
        {
            get
            {
                return (IEnumerable<Language>)GetValue(LanguagesProperty);
            }
            protected set
            {
                SetValue(LanguagesPropertyKey, value);
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
                Translator.LanguagesChanged -= OnLanguagesChanged;
                Translator.LanguageChanged -= OnLanguageChanged;
            }

            // Free any unmanaged objects here. 
            _disposed = true;
        }

        /// <summary>
        /// The on current culture changed.
        /// </summary>
        /// <param name="o">
        /// The dependency object.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnCurrentLanguageChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var language = e.NewValue as Language;
            if (language != null)
            {
                Translator.CurrentCulture = Translator.AllCultures.FirstOrDefault(x => x.TwoLetterISOLanguageName == language.Culture.TwoLetterISOLanguageName);
            }
            else
            {
                Translator.CurrentCulture = null;
            }
        }

        /// <summary>
        /// The on languages changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        private void OnLanguagesChanged(object sender, EventArgs eventArgs)
        {
            Dispatcher.BeginInvoke(
                () =>
                {
                    Languages = Translator.AllCultures.Select(x => new Language(x)).ToArray();
                    var currentCulture = Translator.CurrentCulture;
                    if (currentCulture != null)
                    {
                        CurrentLanguage = Languages.FirstOrDefault(x => x.Culture.TwoLetterISOLanguageName == currentCulture.TwoLetterISOLanguageName);
                    }
                });
        }

        /// <summary>
        /// The on language changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnLanguageChanged(object sender, CultureInfo e)
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
