namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    public class CultureToFlagIconConverter : IValueConverter
    {
        private readonly List<CultureAndFlag> languageBindings = new List<CultureAndFlag>();

        public List<CultureAndFlag> LanguageBindings => this.languageBindings;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cultureInfo = value as CultureInfo;
            if (cultureInfo == null)
            {
                return null;
            }

            var binding = this.languageBindings.FirstOrDefault(x => x.Culture != null && x.Culture.TwoLetterISOLanguageName == cultureInfo.TwoLetterISOLanguageName);
            return binding?.Flag;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
