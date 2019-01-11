using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;
using Xphter.Framework.Data.SqlServer;

namespace Xphter.Framework.Test {


  /// <summary>
  ///This is a test class for SqlInsertFieldValueClauseSqlStringProviderTest and is intended
  ///to contain all SqlInsertFieldValueClauseSqlStringProviderTest Unit Tests
  ///</summary>
  [TestClass()]
  public class SqlInsertFieldValueClauseSqlStringProviderTest {


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
    public void GetSqlStringTest() {
      ISqlObject source = new SqlServerSource("Table");
      ISqlObject field0 = new SqlServerField(source, "ID", null);
      ISqlExpression value0 = new SqlStringExpression("13");
      ISqlObject field1 = new SqlServerField(source, "Type", null);
      ISqlExpression value1 = new SqlStringExpression("14");

      Assert.AreEqual<string>(string.Format("{0} ({1}, {2}) VALUES ({3}, {4})", source.Fullname, field0.Fullname, field1.Fullname, value0.SqlString, value1.SqlString),
        new SqlFieldValueClause(new SqlInsertFieldValueClauseSqlStringProvider()).SetSource(source).AddField(field0, value0).AddField(field1, value1).SqlString);
    }
  }
}
