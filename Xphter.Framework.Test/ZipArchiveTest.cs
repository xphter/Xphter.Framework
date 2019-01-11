using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.IO;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for ZipArchiveTest and is intended
    ///to contain all ZipArchiveTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ZipArchiveTest {


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
        ///A test for OpenOnFile
        ///</summary>
        [TestMethod()]
        public void CreateFromFileTest() {
            string file = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, "Resources\\ZipArchiveTest.zip");
            ZipArchive archive = ZipArchive.CreateFromFile(file, FileMode.Open, FileAccess.Read, FileShare.Read, false);
            IEnumerable<ZipDirectoryInfo> directories = archive.GetDirectories();
            IEnumerable<ZipFileInfo> files = archive.GetFiles();

            Assert.AreEqual<int>(2, directories.Count());
            Assert.AreEqual<int>(2, files.Count());
        }

        /// <summary>
        ///A test for Extract
        ///</summary>
        [TestMethod()]
        public void ExtractTest_Null() {
            string file = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, "Resources\\ZipArchiveTest.zip");
            ZipArchive archive = ZipArchive.CreateFromFile(file, FileMode.Open, FileAccess.Read, FileShare.Read, false);
            archive.Extract(null);
        }

        /// <summary>
        ///A test for Extract
        ///</summary>
        [TestMethod()]
        public void ExtractTest_Relative() {
            string file = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, "Resources\\ZipArchiveTest.zip");
            ZipArchive archive = ZipArchive.CreateFromFile(file, FileMode.Open, FileAccess.Read, FileShare.Read, false);
            archive.Extract("ZipArchiveTest.zip");
        }

        /// <summary>
        ///A test for Extract
        ///</summary>
        [TestMethod()]
        public void ExtractTest_Absolute() {
            string file = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, "Resources\\ZipArchiveTest.zip");
            ZipArchive archive = ZipArchive.CreateFromFile(file, FileMode.Open, FileAccess.Read, FileShare.Read, false);
            archive.Extract(Path.Combine(Environment.CurrentDirectory, "test"));
        }
    }
}
