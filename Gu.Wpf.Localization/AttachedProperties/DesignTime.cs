namespace Gu.Wpf.Localization
{
    using System.Globalization;
    using System.Windows;

    using Gu.Localization;

    /// <summary>Designtime properties for the localization extension.</summary>
    public static class DesignTime
    {
        /// <summary>Identifies the <see cref="Culture"/> dependency property.</summary>
        public static readonly DependencyProperty CultureProperty = DependencyProperty.RegisterAttached(
            "Culture",
            typeof(CultureInfo),
            typeof(DesignTime),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.Inherits,
                OnCultureChanged));

        /// <summary>Helper for getting <see cref="CultureProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="CultureProperty"/> from.</param>
        /// <returns>Culture property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static CultureInfo? GetCulture(this UIElement element)
        {
            if (element is null)
            {
                throw new System.ArgumentNullException(nameof(element));
            }

            return (CultureInfo?)element.GetValue(CultureProperty);
        }

        /// <summary>Helper for setting <see cref="CultureProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="CultureProperty"/> on.</param>
        /// <param name="value">Culture property value.</param>
        public static void SetCulture(this UIElement element, CultureInfo? value)
        {
            if (element is null)
            {
                throw new System.ArgumentNullException(nameof(element));
            }

            element.SetValue(CultureProperty, value);
        }

        private static void OnCultureChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (Is.DesignMode && e.NewValue != null)
            {
                Translator.Culture = (CultureInfo?)e.NewValue;
            }
        }
    }
}
