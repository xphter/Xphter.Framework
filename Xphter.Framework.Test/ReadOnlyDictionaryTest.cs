using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Collections;

namespace Xphter.Framework.Test {

    /// <summary>
    ///This is a test class for ReadOnlyDictionaryTest and is intended
    ///to contain all ReadOnlyDictionaryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ReadOnlyDictionaryTest {


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

        IDictionary<string, string> m_target = new Dictionary<string, string> {
            { "1", "one" },
            { "2", "two" },
        }.AsReadOnly();

        [TestMethod()]
        [ExpectedException(typeof(NotSupportedException))]
        public void AddTest() {
            this.m_target.Add("3", "three");
        }

        [TestMethod]
        public void ContainsKeyTest() {
            ICollection<string> keys = this.m_target.Keys;
            ICollection<string> values = this.m_target.Values;
            Assert.AreEqual<bool>(true, this.m_target.ContainsKey("1"));
        }

        [TestMethod()]
        [ExpectedException(typeof(NotSupportedException))]
        public void RemoveTest() {
            this.m_target.Remove("1");
        }
    }
}
