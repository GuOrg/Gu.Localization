namespace Gu.Wpf.Localization
{
    using System.Windows;

    public class ErrorHandling
    {
        public static readonly DependencyProperty ModeProperty = DependencyProperty.RegisterAttached(
            "Mode",
            typeof(Gu.Localization.ErrorHandling?),
            typeof(ErrorHandling),
            new FrameworkPropertyMetadata(
                default(Gu.Localization.ErrorHandling?),
                FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.NotDataBindable));

        public static void SetMode(DependencyObject element, Gu.Localization.ErrorHandling? value)
        {
            element.SetValue(ModeProperty, value);
        }

        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static Gu.Localization.ErrorHandling? GetMode(DependencyObject element)
        {
            return (Gu.Localization.ErrorHandling?)element?.GetValue(ModeProperty);
        }
    }
}