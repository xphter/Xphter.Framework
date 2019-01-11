using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;
using Xphter.Framework.Data.Oracle;

namespace Xphter.Framework.Test {


  /// <summary>
  ///This is a test class for OracleSelectStatementTest and is intended
  ///to contain all OracleSelectStatementTest Unit Tests
  ///</summary>
  [TestClass()]
  public class OracleSelectStatementTest {


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
    ///A test for Count
    ///</summary>
    [TestMethod()]
    public void CountTest() {
      ISqlObject source = new OracleSource("Table");
      ISqlObject field = new OracleField(source, "ID");
      OracleSelectStatement target = new OracleSelectStatement();
      target.SelectClause.AddExpressions(field);
      target.FromClause.Source = source;
      target.WhereClause.Condition = field.GreaterThan(new SqlStringExpression("13"));
      target.OrderClause.AddExpression(field, SqlOrder.Desc);
      string sqlString = target.SqlString;

      target.Count = 13;
      ISqlObject rowNumber = SqlObject.FromVariable("ROWNUM");
      Assert.AreEqual<string>(string.Format("SELECT * FROM ({1}) {0} WHERE {2} ORDER BY {3} ASC", new OracleSource(OracleSelectStatement.MIDDLE_RESULT_NAME), sqlString, rowNumber.GreaterEqual(new SqlStringExpression(target.Count.ToString())), rowNumber.Fullname),
        target.SqlString);
    }
  }
}
