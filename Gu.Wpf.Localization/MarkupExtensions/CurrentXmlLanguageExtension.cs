namespace Gu.Wpf.Localization
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;
    using Gu.Localization;

    /// <summary>MarkupExtension for binging to <see cref="Translator.CurrentCulture"/>.</summary>
    [MarkupExtensionReturnType(typeof(BindingExpression))]
    public class CurrentXmlLanguageExtension : MarkupExtension
    {
        private static readonly PropertyPath ValuePath = new PropertyPath(nameof(CurrentCultureProxy.Value));

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding
                          {
                              Source = CurrentCultureProxy.Instance,
                              Path = ValuePath,
                              UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                              Mode = BindingMode.OneWay,
                              Converter = CultureToXmlLanguageConverter.Default
                          };

            return binding.ProvideValue(serviceProvider);
        }

        private class CultureToXmlLanguageConverter : IValueConverter
        {
            public static readonly IValueConverter Default = new CultureToXmlLanguageConverter();

            private CultureToXmlLanguageConverter()
            {
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var cultureInfo = (CultureInfo)value;
                return cultureInfo?.IetfLanguageTag == null
                    ? XmlLanguage.Empty
                    : XmlLanguage.GetLanguage(cultureInfo.IetfLanguageTag);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotSupportedException($"The {this.GetType().Name} can only be used in one way bindings.");
            }
        }
    }
}