using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Net;
namespace Xphter.Framework.Test {
    [TestClass()]
    public class IpHelperTests {
        [TestMethod()]
        public void GetLocationTest() {
            IEnumerable<string> location = IpHelper.GetLocation(null);
            Assert.AreEqual(3, location.Count());
        }

        [TestMethod()]
        public void BeginGetLocationTest() {
            IEnumerable<string> location = null;
            ManualResetEvent evt = new ManualResetEvent(false);
            IpHelper.BeginGetLocation(null, (ar) => {
                try {
                    location = IpHelper.EndGetLocation(ar);
                } finally {
                    evt.Set();
                }
            }, null);
            evt.WaitOne();

            Assert.IsNotNull(location);
            Assert.AreEqual(3, location.Count());
        }
    }
}
