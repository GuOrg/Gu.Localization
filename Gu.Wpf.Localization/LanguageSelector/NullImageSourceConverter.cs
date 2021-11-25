namespace Gu.Wpf.Localization
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <inheritdoc />
    [ValueConversion(typeof(ImageSource), typeof(object))]
    public sealed class NullImageSourceConverter : IValueConverter
    {
        /// <summary>The default instance.</summary>
        public static readonly NullImageSourceConverter Default = new();

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ?? Binding.DoNothing;
        }

        /// <inheritdoc />
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(NullImageSourceConverter)} can only be used with {nameof(BindingMode)}.{nameof(BindingMode.OneWay)}");
        }
    }
}
