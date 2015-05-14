namespace Gu.Wpf.Localization
{
    using System.ComponentModel;
    using System.Windows;

    internal static class DesignMode
    {
        private static readonly  DependencyObject DependencyObject = new DependencyObject();

        internal static bool IsDesignMode
        {
            get
            {
                return DesignerProperties.GetIsInDesignMode(DependencyObject);
            }
        }
    }
}
