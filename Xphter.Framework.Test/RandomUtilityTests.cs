using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework;

namespace Xphter.Framework.Tests {
    [TestClass()]
    public class RandomUtilityTests {
        [TestMethod()]
        public void GetStringTest() {
            ICollection<string> result = new List<string>();
            for(int i = 0; i < 100; i++) {
                result.Add(RandomUtility.GetString(20));
            }
            Assert.AreEqual(100, result.Distinct().Count());
        }
    }
}

