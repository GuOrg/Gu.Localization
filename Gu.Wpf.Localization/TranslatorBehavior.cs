// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TranslatorBehavior.cs" company="">
//   
// </copyright>
// <summary>
//   The translator behavior.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gu.Wpf.Localization
{
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Windows;

    using Gu.Localization;

    /// <summary>
    /// The translator behavior.
    /// </summary>
    public static class TranslatorBehavior
    {
        /// <summary>
        /// The culture property.
        /// </summary>
        public static readonly DependencyProperty CultureProperty = DependencyProperty.RegisterAttached(
            "Culture",
            typeof(CultureInfo),
            typeof(TranslatorBehavior),
            new FrameworkPropertyMetadata(
                CultureInfo.CurrentUICulture,
                FrameworkPropertyMetadataOptions.Inherits,
                OnCultureChanged));

        /// <summary>
        /// The get culture.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="CultureInfo"/>.
        /// </returns>
        public static CultureInfo GetCulture(this UIElement element)
        {
            return (CultureInfo)element.GetValue(CultureProperty);
        }

        /// <summary>
        /// The set culture.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetCulture(this UIElement element, CultureInfo value)
        {
            element.SetValue(CultureProperty, value);
        }

        /// <summary>
        /// The on culture changed.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnCultureChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (DesignMode.IsDesignMode)
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
