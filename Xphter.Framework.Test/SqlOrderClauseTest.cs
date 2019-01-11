using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;
using Xphter.Framework.Data.SqlServer;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for SqlOrderClauseTest and is intended
    ///to contain all SqlOrderClauseTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SqlOrderClauseTest {


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
        ///A test for SqlString
        ///</summary>
        [TestMethod()]
        public void SqlStringTest_Empty() {
            Assert.AreEqual<string>(string.Empty, new SqlOrderClause().SqlString);
        }


        /// <summary>
        ///A test for SqlString
        ///</summary>
        [TestMethod()]
        public void SqlStringTest_One() {
            ISqlObject source = new SqlServerSource("Table");
            ISqlObject field = new SqlServerField(source, "ID", null);
            ISqlOrderClause target = new SqlOrderClause().AddExpression(field, SqlOrder.Desc);
            Assert.AreEqual<string>(string.Format("{0} ({1}) DESC", target.Keyword, field.Fullname), target.SqlString);
        }

        /// <summary>
        ///A test for SqlString
        ///</summary>
        [TestMethod()]
        public void SqlStringTest_More() {
            ISqlObject source = new SqlServerSource("Table");
            ISqlObject field0 = new SqlServerField(source, "AddTime", null);
            ISqlObject field1 = SqlObject.FromVariable("ROWNUM");
            ISqlOrderClause target = new SqlOrderClause().AddExpression(field0, SqlOrder.Desc).AddExpression(field1, SqlOrder.Asc);
            Assert.AreEqual<string>(string.Format("{0} ({1}) DESC, ({2}) ASC", target.Keyword, field0.Fullname, field1.Fullname), target.SqlString);
        }

        /// <summary>
        ///A test for SqlString
        ///</summary>
        [TestMethod()]
        public void SqlStringTest_Expression() {
            ISqlObject source = new SqlServerSource("Table");
            ISqlExpression expression = new SqlFunction("MAX").AddArgument(new SqlServerField(source, "ID", null));
            ISqlObject field = SqlObject.FromVariable("ROWNUM");
            ISqlOrderClause target = new SqlOrderClause().AddExpression(expression, SqlOrder.Desc).AddExpression(field, SqlOrder.Asc);
            Assert.AreEqual<string>(string.Format("{0} ({1}) DESC, ({2}) ASC", target.Keyword, expression.SqlString, field.Fullname), target.SqlString);
        }
    }
}
