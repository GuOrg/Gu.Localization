// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DesignMode.cs" company="">
//
// </copyright>
// <summary>
//   The design mode.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gu.Wpf.Localization
{
    using System.ComponentModel;
    using System.Windows;

    /// <summary>
    /// The design mode.
    /// </summary>
    internal static class Is
    {
        /// <summary>
        /// The dependency object.
        /// </summary>
        private static readonly DependencyObject DependencyObject = new DependencyObject();

        /// <summary>
        /// Gets a value indicating whether is design mode.
        /// </summary>
        internal static bool DesignMode => DesignerProperties.GetIsInDesignMode(DependencyObject);
    }
}
