using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Net;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for FtpFileInfoTest and is intended
    ///to contain all FtpFileInfoTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FtpFileInfoTest {


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

        private FtpDirectoryInfo m_parent = FtpDirectoryInfo.CreateRoot(new Uri("ftp://localhost"), null);

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTest_MSDOS_Directory() {
            string detail = "01-02-03 08:40AM <DIR> abcd";
            FtpFileInfo file = FtpFileInfo.Create(this.m_parent, detail);
            Assert.AreEqual<DateTime>(new DateTime(2003, 1, 2, 8, 40, 0), file.LastModifyTime);
            Assert.AreEqual<bool>(true, file.IsDirectory);
            Assert.AreEqual<string>("abcd", file.Name);

            detail = "01-02-03 08:40PM    <DIR>   新建 文件夹";
            file = FtpFileInfo.Create(this.m_parent, detail);
            Assert.AreEqual<DateTime>(new DateTime(2003, 1, 2, 20, 40, 0), file.LastModifyTime);
            Assert.AreEqual<bool>(true, file.IsDirectory);
            Assert.AreEqual<string>("新建 文件夹", file.Name);
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTest_MSDOS_File() {
            string detail = "01-02-03 08:40AM 123 abcd";
            FtpFileInfo file = FtpFileInfo.Create(this.m_parent, detail);
            Assert.AreEqual<DateTime>(new DateTime(2003, 1, 2, 8, 40, 0), file.LastModifyTime);
            Assert.AreEqual<bool>(false, file.IsDirectory);
            Assert.AreEqual<long>(123, file.Length);
            Assert.AreEqual<string>("abcd", file.Name);

            detail = "01-02-03 08:40PM    456   新建 文件.txt";
            file = FtpFileInfo.Create(this.m_parent, detail);
            Assert.AreEqual<DateTime>(new DateTime(2003, 1, 2, 20, 40, 0), file.LastModifyTime);
            Assert.AreEqual<bool>(false, file.IsDirectory);
            Assert.AreEqual<long>(456, file.Length);
            Assert.AreEqual<string>("新建 文件.txt", file.Name);
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTest_UNIX_Directory() {
            string detail = "d-x 1 user group 0 Jan 01 08:40 abcd";
            FtpFileInfo file = FtpFileInfo.Create(this.m_parent, detail);
            Assert.AreEqual<DateTime>(new DateTime(DateTime.Today.Year, 1, 1, 8, 40, 0), file.LastModifyTime);
            Assert.AreEqual<bool>(true, file.IsDirectory);
            Assert.AreEqual<string>("abcd", file.Name);

            detail = "d-x 1 user group 0 Dec 31 23:59 abcd";
            file = FtpFileInfo.Create(this.m_parent, detail);
            Assert.AreEqual<DateTime>(new DateTime(DateTime.Today.Year - 1, 12, 31, 23, 59, 0), file.LastModifyTime);
            Assert.AreEqual<bool>(true, file.IsDirectory);
            Assert.AreEqual<string>("abcd", file.Name);

            detail = "d-x 1 user group 0    Jan 01 2001    新建 文件夹";
            file = FtpFileInfo.Create(this.m_parent, detail);
            Assert.AreEqual<DateTime>(new DateTime(2001, 1, 1), file.LastModifyTime);
            Assert.AreEqual<bool>(true, file.IsDirectory);
            Assert.AreEqual<string>("新建 文件夹", file.Name);
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTest_UNIX_File() {
            string detail = "-x 1 user group 123 Jan 01 08:40 abcd";
            FtpFileInfo file = FtpFileInfo.Create(this.m_parent, detail);
            Assert.AreEqual<DateTime>(new DateTime(DateTime.Today.Year, 1, 1, 8, 40, 0), file.LastModifyTime);
            Assert.AreEqual<bool>(false, file.IsDirectory);
            Assert.AreEqual<long>(123, file.Length);
            Assert.AreEqual<string>("abcd", file.Name);

            detail = "-x 1 user group 456 Dec 31 23:59 abcd";
            file = FtpFileInfo.Create(this.m_parent, detail);
            Assert.AreEqual<DateTime>(new DateTime(DateTime.Today.Year - 1, 12, 31, 23, 59, 0), file.LastModifyTime);
            Assert.AreEqual<bool>(false, file.IsDirectory);
            Assert.AreEqual<long>(456, file.Length);
            Assert.AreEqual<string>("abcd", file.Name);

            detail = "-x 1 user group 789 Jan    01 2001   新建 文件.txt";
            file = FtpFileInfo.Create(this.m_parent, detail);
            Assert.AreEqual<DateTime>(new DateTime(2001, 1, 1), file.LastModifyTime);
            Assert.AreEqual<bool>(false, file.IsDirectory);
            Assert.AreEqual<long>(789, file.Length);
            Assert.AreEqual<string>("新建 文件.txt", file.Name);
        }
    }
}
