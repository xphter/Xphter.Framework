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
    ///This is a test class for SqlServerViewFieldProviderTest and is intended
    ///to contain all SqlServerViewFieldProviderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SqlServerViewFieldProviderTest {


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
        ///A test for GetFields
        ///</summary>
        [TestMethod()]
        public void GetFieldsTest() {
            IDbSourceEntity source = new DbSourceEntity(this.m_dataSource, null, new SqlServerDatabaseEntityProvider());
            IEnumerable<IDbDatabaseEntity> databases = source.Databases;

            IEnumerable<IDbViewFieldEntity> fields = from database in databases
                                                     where database.Name.Equals("Xphter.Framework.Test")
                                                     from view in database.Views
                                                     where view.Name.Equals("view0", StringComparison.OrdinalIgnoreCase)
                                                     from field in view.ViewFields
                                                     select field;

            IDbViewFieldEntity idField = (from field in fields
                                          where field.Name.Equals("ID", StringComparison.OrdinalIgnoreCase)
                                          select field).FirstOrDefault();
            Assert.IsTrue(idField != null);
            Assert.AreEqual<string>("int", idField.DatabaseType);
            Assert.AreEqual<Type>(typeof(int), idField.Type);

            IDbViewFieldEntity descriptionField = (from field in fields
                                                   where field.Name.Equals("Description", StringComparison.OrdinalIgnoreCase)
                                                   select field).FirstOrDefault();
            Assert.IsTrue(descriptionField != null);
            Assert.AreEqual<int>(50, descriptionField.MaxLength);
        }
    }
}
