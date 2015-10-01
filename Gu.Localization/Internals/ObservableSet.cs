using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Gu.Localization.Annotations;

namespace Gu.Localization.Internals
{
    class ObservableSet<T> : IObservableSet<T>
    {
        private static readonly PropertyChangedEventArgs CountPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Count));
        private static readonly NotifyCollectionChangedEventArgs ResetEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        private readonly HashSet<T> _inner;

        public ObservableSet()
        {
            _inner = new HashSet<T>();
        }

        public ObservableSet(IEqualityComparer<T> comparer )
        {
            _inner = new HashSet<T>(comparer);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Count => _inner.Count;

        public IEnumerator<T> GetEnumerator() => _inner.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Add(T item)
        {
            var added = _inner.Add(item);
            if (added)
            {
                RaiseReset();
            }

            return added;
        }

        public void UnionWith(IEnumerable<T> other)
        {
            bool added = false;
            foreach (var item in other)
            {
                if (_inner.Add(item))
                {
                    added = true;
                }
            }
            if (added)
            {
                RaiseReset();
            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        private void RaiseReset()
        {
            OnPropertyChanged(CountPropertyChangedEventArgs);
            OnCollectionChanged(ResetEventArgs);
        }
    }
}
