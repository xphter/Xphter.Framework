using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.IO;

namespace Xphter.Framework.Test {
    /// <summary>
    ///This is a test class for PathUtilityTest and is intended
    ///to contain all PathUtilityTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PathUtilityTests {
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
        ///A test for IsValidPath
        ///</summary>
        [TestMethod()]
        public void IsValidPathTest_True() {
            Assert.IsTrue(PathUtility.IsValidLocalPath(@"c:"));
            Assert.IsTrue(PathUtility.IsValidLocalPath(@"c:\"));
            Assert.IsTrue(PathUtility.IsValidLocalPath(@"c:\1"));
            Assert.IsTrue(PathUtility.IsValidLocalPath(@"c:\1\"));
            Assert.IsTrue(PathUtility.IsValidLocalPath(@"c:\1\2\"));
            Assert.IsTrue(PathUtility.IsValidLocalPath(@"c:\1\2\3"));
            Assert.IsTrue(PathUtility.IsValidLocalPath(@"\"));
            Assert.IsTrue(PathUtility.IsValidLocalPath(@"\1"));
            Assert.IsTrue(PathUtility.IsValidLocalPath(@"\1\"));
            Assert.IsTrue(PathUtility.IsValidLocalPath(@"\1\2\"));
            Assert.IsTrue(PathUtility.IsValidLocalPath(@"\1\2\3"));
            Assert.IsTrue(PathUtility.IsValidLocalPath(@"1"));
            Assert.IsTrue(PathUtility.IsValidLocalPath(@"1\"));
            Assert.IsTrue(PathUtility.IsValidLocalPath(@"1\2\"));
            Assert.IsTrue(PathUtility.IsValidLocalPath(@"1\2\3"));
        }

        /// <summary>
        ///A test for IsValidPath
        ///</summary>
        [TestMethod()]
        public void IsValidPathTest_EmptyFolder() {
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"c:\\"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"c:\1\\"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"c:\1\\2"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"\\"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"\1\\"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"\1\\2"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"1\\"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"1\2\\"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"1\2\\3"));
        }

        /// <summary>
        ///A test for IsValidPath
        ///</summary>
        [TestMethod()]
        public void IsValidPathTest_InvalidChar() {
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"c:\*:\"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"c:\1\*:"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"\*:\"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"\1\*:"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"*:"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"*:\"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"1\*:\"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"1\*:"));
        }

        /// <summary>
        ///A test for IsValidPath
        ///</summary>
        [TestMethod()]
        public void IsValidPathTest_InvalidName() {
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"c:\con\"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"c:\a\com1.txt"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"c:\a\lpt2"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"\prn"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"\a\com3"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"\a\b\lpt5.html"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"a\aux"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"a\b\prn.exe"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"nul"));
            Assert.IsFalse(PathUtility.IsValidLocalPath(@"prn.html"));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void MakeRelativePathTest_Null() {
            PathUtility.MakeRelativePath(null, null);
        }

        [TestMethod()]
        public void MakeRelativePathTest_Empty() {
            Assert.AreEqual(@"d:\1\2\3", PathUtility.MakeRelativePath(string.Empty, @"d:\1\2\3"));
        }

        [TestMethod()]
        public void MakeRelativePathTest_NotSameParent() {
            Assert.AreEqual(@"d:\1\2\3", PathUtility.MakeRelativePath(@"d:\2\3\4", @"d:\1\2\3"));
        }

        [TestMethod()]
        public void MakeRelativePathTest() {
            Assert.AreEqual(@"3\4", PathUtility.MakeRelativePath(@"d:\1\2", @"d:\1\2\3\4"));
            Assert.AreEqual(string.Empty, PathUtility.MakeRelativePath(@"d:\1\2", @"d:\1\2"));
            Assert.AreEqual(string.Empty, PathUtility.MakeRelativePath(@"d:\1\2\", @"d:\1\2"));

            Assert.AreEqual(@"2\3.txt", PathUtility.MakeRelativePath(@"\\localhost\1", @"\\localhost\1\2\3.txt"));
        }
    }
}
