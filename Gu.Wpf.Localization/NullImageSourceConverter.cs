namespace Gu.Wpf.Localization
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class NullImageSourceConverter : IValueConverter
    {
        public static readonly NullImageSourceConverter Default = new NullImageSourceConverter();

        public NullImageSourceConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Binding.DoNothing;
            }

            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(NullImageSourceConverter)} can only be used with {nameof(BindingMode)}.{nameof(BindingMode.OneWay)}");
        }
    }
}