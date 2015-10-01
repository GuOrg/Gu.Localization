using System;
using System.ComponentModel;
using System.Globalization;

namespace Gu.Wpf.Localization
{
    internal class LanguageConverter : TypeConverter
    {
        private static readonly CultureInfoConverter CultureInfoConverter = new CultureInfoConverter();
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return CultureInfoConverter.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var cultureInfo =(CultureInfo) CultureInfoConverter.ConvertFrom(context, culture, value);
                return new Language(cultureInfo);
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}