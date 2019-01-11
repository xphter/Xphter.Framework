using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;
using Xphter.Framework.Data.SqlServer;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for SqlHelperTest and is intended
    ///to contain all SqlHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SqlHelperTest {


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

        public const string TABLE_NAME = "tableSqlHelperTest";

        /// <summary>
        ///A test for ExecuteReader
        ///</summary>
        [TestMethod()]
        public void ExecuteSelectTest() {
            ISqlObject source = new SqlServerSource(TABLE_NAME);
            ISqlSelectStatement statement = new SqlServerSelectStatement();
            statement.SelectClause.AddExpressions(new SqlAllField(source));
            statement.FromClause.Source = source;
            using(IDataReader reader = SqlHelper.ExecuteSelect(new SqlConnection(Properties.Settings.Default.SqlConnectionString), statement)) {
                object value = null;
                do {
                    while(reader.Read()) {
                        for(int i = 0; i < reader.FieldCount; i++) {
                            value = reader[i];
                        }
                    }
                } while(reader.NextResult());
            }
        }

        [TestMethod()]
        public void ExecuteSelectScalarTest() {
            ISqlObject source = new SqlServerSource(TABLE_NAME);
            ISqlObject field = new SqlServerField(source, "ID", null);
            ISqlSelectStatement statement = new SqlServerSelectStatement();
            statement.SelectClause.AddExpressions(field);
            statement.FromClause.Source = source;
            statement.OrderClause.AddExpression(field, SqlOrder.Asc);
            Assert.AreEqual<int>(1, SqlHelper.ExecuteSelectScalar<int>(new SqlConnection(Properties.Settings.Default.SqlConnectionString), statement));
        }

        [TestMethod()]
        public void ExecuteInsertTest() {
            int? id = null;
            ISqlObject source = new SqlServerSource(TABLE_NAME);
            DbConnection connection = new SqlConnection(Properties.Settings.Default.SqlConnectionString);
            try {
                ISqlSelectStatement identityStatement = new SqlServerSelectStatement();
                identityStatement.SelectClause.AddExpressions(new SqlFunction("IDENT_CURRENT").AddArgument(new SqlServerExpressionFactory().ConstantFactory.Create(TABLE_NAME)));
                int identity = Convert.ToInt32(SqlHelper.ExecuteSelectScalar<decimal>(connection, identityStatement));

                ISqlExpression name = SqlStringExpression.FromParameter(new SqlParameter("@name", "Insert Test"));
                ISqlInsertStatement insertStatement = new SqlInsertStatement();
                insertStatement.FieldValueClause.SetSource(source).AddField(new SqlServerField(source, "Name", null), name);
                Assert.AreEqual<int>(identity + 1, (id = Convert.ToInt32(SqlHelper.ExecuteInsert(connection, insertStatement, identityStatement))).Value);
            } finally {
                if(id.HasValue) {
                    ISqlDeleteStatement deleteStatement = new SqlDeleteStatement();
                    deleteStatement.FromClause.Source = source;
                    deleteStatement.WhereClause.Condition = new SqlServerField(source, "ID", null).Equal(new SqlServerExpressionFactory().ConstantFactory.Create(id.Value));
                    SqlHelper.ExecuteDelete(connection, deleteStatement);
                }
            }
        }

        /// <summary>
        ///A test for ExecuteUpdate
        ///</summary>
        [TestMethod()]
        public void ExecuteUpdateTest() {
            int? id = null;
            ISqlObject source = new SqlServerSource(TABLE_NAME);
            DbConnection connection = new SqlConnection(Properties.Settings.Default.SqlConnectionString);
            try {
                ISqlSelectStatement identityStatement = new SqlServerSelectStatement();
                identityStatement.SelectClause.AddExpressions(new SqlFunction("IDENT_CURRENT").AddArgument(new SqlServerExpressionFactory().ConstantFactory.Create(TABLE_NAME)));

                ISqlObject nameField = new SqlServerField(source, "Name", null);
                ISqlExpression name = SqlStringExpression.FromParameter(new SqlParameter("@name", "Insert Test"));
                ISqlInsertStatement insertStatement = new SqlInsertStatement();
                insertStatement.FieldValueClause.SetSource(source).AddField(nameField, name);
                id = SqlHelper.ExecuteInsert(connection, insertStatement, identityStatement);

                ISqlSelectStatement selectStatement = new SqlServerSelectStatement();
                selectStatement.SelectClause.AddExpressions(nameField);
                selectStatement.FromClause.Source = source;
                selectStatement.WhereClause.Condition = new SqlServerField(source, "ID", null).Equal(new SqlServerExpressionFactory().ConstantFactory.Create(id.Value));
                Assert.AreEqual<string>((string) name.Parameters[0].Value, SqlHelper.ExecuteSelectScalar<string>(connection, selectStatement));

                name = SqlStringExpression.FromParameter(new SqlParameter("@name", "Insert Test Updated"));
                ISqlUpdateStatement updateStatement = new SqlUpdateStatement();
                updateStatement.FieldValueClause.SetSource(source).AddField(nameField, name);
                updateStatement.WhereClause.Condition = new SqlServerField(source, "ID", null).Equal(new SqlServerExpressionFactory().ConstantFactory.Create(id.Value));
                SqlHelper.ExecuteUpdate(connection, updateStatement);
                Assert.AreEqual<string>((string) name.Parameters[0].Value, SqlHelper.ExecuteSelectScalar<string>(connection, selectStatement));
            } finally {
                if(id.HasValue) {
                    ISqlDeleteStatement deleteStatement = new SqlDeleteStatement();
                    deleteStatement.FromClause.Source = source;
                    deleteStatement.WhereClause.Condition = new SqlServerField(source, "ID", null).Equal(new SqlServerExpressionFactory().ConstantFactory.Create(id.Value));
                    SqlHelper.ExecuteDelete(connection, deleteStatement);
                }
            }
        }

        /// <summary>
        ///A test for ExecuteDelete
        ///</summary>
        [TestMethod()]
        public void ExecuteDeleteTest() {
            int? id = null;
            ISqlObject source = new SqlServerSource(TABLE_NAME);
            DbConnection connection = new SqlConnection(Properties.Settings.Default.SqlConnectionString);
            try {
                ISqlSelectStatement identityStatement = new SqlServerSelectStatement();
                identityStatement.SelectClause.AddExpressions(new SqlFunction("IDENT_CURRENT").AddArgument(new SqlServerExpressionFactory().ConstantFactory.Create(TABLE_NAME)));

                ISqlObject nameField = new SqlServerField(source, "Name", null);
                ISqlExpression name = SqlStringExpression.FromParameter(new SqlParameter("@name", "Insert Test"));
                ISqlInsertStatement insertStatement = new SqlInsertStatement();
                insertStatement.FieldValueClause.SetSource(source).AddField(nameField, name);
                id = SqlHelper.ExecuteInsert(connection, insertStatement, identityStatement);
            } finally {
                if(id.HasValue) {
                    ISqlDeleteStatement deleteStatement = new SqlDeleteStatement();
                    deleteStatement.FromClause.Source = source;
                    deleteStatement.WhereClause.Condition = new SqlServerField(source, "ID", null).Equal(SqlStringExpression.FromParameter(new SqlParameter("@ID", id.Value)));
                    SqlHelper.ExecuteDelete(connection, deleteStatement);
                }
            }
        }

        /// <summary>
        ///A test for ExecuteSchema
        ///</summary>
        [TestMethod()]
        public void ExecuteSchemaTest() {
            ISqlObject source = new SqlServerSource("tableTypeList");
            ISqlSelectStatement statement = new SqlServerSelectStatement();
            statement.SelectClause.AddExpressions(new SqlAllField(source));
            statement.FromClause.Source = source;
            statement.WhereClause.Condition = new SqlStringExpression("1 = 2");

            DbConnection connection = new SqlConnection(Properties.Settings.Default.SqlConnectionString);
            DataTable schema = SqlHelper.ExecuteSchema(connection, statement);
        }

        /// <summary>
        ///A test for ExecuteReader
        ///</summary>
        [TestMethod()]
        public void ExecuteReaderTest_StoredProcedure() {
            using(IDataReader reader = SqlHelper.ExecuteReader(new SqlConnection(Properties.Settings.Default.SqlConnectionString), "sp_help", SqlHelper.CreateParameter<SqlParameter>("@objname", DbType.AnsiString, "tableSqlHelperTest"))) {
            }
        }

        /// <summary>
        ///A test for ExecuteReader
        ///</summary>
        [TestMethod()]
        public void ExecuteReaderTest_StoredProcedure_Transaction() {
            using(IDataReader reader = SqlHelper.ExecuteReader(IsolationLevel.Serializable, new SqlConnection(Properties.Settings.Default.SqlConnectionString), "sp_help", SqlHelper.CreateParameter<SqlParameter>("@objname", DbType.AnsiString, "tableSqlHelperTest"))) {
            }
        }

        /// <summary>
        ///A test for ExecuteScalar
        ///</summary>
        [TestMethod()]
        public void ExecuteScalarTest_StoredProcedure() {
            object obj = SqlHelper.ExecuteScalar<object>(new SqlConnection(Properties.Settings.Default.SqlConnectionString), "sp_help", SqlHelper.CreateParameter<SqlParameter>("@objname", DbType.AnsiString, "tableSqlHelperTest"));
        }

        /// <summary>
        ///A test for ExecuteScalar
        ///</summary>
        [TestMethod()]
        public void ExecuteScalarTest_StoredProcedure_Transaction() {
            object obj = SqlHelper.ExecuteScalar<object>(IsolationLevel.Serializable, new SqlConnection(Properties.Settings.Default.SqlConnectionString), "sp_help", SqlHelper.CreateParameter<SqlParameter>("@objname", DbType.AnsiString, "tableSqlHelperTest"));
        }

        /// <summary>
        ///A test for ExecuteNoQuery
        ///</summary>
        [TestMethod()]
        public void ExecuteNoQueryTest_StoredProcedure() {
            int count = SqlHelper.ExecuteNoQuery(new SqlConnection(Properties.Settings.Default.SqlConnectionString), "sp_help", SqlHelper.CreateParameter<SqlParameter>("@objname", DbType.AnsiString, "tableSqlHelperTest"));
        }

        /// <summary>
        ///A test for ExecuteNoQuery
        ///</summary>
        [TestMethod()]
        public void ExecuteNoQueryTest_StoredProcedure_Transaction() {
            int count = SqlHelper.ExecuteNoQuery(IsolationLevel.Serializable, new SqlConnection(Properties.Settings.Default.SqlConnectionString), "sp_help", SqlHelper.CreateParameter<SqlParameter>("@objname", DbType.AnsiString, "tableSqlHelperTest"));
        }
    }
}
