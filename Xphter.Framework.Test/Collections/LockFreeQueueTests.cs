using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Collections;

namespace Xphter.Framework.Collections.Tests {
    [TestClass()]
    public class LockFreeQueueTests {
        private int m_threadCount = 10000;
        private int m_dataCount = 6000;

        private void PrepareQueueData(LockFreeQueue<int> queue, int dataCount) {
            int seed = 0;
            for(int j = 0; j < dataCount; j++) {
                queue.Enqueue(Interlocked.Increment(ref seed));
            }
        }

        [TestMethod()]
        public void EnqueueTest_OneThread() {
            LockFreeQueue<int> queue = new LockFreeQueue<int>();
            this.PrepareQueueData(queue, this.m_threadCount * this.m_dataCount);

            Assert.AreEqual(this.m_threadCount * this.m_dataCount, queue.Count);
            foreach(int item in queue) {
                Assert.AreNotEqual(0, item);
            }
        }

        [TestMethod()]
        public void EnqueueTest_MutipleThread() {
            int seed = 0;
            Thread[] threads = new Thread[this.m_threadCount];
            LockFreeQueue<int> queue = new LockFreeQueue<int>();

            for(int i = 0; i < this.m_threadCount; i++) {
                threads[i] = new Thread(() => {
                    for(int j = 0; j < this.m_dataCount; j++) {
                        queue.Enqueue(Interlocked.Increment(ref seed));
                    }
                });
            }

            for(int i = 0; i < this.m_threadCount; i++) {
                threads[i].Start();
            }

            for(int i = 0; i < this.m_threadCount; i++) {
                threads[i].Join();
            }

            Assert.AreEqual(this.m_threadCount * this.m_dataCount, queue.Count);
            foreach(int item in queue) {
                Assert.AreNotEqual(0, item);
            }
        }

        [TestMethod]
        public void DequeueTest_OneThread() {
            int value = 0;
            int[] values = new int[this.m_threadCount * this.m_dataCount];
            LockFreeQueue<int> queue = new LockFreeQueue<int>();

            Assert.IsFalse(queue.TryDequeue(out value));
            this.PrepareQueueData(queue, this.m_threadCount * this.m_dataCount);

            for(int j = 0; j < this.m_threadCount * this.m_dataCount; j++) {
                if(queue.TryDequeue(out value)) {
                    Assert.AreNotEqual(0, values[j] = value);
                }
            }

            Assert.AreEqual(0, queue.Count);
        }

        [TestMethod]
        public void DequeueTest_MultipleThread() {
            Thread[] threads = new Thread[this.m_threadCount];
            ICollection<int>[] values = new ICollection<int>[this.m_threadCount];
            LockFreeQueue<int> queue = new LockFreeQueue<int>();

            this.PrepareQueueData(queue, this.m_threadCount * this.m_dataCount);

            for(int i = 0; i < this.m_threadCount; i++) {
                values[i] = new List<int>(this.m_dataCount);

                threads[i] = new Thread((state) => {
                    int value = 0;
                    int index = (int) state;

                    for(int j = 0; j < this.m_dataCount; j++) {
                        Assert.IsTrue(queue.TryDequeue(out value));
                        Assert.AreNotEqual(0, value);

                        values[index].Add(value);
                    }
                });
            }

            for(int i = 0; i < this.m_threadCount; i++) {
                threads[i].Start(i);
            }

            for(int i = 0; i < this.m_threadCount; i++) {
                threads[i].Join();
            }

            Assert.AreEqual(0, queue.Count);
            for(int i = 0; i < this.m_threadCount; i++) {
                Assert.AreEqual(this.m_dataCount, values[i].Count);
            }
            for(int i = 0; i < this.m_threadCount; i++) {
                Assert.AreEqual(this.m_dataCount, values[i].Distinct().Count());
            }
        }

        [TestMethod]
        public void AllTest() {
            int seed = 0;
            Thread[] producerThreads = new Thread[this.m_threadCount];
            Thread[] consumerThreads = new Thread[this.m_threadCount];
            LockFreeQueue<int> queue = new LockFreeQueue<int>();

            for(int i = 0; i < this.m_threadCount; i++) {
                producerThreads[i] = new Thread(() => {
                    for(int j = 0; j < this.m_dataCount; j++) {
                        queue.Enqueue(Interlocked.Increment(ref seed));
                    }
                });
            }
            for(int i = 0; i < this.m_threadCount; i++) {
                consumerThreads[i] = new Thread(() => {
                    int value = 0;
                    for(int j = 0; j < this.m_dataCount; ) {
                        if(queue.TryDequeue(out value)) {
                            Assert.AreNotEqual(0, value);

                            j++;
                        } else {
                            Thread.Sleep(1);
                        }
                    }
                });
            }

            for(int i = 0; i < this.m_threadCount; i++) {
                producerThreads[i].Start();
                consumerThreads[i].Start();
            }

            for(int i = 0; i < this.m_threadCount; i++) {
                producerThreads[i].Join();
                consumerThreads[i].Join();
            }

            Assert.AreEqual(0, queue.Count);
        }
    }
}
