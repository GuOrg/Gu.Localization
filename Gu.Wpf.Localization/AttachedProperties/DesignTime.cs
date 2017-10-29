#pragma warning disable SA1600 // Elements must be documented
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1611 // Element parameters must be documented
#pragma warning disable SA1615 // Element return value must be documented
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

                              /// <summary> The <see cref="CultureInfo"/> to use in designtime </summary>
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
                Translator.Culture = (CultureInfo)e.NewValue;
            }
        }
    }
}