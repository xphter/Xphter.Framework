using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;

namespace Xphter.Framework.Test {


  /// <summary>
  ///This is a test class for SqlInOperatorResultSqlStringProviderTest and is intended
  ///to contain all SqlInOperatorResultSqlStringProviderTest Unit Tests
  ///</summary>
  [TestClass()]
  public class SqlInOperatorResultSqlStringProviderTest {


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
    public void GetSqlStringTest_OneOperand() {
      ISqlExpression expression0 = new SqlStringExpression("ID");
      ISqlExpression expression1 = new SqlStringExpression("1");
      SqlOperator op = new SqlOperator("IN", new SqlInOperatorResultSqlStringProvider(), new SqlOperatorResultAllParametersProvider());
      Assert.AreEqual<string>(string.Format("({0}) IN ({1})", expression0.SqlString, expression1.SqlString), op.Compute(expression0, expression1).SqlString);
    }

    /// <summary>
    ///A test for GetSqlString
    ///</summary>
    [TestMethod()]
    public void GetSqlStringTest_MoreOperands() {
      ISqlExpression expression0 = new SqlStringExpression("ID");
      ISqlExpression expression1 = new SqlStringExpression("1");
      ISqlExpression expression2 = new SqlStringExpression("2");
      SqlOperator op = new SqlOperator("IN", new SqlInOperatorResultSqlStringProvider(), new SqlOperatorResultAllParametersProvider());
      Assert.AreEqual<string>(string.Format("({0}) IN ({1}, {2})", expression0.SqlString, expression1.SqlString, expression2.SqlString), op.Compute(expression0, expression1, expression2).SqlString);
    }
  }
}
