using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;

namespace Xphter.Framework.Test {


  /// <summary>
  ///This is a test class for SqlObjectTest and is intended
  ///to contain all SqlObjectTest Unit Tests
  ///</summary>
  [TestClass()]
  public class SqlObjectTest {


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
    ///A test for FromVariable
    ///</summary>
    [TestMethod()]
    public void FromVariableTest() {
      string variable = "@@error";
      ISqlObject obj = SqlObject.FromVariable(variable);
      Assert.AreEqual<string>(variable, obj.Fullname);
      Assert.AreEqual<string>(variable, obj.SqlString);
    }

    /// <summary>
    ///A test for FromFunction
    ///</summary>
    [TestMethod()]
    public void FromFunctionTest() {
      ISqlFunction function = new SqlFunction("SUM");
      function.AddArgument(new SqlStringExpression("1"));
      function.AddArgument(new SqlStringExpression("2"));
      ISqlObject obj = SqlObject.FromFunction(function);
      Assert.AreEqual<string>(function.SqlString, obj.Fullname);
      Assert.AreEqual<string>(function.SqlString, obj.SqlString);
    }
  }
}
