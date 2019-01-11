using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;
using Xphter.Framework.Data.Oracle;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for OracleSourceTest and is intended
    ///to contain all OracleSourceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OracleSourceTest {


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
        ///A test for OracleSource Constructor
        ///</summary>
        [TestMethod()]
        public void OracleSourceConstructorTest_SourceName() {
            OracleSource target = new OracleSource("Table");
            Assert.AreEqual<string>(string.Format("\"{0}\"", target.Name.ToUpper()), target.Fullname);
            Assert.AreEqual<string>(string.Format("\"{0}\"", target.Name.ToUpper()), target.SqlString);
        }

        /// <summary>
        ///A test for OracleSource Constructor
        ///</summary>
        [TestMethod()]
        public void OracleSourceConstructorTest_SourceOwnerName() {
            OracleSource owner = new OracleSource("INFORMATION_SCHEMA");
            OracleSource target = new OracleSource(owner, "Table");
            Assert.AreEqual<string>(string.Format("{0}.\"{1}\"", owner.Fullname, target.Name.ToUpper()), target.Fullname);
            Assert.AreEqual<string>(string.Format("{0}.\"{1}\"", owner.Fullname, target.Name.ToUpper()), target.SqlString);
        }

        /// <summary>
        ///A test for OracleSource Constructor
        ///</summary>
        [TestMethod()]
        public void OracleSourceConstructorTest_SourceAlias() {
            OracleSource target = new OracleSource("Table", "a");
            Assert.AreEqual<string>(string.Format("\"{0}\"", target.Alias.ToUpper()), target.Fullname);
            Assert.AreEqual<string>(string.Format("{0} \"{1}\"", target.Object.SqlString, target.Alias.ToUpper()), target.SqlString);
        }

        /// <summary>
        ///A test for OracleSource Constructor
        ///</summary>
        [TestMethod()]
        public void OracleSourceConstructorTest_SourceOwnerNameAlias() {
            OracleSource owner = new OracleSource("INFORMATION_SCHEMA");
            OracleSource target = new OracleSource(owner, "Table", "a");
            Assert.AreEqual<string>(string.Format("\"{0}\"", target.Alias.ToUpper()), target.Fullname);
            Assert.AreEqual<string>(string.Format("{0} \"{1}\"", target.Object.SqlString, target.Alias.ToUpper()), target.SqlString);
        }

        /// <summary>
        ///A test for OracleSource Constructor
        ///</summary>
        [TestMethod()]
        public void OracleSourceConstructorTest_Subquery() {
            OracleSource source = new OracleSource("Table");
            ISqlSelectStatement statement = new OracleSelectStatement();
            statement.SelectClause.AddExpressions(new SqlAllField(source));
            statement.FromClause.Source = source;
            OracleSource target = new OracleSource(statement, "Source");
            Assert.AreEqual<string>(string.Format("\"{0}\"", target.Name.ToUpper()), target.Fullname);
            Assert.AreEqual<string>(string.Format("({0}) \"{1}\"", statement.SqlString, target.Name.ToUpper()), target.SqlString);
        }
    }
}
