using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Xphter.Framework.Collections {
    /// <summary>
    /// Represents a thread-safe collection of key/value pairs that can be check, insert then get value by multiple threads concurrently. Not support remove operation.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class ConcurrentGrowOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TValue : class {
        /// <inheritdoc />
        public ConcurrentGrowOnlyDictionary() {
            this.m_internalDictionary = new Dictionary<TKey, TValue>();
        }

        /// <inheritdoc />
        public ConcurrentGrowOnlyDictionary(IDictionary<TKey, TValue> dictionary) {
            this.m_internalDictionary = new Dictionary<TKey, TValue>(dictionary);
        }

        /// <inheritdoc />
        public ConcurrentGrowOnlyDictionary(IEqualityComparer<TKey> comparer) {
            this.m_internalDictionary = new Dictionary<TKey, TValue>(comparer);
        }

        /// <inheritdoc />
        public ConcurrentGrowOnlyDictionary(int capacity) {
            this.m_internalDictionary = new Dictionary<TKey, TValue>(capacity);
        }

        /// <inheritdoc />
        public ConcurrentGrowOnlyDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) {
            this.m_internalDictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        /// <inheritdoc />
        public ConcurrentGrowOnlyDictionary(int capacity, IEqualityComparer<TKey> comparer) {
            this.m_internalDictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        private volatile int m_isAdding;
        private IDictionary<TKey, TValue> m_internalDictionary;

        /// <summary>
        /// Adds a key/value pair to this dictionary by using the specified function, if the key does not already exist.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valueSelector"></param>
        /// <returns></returns>
        public virtual TValue GetOrAdd(TKey key, Func<TKey, TValue> valueSelector) {
            if(valueSelector == null) {
                throw new ArgumentNullException("valueSelector");
            }

            TValue value = default(TValue);

            if(!this.m_internalDictionary.ContainsKey(key)) {
                lock(((ICollection) this.m_internalDictionary).SyncRoot) {
                    if(!this.m_internalDictionary.ContainsKey(key)) {
                        this.m_isAdding = 1;

                        try {
                            this.m_internalDictionary[key] = value = valueSelector(key);
                        } finally {
                            this.m_isAdding = 0;
                        }
                    }
                }
            }

            if(value == null) {
                SpinWait.SpinUntil(() => this.m_isAdding == 0);
                value = this.m_internalDictionary[key];
            }

            return value;
        }

        #region IDictionary<TKey,TValue> Members

        /// <inheritdoc />
        public void Add(TKey key, TValue value) {
            this.m_internalDictionary.Add(key, value);
        }

        /// <inheritdoc />
        public bool ContainsKey(TKey key) {
            return this.m_internalDictionary.ContainsKey(key);
        }

        /// <inheritdoc />
        public ICollection<TKey> Keys {
            get {
                return this.m_internalDictionary.Keys;
            }
        }

        /// <inheritdoc />
        public bool Remove(TKey key) {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public bool TryGetValue(TKey key, out TValue value) {
            return this.m_internalDictionary.TryGetValue(key, out value);
        }

        /// <inheritdoc />
        public ICollection<TValue> Values {
            get {
                return this.m_internalDictionary.Values;
            }
        }

        /// <inheritdoc />
        public TValue this[TKey key] {
            get {
                return this.m_internalDictionary[key];
            }
            set {
                this.m_internalDictionary[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        /// <inheritdoc />
        public void Add(KeyValuePair<TKey, TValue> item) {
            this.m_internalDictionary.Add(item);
        }

        /// <inheritdoc />
        public void Clear() {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return this.m_internalDictionary.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            this.m_internalDictionary.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public int Count {
            get {
                return this.m_internalDictionary.Count;
            }
        }

        /// <inheritdoc />
        public bool IsReadOnly {
            get {
                return false;
            }
        }

        /// <inheritdoc />
        public bool Remove(KeyValuePair<TKey, TValue> item) {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return this.m_internalDictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() {
            return this.m_internalDictionary.GetEnumerator();
        }

        #endregion
    }
}
