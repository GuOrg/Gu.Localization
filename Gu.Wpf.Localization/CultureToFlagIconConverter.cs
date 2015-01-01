namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Media;

    public class CultureToFlagIconConverter : IValueConverter
    {
        private readonly List<CultureAndFlag> _languageBindings = new List<CultureAndFlag>();

        public List<CultureAndFlag> LanguageBindings
        {
            get
            {
                return _languageBindings;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cultureInfo = value as CultureInfo;
            if (cultureInfo == null)
            {
                return null;
            }
            var binding = _languageBindings.FirstOrDefault(x => x.Culture != null && x.Culture.TwoLetterISOLanguageName == cultureInfo.TwoLetterISOLanguageName);
            if (binding != null)
            {
                return binding.Flag;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CultureAndFlag
    {
        public CultureInfo Culture { get; set; }
        public ImageSource Flag { get; set; }
    }
}
