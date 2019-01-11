using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;
using Xphter.Framework.Data.SqlServer;

namespace Xphter.Framework.Test {


  /// <summary>
  ///This is a test class for SqlFromClauseTest and is intended
  ///to contain all SqlFromClauseTest Unit Tests
  ///</summary>
  [TestClass()]
  public class SqlFromClauseTest {


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
    ///A test for CrossJoin
    ///</summary>
    [TestMethod()]
    public void CrossJoinTest() {
      ISqlObject source0 = new SqlServerSource("Table0");
      ISqlObject source1 = new SqlServerSource("Table1");
      SqlFromClause target = new SqlFromClause();
      Assert.AreEqual<string>(string.Format("FROM {0} CROSS JOIN {1}", source0.SqlString, source1.SqlString),
        target.SetSource(source0).CrossJoin(source1).SqlString);
    }

    /// <summary>
    ///A test for InnerJoin
    ///</summary>
    [TestMethod()]
    public void InnerJoinTest() {
      ISqlObject source0 = new SqlServerSource("Table0");
      ISqlObject source1 = new SqlServerSource("Table1");
      ISqlObject id0 = new SqlServerField(source0, "ID", null);
      ISqlObject id1 = new SqlServerField(source1, "ID", null);
      ISqlExpression condition = id0.Equal(id1);
      SqlFromClause target = new SqlFromClause();
      Assert.AreEqual<string>(string.Format("FROM {0} INNER JOIN {1} ON {2}", source0.SqlString, source1.SqlString, condition.SqlString),
        target.SetSource(source0).InnerJoin(source1, condition).SqlString);
    }

    /// <summary>
    ///A test for InnerJoin
    ///</summary>
    [TestMethod()]
    public void InnerJoinTest_Parametric() {
      ISqlObject source0 = new SqlServerSource("Table0");
      ISqlObject source1 = new SqlServerSource("Table1");
      ISqlObject id0 = new SqlServerField(source0, "ID", null);
      ISqlObject id1 = new SqlServerField(source1, "ID", null);
      ISqlExpression value = SqlStringExpression.FromParameter(new SqlParameter("@p", 13));
      ISqlExpression condition = id0.Equal(id1).And(id0.GreaterEqual(value));
      SqlFromClause target = new SqlFromClause();
      Assert.AreEqual<string>(string.Format("FROM {0} INNER JOIN {1} ON {2}", source0.SqlString, source1.SqlString, condition.SqlString),
        target.SetSource(source0).InnerJoin(source1, condition).SqlString);
      ReadOnlyCollection<IDataParameter> parameters = target.Parameters;
      Assert.AreEqual<int>(1, parameters.Count);
      Assert.IsTrue(parameters.Contains(value.Parameters[0]));
    }

    /// <summary>
    ///A test for FullOuterJoin
    ///</summary>
    [TestMethod()]
    public void FullOuterJoinTest() {
      ISqlObject source0 = new SqlServerSource("Table0");
      ISqlObject source1 = new SqlServerSource("Table1");
      ISqlObject id0 = new SqlServerField(source0, "ID", null);
      ISqlObject id1 = new SqlServerField(source1, "ID", null);
      ISqlExpression condition = id0.Equal(id1);
      SqlFromClause target = new SqlFromClause();
      Assert.AreEqual<string>(string.Format("FROM {0} FULL OUTER JOIN {1} ON {2}", source0.SqlString, source1.SqlString, condition.SqlString),
        target.SetSource(source0).FullOuterJoin(source1, condition).SqlString);
    }

    /// <summary>
    ///A test for LeftOuterJoin
    ///</summary>
    [TestMethod()]
    public void LeftOuterJoinTest() {
      ISqlObject source0 = new SqlServerSource("Table0");
      ISqlObject source1 = new SqlServerSource("Table1");
      ISqlObject id0 = new SqlServerField(source0, "ID", null);
      ISqlObject id1 = new SqlServerField(source1, "ID", null);
      ISqlExpression condition = id0.Equal(id1);
      SqlFromClause target = new SqlFromClause();
      Assert.AreEqual<string>(string.Format("FROM {0} LEFT OUTER JOIN {1} ON {2}", source0.SqlString, source1.SqlString, condition.SqlString),
        target.SetSource(source0).LeftOuterJoin(source1, condition).SqlString);
    }

    /// <summary>
    ///A test for RightOuterJoin
    ///</summary>
    [TestMethod()]
    public void RightOuterJoinTest() {
      ISqlObject source0 = new SqlServerSource("Table0");
      ISqlObject source1 = new SqlServerSource("Table1");
      ISqlObject id0 = new SqlServerField(source0, "ID", null);
      ISqlObject id1 = new SqlServerField(source1, "ID", null);
      ISqlExpression condition = id0.Equal(id1);
      SqlFromClause target = new SqlFromClause();
      Assert.AreEqual<string>(string.Format("FROM {0} RIGHT OUTER JOIN {1} ON {2}", source0.SqlString, source1.SqlString, condition.SqlString),
        target.SetSource(source0).RightOuterJoin(source1, condition).SqlString);
    }
  }
}
