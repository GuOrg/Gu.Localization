namespace Gu.Wpf.Localization
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using Gu.Localization;

    /// <summary>Class exposing a couple of binding friendly properties for a <see cref="CultureInfo"/>.</summary>
    [DebuggerDisplay("Language: {Culture.DisplayName}")]
    //// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class Language : INotifyPropertyChanged
    {
        private CultureInfo? culture;
        private Uri? flagSource;

        /// <summary> Initializes a new instance of the <see cref="Language"/> class.</summary>
        public Language()
        {
            CultureChangedEventManager.UpdateHandler((_, x) => this.IsSelected = Gu.Localization.Culture.NameEquals(Translator.CurrentCulture, this.Culture));
        }

        /// <summary>Initializes a new instance of the <see cref="Language"/> class.</summary>
        /// <param name="culture">The culture.</param>
        public Language(CultureInfo culture)
        {
            this.culture = culture;
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>Gets a value indicating whether gets a value indicating whether the <see cref="Culture"/> can be used as <see cref="Translator.Culture"/>.</summary>
        public bool CanSelect
        {
            get
            {
                if (this.culture is null)
                {
                    return false;
                }

                return Translator.ContainsCulture(this.culture);
            }
        }

        /// <summary>Gets or sets the <see cref="CultureInfo"/>.</summary>
        public CultureInfo? Culture
        {
            get => this.culture;

            set
            {
                if (Equals(value, this.culture))
                {
                    return;
                }

                this.culture = value;
                this.OnPropertyChanged(string.Empty);
            }
        }

        /// <summary>Gets <see cref="Culture"/> NativeName TitleCased and trimmed to text only.</summary>
        public string LanguageName
        {
            get
            {
                var indexOf = this.NativeName.IndexOf(" (", StringComparison.Ordinal);
                if (indexOf > 0)
                {
                    return this.NativeName.Substring(0, indexOf);
                }

                return this.NativeName;
            }
        }

        /// <summary>Gets <see cref="Culture"/> NativeName TitleCased.</summary>
        public string NativeName => ToFirstCharUpper(this.culture?.NativeName);

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> to the flag for the <see cref="Culture"/>.
        /// </summary>
        public Uri? FlagSource
        {
            get => this.flagSource;

            set
            {
                if (Equals(value, this.flagSource))
                {
                    return;
                }

                this.flagSource = value;
                this.OnPropertyChanged();
            }
        }

#pragma warning disable INPC010 // The property sets a different field than it returns.
        /// <summary>Gets or sets a value indicating whether gets or sets if the <see cref="Culture"/> is the same as <see cref="Translator.Culture"/>.</summary>
        public bool IsSelected
        {
            get => Gu.Localization.Culture.NameEquals(Translator.CurrentCulture, this.Culture);

            set
            {
                if (value == this.IsSelected)
                {
                    this.OnPropertyChanged();
                    return;
                }

                if (value &&
                    !Gu.Localization.Culture.NameEquals(this.culture, Translator.Culture))
                {
                    Translator.Culture = this.culture;
                }

                this.OnPropertyChanged();
            }
        }
#pragma warning restore INPC010 // The property sets a different field than it returns.

        /// <summary>Raises <see cref="PropertyChanged"/>.</summary>
        /// <param name="propertyName">The name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static string ToFirstCharUpper(string text)
        {
            if (string.IsNullOrEmpty(text) || text.Length == 1)
            {
                return text;
            }

            if (char.IsUpper(text[0]))
            {
                return text;
            }

            return $"{char.ToUpper(text[0])}{text.Substring(1)}";
        }
    }
}
