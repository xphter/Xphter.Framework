using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;
using Xphter.Framework.Data.SqlServer;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for SqlGroupClauseTest and is intended
    ///to contain all SqlGroupClauseTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SqlGroupClauseTest {


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
            Assert.AreEqual<string>(string.Empty, new SqlGroupClause().SqlString);
        }


        /// <summary>
        ///A test for SqlString
        ///</summary>
        [TestMethod()]
        public void SqlStringTest_One() {
            ISqlObject field = new SqlServerField(new SqlServerSource("Table"), "Field", null);
            ISqlGroupClause target = new SqlGroupClause().AddExpressions(field);
            Assert.AreEqual<string>(string.Format("{0} {1}", target.Keyword, field.Fullname), target.SqlString);
        }

        /// <summary>
        ///A test for SqlString
        ///</summary>
        [TestMethod()]
        public void SqlStringTest_More() {
            ISqlObject source = new SqlServerSource("Table");
            ISqlObject field0 = new SqlServerField(source, "Field0", null);
            ISqlObject field1 = SqlObject.FromVariable("ROWNUM");
            ISqlGroupClause target = new SqlGroupClause().AddExpressions(field0, field1);
            Assert.AreEqual<string>(string.Format("{0} {1}, {2}", target.Keyword, field0.Fullname, field1.Fullname), target.SqlString);
        }

        /// <summary>
        ///A test for SqlString
        ///</summary>
        [TestMethod()]
        public void SqlStringTest_Expression() {
            ISqlObject source = new SqlServerSource("Table");
            ISqlExpression expression = new SqlFunction("MAX").AddArgument(new SqlServerField(source, "ID", null));
            ISqlObject field = SqlObject.FromVariable("ROWNUM");
            ISqlGroupClause target = new SqlGroupClause().AddExpressions(expression, field);
            Assert.AreEqual<string>(string.Format("{0} {1}, {2}", target.Keyword, expression.SqlString, field.Fullname), target.SqlString);
        }
    }
}
