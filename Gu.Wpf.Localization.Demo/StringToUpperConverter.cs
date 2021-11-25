namespace Gu.Wpf.Localization.Demo
{
    using System;
    using System.Windows.Data;

    [ValueConversion(typeof(string), typeof(string))]
    public sealed class StringToUpperConverter : IValueConverter
    {
        public static readonly StringToUpperConverter Default = new();

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value switch
            {
                string { } s => s.ToUpper(culture),
                null => null,
                _ => throw new ArgumentException($"Expected a string, was: {value.GetType()} with value: {value}"),
            };
        }

        object IValueConverter.ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotSupportedException($"{nameof(StringToUpperConverter)} can only be used in OneWay bindings");
        }
    }
}
