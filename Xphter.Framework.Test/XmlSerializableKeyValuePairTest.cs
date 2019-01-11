using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Collections;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for XmlSerializableKeyValuePairTest and is intended
    ///to contain all XmlSerializableKeyValuePairTest Unit Tests
    ///</summary>
    [TestClass()]
    public class XmlSerializableKeyValuePairTest {


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
                Instance = new XmlSerializableKeyValuePair<string, string>("InstanceKey", "InstanceValue"),
                List = new List<XmlSerializableKeyValuePair<string, string>> {
                    new XmlSerializableKeyValuePair<string, string>("123", "456"),
                    new XmlSerializableKeyValuePair<string, string>("abc", "def"),
                },
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
                Instance = new XmlSerializableKeyValuePair<string, string>("InstanceKey", "InstanceValue"),
                List = new List<XmlSerializableKeyValuePair<string, string>> {
                    new XmlSerializableKeyValuePair<string, string>("123", "456"),
                    new XmlSerializableKeyValuePair<string, string>("abc", "def"),
                },
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
            public XmlSerializableKeyValuePair<string, string> Instance;
            public List<XmlSerializableKeyValuePair<string, string>> List;
            public string No;
        }
    }
}
