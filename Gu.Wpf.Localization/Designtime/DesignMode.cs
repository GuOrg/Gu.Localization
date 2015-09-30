namespace Gu.Wpf.Localization.Designtime
{
    using System.ComponentModel;
    using System.Windows;

    /// <summary>
    /// The design mode.
    /// </summary>
    public static class DesignMode
    {
        private static readonly  DependencyObject DependencyObject = new DependencyObject();

        /// <summary>
        /// Gets a value indicating whether is design mode.
        /// </summary>
        public static bool IsDesignMode => DesignerProperties.GetIsInDesignMode(DependencyObject);
    }
}
