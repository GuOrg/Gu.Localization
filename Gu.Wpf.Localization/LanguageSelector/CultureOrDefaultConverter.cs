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
        /// <summary> Gets the default instance. </summary>
        public static readonly CultureOrDefaultConverter Default = new CultureOrDefaultConverter();

        /// <inheritdoc />
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
        public object Convert(object value, Type targetType, object parameter, CultureInfo _)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
        {
            if (value is CultureInfo culture)
            {
                return culture;
            }

            return Translator.CurrentCulture;
        }

        /// <inheritdoc />
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo _)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
        {
            if (value is CultureInfo culture)
            {
                return culture;
            }

            return Translator.CurrentCulture;
        }
    }
}
