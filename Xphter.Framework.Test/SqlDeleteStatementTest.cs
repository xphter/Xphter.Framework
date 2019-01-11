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
  ///This is a test class for SqlDeleteStatementTest and is intended
  ///to contain all SqlDeleteStatementTest Unit Tests
  ///</summary>
  [TestClass()]
  public class SqlDeleteStatementTest {


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
    public void SqlStringTest() {
      ISqlObject source = new SqlServerSource("Table");
      ISqlObject field = new SqlServerField(source, "ID", null);
      ISqlDeleteStatement statement = new SqlDeleteStatement();
      statement.FromClause.SetSource(source);
      statement.WhereClause.Condition = field.GreaterThan(new SqlStringExpression("13"));

      Assert.AreEqual<string>(string.Format("{0} {1} {2}", statement.Keyword, statement.FromClause.SqlString, statement.WhereClause.SqlString), statement.SqlString);
    }

    /// <summary>
    ///A test for SqlString
    ///</summary>
    [TestMethod()]
    public void SqlStringTest_NoWhere() {
      ISqlObject source = new SqlServerSource("Table");
      ISqlDeleteStatement statement = new SqlDeleteStatement();
      statement.FromClause.SetSource(source);

      Assert.AreEqual<string>(string.Format("{0} {1}", statement.Keyword, statement.FromClause.SqlString), statement.SqlString);
    }

    /// <summary>
    ///A test for SqlString
    ///</summary>
    [TestMethod()]
    public void SqlStringTest_Parametric() {
      ISqlObject source = new SqlServerSource("Table");
      ISqlObject field = new SqlServerField(source, "ID", null);
      ISqlExpression value = SqlStringExpression.FromParameter(new SqlParameter("@p", 13));
      ISqlDeleteStatement statement = new SqlDeleteStatement();
      statement.FromClause.SetSource(source);
      statement.WhereClause.Condition = field.GreaterThan(value);

      Assert.AreEqual<string>(string.Format("{0} {1} {2}", statement.Keyword, statement.FromClause.SqlString, statement.WhereClause.SqlString), statement.SqlString);
      ReadOnlyCollection<IDataParameter> parameters = statement.Parameters;
      Assert.AreEqual<int>(1, parameters.Count);
      Assert.IsTrue(parameters.Contains(value.Parameters[0]));
    }
  }
}
