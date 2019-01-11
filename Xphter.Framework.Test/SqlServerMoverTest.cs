using System;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data.SqlServer;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for SqlServerMoverTest and is intended
    ///to contain all SqlServerMoverTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SqlServerMoverTest {


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

        private readonly string m_connectionString = Properties.Settings.Default.SqlConnectionString;

        private readonly string m_name = "Xphter.Framework.Test";

        private readonly string m_file = "Xphter.Framework.Test.bak";

        /// <summary>
        ///A test for Backup and Restore.
        ///</summary>
        [TestMethod()]
        public void SyncTest() {
            SqlServerMover target = new SqlServerMover(this.m_connectionString);
            target.Backup(this.m_name, Path.GetFullPath(this.m_file));
            //target.Restore(this.m_name, Path.GetFullPath(this.m_file));
        }

        /// <summary>
        ///A test for Backup and Restore.
        ///</summary>
        [TestMethod()]
        public void AsyncTest() {
            int counter = 0;
            SqlServerMover target = new SqlServerMover(this.m_connectionString);
            target.BackupCompleted += (sender, e) => {
                counter++;
                target.RestoreAsync(this.m_name, Path.GetFullPath(this.m_file), null);
            };
            target.RestoreCompleted += (sender, e) => {
                counter++;
            };
            target.BackupAsync(this.m_name, Path.GetFullPath(this.m_file), null);
            while(counter < 2) {
                Thread.Sleep(0);
            }
        }

        /// <summary>
        ///A test for Backup and Restore.
        ///</summary>
        [TestMethod()]
        public void BeginEndTest() {
            int counter = 0;
            SqlServerMover target = new SqlServerMover(this.m_connectionString);
            target.BeginBackup(this.m_name, Path.GetFullPath(this.m_file), (ar) => {
                counter++;
                target.EndBackup(ar);

                target.BeginRestore(this.m_name, Path.GetFullPath(this.m_file), (ar2) => {
                    counter++;
                    target.EndRestore(ar2);
                }, null);
            }, null);
            while(counter < 2) {
                Thread.Sleep(0);
            }
        }
    }
}
