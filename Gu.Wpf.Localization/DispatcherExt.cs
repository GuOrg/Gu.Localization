// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatcherExt.cs" company="">
//   
// </copyright>
// <summary>
//   The dispatcher ext.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gu.Wpf.Localization
{
    using System;
    using System.Windows.Threading;

    /// <summary>
    /// The dispatcher ext.
    /// </summary>
    public static class DispatcherExt
    {
        /// <summary>
        /// The begin invoke.
        /// </summary>
        /// <param name="dispatcher">
        /// The dispatcher.
        /// </param>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <returns>
        /// The <see cref="DispatcherOperation"/>.
        /// </returns>
        public static DispatcherOperation BeginInvoke(this Dispatcher dispatcher, Action a)
        {
            return dispatcher.BeginInvoke(DispatcherPriority.Normal, a);
        }
    }
}
