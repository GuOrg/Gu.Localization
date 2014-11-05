namespace Gu.Wpf.Localization
{
    using System.ComponentModel;
    using System.Windows;

    public static class DesignMode
    {
        private static readonly  DependencyObject DependencyObject = new DependencyObject();

        public static bool IsDesignMode
        {
            get
            {
                return DesignerProperties.GetIsInDesignMode(DependencyObject);
            }
        }
    }
}
