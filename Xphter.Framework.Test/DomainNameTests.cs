using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework;

namespace Xphter.Framework.Tests {
    [TestClass()]
    public class DomainNameTests {
        [TestMethod()]
        public void IsWellFormedDomainNameStringTest() {
            Assert.IsTrue(DomainName.IsWellFormedDomainNameString(" \t  http://www.163.com/   \t\r  \n  "));
            Assert.IsTrue(DomainName.IsWellFormedDomainNameString(" \t  qq.com   \t\r  \n  "));
            Assert.IsTrue(DomainName.IsWellFormedDomainNameString("china.com.cn"));
            Assert.IsTrue(DomainName.IsWellFormedDomainNameString("网易。中国"));

            Assert.IsFalse(DomainName.IsWellFormedDomainNameString("http://192.168.1.1/index.html"));
            Assert.IsFalse(DomainName.IsWellFormedDomainNameString("com"));
            Assert.IsFalse(DomainName.IsWellFormedDomainNameString(".com"));
            Assert.IsFalse(DomainName.IsWellFormedDomainNameString("。中国"));
        }

        [TestMethod()]
        public void LevelTest() {
            Assert.AreEqual(1, new DomainName("a.com").Level);
            Assert.AreEqual(1, new DomainName("a.中国").Level);
            Assert.AreEqual(1, new DomainName("www.com").Level);
            Assert.AreEqual(1, new DomainName("com.cn").Level);
            Assert.AreEqual(1, new DomainName("ha.cn").Level);

            Assert.AreEqual(2, new DomainName("www.a.com").Level);
            Assert.AreEqual(2, new DomainName("www.edu.cn").Level);
            Assert.AreEqual(2, new DomainName("a.edu.com").Level);
            Assert.AreEqual(1, new DomainName("a.com.cn").Level);
            Assert.AreEqual(1, new DomainName("a.ha.cn").Level);

            Assert.AreEqual(2, new DomainName("a.b.org.cn").Level);
            Assert.AreEqual(2, new DomainName("a.b.ha.cn").Level);
            Assert.AreEqual(3, new DomainName("a.b.c.com").Level);
            Assert.AreEqual(2, new DomainName("a.b.co.uk").Level);
        }

        [TestMethod()]
        public void GetRightPartTest() {
            Assert.AreEqual("www.163.com", new DomainName(" \t  http://www.163.com/   \t\r  \n  ").GetRightPart(2));
            Assert.AreEqual("qq.com", new DomainName(" \t  http://qq.com/   \t\r  \n  ").GetRightPart(1));
            Assert.AreEqual("www.google.com.hk", new DomainName(" \t  www.google.com.hk   \t\r  \n  ").GetRightPart(2));
            Assert.AreEqual("google.com.hk", new DomainName(" \t  www.google.com.hk   \t\r  \n  ").GetRightPart(1));
        }
    }
}
