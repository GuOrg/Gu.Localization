namespace Gu.Wpf.Localization
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Markup;
    using Gu.Localization;

    /// <summary>MarkupExtension for binging to <see cref="Translator.CurrentCulture"/>.</summary>
    [MarkupExtensionReturnType(typeof(BindingExpression))]
    public class CurrentXmlLanguageExtension : MarkupExtension
    {
        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding
            {
                Path = CurrentCultureExtension.TranslatorCurrentCulturePath,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.OneWay,
                Converter = CultureToXmlLanguageConverter.Default,
            };

            return binding.ProvideValue(serviceProvider);
        }

        private sealed class CultureToXmlLanguageConverter : IValueConverter
        {
            internal static readonly IValueConverter Default = new CultureToXmlLanguageConverter();

            private CultureToXmlLanguageConverter()
            {
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var cultureInfo = (CultureInfo)value;
                return cultureInfo?.IetfLanguageTag is null
                    ? XmlLanguage.Empty
                    : XmlLanguage.GetLanguage(cultureInfo.IetfLanguageTag);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotSupportedException($"The {nameof(CultureToXmlLanguageConverter)} can only be used in one way bindings.");
            }
        }
    }
}
