using Xphter.Framework.Data.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for SqlServerUtilityTest and is intended
    ///to contain all SqlServerUtilityTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SqlServerUtilityTest {


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


        /// <summary>
        ///A test for EncodeLikeString
        ///</summary>
        [TestMethod()]
        public void EncodeLikeStringTest() {
            Assert.AreEqual<string>(null, SqlServerUtility.EncodeLikeString(null));
            Assert.AreEqual<string>(" \t \r \n ", SqlServerUtility.EncodeLikeString(" \t \r \n "));
            Assert.AreEqual<string>("abcd", SqlServerUtility.EncodeLikeString("abcd"));
            Assert.AreEqual<string>("a[%]b[_]c[[]d]e[^]f", SqlServerUtility.EncodeLikeString("a%b_c[d]e^f"));
        }
    }
}
