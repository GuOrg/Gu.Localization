namespace Gu.Localization
{
    using System.Collections.Specialized;
    using System.ComponentModel;

    internal static class CachedEventArgs
    {
        internal static readonly PropertyChangedEventArgs CountPropertyChanged = new("Count");
        internal static readonly NotifyCollectionChangedEventArgs NotifyCollectionReset = new(NotifyCollectionChangedAction.Reset);
    }
}