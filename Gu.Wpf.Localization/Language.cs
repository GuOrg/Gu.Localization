namespace Gu.Wpf.Localization
{
    using System.Globalization;

    public class Language 
    {
        private readonly CultureInfo culture;

        public Language(CultureInfo culture)
        {
            this.culture = culture;
        }

        public CultureInfo Culture => this.culture;

        public string Name => this.culture.Name;

        public string EnglishName => this.culture.EnglishName;

        public string NativeName
        {
            get
            {
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
            return $"EnglishName: {this.EnglishName}";
        }
    }
}
