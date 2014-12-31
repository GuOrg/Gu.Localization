namespace Gu.Wpf.Localization
{
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Windows;

    public static class Translator
    {
        public static ObservableCollection<CultureInfo> Cultures = new ObservableCollection<CultureInfo>();
        public static readonly DependencyProperty CultureProperty = DependencyProperty.RegisterAttached(
            "Culture",
            typeof(CultureInfo),
            typeof(Translator),
            new FrameworkPropertyMetadata(
                CultureInfo.CurrentUICulture,
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
            if (DesignMode.IsDesignMode)
            {
                var frameworkElement = o as FrameworkElement;
                if (frameworkElement != null && !frameworkElement.IsLoaded)
                {
                    frameworkElement.Loaded += (sender, args) =>
                    {
                        Gu.Localization.Translator.CurrentCulture = null;
                        Gu.Localization.Translator.CurrentCulture = (CultureInfo)e.NewValue;
                    };
                }
                if (frameworkElement != null && !frameworkElement.IsVisible)
                {
                    frameworkElement.IsVisibleChanged += (sender, args) =>
                    {
                        Gu.Localization.Translator.CurrentCulture = null;
                        Gu.Localization.Translator.CurrentCulture = (CultureInfo)e.NewValue;
                    };
                }
            }
            else
            {
                Gu.Localization.Translator.CurrentCulture = (CultureInfo)e.NewValue;
            }
        }
    }
}
