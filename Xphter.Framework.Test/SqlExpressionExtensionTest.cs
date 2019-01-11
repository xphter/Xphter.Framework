using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;

namespace Xphter.Framework.Test {

  /// <summary>
  ///This is a test class for SqlExpressionExtensionTest and is intended
  ///to contain all SqlExpressionExtensionTest Unit Tests
  ///</summary>
  [TestClass()]
  public class SqlExpressionExtensionTest {


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
    ///A test for And
    ///</summary>
    [TestMethod()]
    public void AndTest() {
      Assert.AreEqual<string>("(13) AND (14)", SqlExpressionExtension.And(new SqlStringExpression("13"), new SqlStringExpression("14")).SqlString);
    }

    /// <summary>
    ///A test for Or
    ///</summary>
    [TestMethod()]
    public void OrTest() {
      Assert.AreEqual<string>("(13) OR (14)", SqlExpressionExtension.Or(new SqlStringExpression("13"), new SqlStringExpression("14")).SqlString);
    }

    /// <summary>
    ///A test for Not
    ///</summary>
    [TestMethod()]
    public void NotTest() {
      Assert.AreEqual<string>("NOT (13)", SqlExpressionExtension.Not(new SqlStringExpression("13")).SqlString);
    }

    /// <summary>
    ///A test for In
    ///</summary>
    [TestMethod()]
    public void InTest() {
      Assert.AreEqual<string>("(ID) IN (13, 14)", SqlExpressionExtension.In(new SqlStringExpression("ID"), new SqlStringExpression("13"), new SqlStringExpression("14")).SqlString);
    }

    /// <summary>
    ///A test for Like
    ///</summary>
    [TestMethod()]
    public void LikeTest() {
      Assert.AreEqual<string>("(13) LIKE (14)", SqlExpressionExtension.Like(new SqlStringExpression("13"), new SqlStringExpression("14")).SqlString);
    }

    /// <summary>
    ///A test for Between
    ///</summary>
    [TestMethod()]
    public void BetweenTest() {
      Assert.AreEqual<string>("(ID) BETWEEN (13) AND (14)", SqlExpressionExtension.Between(new SqlStringExpression("ID"), new SqlStringExpression("13"), new SqlStringExpression("14")).SqlString);
    }

    /// <summary>
    ///A test for Equal
    ///</summary>
    [TestMethod()]
    public void EqualTest() {
      Assert.AreEqual<string>("(13) = (14)", SqlExpressionExtension.Equal(new SqlStringExpression("13"), new SqlStringExpression("14")).SqlString);
    }

    /// <summary>
    ///A test for GreaterThan
    ///</summary>
    [TestMethod()]
    public void GreaterThanTest() {
      Assert.AreEqual<string>("(13) > (14)", SqlExpressionExtension.GreaterThan(new SqlStringExpression("13"), new SqlStringExpression("14")).SqlString);
    }

    /// <summary>
    ///A test for LessThan
    ///</summary>
    [TestMethod()]
    public void LessThanTest() {
      Assert.AreEqual<string>("(13) < (14)", SqlExpressionExtension.LessThan(new SqlStringExpression("13"), new SqlStringExpression("14")).SqlString);
    }

    /// <summary>
    ///A test for GreaterEqual
    ///</summary>
    [TestMethod()]
    public void GreaterEqualTest() {
      Assert.AreEqual<string>("(13) >= (14)", SqlExpressionExtension.GreaterEqual(new SqlStringExpression("13"), new SqlStringExpression("14")).SqlString);
    }

    /// <summary>
    ///A test for LessEqual
    ///</summary>
    [TestMethod()]
    public void LessEqualTest() {
      Assert.AreEqual<string>("(13) <= (14)", SqlExpressionExtension.LessEqual(new SqlStringExpression("13"), new SqlStringExpression("14")).SqlString);
    }

    /// <summary>
    ///A test for BitwiseAnd
    ///</summary>
    [TestMethod()]
    public void BitwiseAndTest() {
      Assert.AreEqual<string>("(13) & (14)", SqlExpressionExtension.BitwiseAnd(new SqlStringExpression("13"), new SqlStringExpression("14")).SqlString);
    }

    /// <summary>
    ///A test for BitwiseOr
    ///</summary>
    [TestMethod()]
    public void BitwiseOrTest() {
      Assert.AreEqual<string>("(13) | (14)", SqlExpressionExtension.BitwiseOr(new SqlStringExpression("13"), new SqlStringExpression("14")).SqlString);
    }

    /// <summary>
    ///A test for ExclusiveOr
    ///</summary>
    [TestMethod()]
    public void ExclusiveOrTest() {
      Assert.AreEqual<string>("^ (13)", SqlExpressionExtension.ExclusiveOr(new SqlStringExpression("13")).SqlString);
    }

    /// <summary>
    ///A test for Add
    ///</summary>
    [TestMethod()]
    public void AddTest() {
      Assert.AreEqual<string>("(13) + (14)", SqlExpressionExtension.Add(new SqlStringExpression("13"), new SqlStringExpression("14")).SqlString);
    }

    /// <summary>
    ///A test for Subtract
    ///</summary>
    [TestMethod()]
    public void SubtractTest() {
      Assert.AreEqual<string>("(13) - (14)", SqlExpressionExtension.Subtract(new SqlStringExpression("13"), new SqlStringExpression("14")).SqlString);
    }

    /// <summary>
    ///A test for Multiply
    ///</summary>
    [TestMethod()]
    public void MultiplyTest() {
      Assert.AreEqual<string>("(13) * (14)", SqlExpressionExtension.Multiply(new SqlStringExpression("13"), new SqlStringExpression("14")).SqlString);
    }

    /// <summary>
    ///A test for Divide
    ///</summary>
    [TestMethod()]
    public void DivideTest() {
      Assert.AreEqual<string>("(13) / (14)", SqlExpressionExtension.Divide(new SqlStringExpression("13"), new SqlStringExpression("14")).SqlString);
    }

    /// <summary>
    ///A test for Mode
    ///</summary>
    [TestMethod()]
    public void ModeTest() {
      Assert.AreEqual<string>("(13) % (14)", SqlExpressionExtension.Mode(new SqlStringExpression("13"), new SqlStringExpression("14")).SqlString);
    }

    /// <summary>
    ///A test for Assign
    ///</summary>
    [TestMethod()]
    public void AssignTest() {
      Assert.AreEqual<string>("13 = (14)", SqlExpressionExtension.Assign(new SqlStringExpression("13"), new SqlStringExpression("14")).SqlString);
    }

    /// <summary>
    ///A test for IsNull
    ///</summary>
    [TestMethod()]
    public void IsNullTest() {
      Assert.AreEqual<string>("(ID) IS NULL", SqlExpressionExtension.IsNull(new SqlStringExpression("ID")).SqlString);
    }
  }
}
