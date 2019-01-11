using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;
using Xphter.Framework.Data.SqlServer;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for SqlServerDataEntityProviderTest and is intended
    ///to contain all SqlServerDataEntityProviderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SqlServerDataEntityProviderTest {


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
        ///A test for GetTables
        ///</summary>
        [TestMethod()]
        public void GetTablesTest() {
            IDbSourceEntity source = new DbSourceEntity(this.m_dataSource, null, new SqlServerDatabaseEntityProvider());
            IDbDatabaseEntity database = new DbDatabaseEntity(source, "Xphter.Framework.Test", new SqlServerDataEntityProvider());

            Assert.IsTrue((from table in database.Tables
                           where table.Name.Equals("tableSqlHelperTest")
                           select table).Count() == 1);
        }

        /// <summary>
        ///A test for GetViews
        ///</summary>
        [TestMethod()]
        public void GetViewsTest() {
            IDbSourceEntity source = new DbSourceEntity(this.m_dataSource, null, new SqlServerDatabaseEntityProvider());
            IDbDatabaseEntity database = new DbDatabaseEntity(source, "Xphter.Framework.Test", new SqlServerDataEntityProvider());

            Assert.IsTrue((from table in database.Views
                           where table.Name.Equals("viewSqlHelperTest")
                           select table).Count() == 1);
        }
    }
}
