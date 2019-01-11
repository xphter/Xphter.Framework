using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;
using Xphter.Framework.Data.MySql;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for MySqlSourceTest and is intended
    ///to contain all MySqlSourceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MySqlSourceTest {


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
        ///A test for MySqlSource Constructor
        ///</summary>
        [TestMethod()]
        public void MySqlSourceConstructorTest_SourceName() {
            MySqlSource target = new MySqlSource("Table");
            Assert.AreEqual<string>(string.Format("`{0}`", target.Name), target.Fullname);
            Assert.AreEqual<string>(string.Format("`{0}`", target.Name), target.SqlString);
        }

        /// <summary>
        ///A test for MySqlSource Constructor
        ///</summary>
        [TestMethod()]
        public void MySqlSourceConstructorTest_SourceOwnerName() {
            MySqlSource owner = new MySqlSource("INFORMATION_SCHEMA");
            MySqlSource target = new MySqlSource(owner, "Table");
            Assert.AreEqual<string>(string.Format("{0}.`{1}`", owner.Fullname, target.Name), target.Fullname);
            Assert.AreEqual<string>(string.Format("{0}.`{1}`", owner.Fullname, target.Name), target.SqlString);
        }

        /// <summary>
        ///A test for MySqlSource Constructor
        ///</summary>
        [TestMethod()]
        public void MySqlSourceConstructorTest_SourceAlias() {
            MySqlSource target = new MySqlSource("Table", "a");
            Assert.AreEqual<string>(string.Format("`{0}`", target.Alias), target.Fullname);
            Assert.AreEqual<string>(string.Format("{0} AS `{1}`", target.Object.SqlString, target.Alias), target.SqlString);
        }

        /// <summary>
        ///A test for MySqlSource Constructor
        ///</summary>
        [TestMethod()]
        public void MySqlSourceConstructorTest_SourceOwnerAlias() {
            MySqlSource owner = new MySqlSource("INFORMATION_SCHEMA");
            MySqlSource target = new MySqlSource(owner, "Table", "a");
            Assert.AreEqual<string>(string.Format("`{0}`", target.Alias), target.Fullname);
            Assert.AreEqual<string>(string.Format("{0} AS `{1}`", target.Object.SqlString, target.Alias), target.SqlString);
        }

        /// <summary>
        ///A test for MySqlSource Constructor
        ///</summary>
        [TestMethod()]
        public void MySqlSourceConstructorTest_Subquery() {
            MySqlSource source = new MySqlSource("Table");
            ISqlSelectStatement statement = new MySqlSelectStatement();
            statement.SelectClause.AddExpressions(new SqlAllField(source));
            statement.FromClause.Source = source;
            MySqlSource target = new MySqlSource(statement, "Source");
            Assert.AreEqual<string>(string.Format("`{0}`", target.Name), target.Fullname);
            Assert.AreEqual<string>(string.Format("({0}) AS `{1}`", statement.SqlString, target.Name), target.SqlString);
        }
    }
}
