using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;
using Xphter.Framework.Data.SqlServer;

namespace Xphter.Framework.Test {


  /// <summary>
  ///This is a test class for SqlKeywordsOperatorResultSqlStringProviderTest and is intended
  ///to contain all SqlKeywordsOperatorResultSqlStringProviderTest Unit Tests
  ///</summary>
  [TestClass()]
  public class SqlKeywordsOperatorResultSqlStringProviderTest {


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
    public void GetSqlStringTest_OneKeyword() {
      ISqlExpression expression = new SqlStringExpression("1 > 2");
      SqlOperator op = new SqlOperator("NOT", new SqlKeywordsOperatorResultSqlStringProvider("NOT"), new SqlKeywordsOperatorResultParametersProvider(1));
      Assert.AreEqual<string>(string.Format("NOT ({0})", expression.SqlString), op.Compute(expression).SqlString);
    }

    /// <summary>
    ///A test for GetSqlString
    ///</summary>
    [TestMethod()]
    public void GetSqlStringTest_MoreKeywords() {
      ISqlExpression expression0 = new SqlStringExpression("1");
      ISqlExpression expression1 = new SqlStringExpression("2");
      SqlOperator op = new SqlOperator("ADD", new SqlKeywordsOperatorResultSqlStringProvider("+"), new SqlKeywordsOperatorResultParametersProvider(1));
      Assert.AreEqual<string>(string.Format("({0}) + ({1})", expression0.SqlString, expression1.SqlString), op.Compute(expression0, expression1).SqlString);
    }
  }
}
