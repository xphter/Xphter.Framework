using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;
using Xphter.Framework.Data.Oracle;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for OracleFieldSqlStringProviderTest and is intended
    ///to contain all OracleFieldSqlStringProviderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OracleAliasObjectSqlStringProviderTest {


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
        ///A test for GetSqlString
        ///</summary>
        [TestMethod()]
        public void GetSqlStringTest() {
            ISqlObject obj = new SqlObject(new SqlStringExpression("ID"), "ID", new OracleObjectFullnameProvider(), new SqlObjectFullnameSqlStringProvider());
            SqlAliasObject field = new SqlAliasObject(obj, "ItemID", new SqlAliasObjectFullnameProvider(), new OracleAliasObjectSqlStringProvider());
            Assert.AreEqual<string>(string.Format("{0} AS \"{1}\"", obj.SqlString, field.Alias.ToUpper()), field.SqlString);
        }
    }
}
