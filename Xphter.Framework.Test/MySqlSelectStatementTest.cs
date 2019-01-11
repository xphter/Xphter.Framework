using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data.MySql;
using Xphter.Framework.Data;

namespace Xphter.Framework.Test {


  /// <summary>
  ///This is a test class for MySqlSelectStatementTest and is intended
  ///to contain all MySqlSelectStatementTest Unit Tests
  ///</summary>
  [TestClass()]
  public class MySqlSelectStatementTest {


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
      ISqlObject source = new MySqlSource("Table");
      ISqlObject field = new MySqlField(source, "ID");
      MySqlSelectStatement target = new MySqlSelectStatement();
      target.SelectClause.AddExpressions(field);
      target.FromClause.SetSource(source);
      target.Count = 13;

      Assert.AreEqual<string>(string.Format("{0} {1} {2}", target.SelectClause.SqlString, target.FromClause.SqlString, target.LimitClause.SqlString),
        target.SqlString);
    }
  }
}
