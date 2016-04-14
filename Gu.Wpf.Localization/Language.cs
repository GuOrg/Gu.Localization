namespace Gu.Wpf.Localization
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Gu.Localization;

    using JetBrains.Annotations;

    public class Language : INotifyPropertyChanged
    {
        private CultureInfo culture;

        private Uri flagSource;

        public Language()
        {
        }

        public Language(CultureInfo culture)
        {
            this.culture = culture;
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

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
                this.OnPropertyChanged("");
            }
        }

        public bool IsSelected
        {
            get { return CultureInfoComparer.Default.Equals(Translator.CurrentCulture, this.Culture); }

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

        public string Name => this.culture?.Name;

        public string EnglishName => this.culture?.EnglishName;

        public string LanguageName
        {
            get
            {
                var indexOf = this.NativeName.IndexOf(" (");
                if (indexOf > 0)
                {
                    return this.NativeName.Substring(0, indexOf);
                }

                return this.NativeName;
            }
        }

        public string NativeName
        {
            get
            {
                if (this.culture == null)
                {
                    return string.Empty;
                }

                var nativeName = this.culture.NativeName;
                if (char.IsUpper(nativeName[0]))
                {
                    return nativeName;
                }

                return nativeName.Substring(0, 1).ToUpper() + nativeName.Substring(1);
            }
        }

        public override string ToString()
        {
            return this.EnglishName;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
