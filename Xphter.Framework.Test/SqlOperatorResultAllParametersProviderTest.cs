using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;

namespace Xphter.Framework.Test {


  /// <summary>
  ///This is a test class for SqlOperatorResultAllParametersProviderTest and is intended
  ///to contain all SqlOperatorResultAllParametersProviderTest Unit Tests
  ///</summary>
  [TestClass()]
  public class SqlOperatorResultAllParametersProviderTest {


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
    public void GetParametersTest() {
      ISqlExpression expression0 = new SqlStringExpression("ID");
      ISqlExpression expression1 = SqlStringExpression.FromParameter(new SqlParameter("@p1", 1));
      ISqlExpression expression2 = SqlStringExpression.FromParameter(new SqlParameter("@p2", 2));
      SqlOperator op = new SqlOperator("IN", new SqlInOperatorResultSqlStringProvider(), new SqlOperatorResultAllParametersProvider());
      ReadOnlyCollection<IDataParameter> parameters = op.Compute(expression0, expression1, expression2).Parameters;
      Assert.AreEqual<int>(2, parameters.Count);
      Assert.IsTrue(parameters.Contains(expression1.Parameters[0]));
      Assert.IsTrue(parameters.Contains(expression2.Parameters[0]));
    }
  }
}
