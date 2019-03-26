using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Xphter.Framework.Collections {
    /// <summary>
    /// Represents a generic collection of key/value pairs which's key can be null.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public class NullableKeyDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
        /// <summary>
        /// Initialize a instance of NullableKeyDictionary class.
        /// </summary>
        public NullableKeyDictionary() {
            this.m_internalDictionary = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Initialize a instance of NullableKeyDictionary class.
        /// </summary>
        /// <param name="dictionary"></param>
        public NullableKeyDictionary(IDictionary<TKey, TValue> dictionary) {
            this.m_internalDictionary = new Dictionary<TKey, TValue>(dictionary);
        }

        /// <summary>
        /// Initialize a instance of NullableKeyDictionary class.
        /// </summary>
        /// <param name="comparer"></param>
        public NullableKeyDictionary(IEqualityComparer<TKey> comparer) {
            this.m_internalDictionary = new Dictionary<TKey, TValue>(comparer);
        }

        /// <summary>
        /// Initialize a instance of NullableKeyDictionary class.
        /// </summary>
        /// <param name="capacity"></param>
        public NullableKeyDictionary(int capacity) {
            this.m_internalDictionary = new Dictionary<TKey, TValue>(capacity);
        }

        /// <summary>
        /// Initialize a instance of NullableKeyDictionary class.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="comparer"></param>
        public NullableKeyDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) {
            this.m_internalDictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        /// <summary>
        /// Initialize a instance of NullableKeyDictionary class.
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="comparer"></param>
        public NullableKeyDictionary(int capacity, IEqualityComparer<TKey> comparer) {
            this.m_internalDictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        /// <summary>
        ///  The null-key/value pair.
        /// </summary>
        private KeyValuePair<TKey, TValue>? m_nullKeyPair;

        /// <summary>
        /// The internal dictionay object.
        /// </summary>
        private IDictionary<TKey, TValue> m_internalDictionary;

        #region IDictionary<TKey,TValue> Members

        /// <inheritdoc />
        public void Add(TKey key, TValue value) {
            if(key == null) {
                this.m_nullKeyPair = new KeyValuePair<TKey, TValue>(key, value);
            } else {
                this.m_internalDictionary.Add(key, value);
            }
        }

        /// <inheritdoc />
        public bool ContainsKey(TKey key) {
            if(key == null) {
                return this.m_nullKeyPair.HasValue;
            } else {
                return this.m_internalDictionary.ContainsKey(key);
            }
        }

        /// <inheritdoc />
        public ICollection<TKey> Keys {
            get {
                if(this.m_nullKeyPair.HasValue) {
                    List<TKey> keys = new List<TKey>(1 + this.m_internalDictionary.Count);
                    keys.Add(this.m_nullKeyPair.Value.Key);
                    keys.AddRange(this.m_internalDictionary.Keys);
                    return keys;
                } else {
                    return this.m_internalDictionary.Keys;
                }
            }
        }

        /// <inheritdoc />
        public bool Remove(TKey key) {
            if(key == null) {
                if(this.m_nullKeyPair.HasValue) {
                    this.m_nullKeyPair = null;
                    return true;
                } else {
                    return false;
                }
            } else {
                return this.m_internalDictionary.Remove(key);
            }
        }

        /// <inheritdoc />
        public bool TryGetValue(TKey key, out TValue value) {
            if(key == null) {
                if(this.m_nullKeyPair.HasValue) {
                    value = this.m_nullKeyPair.Value.Value;
                    return true;
                } else {
                    value = default(TValue);
                    return false;
                }
            } else {
                return this.m_internalDictionary.TryGetValue(key, out value);
            }
        }

        /// <inheritdoc />
        public ICollection<TValue> Values {
            get {
                if(this.m_nullKeyPair.HasValue) {
                    List<TValue> values = new List<TValue>(1 + this.m_internalDictionary.Count);
                    values.Add(this.m_nullKeyPair.Value.Value);
                    values.AddRange(this.m_internalDictionary.Values);
                    return values;
                } else {
                    return this.m_internalDictionary.Values;
                }
            }
        }

        /// <inheritdoc />
        public TValue this[TKey key] {
            get {
                if(key == null) {
                    if(!this.m_nullKeyPair.HasValue) {
                        throw new KeyNotFoundException();
                    }
                    return this.m_nullKeyPair.Value.Value;
                } else {
                    return this.m_internalDictionary[key];
                }
            }
            set {
                if(key == null) {
                    this.m_nullKeyPair = new KeyValuePair<TKey, TValue>(key, value);
                } else {
                    this.m_internalDictionary[key] = value;
                }
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        /// <inheritdoc />
        public void Add(KeyValuePair<TKey, TValue> item) {
            if(item.Key == null) {
                this.m_nullKeyPair = new KeyValuePair<TKey, TValue>(item.Key, item.Value);
            } else {
                this.m_internalDictionary.Add(item);
            }
        }

        /// <inheritdoc />
        public void Clear() {
            this.m_nullKeyPair = null;
            this.m_internalDictionary.Clear();
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<TKey, TValue> item) {
            if(item.Key == null) {
                return this.m_nullKeyPair.HasValue;
            } else {
                return this.m_internalDictionary.Contains(item);
            }
        }

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            if(this.m_nullKeyPair.HasValue) {
                if(array == null) {
                    throw new ArgumentNullException("array");
                }
                if(arrayIndex < 0) {
                    throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex is less than zero.");
                }
                if(array.Length - arrayIndex < 1 + this.m_internalDictionary.Count) {
                    throw new ArgumentException("The number of elements in the source is greater than the available space from arrayIndex to the end of the destination array.", "arrayIndex");
                }

                array[arrayIndex] = this.m_nullKeyPair.Value;
                this.m_internalDictionary.CopyTo(array, arrayIndex + 1);
            } else {
                this.m_internalDictionary.CopyTo(array, arrayIndex);
            }
        }

        /// <inheritdoc />
        public int Count {
            get {
                return (this.m_nullKeyPair.HasValue ? 1 : 0) + this.m_internalDictionary.Count;
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
            if(item.Key == null) {
                if(this.m_nullKeyPair.HasValue) {
                    this.m_nullKeyPair = null;
                    return true;
                } else {
                    return false;
                }
            } else {
                return this.m_internalDictionary.Remove(item);
            }
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            if(this.m_nullKeyPair.HasValue) {
                return new KeyValuePair<TKey, TValue>[] { this.m_nullKeyPair.Value }.Concat(this.m_internalDictionary).GetEnumerator();
            } else {
                return this.m_internalDictionary.GetEnumerator();
            }
        }

        #endregion

        #region IEnumerable Members

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        #endregion
    }
}
