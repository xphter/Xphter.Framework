using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Caching;

namespace Xphter.Framework.Test {

    /// <summary>
    ///This is a test class for ObjectPoolTest and is intended
    ///to contain all ObjectPoolTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ObjectPoolTest {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        [TestMethod()]
        public void TakeTest() {
            using(ObjectPool<TestClass> pool = new ObjectPool<TestClass>(1, new EmptyPoolObjectProvider<TestClass>())) {
                using(IObjectPoolItem<TestClass> item = pool.TakeItem()) {
                    Assert.IsNotNull(item);
                    Assert.AreEqual(0, pool.CurrentPoolSize);

                    new Thread(() => {
                        TestClass obj1 = null;

                        try {
                            obj1 = pool.TakeObject();
                        } finally {
                            pool.ReturnObject(obj1);
                        }

                        Assert.AreEqual(1, pool.CurrentPoolSize);
                    }) {
                        IsBackground = false,
                    }.Start();
                    Thread.Sleep(1000);

                    TestClass obj2 = null;
                    try {
                        obj2 = pool.TakeObject();
                        Assert.AreEqual(0, pool.CurrentPoolSize);
                    } finally {
                        pool.ReturnObject(obj2);
                    }

                    Assert.AreEqual(1, pool.CurrentPoolSize);
                }

                Assert.AreEqual(2, pool.CurrentPoolSize);
            }
        }

        private class TestClass {
        }
    }
}
