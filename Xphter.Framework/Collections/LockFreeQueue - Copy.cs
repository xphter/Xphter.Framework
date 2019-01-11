using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Xphter.Framework.Collections {
#pragma warning disable 420

    /// <summary>
    /// Provides a lock-free queue implementation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LockFreeQueue<T> : IEnumerable<T> {
        public LockFreeQueue() {
            this.m_nextID = 0;
            this.m_head = new Node(this.m_nextID, default(T), null);
            this.m_tail = this.m_head;
        }

        private volatile Node m_head;
        private volatile Node m_tail;
        private volatile int m_nextID;
        private volatile int m_count;

        public int Count {
            get {
                return this.m_count;
            }
        }

        public void Enqueue(T value) {
            Node tail = null, next = null;
            Node node = new Node(Interlocked.Increment(ref this.m_nextID), value, null);

            while(true) {
                tail = this.m_tail;
                next = tail.Next;

                if(!object.ReferenceEquals(this.m_tail, tail)) {
                    continue;
                }
                if(next != null) {
                    Interlocked.CompareExchange(ref this.m_tail, next, tail);
                    continue;
                }
                if(object.ReferenceEquals(Interlocked.CompareExchange(ref tail.Next, node, next), next)) {
                    Interlocked.Increment(ref this.m_count);
                    break;
                }
            }

            Interlocked.CompareExchange(ref this.m_tail, node, tail);
        }

        public T Dequeue() {
            T value = default(T);
            Node head = null, tail = null, next = null;

            while(true) {
                head = this.m_head;
                tail = this.m_tail;
                next = head.Next;

                if(this.m_head.ID != head.ID || !object.ReferenceEquals(this.m_head, head)) {
                    continue;
                }
                if(next == null) {
                    throw new InvalidOperationException("queue is empty.");
                }
                if(object.ReferenceEquals(head, tail)) {
                    Interlocked.CompareExchange(ref this.m_tail, next, tail);
                    continue;
                }

                if(this.m_head.ID == head.ID && object.ReferenceEquals(Interlocked.CompareExchange(ref this.m_head, next, head), head)) {
                    value = next.Value;

                    // release memory
                    head.Next = null;
                    next.Value = default(T);

                    Interlocked.Decrement(ref this.m_count);
                    break;
                }
            }

            return value;
        }

        public void Clear() {
            Interlocked.Exchange(ref this.m_head, this.m_tail);
        }

        public T Peek() {
            T value = default(T);
            Node head = null, tail = null, next = null;

            while(true) {
                head = this.m_head;
                tail = this.m_tail;
                next = head.Next;

                if(this.m_head.ID != head.ID || !object.ReferenceEquals(this.m_head, head)) {
                    continue;
                }
                if(next == null) {
                    throw new InvalidOperationException("queue is empty.");
                }
                if(object.ReferenceEquals(head, tail)) {
                    Interlocked.CompareExchange(ref this.m_tail, next, tail);
                    continue;
                }

                if(this.m_head.ID == head.ID && object.ReferenceEquals(this.m_head, head)) {
                    if(next != null) {
                        value = next.Value;
                    }
                    break;
                }
            }

            return value;
        }

        private class Node {
            public Node(long id, T value, Node next) {
                this.ID = id;
                this.Value = value;
                this.Next = next;
            }

            public long ID;
            public T Value;
            public Node Next;

            public override string ToString() {
                return string.Format("{0}", this.Value);
            }
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator() {
            Node head = this.m_head, tail = this.m_tail, next = this.m_head.Next;

            while(next != null) {
                if(!object.ReferenceEquals(this.m_head, head) || !object.ReferenceEquals(this.m_tail, tail)) {
                    throw new InvalidOperationException("The queue has changed.");
                }

                yield return next.Value;

                next = next.Next;
            }

            yield break;
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        #endregion
    }

#pragma warning restore 420
}
