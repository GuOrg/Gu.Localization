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

        /// <summary>Initializes a new instance of the <see cref="Language"/> class.</summary>
        /// <param name="culture">The culture.</param>
        public Language(CultureInfo? culture = null)
        {
            this.culture = culture;
            CultureChangedEventManager.UpdateHandler((_, x) => this.OnPropertyChanged(nameof(this.IsSelected)));
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
        public string? LanguageName
        {
            get
            {
                if (this.NativeName is { } nativeName)
                {
                    var indexOf = nativeName.IndexOf(" (", StringComparison.Ordinal);
                    if (indexOf > 0)
                    {
                        return nativeName.Substring(0, indexOf);
                    }
                }

                return this.NativeName;
            }
        }

        /// <summary>Gets <see cref="Culture"/> NativeName TitleCased.</summary>
        public string? NativeName => ToFirstCharUpper(this.culture?.NativeName, this.culture);

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

        /// <summary>Gets or sets a value indicating whether gets or sets if the <see cref="Culture"/> is the same as <see cref="Translator.Culture"/>.</summary>
        public bool IsSelected
        {
            get => this.culture is { } temp && Gu.Localization.Culture.NameEquals(Translator.CurrentCulture, temp);

            set
            {
                if (value == this.IsSelected)
                {
                    return;
                }

                if (value &&
                    !Gu.Localization.Culture.NameEquals(Translator.Culture, this.culture))
                {
                    Translator.Culture = this.culture;
                }

                this.OnPropertyChanged();
            }
        }

        /// <summary>Raises <see cref="PropertyChanged"/>.</summary>
        /// <param name="propertyName">The name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static string? ToFirstCharUpper(string? text, CultureInfo? culture)
        {
            if (string.IsNullOrEmpty(text) || text.Length == 1)
            {
                return text;
            }

            if (char.IsUpper(text[0]))
            {
                return text;
            }

            return $"{char.ToUpper(text[0], culture ?? CultureInfo.InvariantCulture)}{text.Substring(1)}";
        }
    }
}
