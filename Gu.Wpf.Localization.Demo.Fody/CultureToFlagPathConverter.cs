namespace Gu.Wpf.Localization.Demo.Fody
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public sealed class CultureToFlagPathConverter : IValueConverter
    {
        /// <summary>The default instance.</summary>
        public static readonly CultureToFlagPathConverter Default = new CultureToFlagPathConverter();

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CultureInfo cultureInfo)
            {
                return "pack://application:,,,/Gu.Wpf.Localization;component/Flags/" + cultureInfo.TwoLetterISOLanguageName + ".png";
            }

            return Binding.DoNothing;
        }

        /// <inheritdoc />
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(CultureToFlagPathConverter)} can only be used with {nameof(BindingMode)}.{nameof(BindingMode.OneWay)}");
        }
    }
}
