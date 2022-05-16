namespace Gu.Localization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <inheritdoc />
    internal sealed class EmptyReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
        where TKey : notnull
    {
        /// <summary> A cached instance. </summary>
        internal static readonly EmptyReadOnlyDictionary<TKey, TValue> Default = new();

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
        TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key] => throw new ArgumentOutOfRangeException(nameof(key));

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc />
        public bool ContainsKey(TKey key) => false;

        /// <inheritdoc />
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        {
            value = default!;
            return false;
        }
    }
}
