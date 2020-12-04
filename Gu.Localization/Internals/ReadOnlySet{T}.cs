namespace Gu.Localization
{
    using System.Collections;
    using System.Collections.Generic;

    internal sealed class ReadOnlySet<T> : IReadOnlyCollection<T>
    {
        internal static readonly ReadOnlySet<T> Empty = new ReadOnlySet<T>(new T[0]);

        private readonly HashSet<T> set;

        internal ReadOnlySet(IEnumerable<T> items)
        {
            this.set = new HashSet<T>(items);
        }

        public int Count => this.set.Count;

        public IEnumerator<T> GetEnumerator() => this.set.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        internal bool Contains(T item)
        {
            return this.set.Contains(item);
        }
    }
}
