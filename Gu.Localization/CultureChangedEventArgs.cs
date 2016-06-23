namespace Gu.Localization
{
    using System;
    using System.Globalization;

    /// <summary>Event data for culture change events.</summary>
    public class CultureChangedEventArgs : EventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="CultureChangedEventArgs"/> class.</summary>
        /// <param name="culture">The culture</param>
        public CultureChangedEventArgs(CultureInfo culture)
        {
            this.Culture = culture;
        }

        /// <summary>Gets the culture.</summary>
        public CultureInfo Culture { get; }
    }
}