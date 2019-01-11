using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework;
using Xphter.Framework.Collections;

namespace Xphter.Framework.Tests {
    [TestClass()]
    public class StringUtilityTests {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void BetweenTest_StartIndexLess() {
            string.Empty.Between(-1, "a", "b");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void BetweenTest_StartIndexGreater() {
            "abc".Between(3, "a", "b");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void BetweenTest_EmptyStartValue() {
            string.Empty.Between(null, "b");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void BetweenTest_EmptyEndValue() {
            string.Empty.Between("a", string.Empty);
        }

        [TestMethod()]
        public void BetweenTest_Null() {
            Assert.AreEqual(0, StringUtility.Between(null, "a", "b").Count());
        }

        [TestMethod()]
        public void BetweenTest_Empty() {
            Assert.AreEqual(0, string.Empty.Between("a", "b").Count());
        }

        [TestMethod()]
        public void BetweenTest() {
            "1a2b3a4b".Between("a", "b").Equals(new string[] {
                "2", "4",
            });
            "1a2b3a4b5a".Between("a", "b").Equals(new string[] {
                "2", "4",
            });
            "1a2b3a4b5a6".Between("a", "b").Equals(new string[] {
                "2", "4",
            });
        }
    }
}
