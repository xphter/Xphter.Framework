using Xphter.Framework.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Xphter.Framework.Test {


  /// <summary>
  ///This is a test class for RangeTest and is intended
  ///to contain all RangeTest Unit Tests
  ///</summary>
  [TestClass()]
  public class RangeTest {


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
    ///A test for Intersect
    ///</summary>
    [TestMethod()]
    public void IntersectTest_Left() {
      Range target = new Range(13, 8).Intersect(new Range(100, 10));
      Assert.AreEqual<long>(0, target.Length);
    }

    /// <summary>
    ///A test for Intersect
    ///</summary>
    [TestMethod()]
    public void IntersectTest_Right() {
      Range target = new Range(193, 8).Intersect(new Range(100, 10));
      Assert.AreEqual<long>(0, target.Length);
    }

    /// <summary>
    ///A test for Intersect
    ///</summary>
    [TestMethod()]
    public void IntersectTest_LeftCross() {
      Range target = new Range(99, 10).Intersect(new Range(100, 10));
      Assert.AreEqual<long>(100, target.StartIndex);
      Assert.AreEqual<long>(9, target.Length);
    }

    /// <summary>
    ///A test for Intersect
    ///</summary>
    [TestMethod()]
    public void IntersectTest_RightCross() {
      Range target = new Range(105, 10).Intersect(new Range(100, 10));
      Assert.AreEqual<long>(105, target.StartIndex);
      Assert.AreEqual<long>(5, target.Length);
    }

    /// <summary>
    ///A test for Intersect
    ///</summary>
    [TestMethod()]
    public void IntersectTest_In() {
      Range target = new Range(103, 6).Intersect(new Range(100, 10));
      Assert.AreEqual<long>(103, target.StartIndex);
      Assert.AreEqual<long>(6, target.Length);
    }

    /// <summary>
    ///A test for Contains
    ///</summary>
    [TestMethod()]
    public void ContainsTest_Yes() {
      Assert.IsTrue(new Range(100, 10).Contains(new Range(103, 6)));
    }

    /// <summary>
    ///A test for Contains
    ///</summary>
    [TestMethod()]
    public void ContainsTest_No() {
      Assert.IsFalse(new Range(100, 5).Contains(new Range(103, 6)));
    }
  }
}
