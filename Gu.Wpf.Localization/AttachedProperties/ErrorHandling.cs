namespace Gu.Wpf.Localization
{
    using System.Windows;

    /// <summary>Provides an attached property for setting how the <see cref="StaticExtension"/> handles errors.</summary>
    public static class ErrorHandling
    {
        /// <summary>
        /// Identifies the mode property.
        /// Inherits so setting it on window or panel may make sense.
        /// Not data bindable.
        /// </summary>
        public static readonly DependencyProperty ModeProperty = DependencyProperty.RegisterAttached(
            "Mode",
            typeof(Gu.Localization.ErrorHandling?),
            typeof(ErrorHandling),
            new FrameworkPropertyMetadata(
                default(Gu.Localization.ErrorHandling?),
                FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.NotDataBindable));

        /// <summary>Helper for setting <see cref="ModeProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="ModeProperty"/> on.</param>
        /// <param name="value">Mode property value.</param>
        public static void SetMode(DependencyObject element, Gu.Localization.ErrorHandling? value)
        {
            if (element is null)
            {
                throw new System.ArgumentNullException(nameof(element));
            }

            element.SetValue(ModeProperty, value);
        }

        /// <summary>Helper for getting <see cref="ModeProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="ModeProperty"/> from.</param>
        /// <returns>Mode property value.</returns>
        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static Gu.Localization.ErrorHandling? GetMode(DependencyObject element)
        {
            if (element is null)
            {
                throw new System.ArgumentNullException(nameof(element));
            }

            return (Gu.Localization.ErrorHandling?)element.GetValue(ModeProperty);
        }
    }
}
