using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Web.JavaScript;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for JsonSerializerTest and is intended
    ///to contain all JsonSerializerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class JsonSerializerTest {


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
        ///A test for Serialize
        ///</summary>
        [TestMethod()]
        public void SerializeTest_True() {
            JsonSerializer target = new JsonSerializer();
            object obj = true;
            string expected = "true";
            string actual = target.Serialize(obj);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Serialize
        ///</summary>
        [TestMethod()]
        public void SerializeTest_False() {
            JsonSerializer target = new JsonSerializer();
            object obj = false;
            string expected = "false";
            string actual = target.Serialize(obj);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Serialize
        ///</summary>
        [TestMethod()]
        public void SerializeTest_Integer() {
            JsonSerializer target = new JsonSerializer();
            object obj = 123;
            string expected = "123";
            string actual = target.Serialize(obj);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Serialize
        ///</summary>
        [TestMethod()]
        public void SerializeTest_Float() {
            JsonSerializer target = new JsonSerializer();
            object obj = 1.234;
            string expected = "1.234";
            string actual = target.Serialize(obj);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Serialize
        ///</summary>
        [TestMethod()]
        public void SerializeTest_Char() {
            JsonSerializer target = new JsonSerializer();
            object obj = 'a';
            string expected = "\"a\"";
            string actual = target.Serialize(obj);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Serialize
        ///</summary>
        [TestMethod()]
        public void SerializeTest_String() {
            JsonSerializer target = new JsonSerializer();
            object obj = "abcd\r\n\t\"\'\\";
            string expected = "\"abcd\\r\\n\\t\\\"\\\'\\\\\"";
            string actual = target.Serialize(obj);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Serialize
        ///</summary>
        [TestMethod()]
        public void SerializeTest_DBNull() {
            JsonSerializer target = new JsonSerializer();
            object obj = DBNull.Value;
            string expected = "null";
            string actual = target.Serialize(obj);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Serialize
        ///</summary>
        [TestMethod()]
        public void SerializeTest_DateTime() {
            JsonSerializer target = new JsonSerializer();
            DateTime obj = DateTime.UtcNow;
            string expected = string.Format("new Date({0})", (obj - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks / 10000);
            string actual = target.Serialize(obj);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Serialize
        ///</summary>
        [TestMethod()]
        public void SerializeTest_Nullable() {
            JsonSerializer target = new JsonSerializer();
            int? obj = 13;
            string expected = "13";
            string actual = target.Serialize(obj);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Serialize
        ///</summary>
        [TestMethod()]
        public void SerializeTest_Nullable_Null() {
            JsonSerializer target = new JsonSerializer();
            int? obj = null;
            string expected = "null";
            string actual = target.Serialize(obj);
            Assert.AreEqual(expected, actual);
        }

        private struct A {
            public A(bool m0, char m1, int m2, float m3, string m4)
                : this() {
                M0 = m0;
                M1 = m1;
                M2 = m2;
                M3 = m3;
                M4 = m4;
            }

            public bool M0;
            public char M1 {
                get;
                set;
            }
            public int M2;
            public float M3 {
                get;
                set;
            }
            public string M4;
            //public string M5;
        }

        /// <summary>
        ///A test for Serialize
        ///</summary>
        [TestMethod()]
        public void SerializeTest_Structure() {
            JsonSerializer target = new JsonSerializer();
            A obj = new A(true, 'a', 123, 1.23F, "abcd");
            string actual = target.Serialize(obj);

            Assert.IsTrue(actual.StartsWith("{"));
            Assert.IsTrue(actual.EndsWith("}"));
            string[] members = actual.Substring(1, actual.Length - 2).Split(',');
            foreach(string member in members) {
                Assert.IsTrue(member == "\"M0\":true" ||
                  member == "\"M1\":\"a\"" ||
                  member == "\"M2\":123" ||
                  member == "\"M3\":1.23" ||
                  member == "\"M4\":\"abcd\"" ||
                  member == "\"M5\":null");
            }
        }

        private class B {
            public B(bool m0, char m1, int m2, float m3, string m4) {
                M0 = m0;
                M1 = m1;
                M2 = m2;
                M3 = m3;
                M4 = m4;
            }

            public bool M0;
            public char M1 {
                get;
                set;
            }
            public int M2;
            public float M3 {
                get;
                set;
            }
            public string M4;
            //public string M5;
        }

        /// <summary>
        ///A test for Serialize
        ///</summary>
        [TestMethod()]
        public void SerializeTest_Class() {
            JsonSerializer target = new JsonSerializer();
            B obj = new B(true, 'a', 123, 1.23f, "abcd");
            string actual = target.Serialize(obj);

            Assert.IsTrue(actual.StartsWith("{"));
            Assert.IsTrue(actual.EndsWith("}"));
            string[] members = actual.Substring(1, actual.Length - 2).Split(',');
            foreach(string member in members) {
                Assert.IsTrue(member == "\"M0\":true" ||
                  member == "\"M1\":\"a\"" ||
                  member == "\"M2\":123" ||
                  member == "\"M3\":1.23" ||
                  member == "\"M4\":\"abcd\"" ||
                  member == "\"M5\":null");
            }
        }

        /// <summary>
        ///A test for Serialize
        ///</summary>
        [TestMethod()]
        public void SerializeTest_Array() {
            JsonSerializer target = new JsonSerializer();
            object[] array = {true, 
                         'a',
                         123,
                         1.23,
                         "abcd",
                         new B(true, 'a', 123, 1.23f, "abcd"),
                       };
            string actual = target.Serialize(array);
        }
    }
}
