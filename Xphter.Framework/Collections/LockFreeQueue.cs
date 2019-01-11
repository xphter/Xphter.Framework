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
            this.m_nextNodeID = 0;
            this.m_head = new Node(this.m_nextNodeID, default(T), null);
            this.m_tail = this.m_head;
        }

        private volatile Node m_head;
        private volatile Node m_tail;
        private volatile int m_nextNodeID;
        private volatile int m_inCount;
        private volatile int m_outCount;

        private void InternalEnqueue(Node node) {
            Node tail = null, next = null;
            SpinWait spin = new SpinWait();

            do {
                spin.SpinOnce();

                tail = this.m_tail;
                next = tail.Next;

                if(this.m_tail != tail) {
                    spin.SpinOnce();
                    continue;
                }

                if(next != null) {
                    // help other thread to complete enqueue operation
                    Interlocked.CompareExchange(ref this.m_tail, next, tail);

                    spin.SpinOnce();
                    continue;
                }
            } while(Interlocked.CompareExchange(ref tail.Next, node, null) != null);
            Interlocked.Increment(ref this.m_inCount);

            Interlocked.CompareExchange(ref this.m_tail, node, tail);
        }

        private bool InternalTryDequeue(out T result) {
            Node head = null, tail = null, next = null;
            SpinWait spin = new SpinWait();

            do {
                spin.SpinOnce();

                head = this.m_head;
                tail = this.m_tail;

                if(head == tail) {
                    result = default(T);
                    return false;
                }

                next = head.Next;
            } while(Interlocked.CompareExchange(ref this.m_head, next, head) != head);
            Interlocked.Increment(ref this.m_outCount);
            result = next.Value;
            head.Next = null;

            return true;
        }

        public int Count {
            get {
                int count = 0;
                Node head = this.m_head;

                while((head = head.Next) != null) {
                    ++count;
                }

                return count;
            }
        }

        public bool IsEmpty {
            get {
                Node head = this.m_head;
                Node tail = this.m_tail;

                return head == tail;
            }
        }

        public void Enqueue(T value) {
            if(value == null) {
                throw new ArgumentNullException("value");
            }

            Node node = new Node(Interlocked.Increment(ref this.m_nextNodeID), value, null);

            Node tail = this.m_tail;

            if(Interlocked.CompareExchange(ref tail.Next, node, null) == null) {
                Interlocked.Increment(ref this.m_inCount);
                Interlocked.CompareExchange(ref this.m_tail, node, tail);
            } else {
                this.InternalEnqueue(node);
            }
        }

        public bool TryDequeue(out T result) {
            Node head = this.m_head, tail = this.m_tail;

            /*
             * not care whether the enqueue operation is executing, this.m_tail.Next may not be null at this time.
             */
            if(head == tail) {
                result = default(T);
                return false;
            }

            Node next = head.Next;

            if(Interlocked.CompareExchange(ref this.m_head, next, head) == head) {
                /*
                 * break chain to free memory, but not clear head node value for TryPeek method.
                 */
                Interlocked.Increment(ref this.m_outCount);
                result = next.Value;
                head.Next = null;

                return true;
            } else {
                return this.InternalTryDequeue(out result);
            }
        }

        public bool TryPeek(out T result) {
            if(this.m_head == this.m_tail) {
                result = default(T);
                return false;
            }

            Node head = null, next = null;

            do {
                head = this.m_head;
                next = head.Next;
            } while(this.m_head != head);

            if(next == null) {
                result = default(T);
            } else {
                result = next.Value;
            }

            return next != null;
        }

        public void Clear() {
            Interlocked.Exchange(ref this.m_head, this.m_tail);
        }

        private class Node {
            public Node(int id, T value, Node next) {
                this.ID = id;
                this.Value = value;
                this.Next = next;
            }

            public int ID;
            public T Value;
            public Node Next;

            public override string ToString() {
                return string.Format("{0}", this.Value);
            }
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator() {
            Node next = this.m_head.Next;

            while(next != null) {
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
