using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Web;

namespace Xphter.Framework.Test {


  /// <summary>
  ///This is a test class for KeywordsRankingQuerierTest and is intended
  ///to contain all KeywordsRankingQuerierTest Unit Tests
  ///</summary>
  [TestClass()]
  public class KeywordsRankingQuerierTest {


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
    ///A test for Query
    ///</summary>
    [TestMethod()]
    public void QueryTest_Baidu_One() {
      IKeywordsRanking ranking = new BaiduKeywordsRanking();
      KeywordsRankingQuerier target = new KeywordsRankingQuerier(ranking);
      string keyword = "中国";
      string domain = "163.com";
      int result = target.Query(keyword, domain);
    }

    /// <summary>
    ///A test for Query
    ///</summary>
    [TestMethod()]
    public void QueryTest_Baidu_More() {
      IKeywordsRanking ranking = new BaiduKeywordsRanking();
      KeywordsRankingQuerier target = new KeywordsRankingQuerier(ranking);
      string keyword = "中国 足球";
      string domain = "163.com";
      int result = target.Query(keyword, domain);
    }
  }
}
