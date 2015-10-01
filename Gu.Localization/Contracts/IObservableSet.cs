namespace Gu.Localization
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    public interface IObservableSet<out T> : IReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
    }
}
