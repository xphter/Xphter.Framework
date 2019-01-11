using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;
using Xphter.Framework.Data.SqlServer;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for SqlServerSourceTest and is intended
    ///to contain all SqlServerSourceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SqlServerSourceTest {


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
        ///A test for SqlServerSource Constructor
        ///</summary>
        [TestMethod()]
        public void SqlServerSourceConstructor_SourceName() {
            SqlServerSource target = new SqlServerSource("Table");
            Assert.AreEqual<string>(string.Format("[{0}]", target.Name), target.Fullname);
            Assert.AreEqual<string>(string.Format("[{0}]", target.Name), target.SqlString);
        }

        /// <summary>
        ///A test for SqlServerSource Constructor
        ///</summary>
        [TestMethod()]
        public void SqlServerSourceConstructor_SourceAlias() {
            SqlServerSource target = new SqlServerSource("Table", "a");
            Assert.AreEqual<string>(string.Format("[{0}]", target.Alias), target.Fullname);
            Assert.AreEqual<string>(string.Format("{0} AS [{1}]", target.Object.SqlString, target.Alias), target.SqlString);

            target.TableHints.AddArgument(SqlServerTableHints.UpdateLock);
            Assert.AreEqual<string>(string.Format("[{0}]", target.Alias), target.Fullname);
            Assert.AreEqual<string>(string.Format("{0} AS [{1}] {2}", target.Object.SqlString, target.Alias, target.TableHints.SqlString), target.SqlString);

            target.TableHints.AddArgument(SqlServerTableHints.TableLock, SqlServerTableHints.XLock);
            Assert.AreEqual<string>(string.Format("[{0}]", target.Alias), target.Fullname);
            Assert.AreEqual<string>(string.Format("{0} AS [{1}] {2}", target.Object.SqlString, target.Alias, target.TableHints.SqlString), target.SqlString);
        }

        /// <summary>
        ///A test for SqlServerSource Constructor
        ///</summary>
        [TestMethod()]
        public void SqlServerSourceConstructor_SourceOwnerName() {
            SqlServerSource owner = new SqlServerSource("sys");
            SqlServerSource target = new SqlServerSource(owner, "Table");
            Assert.AreEqual<string>(string.Format("{0}.[{1}]", owner.Fullname, target.Name), target.Fullname);
            Assert.AreEqual<string>(string.Format("{0}.[{1}]", owner.Fullname, target.Name), target.SqlString);

            target.TableHints.AddArgument(SqlServerTableHints.UpdateLock);
            Assert.AreEqual<string>(string.Format("{0}.[{1}]", owner.Fullname, target.Name), target.Fullname);
            Assert.AreEqual<string>(string.Format("{0}.[{1}] {2}", owner.Fullname, target.Name, target.TableHints.SqlString), target.SqlString);

            target.TableHints.AddArgument(SqlServerTableHints.TableLock, SqlServerTableHints.XLock);
            Assert.AreEqual<string>(string.Format("{0}.[{1}]", owner.Fullname, target.Name), target.Fullname);
            Assert.AreEqual<string>(string.Format("{0}.[{1}] {2}", owner.Fullname, target.Name, target.TableHints.SqlString), target.SqlString);
        }

        /// <summary>
        ///A test for SqlServerSource Constructor
        ///</summary>
        [TestMethod()]
        public void SqlServerSourceConstructor_SourceOwnerAlias() {
            SqlServerSource owner = new SqlServerSource("sys");
            SqlServerSource target = new SqlServerSource(owner, "Table", "a");
            Assert.AreEqual<string>(string.Format("[{0}]", target.Alias), target.Fullname);
            Assert.AreEqual<string>(string.Format("{0} AS [{1}]", target.Object.SqlString, target.Alias), target.SqlString);

            target.TableHints.AddArgument(SqlServerTableHints.UpdateLock);
            Assert.AreEqual<string>(string.Format("[{0}]", target.Alias), target.Fullname);
            Assert.AreEqual<string>(string.Format("{0} AS [{1}] {2}", target.Object.SqlString, target.Alias, target.TableHints.SqlString), target.SqlString);

            target.TableHints.AddArgument(SqlServerTableHints.TableLock, SqlServerTableHints.XLock);
            Assert.AreEqual<string>(string.Format("[{0}]", target.Alias), target.Fullname);
            Assert.AreEqual<string>(string.Format("{0} AS [{1}] {2}", target.Object.SqlString, target.Alias, target.TableHints.SqlString), target.SqlString);
        }

        /// <summary>
        ///A test for SqlServerSource Constructor
        ///</summary>
        [TestMethod()]
        public void SqlServerSourceConstructorTest_Subquery() {
            SqlServerSource source = new SqlServerSource("Table");
            ISqlSelectStatement statement = new SqlServerSelectStatement();
            statement.SelectClause.AddExpressions(new SqlAllField(source));
            statement.FromClause.Source = source;
            SqlServerSource target = new SqlServerSource(statement, "Source");
            Assert.AreEqual<string>(string.Format("[{0}]", target.Name), target.Fullname);
            Assert.AreEqual<string>(string.Format("({0}) AS [{1}]", statement.SqlString, target.Name), target.SqlString);
        }
    }
}
