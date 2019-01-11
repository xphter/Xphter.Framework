using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;
using Xphter.Framework.Data.SqlServer;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for SqlServerDatabaseEntityProviderTest and is intended
    ///to contain all SqlServerDatabaseEntityProviderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SqlServerDatabaseEntityProviderTest {


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

        private string m_dataSource = new SqlConnectionStringBuilder(Properties.Settings.Default.SqlConnectionString).DataSource;

        /// <summary>
        ///A test for GetDatabases
        ///</summary>
        [TestMethod()]
        public void GetDatabasesTest() {
            IDbSourceEntity source = new DbSourceEntity(this.m_dataSource, null, new SqlServerDatabaseEntityProvider());
            IEnumerable<IDbDatabaseEntity> databases = source.Databases;

            Assert.IsTrue((from database in databases
                           where database.Name.Equals("Xphter.Framework.Test")
                           select database).Count() == 1);
        }
    }
}
