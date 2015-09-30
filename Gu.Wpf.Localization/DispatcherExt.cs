namespace Gu.Wpf.Localization
{
    using System;
    using System.Windows.Threading;

    /// <summary>
    /// The dispatcher ext.
    /// </summary>
    public static class DispatcherExt
    {
        internal static DispatcherOperation BeginInvoke(this Dispatcher dispatcher, Action a)
        {
            return dispatcher.BeginInvoke(DispatcherPriority.Normal, a);
        }
    }
}
