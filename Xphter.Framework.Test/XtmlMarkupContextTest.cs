using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Xtml;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for XtmlMarkupContextTest and is intended
    ///to contain all XtmlMarkupContextTest Unit Tests
    ///</summary>
    [TestClass()]
    public class XtmlMarkupContextTest {


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

        private XtmlMarkupContext m_context = new XtmlMarkupContext {
            OpenChar = '[',
            OpenCharEntityName = "lte",
            CloseChar = ']',
            CloseCharEntityName = "gte",
            EndChar = '/',
        };

        /// <summary>
        ///A test for Decode
        ///</summary>
        [TestMethod()]
        public void DecodeTest_Null() {
            Assert.IsNull(this.m_context.Decode(null));
        }

        /// <summary>
        ///A test for Decode
        ///</summary>
        [TestMethod()]
        public void DecodeTest_Empty() {
            Assert.IsTrue(string.IsNullOrEmpty(this.m_context.Decode(string.Empty)));
        }

        /// <summary>
        ///A test for Decode
        ///</summary>
        [TestMethod()]
        public void DecodeTest_OnlyOne() {
            Assert.AreEqual<string>("[", this.m_context.Decode("&lte;"));
        }

        /// <summary>
        ///A test for Decode
        ///</summary>
        [TestMethod()]
        public void DecodeTest_One() {
            Assert.AreEqual<string>("&amp;]&amp;", this.m_context.Decode("&amp;&gte;&amp;"));
        }

        /// <summary>
        ///A test for Decode
        ///</summary>
        [TestMethod()]
        public void DecodeTest_Multiple() {
            Assert.AreEqual<string>("ab][&amp;[", this.m_context.Decode("ab&gte;&lte;&amp;&lte;"));
        }

        /// <summary>
        ///A test for Decode
        ///</summary>
        [TestMethod()]
        public void DecodeTest_Multiple_Nesting() {
            Assert.AreEqual<string>("&amp;[&lt]abc", this.m_context.Decode("&amp;&lte;&lt&gte;abc"));
        }
    }
}
