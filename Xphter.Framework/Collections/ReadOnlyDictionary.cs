using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Collections {
    /// <summary>
    /// Represents a read-only, generic collection of key/value pairs.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
        /// <summary>
        /// Initializes a new instance of ReadOnlyDictionary class.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="dictionary"/> is null.</exception>
        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary) {
            if(dictionary == null) {
                throw new ArgumentNullException("dictionary");
            }

            this.m_dictionary = dictionary;
        }

        /// <summary>
        /// The internal dictionary.
        /// </summary>
        private IDictionary<TKey, TValue> m_dictionary;

        #region IDictionary<TKey,TValue> Members

        /// <inheritdoc />
        public void Add(TKey key, TValue value) {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public bool ContainsKey(TKey key) {
            return this.m_dictionary.ContainsKey(key);
        }

        /// <inheritdoc />
        public ICollection<TKey> Keys {
            get {
                return new ReadOnlyCollection<TKey>(new List<TKey>(this.m_dictionary.Keys));
            }
        }

        /// <inheritdoc />
        public bool Remove(TKey key) {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public bool TryGetValue(TKey key, out TValue value) {
            return this.m_dictionary.TryGetValue(key, out value);
        }

        /// <inheritdoc />
        public ICollection<TValue> Values {
            get {
                return new ReadOnlyCollection<TValue>(new List<TValue>(this.m_dictionary.Values));
            }
        }

        /// <inheritdoc />
        public TValue this[TKey key] {
            get {
                return this.m_dictionary[key];
            }
            set {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        /// <inheritdoc />
        public void Add(KeyValuePair<TKey, TValue> item) {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public void Clear() {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return ((ICollection<KeyValuePair<TKey, TValue>>) this.m_dictionary).Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            ((ICollection<KeyValuePair<TKey, TValue>>) this.m_dictionary).CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public int Count {
            get {
                return this.m_dictionary.Count;
            }
        }

        /// <inheritdoc />
        public bool IsReadOnly {
            get {
                return true;
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
            return this.m_dictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() {
            return this.m_dictionary.GetEnumerator();
        }

        #endregion
    }
}
