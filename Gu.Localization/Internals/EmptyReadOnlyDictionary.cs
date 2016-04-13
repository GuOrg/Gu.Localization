namespace Gu.Localization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal class EmptyReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        public static readonly EmptyReadOnlyDictionary<TKey, TValue> Default = new EmptyReadOnlyDictionary<TKey, TValue>();

        private EmptyReadOnlyDictionary()
        {
        }

        public int Count => 0;

        public IEnumerable<TKey> Keys => Enumerable.Empty<TKey>();

        public IEnumerable<TValue> Values => Enumerable.Empty<TValue>();

        public TValue this[TKey key]
        {
            get { throw new ArgumentOutOfRangeException(nameof(key)); }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public bool ContainsKey(TKey key) => false;

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            return false;
        }
    }
}