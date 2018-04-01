namespace Gu.Wpf.Localization
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using Gu.Localization;

    /// <inheritdoc />
    [ValueConversion(typeof(CultureInfo), typeof(CultureInfo))]
    public sealed class CultureOrDefaultConverter : IValueConverter
    {
        /// <summary> Gets the default instance </summary>
        public static readonly CultureOrDefaultConverter Default = new CultureOrDefaultConverter();

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo _)
        {
            if (value is CultureInfo culture)
            {
                return culture;
            }

            return Translator.CurrentCulture;
        }

        /// <inheritdoc />
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(CultureOrDefaultConverter)} can only be used in OneWay bindings");
        }
    }
}
