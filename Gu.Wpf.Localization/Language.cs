namespace Gu.Wpf.Localization
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Gu.Localization;
    using JetBrains.Annotations;

    [TypeConverter(typeof(LanguageConverter))]
    public class Language : INotifyPropertyChanged
    {
        private CultureInfo _culture;

        public Language()
        {
        }

        public Language(CultureInfo culture)
        {
            _culture = culture;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CultureInfo Culture
        {
            get
            {
                return _culture;
            }
            set
            {
                if (Equals(value, _culture))
                {
                    return;
                }
                _culture = value;
                OnPropertyChanged("");
            }
        }

        public bool IsSelected
        {
            get { return Equals(Translator.CurrentCulture, Culture); }
            set
            {
                if (value && !Equals(_culture, Translator.CurrentCulture))
                {
                    Translator.CurrentCulture = _culture;
                }
                OnPropertyChanged();
            }
        }

        public bool CanSelect
        {
            get
            {
                if (_culture == null)
                {
                    return false;
                }
                return Translator.AllCultures.FirstOrDefault(x => Equals(x, _culture)) != null;
            }
        }
        public Uri FlagSource { get; set; }

        public string Name => _culture?.Name;

        public string EnglishName => _culture?.EnglishName;

        public string NativeName
        {
            get
            {
                if (_culture == null)
                {
                    return "";
                }
                var nativeName = _culture.NativeName;
                if (char.IsUpper(nativeName[0]))
                {
                    return nativeName;
                }
                return nativeName.Substring(0, 1).ToUpper() + nativeName.Substring(1);
            }
        }

        public override string ToString()
        {
            return EnglishName;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
