using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Alibaba;
using Xphter.Framework.Alibaba.ChinaAlibaba;

namespace Xphter.Framework.Alibaba.Tests {
    [TestClass()]
    public class AlibabaHelperTests {
        [TestMethod()]
        public void GetParamSignatureTest() {
            string actual = AlibabaHelper.GetParamSignature(new Dictionary<string, string> {
                { "client_id", "2988940" },
                { "site", "china" },
                { "redirect_uri", "http://www.xphter.com" },
                { "state", "xphter" },
            }, "3tzdn2c9CU");
            Assert.AreEqual<string>("A6EA9CEBF3C5F15A48E9D5C36E646789993F9677", actual);
        }

        [TestMethod()]
        public void GetParamSignatureTest_Object() {
            string actual = AlibabaHelper.GetParamSignature(new {
                client_id = "2988940",
                site = "china",
                redirect_uri = "http://www.xphter.com",
                state = "xphter",
            }, "3tzdn2c9CU");
            Assert.AreEqual<string>("A6EA9CEBF3C5F15A48E9D5C36E646789993F9677", actual);
        }

        [TestMethod()]
        public void GetParamSignatureTest_Null() {
            string actual = AlibabaHelper.GetParamSignature(new Dictionary<string, string> {
                { "client_id", "2988940" },
                { "site", "china" },
                { "redirect_uri", "http://www.xphter.com" },
                { "state", null },
            }, "3tzdn2c9CU");
            Assert.AreEqual<string>("9732B9F140CB44FB8AA3D8010BE547D788E7A297", actual);
        }

        [TestMethod()]
        public void GetApiSignatureTest() {
            string actual = AlibabaHelper.GetApiSignature("param2", "1", "system", "currentTime", "2988940", new Dictionary<string, string> {
                { "client_id", "2988940" },
                { "site", "china" },
                { "redirect_uri", "http://www.xphter.com" },
                { "state", "xphter" },
            }, "3tzdn2c9CU");
            Assert.AreEqual<string>("A70625750557474FFD0F65973D3CA23D237CF472", actual);
        }

        [TestMethod()]
        public void GetApiSignatureTest_Object() {
            string actual = AlibabaHelper.GetApiSignature("param2", "1", "system", "currentTime", "2988940", new {
                client_id = "2988940",
                site = "china",
                redirect_uri = "http://www.xphter.com",
                state = "xphter",
            }, "3tzdn2c9CU");
            Assert.AreEqual<string>("A70625750557474FFD0F65973D3CA23D237CF472", actual);
        }

        [TestMethod()]
        public void GetApiSignatureTest_Absolute() {
            string actual = AlibabaHelper.GetApiSignature("http://gw.open.1688.com/openapi/param2/1/system/currentTime/2988940", new {
                client_id = "2988940",
                site = "china",
                redirect_uri = "http://www.xphter.com",
                state = "xphter",
            }, "3tzdn2c9CU");
            Assert.AreEqual<string>("A70625750557474FFD0F65973D3CA23D237CF472", actual);
        }

        [TestMethod()]
        public void GetApiSignatureTest_Null() {
            string actual = AlibabaHelper.GetApiSignature("param2", "1", "system", "currentTime", "2988940", new Dictionary<string, string> {
                { "client_id", "2988940" },
                { "site", "china" },
                { "redirect_uri", "http://www.xphter.com" },
                { "state", string.Empty },
            }, "3tzdn2c9CU");
            Assert.AreEqual<string>("AA4B09182881EBC69D9CC718AB992696A658B153", actual);
        }

        [TestMethod()]
        public void NetTimeToAlibabaTimeTest() {
            DateTime time = new DateTime(2012, 8, 1, 15, 42, 20, 368, DateTimeKind.Local);
            string actual = AlibabaHelper.NetTimeToAlibabaTime(time);
            Assert.AreEqual<string>("20120801154220368+0800", actual);
        }

        [TestMethod()]
        public void AlibabaTimeToNetTimeTest() {
            DateTime actual = AlibabaHelper.AlibabaTimeToLocalTime("20120801154220368+0800");

            Assert.IsTrue(actual.Year == 2012);
            Assert.IsTrue(actual.Month == 8);
            Assert.IsTrue(actual.Day == 1);
            Assert.IsTrue(actual.Hour == 15);
            Assert.IsTrue(actual.Minute == 42);
            Assert.IsTrue(actual.Second == 20);
            Assert.IsTrue(actual.Millisecond == 368);
            Assert.IsTrue(actual.Kind == DateTimeKind.Local);
        }

        [TestMethod()]
        public void GetArgumentsDictionaryTest() {
            DateTime now = DateTime.Now;
            object args = new {
                a = 13,
                b = true,
                c = "xphter",
                d = now,
                e = new int[] {
                    123, 456,
                },
                f = new string[] {
                    "123", "456",
                },
            };
            IDictionary<string, string> actual = AlibabaHelper.GetArgumentsDictionary(args);
            Assert.AreEqual(actual["a"], "13");
            Assert.AreEqual(actual["b"], "true");
            Assert.AreEqual(actual["c"], "xphter");
            Assert.AreEqual(actual["d"], AlibabaHelper.NetTimeToAlibabaTime(now));
            Assert.AreEqual(actual["e"], "[123,456]");
            Assert.AreEqual(actual["f"], "[\"123\",\"456\"]");
        }
    }
}
