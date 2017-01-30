namespace Gu.Localization
{
    using System.Collections.Specialized;
    using System.ComponentModel;

    internal static class CachedEventArgs
    {
        internal static readonly PropertyChangedEventArgs CountPropertyChanged = new PropertyChangedEventArgs("Count");
        internal static readonly NotifyCollectionChangedEventArgs NotifyCollectionReset = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
    }
}