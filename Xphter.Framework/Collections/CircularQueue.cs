using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Collections {
    /// <summary>
    /// Represents a circular first-in, first-out collection of objects.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the queue.</typeparam>
    public class CircularQueue<T> : IEnumerable<T>, ICollection, IEnumerable {
        /// <summary>
        /// Initialize a new instance of CircularQueue class that is empty and has the default initial capacity.
        /// </summary>
        public CircularQueue() {
            this.m_internalQueue = new Queue<T>();
        }

        /// <summary>
        /// Initialize a new instance of CircularQueue class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection"></param>
        public CircularQueue(IEnumerable<T> collection) {
            this.m_internalQueue = new Queue<T>(collection);
        }

        /// <summary>
        /// Initialize a new instance of CircularQueue class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity"></param>
        public CircularQueue(int capacity) {
            this.m_internalQueue = new Queue<T>(capacity);
        }

        protected Queue<T> m_internalQueue;

        /// <summary>
        /// Gets the number of elements contained in the CircularQueue<T>.
        /// </summary>
        public int Count {
            get {
                return this.m_internalQueue.Count;
            }
        }

        /// <summary>
        /// Removes all objects from the CircularQueue<T>.
        /// </summary>
        public void Clear() {
            this.m_internalQueue.Clear();
        }

        /// <summary>
        /// Determines whether an element is in the CircularQueue<T>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item) {
            return this.m_internalQueue.Contains(item);
        }

        /// <summary>
        /// Copies the CircularQueue<T> elements to an existing one-dimensional Array, starting at the specified array index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex) {
            this.m_internalQueue.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns the object at the beginning of the CircularQueue<T> and move it to end of the CircularQueue<T>.
        /// </summary>
        /// <returns></returns>
        public T Dequeue() {
            T item = this.m_internalQueue.Dequeue();
            this.m_internalQueue.Enqueue(item);
            return item;
        }

        /// <summary>
        /// Adds an object to the end of the CircularQueue<T>.
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item) {
            this.m_internalQueue.Enqueue(item);
        }

        /// <summary>
        /// Returns the object at the beginning of the CircularQueue<T> without removing it.
        /// </summary>
        /// <returns></returns>
        public T Peek() {
            return this.m_internalQueue.Peek();
        }

        /// <summary>
        /// Copies the CircularQueue<T> elements to a new array.
        /// </summary>
        /// <returns></returns>
        public T[] ToArray() {
            return this.m_internalQueue.ToArray();
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the CircularQueue<T>, if that number is less than 90 percent of current capacity.
        /// </summary>
        public void TrimExcess() {
            this.m_internalQueue.TrimExcess();
        }

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index) {
            ((ICollection) this.m_internalQueue).CopyTo(array, index);
        }

        bool ICollection.IsSynchronized {
            get {
                return ((ICollection) this.m_internalQueue).IsSynchronized;
            }
        }

        object ICollection.SyncRoot {
            get {
                return ((ICollection) this.m_internalQueue).SyncRoot;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable) this.m_internalQueue).GetEnumerator();
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return ((IEnumerable<T>) this.m_internalQueue).GetEnumerator();
        }

        #endregion
    }
}
