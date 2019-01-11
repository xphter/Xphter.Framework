using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;
using Xphter.Framework.Data.SqlServer;

namespace Xphter.Framework.Test {


  /// <summary>
  ///This is a test class for SqlSelectClauseTest and is intended
  ///to contain all SqlSelectClauseTest Unit Tests
  ///</summary>
  [TestClass()]
  public class SqlSelectClauseTest {


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
      ISqlObject field0 = new SqlServerField(source, "Field0", null);
      ISqlFunction field1 = new SqlFunction("COUNT").AddArgument(new SqlAllField(null));
      ISqlSelectClause target = new SqlSelectClause().AddExpressions(field0, field1);
      Assert.AreEqual<string>(string.Format("{0} {1}, {2}", target.Keyword, field0.SqlString, field1.SqlString), target.SqlString);
    }

    /// <summary>
    ///A test for SqlString
    ///</summary>
    [TestMethod()]
    public void SqlStringTest_Distinct() {
      ISqlObject source = new SqlServerSource("Table");
      ISqlObject field0 = new SqlServerField(source, "Field0", null);
      ISqlFunction field1 = new SqlFunction("COUNT").AddArgument(new SqlAllField(null));
      ISqlSelectClause target = new SqlSelectClause().SetIsDistinct(true).AddExpressions(field0, field1);
      Assert.AreEqual<string>(string.Format("{0} DISTINCT {1}, {2}", target.Keyword, field0.SqlString, field1.SqlString), target.SqlString);
    }
  }
}
