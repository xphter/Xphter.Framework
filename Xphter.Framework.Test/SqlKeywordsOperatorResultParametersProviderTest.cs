using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;

namespace Xphter.Framework.Test {


  /// <summary>
  ///This is a test class for SqlKeywordsOperatorResultParametersProviderTest and is intended
  ///to contain all SqlKeywordsOperatorResultParametersProviderTest Unit Tests
  ///</summary>
  [TestClass()]
  public class SqlKeywordsOperatorResultParametersProviderTest {


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
    ///A test for GetParameters
    ///</summary>
    [TestMethod()]
    public void GetParametersTest_OneKeyword() {
      ISqlExpression expression = SqlStringExpression.FromParameter(new SqlParameter("@p", true));
      SqlOperator op = new SqlOperator("NOT", new SqlKeywordsOperatorResultSqlStringProvider("NOT"), new SqlKeywordsOperatorResultParametersProvider(1));
      ReadOnlyCollection<IDataParameter> parameters = op.Compute(expression).Parameters;
      Assert.AreEqual<int>(1, parameters.Count);
      Assert.IsTrue(parameters.Contains(expression.Parameters[0]));
    }

    /// <summary>
    ///A test for GetParameters
    ///</summary>
    [TestMethod()]
    public void GetParametersTest_MoreKeywords() {
      ISqlExpression expression0 = SqlStringExpression.FromParameter(new SqlParameter("@p0", true));
      ISqlExpression expression1 = SqlStringExpression.FromParameter(new SqlParameter("@p1", true));
      SqlOperator op = new SqlOperator("ADD", new SqlKeywordsOperatorResultSqlStringProvider("+"), new SqlKeywordsOperatorResultParametersProvider(1));
      ReadOnlyCollection<IDataParameter> parameters = op.Compute(expression0, expression1).Parameters;
      Assert.AreEqual<int>(2, parameters.Count);
      Assert.IsTrue(parameters.Contains(expression0.Parameters[0]));
      Assert.IsTrue(parameters.Contains(expression1.Parameters[0]));
    }
  }
}
