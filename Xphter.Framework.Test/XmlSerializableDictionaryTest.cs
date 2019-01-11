using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Collections;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for XmlSerializableDictionaryTest and is intended
    ///to contain all XmlSerializableDictionaryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class XmlSerializableDictionaryTest {


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
        public void WriteXmlTest() {
            TestClass obj = new TestClass {
                ID = 13,
                Data = new XmlSerializableDictionary<string, string> {
                    {"123", "456"},
                    {"abc", "def"},
                },
                No = "Test",
            };
            XmlSerializer xs = new XmlSerializer(typeof(TestClass));
            using(StreamWriter writer = new StreamWriter(new FileStream("test.xml", FileMode.Create, FileAccess.Write, FileShare.None), Encoding.UTF8)) {
                xs.Serialize(writer, obj);
            }
        }

        [TestMethod]
        public void ReadXmlTest() {
            TestClass obj = new TestClass {
                ID = 13,
                Data = new XmlSerializableDictionary<string, string> {
                    {"123", "456"},
                    {"abc", "def"},
                },
                No = "Test",
            };
            XmlSerializer xs = new XmlSerializer(typeof(TestClass));
            using(StreamWriter writer = new StreamWriter(new FileStream("test.xml", FileMode.Create, FileAccess.Write, FileShare.None), Encoding.UTF8)) {
                xs.Serialize(writer, obj);
            }

            using(StreamReader reader = new StreamReader(new FileStream("test.xml", FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.UTF8)) {
                obj = (TestClass) xs.Deserialize(reader);
            }
        }

        public class TestClass {
            public int ID;
            public XmlSerializableDictionary<string, string> Data;
            public string No;
        }
    }
}
