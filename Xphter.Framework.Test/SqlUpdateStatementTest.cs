using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;
using Xphter.Framework.Data.SqlServer;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for SqlUpdateStatementTest and is intended
    ///to contain all SqlUpdateStatementTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SqlUpdateStatementTest {


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
        ///A test for WhereClause
        ///</summary>
        [TestMethod()]
        public void SqlStringTest() {
            ISqlObject target = new SqlServerSource("Table1");
            ISqlObject field0 = new SqlServerField(target, "ID", null);
            ISqlExpression value0 = new SqlStringExpression("13");
            ISqlObject field1 = new SqlServerField(target, "Type", null);
            ISqlObject source = new SqlServerSource("Table2");
            ISqlExpression value1 = new SqlServerField(source, "Type", null);
            ISqlUpdateStatement statement = new SqlUpdateStatement();
            statement.FieldValueClause.SetSource(target).AddField(field0, value0).AddField(field1, value1);
            statement.FromClause.Source = source;
            statement.WhereClause.Condition = new SqlServerField(target, "Flag", null).Equal(new SqlStringExpression("1"));

            Assert.AreEqual<string>(string.Format("{0} {1} {2} {3}", statement.Keyword, statement.FieldValueClause.SqlString, statement.FromClause.SqlString, statement.WhereClause.SqlString),
              statement.SqlString);
        }

        /// <summary>
        ///A test for WhereClause
        ///</summary>
        [TestMethod()]
        public void SqlStringTest_NoWhere() {
            ISqlObject source = new SqlServerSource("Table");
            ISqlObject field0 = new SqlServerField(source, "ID", null);
            ISqlExpression value0 = new SqlStringExpression("13");
            ISqlObject field1 = new SqlServerField(source, "Type", null);
            ISqlExpression value1 = new SqlStringExpression("14");
            ISqlUpdateStatement statement = new SqlUpdateStatement();
            statement.FieldValueClause.SetSource(source).AddField(field0, value0).AddField(field1, value1);

            Assert.AreEqual<string>(string.Format("{0} {1}", statement.Keyword, statement.FieldValueClause.SqlString),
              statement.SqlString);
        }

        /// <summary>
        ///A test for WhereClause
        ///</summary>
        [TestMethod()]
        public void SqlStringTest_Parametric() {
            ISqlObject source = new SqlServerSource("Table");
            ISqlObject field0 = new SqlServerField(source, "ID", null);
            ISqlExpression value0 = SqlStringExpression.FromParameter(new SqlParameter("@ID", 13));
            ISqlObject field1 = new SqlServerField(source, "Type", null);
            ISqlExpression value1 = SqlStringExpression.FromParameter(new SqlParameter("@Type", 14));
            ISqlUpdateStatement statement = new SqlUpdateStatement();
            statement.FieldValueClause.SetSource(source).AddField(field0, value0).AddField(field1, value1);
            ISqlExpression value2 = SqlStringExpression.FromParameter(new SqlParameter("@Flag", 1));
            statement.WhereClause.Condition = new SqlServerField(source, "Flag", null).Equal(value2);

            Assert.AreEqual<string>(string.Format("{0} {1} {2}", statement.Keyword, statement.FieldValueClause.SqlString, statement.WhereClause.SqlString),
              statement.SqlString);
            ReadOnlyCollection<IDataParameter> parameters = statement.Parameters;
            Assert.AreEqual<int>(3, parameters.Count);
            Assert.IsTrue(parameters.Contains(value0.Parameters[0]));
            Assert.IsTrue(parameters.Contains(value1.Parameters[0]));
            Assert.IsTrue(parameters.Contains(value2.Parameters[0]));
        }
    }
}
