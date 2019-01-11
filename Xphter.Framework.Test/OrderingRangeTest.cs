using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework;

namespace Xphter.Framework.Test {
  /// <summary>
  ///This is a test class for OrderingRangeTest and is intended
  ///to contain all OrderingRangeTest Unit Tests
  ///</summary>
  [TestClass()]
  public class OrderingRangeTest {


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

    private class A : IComparable<A> {
      public A(int grade) {
        this.Grade = grade;
      }

      public int Grade { get; private set; }

      #region IComparable<A> Members

      public int CompareTo(A other) {
        return other != null ? this.Grade - other.Grade : 1;
      }

      #endregion

      public override string ToString() {
        return this.Grade.ToString();
      }

      public override int GetHashCode() {
        return this.Grade.GetHashCode();
      }

      public override bool Equals(object obj) {
        return obj == null
                 ? false
                 : object.ReferenceEquals(this, obj)
                     ? true
                     : !(obj is A)
                         ? false
                         : ((A)obj).Grade == this.Grade;
      }
    }

    [TestMethod]
    public void TestValueType() {
      int i = 0;
      DateTime start = new DateTime(1900, 1, 1), end = new DateTime(1900, 12, 31), current = new DateTime();
      foreach (DateTime date in new OrderingRange<DateTime>(start, end, d => d.AddDays(1))) {
        current = start.AddDays(i++);
        Assert.AreEqual<DateTime>(current, date);
      }
    }

    [TestMethod]
    public void TestReferenceType() {
      A start = new A(0), end = new A(99), current = new A(0);
      foreach (A a in new OrderingRange<A>(start, end, item => new A(item.Grade + 1))) {
        Assert.AreEqual<A>(current, a);
        current = new A(current.Grade + 1);
      }
    }

    [TestMethod]
    public void TestNullToNotNull() {
      A start = null, end = new A(99), current = null;
      foreach (A a in new OrderingRange<A>(start, end, item => new A((item ?? new A(-1)).Grade + 1))) {
        Assert.AreEqual<A>(current, a);
        current = new A((current ?? new A(-1)).Grade + 1);
      }
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestNotNullToNull() {
      OrderingRange<A> range = new OrderingRange<A>(new A(0), null, null);
    }

    [TestMethod]
    public void TestNullToNull() {
      A start = null, end = null;
      foreach (A a in new OrderingRange<A>(start, end, item => new A(0))) {
        Assert.IsNull(a);
      }
    }
  }
}
