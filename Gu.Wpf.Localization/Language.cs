namespace Gu.Wpf.Localization
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Gu.Localization;

    using JetBrains.Annotations;

    /// <summary>Class exposing a couple of binding friendly properties for a <see cref="CultureInfo"/></summary>
    [DebuggerDisplay("Language: {Culture.DisplayName}")]
    //// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class Language : INotifyPropertyChanged
    {
        private CultureInfo culture;
        private Uri flagSource;

        /// <summary> Initializes a new instance of the <see cref="Language"/> class.</summary>
        public Language()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="Language"/> class.</summary>
        /// <param name="culture">The culture</param>
        public Language(CultureInfo culture)
        {
            this.culture = culture;
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Gets or sets the <see cref="CultureInfo"/></summary>
        public CultureInfo Culture
        {
            get
            {
                return this.culture;
            }

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

        /// <summary>Gets or sets a value indicating whether gets or sets if the <see cref="Culture"/> is the same as <see cref="Translator.CurrentCulture"/></summary>
        public bool IsSelected
        {
            get
            {
                return CultureInfoComparer.Default.Equals(Translator.CurrentCulture, this.Culture);
            }

            set
            {
                if (value == this.IsSelected)
                {
                    this.OnPropertyChanged();
                    return;
                }

                if (value &&
                    !CultureInfoComparer.Default.Equals(this.culture, Translator.CurrentCulture))
                {
                    Translator.CurrentCulture = this.culture;
                }

                this.OnPropertyChanged();
            }
        }

        /// <summary>Gets a value indicating whether gets a value indicating wheter the <see cref="Culture"/> can be used as <see cref="Translator.CurrentCulture"/></summary>
        public bool CanSelect
        {
            get
            {
                if (this.culture == null)
                {
                    return false;
                }

                return Translator.AllCultures.Contains(this.culture, CultureInfoComparer.Default);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> to the flag for the <see cref="Culture"/>
        /// </summary>
        public Uri FlagSource
        {
            get
            {
                return this.flagSource;
            }

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

        /// <summary>Gets <see cref="Culture"/> NativeName Titlecased and trimmed to text only</summary>
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

        /// <summary>Gets <see cref="Culture"/> NativeName Titlecased</summary>
        public string NativeName => ToFirstCharUpper(this.culture?.NativeName);

        /// <summary>Raises <see cref="PropertyChanged"/></summary>
        /// <param name="propertyName">The name of the property</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
