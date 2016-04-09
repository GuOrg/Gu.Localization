namespace Gu.Wpf.Localization
{
    using System.Globalization;
    using System.Windows;

    using Gu.Localization;

    public static class TranslatorBehavior
    {
        public static readonly DependencyProperty CultureProperty = DependencyProperty.RegisterAttached(
            "Culture",
            typeof(CultureInfo),
            typeof(TranslatorBehavior),
            new FrameworkPropertyMetadata(
                CultureInfo.CurrentUICulture,
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
            if (Is.DesignMode)
            {
                var frameworkElement = o as FrameworkElement;
                if (frameworkElement != null && !frameworkElement.IsLoaded)
                {
                    frameworkElement.Loaded += (sender, args) =>
                    {
                        Translator.CurrentCulture = (CultureInfo)e.NewValue;
                    };
                }

                if (frameworkElement != null && !frameworkElement.IsVisible)
                {
                    frameworkElement.IsVisibleChanged += (sender, args) =>
                    {
                        Translator.CurrentCulture = (CultureInfo)e.NewValue;
                    };
                }
            }
            else
            {
                Translator.CurrentCulture = (CultureInfo)e.NewValue;
            }
        }
    }
}
