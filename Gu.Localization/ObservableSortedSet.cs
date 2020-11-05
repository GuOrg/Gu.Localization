namespace Gu.Localization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    /// <summary>
    /// A wrapper for <see cref="SortedSet{T}"/> that notifies about changes.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    [Serializable]
    public class ObservableSortedSet<T> : ISet<T>, IReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly SortedSet<T> inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableSortedSet{T}"/> class.
        /// Uses <see cref="EqualityComparer{T}.Default"/>.
        /// </summary>
        public ObservableSortedSet()
            : this(Comparer<T>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableSortedSet{T}"/> class.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}"/>.</param>
        public ObservableSortedSet(IComparer<T> comparer)
        {
            this.inner = new SortedSet<T>(comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableSortedSet{T}"/> class.
        /// Uses <see cref="EqualityComparer{T}.Default"/>.
        /// </summary>
        /// <param name="collection">The <see cref="IEnumerable{T}"/>.</param>
        public ObservableSortedSet(IEnumerable<T> collection)
            : this(collection, Comparer<T>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableSortedSet{T}"/> class.
        /// </summary>
        /// <param name="collection">The <see cref="IEnumerable{T}"/>.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/>.</param>
        public ObservableSortedSet(IEnumerable<T> collection, IComparer<T> comparer)
        {
            this.inner = new SortedSet<T>(collection, comparer);
        }

        /// <inheritdoc/>
        [field: NonSerialized]
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc/>
        [field: NonSerialized]
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <summary>
        /// Gets <see cref="IReadOnlyCollection{T}.Count"/>.
        /// </summary>
        public int Count => this.inner.Count;

        /// <inheritdoc/>
#pragma warning disable CA1033 // Interface methods should be callable by child types
        bool ICollection<T>.IsReadOnly => false;
#pragma warning restore CA1033 // Interface methods should be callable by child types

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => this.inner.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Clear then union with <paramref name="items"/>.
        /// </summary>
        /// <param name="items">The <see cref="IReadOnlyCollection{T}"/>.</param>
        public void UpdateWith(IReadOnlyCollection<T> items)
        {
            if (this.inner.SetEquals(items))
            {
                return;
            }

            this.inner.Clear();
            this.UnionWith(items);
            this.RaiseReset();
        }

        /// <inheritdoc/>
        public bool Add(T item)
        {
            var changed = this.inner.Add(item);
            if (changed)
            {
                this.RaiseReset();
            }

            return changed;
        }

        /// <inheritdoc/>
        void ICollection<T>.Add(T item) => this.Add(item);

        /// <inheritdoc/>
        public void UnionWith(IEnumerable<T> other)
        {
            var count = this.inner.Count;
            this.inner.UnionWith(other);
            if (count != this.inner.Count)
            {
                this.RaiseReset();
            }
        }

        /// <inheritdoc/>
        public void IntersectWith(IEnumerable<T> other)
        {
            var count = this.inner.Count;
            this.inner.IntersectWith(other);
            if (count != this.inner.Count)
            {
                this.RaiseReset();
            }
        }

        /// <inheritdoc/>
        public void ExceptWith(IEnumerable<T> other)
        {
            var count = this.inner.Count;
            this.inner.ExceptWith(other);
            if (count != this.inner.Count)
            {
                this.RaiseReset();
            }
        }

        /// <inheritdoc/>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            var count = this.inner.Count;
            this.inner.SymmetricExceptWith(other);
            if (count != this.inner.Count)
            {
                this.RaiseReset();
            }
        }

        /// <inheritdoc/>
        public bool IsSubsetOf(IEnumerable<T> other) => this.inner.IsSubsetOf(other);

        /// <inheritdoc/>
        public bool IsSupersetOf(IEnumerable<T> other) => this.inner.IsSupersetOf(other);

        /// <inheritdoc/>
        public bool IsProperSupersetOf(IEnumerable<T> other) => this.inner.IsProperSupersetOf(other);

        /// <inheritdoc/>
        public bool IsProperSubsetOf(IEnumerable<T> other) => this.inner.IsProperSubsetOf(other);

        /// <inheritdoc/>
        public bool Overlaps(IEnumerable<T> other) => this.inner.Overlaps(other);

        /// <inheritdoc/>
        public bool SetEquals(IEnumerable<T> other) => this.inner.SetEquals(other);

        /// <inheritdoc/>
        public void Clear()
        {
            var count = this.inner.Count;
            this.inner.Clear();
            if (count > 0)
            {
                this.RaiseReset();
            }
        }

        /// <inheritdoc/>
        public bool Contains(T item) => this.inner.Contains(item);

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex) => this.inner.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            var removed = this.inner.Remove(item);
            if (removed)
            {
                this.RaiseReset();
            }

            return removed;
        }

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// Properties/methods modifying this <see cref="ObservableSortedSet{T}"/> will raise
        /// a property changed event through this virtual method.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/>.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raise CollectionChanged event to any listeners.
        /// Properties/methods modifying this <see cref="ObservableSortedSet{T}"/> will raise
        /// a collection changed event through this virtual method.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/>.</param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);
        }

        private void RaiseReset()
        {
            this.OnPropertyChanged(CachedEventArgs.CountPropertyChanged);
            this.OnCollectionChanged(CachedEventArgs.NotifyCollectionReset);
        }
    }
}
