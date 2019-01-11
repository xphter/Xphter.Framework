using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Net;

namespace Xphter.Framework.Net.Tests {
    [TestClass()]
    public class IPAddressExntensionTests {
        [TestMethod()]
        public void IsPrivateIPv4AddressTest() {
            Assert.IsTrue(IPAddress.Loopback.IsPrivateIPv4Address());

            Assert.IsTrue(IPAddress.Parse("10.13.13.13").IsPrivateIPv4Address());
            Assert.IsTrue(IPAddress.Parse("172.16.13.13").IsPrivateIPv4Address());
            Assert.IsTrue(IPAddress.Parse("172.31.13.13").IsPrivateIPv4Address());
            Assert.IsTrue(IPAddress.Parse("192.168.13.13").IsPrivateIPv4Address());

            Assert.IsFalse(IPAddress.Parse("117.13.13.13").IsPrivateIPv4Address());
            Assert.IsFalse(IPAddress.Parse("172.15.13.13").IsPrivateIPv4Address());
            Assert.IsFalse(IPAddress.Parse("172.32.13.13").IsPrivateIPv4Address());
            Assert.IsFalse(IPAddress.Parse("191.168.13.13").IsPrivateIPv4Address());
        }
    }
}
