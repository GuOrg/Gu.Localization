namespace Gu.Wpf.Localization
{
    using System;
    using System.Globalization;

    public class Language
    {
        private CultureInfo _culture;

        public Language()
        {
        }

        public Language(CultureInfo culture)
        {
            _culture = culture;
        }

        public CultureInfo Culture
        {
            get
            {
                return _culture;
            }
            set
            {
                _culture = value;
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
    }
}
