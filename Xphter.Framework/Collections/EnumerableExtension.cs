using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Collections {
    /// <summary>
    /// Provides functions for operate IEnumerable.
    /// </summary>
    public static class EnumerableExtension {
        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int Number(this IEnumerable source) {
            if(source == null) {
                throw new ArgumentNullException("source");
            }

            if(source is ICollection) {
                return ((ICollection) source).Count;
            }

            int count = 0;
            IEnumerator enumerator = source.GetEnumerator();

            try {
                while(enumerator.MoveNext()) {
                    ++count;
                }
            } finally {
                enumerator.Reset();
            }
            
            return count;
        }

        /// <summary>
        /// Gets a value to indicate whether the specified IEnumerable only has one element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool OnlyOne<T>(this IEnumerable<T> source) {
            if(source == null) {
                throw new ArgumentNullException("source");
            }

            if(source is T[]) {
                return ((T[]) source).Length == 1;
            } else if(source is ICollection<T>) {
                return ((ICollection<T>) source).Count == 1;
            } else {
                using(IEnumerator<T> enumerator = source.GetEnumerator()) {
                    return enumerator.MoveNext() && !enumerator.MoveNext();
                }
            }
        }

        /// <summary>
        /// Gets a value to indicate whether the specified IEnumerable has repeated elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool HasRepeated<T>(this IEnumerable<T> source) {
            return HasRepeated<T>(source, null);
        }

        /// <summary>
        /// Gets a value to indicate whether the specified IEnumerable has repeated elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool HasRepeated<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer) {
            if(source == null) {
                throw new ArgumentNullException("source");
            }

            return source.GroupBy((item) => item, comparer).Any((item) => item.Count() > 1);
        }

        /// <summary>
        /// Determines whether the specified two IEnumerable objects have same elements. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool Equals<T>(this IEnumerable<T> first, IEnumerable<T> second) {
            return Equals<T>(first, second, null);
        }

        /// <summary>
        /// Determines whether the specified two IEnumerable objects have same elements. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool Equals<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer) {
            if(second == null) {
                return false;
            }
            if(object.ReferenceEquals(first, second)) {
                return true;
            }

            T firstItem = default(T);
            T secondItem = default(T);
            bool firstHasMore = false;
            bool secondHasMore = false;
            IEnumerator<T> fo = first.GetEnumerator();
            IEnumerator<T> so = second.GetEnumerator();
            while((firstHasMore = fo.MoveNext()) & (secondHasMore = so.MoveNext())) {
                if(object.ReferenceEquals(firstItem = fo.Current, secondItem = so.Current)) {
                    continue;
                }

                if(firstItem == null && secondItem != null ||
                    firstItem != null && secondItem == null) {
                    return false;
                }
                if(comparer != null) {
                    if(!comparer.Equals(firstItem, secondItem)) {
                        return false;
                    }
                } else {
                    if(!firstItem.Equals(secondItem)) {
                        return false;
                    }
                }
            }

            return firstHasMore == secondHasMore;
        }

        /// <summary>
        /// Performs the specified action on each element of a IEnumerable object.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="source">The source IEnumerable object.</param>
        /// <param name="action">The action to perform on each element.</param>
        /// <exception cref="System.ArgumentException"><paramref name="action"/> is null.</exception>
        /// <remarks>If <paramref name="source"/> may contains vast items, you should use foreach statement instead of this method.</remarks>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
            if(action == null) {
                throw new ArgumentException("action is null.", "action");
            }

            foreach(T item in source) {
                action(item);
            }
        }

        /// <summary>
        /// Performs the specified action on each element of a IEnumerable object and pass the element index to action.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="source">The source IEnumerable object.</param>
        /// <param name="action">The action to perform on each element, the element index will pass to it.</param>
        /// <exception cref="System.ArgumentException"><paramref name="action"/> is null.</exception>
        /// <remarks>If <paramref name="source"/> may contains vast items, you should use foreach statement instead of this method.</remarks>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action) {
            if(action == null) {
                throw new ArgumentException("action is null.", "action");
            }

            int index = 0;
            foreach(T item in source) {
                action(item, index++);
            }
        }

        /// <summary>
        /// Splits <paramref name="source"/> to some sequences.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int length) {
            int total = source.Count();
            if(total <= length) {
                return new IEnumerable<T>[] {
                    source,
                };
            }

            int count = (int) Math.Ceiling(total * 1.0F / length);
            T[][] result = new T[count][];
            for(int i = 0; i < count - 1; i++) {
                result[i] = new T[length];
            }
            result[count - 1] = new T[total % length == 0 ? length : total % length];

            int index = 0;
            foreach(T item in source) {
                result[index / length][index % length] = item;
                ++index;
            }

            return result;
        }

        /// <summary>
        /// Join each item in the specified IEnumerable object by the specified character.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="source">The source IEnumerable object.</param>
        /// <param name="separator">The char used to join each item in <paramref name="source"/>.</param>
        /// <returns>The joined string.</returns>
        public static string StringJoin<T>(this IEnumerable<T> source, char separator) {
            return StringJoin(source, separator, (Func<T, string>) null);
        }

        /// <summary>
        /// Join each item in the specified IEnumerable object by the specified string.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="source">The source IEnumerable object.</param>
        /// <param name="separator">The string used to join each item in <paramref name="source"/>.</param>s
        /// <returns>The joined string.</returns>
        public static string StringJoin<T>(this IEnumerable<T> source, string separator) {
            return StringJoin(source, separator, (Func<T, string>) null);
        }

        /// <summary>
        /// Join each item in the specified IEnumerable object by the specified character.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="source">The source IEnumerable object.</param>
        /// <param name="separator">The char used to join each item in <paramref name="source"/>.</param>
        /// <param name="formater">A delegate used to format a object to a string.</param>
        /// <returns>The joined string.</returns>
        public static string StringJoin<T>(this IEnumerable<T> source, char separator, Func<T, string> formater) {
            return StringJoin(source, new string(separator, 1), formater);
        }

        /// <summary>
        /// Join each item in the specified IEnumerable object by the specified string.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="source">The source IEnumerable object.</param>
        /// <param name="separator">The string used to join each item in <paramref name="source"/>.</param>
        /// <param name="formater">A delegate used to format a object to a string.</param>
        /// <returns>The joined string.</returns>
        public static string StringJoin<T>(this IEnumerable<T> source, string separator, Func<T, string> formater) {
            if(separator == null) {
                separator = string.Empty;
            }

            bool isValueType = typeof(T).IsValueType;
            StringBuilder result = new StringBuilder();
            foreach(T item in source) {
                if(!isValueType && object.ReferenceEquals(item, null)) {
                    continue;
                }

                result.AppendFormat("{0}{1}", formater != null ? formater(item) : item.ToString(), separator);
            }
            if(result.Length > 0 && separator.Length > 0) {
                result.Remove(result.Length - separator.Length, separator.Length);
            }
            return result.ToString();
        }

        /// <summary>
        /// Join each item in the specified IEnumerable object by the specified character.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="source">The source IEnumerable object.</param>
        /// <param name="separator">The char used to join each item in <paramref name="source"/>.</param>
        /// <param name="formater">A delegate used to format a object to a string.</param>
        /// <returns>The joined string.</returns>
        public static string StringJoin<T>(this IEnumerable<T> source, char separator, Func<T, int, string> formater) {
            return StringJoin(source, new string(separator, 1), formater);
        }

        /// <summary>
        /// Join each item in the specified IEnumerable object by the specified string.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="source">The source IEnumerable object.</param>
        /// <param name="separator">The string used to join each item in <paramref name="source"/>.</param>
        /// <param name="formater">A delegate used to format a object to a string.</param>
        /// <returns>The joined string.</returns>
        public static string StringJoin<T>(this IEnumerable<T> source, string separator, Func<T, int, string> formater) {
            if(separator == null) {
                separator = string.Empty;
            }

            int index = 0;
            bool isValueType = typeof(T).IsValueType;
            StringBuilder result = new StringBuilder();
            foreach(T item in source) {
                if(!isValueType && object.ReferenceEquals(item, null)) {
                    continue;
                }

                if(index > 0) {
                    result.Append(separator);
                }
                result.Append(formater != null ? formater(item, index) : item.ToString());
                ++index;
            }
            return result.ToString();
        }

        /// <summary>
        /// Creates a System.Collections.Generic.IDictionary from an System.Collections.Generic.IEnumerable according to a specified key selector function.
        /// <paramref name="keySelector"/> can produces duplicate keys.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <param name="source">An System.Collections.Generic.IEnumerable to create a System.Collections.Generic.IDictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <returns>A System.Collections.Generic.IDictionary that contains values of type TSource selected from the input sequence.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> is null, or <paramref name="keySelector"/> produces a key that is null.</exception>
        public static IDictionary<TKey, TSource> ToIDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) {
            return ToIDictionary<TSource, TKey, TSource>(source, keySelector, (item) => item, null);
        }

        /// <summary>
        /// Creates a System.Collections.Generic.IDictionary from an System.Collections.Generic.IEnumerable according to a specified key selector function and a comparer.
        /// <paramref name="keySelector"/> can produces duplicate keys.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <param name="source">An System.Collections.Generic.IEnumerable to create a System.Collections.Generic.IDictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>        
        /// <param name="comparer">An System.Collections.Generic.IEqualityComparer to compare keys.</param>
        /// <returns>A System.Collections.Generic.IDictionary that contains values of type TSource selected from the input sequence.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> is null, or <paramref name="keySelector"/> produces a key that is null.</exception>
        public static IDictionary<TKey, TSource> ToIDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) {
            return ToIDictionary<TSource, TKey, TSource>(source, keySelector, (item) => item, comparer);
        }

        /// <summary>
        /// Creates a System.Collections.Generic.IDictionary from an System.Collections.Generic.IEnumerable according to a specified key selector function and an element selector function.
        /// <paramref name="keySelector"/> can produces duplicate keys, then the value is the last value produced by <paramref name="elementSelector"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="source">An System.Collections.Generic.IEnumerable to create a System.Collections.Generic.IDictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A System.Collections.Generic.IDictionary that contains values of type TElement selected from the input sequence.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> or <paramref name="elementSelector"/> is null, or <paramref name="keySelector"/> produces a key that is null.</exception>
        public static IDictionary<TKey, TElement> ToIDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) {
            return ToIDictionary<TSource, TKey, TElement>(source, keySelector, elementSelector, null);
        }

        /// <summary>
        /// Creates a System.Collections.Generic.IDictionary from an System.Collections.Generic.IEnumerable according to a specified key selector function, a comparer, and an element selector function.
        /// <paramref name="keySelector"/> can produces duplicate keys, then the value is the last value produced by <paramref name="elementSelector"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="source">An System.Collections.Generic.IEnumerable to create a System.Collections.Generic.IDictionary from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="comparer">An System.Collections.Generic.IEqualityComparer to compare keys.</param>
        /// <returns>A System.Collections.Generic.IDictionary that contains values of type TElement selected from the input sequence.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> or <paramref name="elementSelector"/> is null, or <paramref name="keySelector"/> produces a key that is null.</exception>
        public static IDictionary<TKey, TElement> ToIDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer) {
            if(source == null) {
                throw new ArgumentNullException("source");
            }
            if(keySelector == null) {
                throw new ArgumentNullException("keySelector");
            }
            if(elementSelector == null) {
                throw new ArgumentNullException("elementSelector");
            }

            TKey key = default(TKey);
            bool isValueTypeKey = typeof(TKey).IsValueType;
            IDictionary<TKey, TElement> dictionary = new Dictionary<TKey, TElement>(comparer);
            foreach(TSource item in source) {
                key = keySelector(item);
                if(!isValueTypeKey && object.ReferenceEquals(key, null)) {
                    throw new ArgumentNullException("keySelector");
                }

                dictionary[key] = elementSelector(item);
            }

            return dictionary;
        }

        /// <summary>
        /// Adds an item to the System.Collections.Generic.ICollection<T> and return it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static T AddItem<T>(this ICollection<T> source, T item) {
            source.Add(item);
            return item;
        }

        /// <summary>
        /// Adds all items in <paramref name="data"/> to the System.Collections.Generic.ICollection<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="data"></param>
        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> data) {
            if(data == null) {
                throw new ArgumentNullException("data");
            }

            foreach(T item in data) {
                source.Add(item);
            }
        }

        /// <summary>
        /// Returns a specified number of contiguous elements from the start index of a sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<T> Take<T>(this IEnumerable<T> source, int startIndex, int count) {
            if(startIndex >= source.Count() || count <= 0) {
                return Enumerable.Empty<T>();
            } else {
                return source.Where((item, index) => index >= startIndex && index < startIndex + count);
            }
        }

        /// <summary>
        /// Returns a read-only IList wrapper for the current sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IList<T> AsReadOnly<T>(this IEnumerable<T> source) {
            return new List<T>(source).AsReadOnly();
        }

        /// <summary>
        /// Returns a read-only IDictionary wrapper for the current dictionary.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) {
            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }

        /// <summary>
        /// Completes each days from <paramref name="startTime"/> to <paramref name="endTime"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static IDictionary<DateTime, T> Complete<T>(this IDictionary<DateTime, T> source, DateTime startTime, DateTime endTime) {
            return Complete<T>(source, startTime, endTime, null);
        }

        /// <summary>
        /// Completes each elements from <paramref name="startTime"/> to <paramref name="endTime"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="nextSelector"></param>
        /// <returns></returns>
        public static IDictionary<DateTime, T> Complete<T>(this IDictionary<DateTime, T> source, DateTime startTime, DateTime endTime, Func<DateTime, DateTime> nextSelector) {
            if(startTime > endTime) {
                throw new ArgumentException("startTime is greater than endTime", "startTime");
            }

            if(nextSelector == null) {
                nextSelector = (item) => item.AddDays(1);
            }

            T value;
            DateTime key = startTime;
            IDictionary<DateTime, T> result = new Dictionary<DateTime, T>();

            while(key <= endTime) {
                if(source.ContainsKey(key)) {
                    value = source[key];
                } else {
                    value = default(T);
                }

                result[key] = value;
                key = nextSelector(key);
            }

            return result;
        }
    }
}
