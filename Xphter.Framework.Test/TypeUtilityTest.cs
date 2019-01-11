using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Reflection;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for TypeUtilityTest and is intended
    ///to contain all TypeUtilityTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TypeUtilityTest {


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
        public void IsImplementsTest_True() {
            Assert.IsTrue(typeof(I1).IsImplements<I0>());
            Assert.IsTrue(typeof(C0).IsImplements<I1>());
            Assert.IsTrue(typeof(C0).IsImplements<I0>());
            Assert.IsTrue(typeof(C1).IsImplements<I1>());
            Assert.IsTrue(typeof(C1).IsImplements<I0>());
            Assert.IsTrue(typeof(C2).IsImplements<I1>());
            Assert.IsTrue(typeof(C2).IsImplements<I0>());
        }

        [TestMethod()]
        public void IsImplementsTest_False() {
            Assert.IsFalse(typeof(C0).IsImplements<I2>());
        }

        [TestMethod()]
        public void IsInheritsTest_True() {
            Assert.IsTrue(typeof(C1).IsInherits<C0>());
            Assert.IsTrue(typeof(C2).IsInherits<C0>());
        }

        [TestMethod()]
        public void IsInheritsTest_False() {
            Assert.IsFalse(typeof(C3).IsInherits<C0>());
        }

        [TestMethod]
        public void IsNullableTest() {
            Assert.IsTrue((typeof(int?).IsNullable()));
            Assert.IsTrue((typeof(Nullable<int>).IsNullable()));
            Assert.IsFalse(typeof(int).IsNullable());
        }

        public interface I0 {
        }

        public interface I1 : I0 {
        }

        public interface I2 {
        }

        public class C0 : I1 {
        }

        public class C1 : C0 {
        }

        public class C2 : C1 {
        }

        public class C3 {
        }
    }
}
