using System;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;
using Xphter.Framework.Data.SqlServer;

namespace Xphter.Framework.Test {


  /// <summary>
  ///This is a test class for SqlFunctionTest and is intended
  ///to contain all SqlFunctionTest Unit Tests
  ///</summary>
  [TestClass()]
  public class SqlFunctionTest {


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
    ///A test for SqlFunction Constructor
    ///</summary>
    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void SqlFunctionConstructorTest_Null() {
      new SqlFunction(null);
    }

    /// <summary>
    ///A test for SqlFunction Constructor
    ///</summary>
    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void SqlFunctionConstructorTest_Empty() {
      new SqlFunction(" \r \n \t ");
    }

    /// <summary>
    ///A test for AddArgument
    ///</summary>
    [TestMethod()]
    public void AddArgumentTest_One() {
      SqlFunction target = new SqlFunction("ADD");
      ISqlExpression argument = new SqlStringExpression("13");
      target.AddArgument(argument);
      Assert.AreEqual<string>(string.Format("{0}({1})", target.Name, argument.SqlString), target.SqlString);
    }

    /// <summary>
    ///A test for AddArgument
    ///</summary>
    [TestMethod()]
    public void AddArgumentTest_More() {
      SqlFunction target = new SqlFunction("SUM");
      ISqlExpression argument0 = new SqlStringExpression("13");
      ISqlExpression argument1 = new SqlStringExpression("14");
      target.AddArgument(argument0, argument1);
      Assert.AreEqual<string>(string.Format("{0}({1}, {2})", target.Name, argument0.SqlString, argument1.SqlString), target.SqlString);
    }

    /// <summary>
    ///A test for AddArgument
    ///</summary>
    [TestMethod()]
    public void AddArgumentTest_Parametric() {
      SqlFunction target = new SqlFunction("SUM");
      ISqlExpression argument0 = SqlStringExpression.FromParameter(new SqlParameter("@p0", 13));
      ISqlExpression argument1 = SqlStringExpression.FromParameter(new SqlParameter("@p1", 14));
      target.AddArgument(argument0, argument1);
      Assert.AreEqual<string>(string.Format("{0}({1}, {2})", target.Name, argument0.SqlString, argument1.SqlString), target.SqlString);
      Assert.IsTrue(target.Parameters.Count == 2);
      Assert.IsTrue(target.Parameters.Contains(argument0.Parameters[0]));
      Assert.IsTrue(target.Parameters.Contains(argument1.Parameters[0]));
    }

    /// <summary>
    ///A test for AddArgument
    ///</summary>
    [TestMethod()]
    public void AddArgumentTest_Field() {
      SqlFunction target = new SqlFunction("COUNT");
      ISqlObject field = new SqlServerField(new SqlServerSource("Table"), "ID", null);
      target.AddArgument(field, false);
      Assert.AreEqual<string>(string.Format("{0}({1})", target.Name, field.Fullname), target.SqlString);
    }

    /// <summary>
    ///A test for AddArgument
    ///</summary>
    [TestMethod()]
    public void AddArgumentTest_Field_Distinct() {
      SqlFunction target = new SqlFunction("COUNT");
      ISqlObject field = new SqlServerField(new SqlServerSource("Table"), "ID", null);
      target.AddArgument(field, true);
      Assert.AreEqual<string>(string.Format("{0}(DISTINCT {1})", target.Name, field.Fullname), target.SqlString);
    }

    /// <summary>
    ///A test for RemoveArgument
    ///</summary>
    [TestMethod()]
    public void RemoveArgumentTest() {
      SqlFunction target = new SqlFunction("ADD");
      ISqlExpression argument0 = new SqlStringExpression("13");
      ISqlExpression argument1 = new SqlStringExpression("14");
      ISqlExpression argument2 = new SqlStringExpression("15");
      target.AddArgument(argument0, argument1, argument2);
      Assert.AreEqual<string>(string.Format("{0}({1}, {2}, {3})", target.Name, argument0.SqlString, argument1.SqlString, argument2.SqlString), target.SqlString);
      target.RemoveArgument(argument2);
      Assert.AreEqual<string>(string.Format("{0}({1}, {2})", target.Name, argument0.SqlString, argument1.SqlString), target.SqlString);
    }
  }
}
