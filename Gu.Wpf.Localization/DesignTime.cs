namespace Gu.Wpf.Localization
{
    using System.Globalization;
    using System.Windows;

    using Gu.Localization;

    public static partial class DesignTime
    {
        public static readonly DependencyProperty CultureProperty = DependencyProperty.RegisterAttached(
            "Culture",
            typeof(CultureInfo),
            typeof(DesignTime),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.Inherits,
                OnCultureChanged));

        public static CultureInfo GetCulture(this UIElement element)
        {
            return (CultureInfo)element.GetValue(CultureProperty);
        }

        public static void SetCulture(this UIElement element, CultureInfo value)
        {
            element.SetValue(CultureProperty, value);
        }

        private static void OnCultureChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (Is.DesignMode && e.NewValue != null)
            {
                Translator.CurrentCulture = (CultureInfo)e.NewValue;
            }
        }
    }
}