using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework;

namespace Xphter.Framework.Tests {
    [TestClass()]
    public class MemcacheDistributedLockTests {
        private string[] m_servers = new string[] { "127.0.0.1:11211" };

        [TestMethod()]
        public void EnterTest_One() {
            string key = "123";

            using(MemcacheDistributedLock mdl = new MemcacheDistributedLock(this.m_servers)) {
                mdl.Enter(key);
                mdl.Exit(key);

                using(IDisposable obj = mdl.GetLock(key)) {
                }
            }
        }

        [TestMethod()]
        public void EnterTest_Mutiple() {
            string key = "123";
            int target = 0, timeout = 10000, count = 100000;

            using(MemcacheDistributedLock mdl = new MemcacheDistributedLock(this.m_servers)) {
                ConcurrentQueue<Task> tasks = new ConcurrentQueue<Task>();

                Thread t1 = new Thread(() => {
                    for(int i = 0; i < count; i++) {
                        tasks.Enqueue(Task.Factory.StartNew(() => {
                            bool isEnter = false;

                            try {
                                Assert.IsTrue(isEnter = mdl.Enter(key, timeout));

                                ++target;
                            } finally {
                                mdl.Exit(key);
                            }
                        }));
                    }
                });

                Thread t2 = new Thread(() => {
                    for(int i = 0; i < count; i++) {
                        tasks.Enqueue(Task.Factory.StartNew(() => {
                            using(IDisposable obj = mdl.GetLock(key, timeout)) {
                                Assert.IsNotNull(obj);

                                ++target;
                            }
                        }));
                    }
                });

                t1.Start();
                t2.Start();

                t1.Join();
                t2.Join();

                Task.WaitAll(tasks.ToArray());

                Assert.AreEqual(2 * count, target);
            }
        }
    }
}
