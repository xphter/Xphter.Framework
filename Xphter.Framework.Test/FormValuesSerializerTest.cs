using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Collections;
using Xphter.Framework.Net;
using Xphter.Framework.Reflection;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for FormValuesSerializerTest and is intended
    ///to contain all FormValuesSerializerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FormValuesSerializerTest {


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

        private enum TestEnum {
            Value0,
            Value1,
            Value2,
            Value3,
        }

        private class TestClass1 {
            public int ID1;
            public string Name1;
            public DateTime CreateTime1;
            public ICollection<int> Roles;
            public TestClass2 Object2;
            public TestClass2[] Object2Data;
        }

        private class TestClass2 {
            public TestEnum Type2;
            public string Name2;
        }

        /// <summary>
        ///A test for Serialize
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void SerializeTest_Empty() {
            FormValuesSerializer target = new FormValuesSerializer();
            Assert.AreEqual<string>(string.Empty, target.Serialize(true));
            Assert.AreEqual<string>(string.Empty, target.Serialize(13));
            Assert.AreEqual<string>(string.Empty, target.Serialize(13D));
            Assert.AreEqual<string>(string.Empty, target.Serialize("123"));
            Assert.AreEqual<string>(string.Empty, target.Serialize(DateTime.Now));
            Assert.AreEqual<string>(string.Empty, target.Serialize(TestEnum.Value1));
            Assert.AreEqual<string>(string.Empty, target.Serialize(DBNull.Value));
            Assert.AreEqual<string>(string.Empty, target.Serialize((int?) 13));
            Assert.AreEqual<string>(string.Empty, target.Serialize(null));
        }

        /// <summary>
        ///A test for Serialize
        ///</summary>
        [TestMethod()]
        public void SerializeTest_Single() {
            string name = "name";
            DateTime now = DateTime.Now;
            FormValuesSerializer target = new FormValuesSerializer();
            Assert.AreEqual<string>(string.Format("{0}={1}", name, true), target.Serialize(true, name));
            Assert.AreEqual<string>(string.Format("{0}={1}", name, 13), target.Serialize(13, name));
            Assert.AreEqual<string>(string.Format("{0}={1}", name, 13D), target.Serialize(13D, name));
            Assert.AreEqual<string>(string.Format("{0}={1}", name, "123"), target.Serialize("123", name));
            Assert.AreEqual<string>(string.Format("{0}={1}", name, HttpUtility.UrlEncode(now.ToString("yyyy-MM-dd HH:mm:ss"))), target.Serialize(now, name));
            Assert.AreEqual<string>(string.Format("{0}={1}", name, HttpUtility.UrlEncode(TestEnum.Value1.ToString())), target.Serialize(TestEnum.Value1, name));
            Assert.AreEqual<string>(string.Format("{0}={1}", name, 13), target.Serialize((int?) 13, name));
            Assert.AreEqual<string>(new int[]{
                13, 14
            }.StringJoin('&', (item, index) => string.Format("{0}={1}", HttpUtility.UrlEncode(string.Format("{0}[{1}]", name, index)), item)), target.Serialize(new int[]{
                13, 14
            }, name));
            Assert.AreEqual<string>(new List<int>{
                13, 14
            }.StringJoin('&', (item, index) => string.Format("{0}={1}", HttpUtility.UrlEncode(string.Format("{0}[{1}]", name, index)), item)), target.Serialize(new List<int>{
                13, 14
            }, name));
            Assert.AreEqual<string>(new ArrayList{
                13, 14
            }.Cast<int>().StringJoin('&', (item, index) => string.Format("{0}={1}", HttpUtility.UrlEncode(string.Format("{0}[{1}]", name, index)), item)), target.Serialize(new ArrayList{
                13, 14
            }, name));
        }

        /// <summary>
        ///A test for Serialize
        ///</summary>
        [TestMethod()]
        public void SerializeTest_Object() {
            FormValuesSerializer target = new FormValuesSerializer();
            TestClass1 obj = new TestClass1 {
                ID1 = 13,
                Name1 = "DuPeng",
                CreateTime1 = DateTime.Now,
                Roles = new List<int> {
                    14, 15,
                },
                Object2 = new TestClass2 {
                    Type2 = TestEnum.Value0,
                    Name2 = "Test0",
                },
                Object2Data = new TestClass2[] {
                    new TestClass2 {
                        Type2 = TestEnum.Value1,
                        Name2 = "Test1",
                    },
                    new TestClass2 {
                        Type2 = TestEnum.Value2,
                        Name2 = "Test2",
                    },
                },
            };
            string expected = new string[]{
                string.Format("{0}={1}", TypeUtility.GetMemberName<TestClass1, int>((item) => item.ID1), obj.ID1),
                string.Format("{0}={1}", TypeUtility.GetMemberName<TestClass1, string>((item) => item.Name1), obj.Name1),
                string.Format("{0}={1}", TypeUtility.GetMemberName<TestClass1, DateTime>((item) => item.CreateTime1), HttpUtility.UrlEncode(obj.CreateTime1.ToString("yyyy-MM-dd HH:mm:ss"))),
                obj.Roles.StringJoin('&', (item, index) => string.Format("{0}={1}", HttpUtility.UrlEncode(string.Format("{0}[{1}]", TypeUtility.GetMemberName<TestClass1, ICollection<int>>((t) => t.Roles), index)), item)),
                string.Format("{0}.{1}={2}", TypeUtility.GetMemberName<TestClass1, TestClass2>((item) => item.Object2), TypeUtility.GetMemberName<TestClass2, TestEnum>((item) => item.Type2), obj.Object2.Type2),
                string.Format("{0}.{1}={2}", TypeUtility.GetMemberName<TestClass1, TestClass2>((item) => item.Object2), TypeUtility.GetMemberName<TestClass2, string>((item) => item.Name2), obj.Object2.Name2),
                obj.Object2Data.StringJoin('&', (item, index) => string.Format("{0}{1}{2}{3}.{4}={5}&{0}{1}{2}{3}.{6}={7}", TypeUtility.GetMemberName<TestClass1, TestClass2[]>((t) => t.Object2Data), HttpUtility.UrlEncode("["), index, HttpUtility.UrlEncode("]"), TypeUtility.GetMemberName<TestClass2, TestEnum>((t) => t.Type2), item.Type2, TypeUtility.GetMemberName<TestClass2, string>((t) => t.Name2), item.Name2)),
            }.StringJoin('&');
            Assert.AreEqual<string>(expected, target.Serialize(obj));
        }

        /// <summary>
        ///A test for Serialize
        ///</summary>
        [TestMethod()]
        public void SerializeTest_EncodedStream() {
            FormValuesSerializer target = new FormValuesSerializer();
            TestClass1 obj = new TestClass1 {
                ID1 = 13,
                Name1 = "DuPeng",
                CreateTime1 = DateTime.Now,
                Roles = new List<int> {
                    14, 15,
                },
                Object2 = new TestClass2 {
                    Type2 = TestEnum.Value0,
                    Name2 = "Test0",
                },
                Object2Data = new TestClass2[] {
                    new TestClass2 {
                        Type2 = TestEnum.Value1,
                        Name2 = "Test1",
                    },
                    new TestClass2 {
                        Type2 = TestEnum.Value2,
                        Name2 = "Test2",
                    },
                },
            };
            string expected = new string[]{
                string.Format("{0}={1}", TypeUtility.GetMemberName<TestClass1, int>((item) => item.ID1), obj.ID1),
                string.Format("{0}={1}", TypeUtility.GetMemberName<TestClass1, string>((item) => item.Name1), obj.Name1),
                string.Format("{0}={1}", TypeUtility.GetMemberName<TestClass1, DateTime>((item) => item.CreateTime1), HttpUtility.UrlEncode(obj.CreateTime1.ToString("yyyy-MM-dd HH:mm:ss"))),
                obj.Roles.StringJoin('&', (item, index) => string.Format("{0}={1}", HttpUtility.UrlEncode(string.Format("{0}[{1}]", TypeUtility.GetMemberName<TestClass1, ICollection<int>>((t) => t.Roles), index)), item)),
                string.Format("{0}.{1}={2}", TypeUtility.GetMemberName<TestClass1, TestClass2>((item) => item.Object2), TypeUtility.GetMemberName<TestClass2, TestEnum>((item) => item.Type2), obj.Object2.Type2),
                string.Format("{0}.{1}={2}", TypeUtility.GetMemberName<TestClass1, TestClass2>((item) => item.Object2), TypeUtility.GetMemberName<TestClass2, string>((item) => item.Name2), obj.Object2.Name2),
                obj.Object2Data.StringJoin('&', (item, index) => string.Format("{0}{1}{2}{3}.{4}={5}&{0}{1}{2}{3}.{6}={7}", TypeUtility.GetMemberName<TestClass1, TestClass2[]>((t) => t.Object2Data), HttpUtility.UrlEncode("["), index, HttpUtility.UrlEncode("]"), TypeUtility.GetMemberName<TestClass2, TestEnum>((t) => t.Type2), item.Type2, TypeUtility.GetMemberName<TestClass2, string>((t) => t.Name2), item.Name2)),
            }.StringJoin('&');

            using(MemoryStream writer = new MemoryStream()) {
                target.Serialize(obj, writer);
                Assert.AreEqual<string>(expected, Encoding.ASCII.GetString(writer.ToArray()));
            }
        }

        /// <summary>
        ///A test for Serialize
        ///</summary>
        [TestMethod()]
        public void SerializeTest_UncodedStream() {
            FormValuesSerializer target = new FormValuesSerializer();
            string boundary = "Test_Boundary";
            TestClass1 obj = new TestClass1 {
                ID1 = 13,
                Name1 = "DuPeng",
                CreateTime1 = DateTime.Now,
                Roles = new List<int> {
                    14, 15,
                },
                Object2 = new TestClass2 {
                    Type2 = TestEnum.Value0,
                    Name2 = "Test0",
                },
                Object2Data = new TestClass2[] {
                    new TestClass2 {
                        Type2 = TestEnum.Value1,
                        Name2 = "Test1",
                    },
                    new TestClass2 {
                        Type2 = TestEnum.Value2,
                        Name2 = "Test2",
                    },
                },
            };
            string expected = string.Format("--{0}\r\nContent-Disposition: form-data; ", boundary) + new string[]{
                string.Format("name=\"{0}\"\r\n\r\n{1}", TypeUtility.GetMemberName<TestClass1, int>((item) => item.ID1), obj.ID1),
                string.Format("name=\"{0}\"\r\n\r\n{1}", TypeUtility.GetMemberName<TestClass1, string>((item) => item.Name1), obj.Name1),
                string.Format("name=\"{0}\"\r\n\r\n{1}", TypeUtility.GetMemberName<TestClass1, DateTime>((item) => item.CreateTime1), obj.CreateTime1.ToString("yyyy-MM-dd HH:mm:ss")),
                obj.Roles.StringJoin(string.Format("\r\n--{0}\r\nContent-Disposition: form-data; ", boundary), (item, index) => string.Format("name=\"{0}[{1}]\"\r\n\r\n{2}", TypeUtility.GetMemberName<TestClass1, ICollection<int>>((t) => t.Roles), index, item)),
                string.Format("name=\"{0}.{1}\"\r\n\r\n{2}", TypeUtility.GetMemberName<TestClass1, TestClass2>((item) => item.Object2), TypeUtility.GetMemberName<TestClass2, TestEnum>((item) => item.Type2), obj.Object2.Type2),
                string.Format("name=\"{0}.{1}\"\r\n\r\n{2}", TypeUtility.GetMemberName<TestClass1, TestClass2>((item) => item.Object2), TypeUtility.GetMemberName<TestClass2, string>((item) => item.Name2), obj.Object2.Name2),
                obj.Object2Data.StringJoin(string.Format("\r\n--{0}\r\nContent-Disposition: form-data; ", boundary), (item, index) => string.Format("name=\"{0}[{1}].{2}\"\r\n\r\n{3}{4}name=\"{0}[{1}].{5}\"\r\n\r\n{6}", TypeUtility.GetMemberName<TestClass1, TestClass2[]>((t) => t.Object2Data), index, TypeUtility.GetMemberName<TestClass2, TestEnum>((t) => t.Type2), item.Type2, string.Format("\r\n--{0}\r\nContent-Disposition: form-data; ", boundary), TypeUtility.GetMemberName<TestClass2, string>((t) => t.Name2), item.Name2)),
            }.StringJoin(string.Format("\r\n--{0}\r\nContent-Disposition: form-data; ", boundary)) + string.Format("\r\n--{0}--", boundary);

            using(MemoryStream writer = new MemoryStream()) {
                target.Serialize(obj, null, boundary, writer);
                Assert.AreEqual<string>(expected, Encoding.UTF8.GetString(writer.ToArray()));
            }
        }
    }
}
