using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.IO;

namespace Xphter.Framework.Test {
    [TestClass()]
    public class DirectoryUtilityTests {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetNameTest_Null() {
            DirectoryUtility.GetName(null);
        }

        [TestMethod()]
        public void GetNameTest_Empty() {
            Assert.IsTrue(DirectoryUtility.GetName(string.Empty).Length == 0);
        }

        [TestMethod()]
        public void GetNameTest_One() {
            Assert.IsTrue(DirectoryUtility.GetName(@"\").Length == 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void GetNameTest_Error() {
            DirectoryUtility.GetName(@"\\");
        }

        [TestMethod()]
        public void GetNameTest_Relative() {
            Assert.AreEqual("name", DirectoryUtility.GetName(@"ceshi\a\b\name\"));
        }

        [TestMethod()]
        public void GetNameTest_Absolute() {
            Assert.AreEqual("name", DirectoryUtility.GetName(@"c:\ceshi\a\b\name"));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NormalizeTest_Null() {
            DirectoryUtility.Normalize(null);
        }

        [TestMethod()]
        public void NormalizeTest_Empty() {
            Assert.IsTrue(DirectoryUtility.Normalize(string.Empty).Length == 0);
        }

        [TestMethod()]
        public void NormalizeTest_Same() {
            Assert.AreEqual(@"d:\abc\23sji\", DirectoryUtility.Normalize(@"d:\abc\23sji\"));
        }

        [TestMethod()]
        public void NormalizeTest() {
            Assert.AreEqual(@"d:\abc\23sji\", DirectoryUtility.Normalize(@"d:\abc\23sji"));
        }
    }
}
