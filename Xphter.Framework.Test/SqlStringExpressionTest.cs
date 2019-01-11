using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;

namespace Xphter.Framework.Test {


  /// <summary>
  ///This is a test class for SqlExpressionTest and is intended
  ///to contain all SqlExpressionTest Unit Tests
  ///</summary>
  [TestClass()]
  public class SqlStringExpressionTest {


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
    [ExpectedException(typeof(ArgumentException))]
    public void SqlString_Null() {
      SqlStringExpression target = new SqlStringExpression(null);
    }

    /// <summary>
    ///A test for SqlString
    ///</summary>
    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void SqlString_Empty() {
      SqlStringExpression target = new SqlStringExpression(" \r \n \t ");
    }

    /// <summary>
    ///A test for SqlString
    ///</summary>
    [TestMethod()]
    public void SqlStringTest() {
      string sqlString = "1 = 1";
      SqlStringExpression target = new SqlStringExpression(sqlString);
      Assert.AreEqual<string>(sqlString, target.SqlString);
    }

    /// <summary>
    ///A test for Parameters
    ///</summary>
    [TestMethod()]
    public void ParametersTest() {
      string sqlString = "Name = @name";
      IDataParameter parameter = new SqlParameter("@name", "test");
      SqlStringExpression target = new SqlStringExpression(sqlString, parameter);
      Assert.IsTrue(target.Parameters.Count == 1);
      Assert.IsTrue(target.Parameters.Contains(parameter));
    }
  }
}
