namespace Gu.Wpf.Localization
{
    using System.Globalization;

    public class Language 
    {
        private readonly CultureInfo _culture;
        private bool _disposed = false;

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
        }

        public string Name
        {
            get
            {
                return _culture.Name;
            }
        }

        public string EnglishName
        {
            get
            {
                return _culture.EnglishName;
            }
        }

        public string NativeName
        {
            get
            {
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
            return string.Format("EnglishName: {0}", EnglishName);
        }
    }
}
