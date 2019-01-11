using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Net;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for HttpHelperTest and is intended
    ///to contain all HttpHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HttpHelperTest {


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
        ///A test for GetContentType
        ///</summary>
        [TestMethod()]
        public void GetContentTypeTest() {
            Assert.AreEqual<string>("text/css", HttpHelper.GetContentType(".css"));
            Assert.AreEqual<string>("text/html", HttpHelper.GetContentType(".html"));
            Assert.AreEqual<string>("text/xml", HttpHelper.GetContentType(".xml"));
            Assert.AreEqual<string>("text/javascript", HttpHelper.GetContentType(".js"));
            Assert.AreEqual<string>("image/jpeg", HttpHelper.GetContentType(".jpg"));
        }
    }
}
