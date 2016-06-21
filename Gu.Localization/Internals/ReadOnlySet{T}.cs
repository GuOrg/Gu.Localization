namespace Gu.Localization
{
    using System.Collections;
    using System.Collections.Generic;

    internal class ReadOnlySet<T> : IReadOnlyCollection<T>
    {
        public static readonly ReadOnlySet<T> Empty = new ReadOnlySet<T>(new T[0]);

        private readonly HashSet<T> set;

        public ReadOnlySet(IEnumerable<T> items)
        {
            this.set = new HashSet<T>(items);
        }

        public int Count => this.set.Count;

        public bool Contains(T item)
        {
            return this.set.Contains(item);
        }

        public IEnumerator<T> GetEnumerator() => this.set.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
