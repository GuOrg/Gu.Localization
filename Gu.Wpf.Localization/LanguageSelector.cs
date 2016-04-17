﻿namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Resources;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Threading;

    using Gu.Localization;

    /// <summary>
    /// Control for selecting language
    /// </summary>
    [ContentProperty("Languages")]
    public class LanguageSelector : Control, IDisposable
    {
        /// <summary> Identifies the AutogenerateLanguages property. Default false.</summary>
        public static readonly DependencyProperty AutogenerateLanguagesProperty = DependencyProperty.Register(
            "AutogenerateLanguages",
            typeof(bool),
            typeof(LanguageSelector),
            new PropertyMetadata(
                default(bool),
                OnAutoGenerateLanguagesChanged));

        private static readonly IReadOnlyDictionary<string, string> FlagNameResourceMap;

        private bool disposed = false;

        static LanguageSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LanguageSelector), new FrameworkPropertyMetadata(typeof(LanguageSelector)));
            var assembly = typeof(LanguageSelector).Assembly;
            var names = assembly.GetManifestResourceNames();
            var match = names.Single(x => x.EndsWith(".g.resources"));

            using (var reader = new ResourceReader(assembly.GetManifestResourceStream(match)))
            {
                var flags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var enumerator = reader.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var flag = (string)enumerator.Key;
                    Debug.Assert(flag != null, "flag == null");
                    flags.Add(System.IO.Path.GetFileNameWithoutExtension(flag), flag);
                }

                FlagNameResourceMap = flags;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="LanguageSelector"/> class.</summary>
        public LanguageSelector()
        {
            Translator.CurrentCultureChanged += this.OnCurrentCultureChanged;
            this.Languages.CollectionChanged += (_, __) => this.UpdateSelected();
            this.UpdateSelected();
        }

        /// <summary>
        /// Gets or sets a value indicating whether languages should be autogenerated. If true <see cref="Languages"/> are kept in sync with <see cref="Translator.Cultures"/>
        /// Default flags are picked from ./Flags/.. if a match is found
        /// </summary>
        public bool AutogenerateLanguages
        {
            get { return (bool)this.GetValue(AutogenerateLanguagesProperty); }
            set { this.SetValue(AutogenerateLanguagesProperty, value); }
        }

        /// <summary>
        /// Gets gets or sets the cultures.
        /// </summary>
        public ObservableCollection<Language> Languages { get; } = new ObservableCollection<Language>();

        /// <inheritdoc />
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.Dispose(true);
        }

        /// <summary>Called by <see cref="Dispose()"/></summary>
        /// <param name="disposing">ugh</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Translator.CurrentCultureChanged -= this.OnCurrentCultureChanged;
            }
        }

        private static void OnAutoGenerateLanguagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var languageSelector = (LanguageSelector)d;
            languageSelector.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(languageSelector.SyncLanguages));
        }

        private void OnCurrentCultureChanged(object sender, CultureInfo e)
        {
            this.UpdateSelected();
        }

        private void UpdateSelected()
        {
            foreach (var language in this.Languages)
            {
                language.IsSelected = CultureInfoComparer.DefaultEquals(language.Culture, Translator.CurrentCulture);
            }
        }

        private void SyncLanguages()
        {
            if (this.AutogenerateLanguages)
            {
                for (int i = this.Languages.Count - 1; i >= 0; i--)
                {
                    if (!Translator.Cultures.Contains(this.Languages[i].Culture, CultureInfoComparer.Default))
                    {
                        this.Languages.RemoveAt(i);
                    }
                }

                foreach (var cultureInfo in Translator.Cultures)
                {
                    if (this.Languages.Any(x => CultureInfoComparer.DefaultEquals(x.Culture, cultureInfo)))
                    {
                        continue;
                    }

                    var language = new Language(cultureInfo);
                    string flag;
                    if (FlagNameResourceMap.TryGetValue(cultureInfo.TwoLetterISOLanguageName, out flag))
                    {
                        var key = new Uri($"pack://application:,,,/{this.GetType().Assembly.GetName().Name};component/{flag}", UriKind.Absolute);
                        language.FlagSource = key;
                    }

                    this.Languages.Add(language);
                }

                this.UpdateSelected();
            }
        }
    }
}
