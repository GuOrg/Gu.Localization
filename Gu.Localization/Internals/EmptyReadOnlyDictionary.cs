namespace Gu.Localization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <inheritdoc />
    internal class EmptyReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        /// <summary> A cached instance </summary>
        public static readonly EmptyReadOnlyDictionary<TKey, TValue> Default = new EmptyReadOnlyDictionary<TKey, TValue>();

        private EmptyReadOnlyDictionary()
        {
        }

        /// <inheritdoc />
        public int Count => 0;

        /// <inheritdoc />
        public IEnumerable<TKey> Keys => Enumerable.Empty<TKey>();

        /// <inheritdoc />
        public IEnumerable<TValue> Values => Enumerable.Empty<TValue>();

        /// <inheritdoc />
        TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key]
        {
            get { throw new ArgumentOutOfRangeException(nameof(key)); }
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc />
        public bool ContainsKey(TKey key) => false;

        /// <inheritdoc />
        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            return false;
        }
    }
}