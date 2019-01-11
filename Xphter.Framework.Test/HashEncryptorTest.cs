using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Text;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for HashEncryptorTest and is intended
    ///to contain all HashEncryptorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HashEncryptorTest {


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
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void ValidateTest() {
            string message = "XphteR";
            HashEncryptor encryptor = new HashEncryptor(Encoding.UTF8, new SHA256Managed());
            Assert.IsTrue(encryptor.Validate(message, encryptor.Encrypt(message)));
        }
    }
}
