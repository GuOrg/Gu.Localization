namespace Gu.Wpf.Localization
{
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using Gu.Localization;

    /// <summary>
    /// The design mode.
    /// </summary>
    public static class DesignTime
    {
        public static readonly DependencyProperty CultureProperty = DependencyProperty.RegisterAttached(
            "Culture",
            typeof(CultureInfo),
            typeof(DesignTime),
            new PropertyMetadata(default(CultureInfo), OnCultureChanged));

        private static readonly DependencyObject DependencyObject = new DependencyObject();

        /// <summary>
        /// Gets a value indicating whether is design mode.
        /// </summary>
        public static bool IsDesignMode => DesignerProperties.GetIsInDesignMode(DependencyObject);

        public static void SetCulture(this ContentControl element, CultureInfo value)
        {
            element.SetValue(CultureProperty, value);
        }

        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(UserControl))]
        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static CultureInfo GetCulture(this ContentControl element)
        {
            return (CultureInfo)element.GetValue(CultureProperty);
        }

        private static void OnCultureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!IsDesignMode)
            {
                return;
            }

            Translator.CurrentCulture = (CultureInfo)e.NewValue;
        }
    }
}
