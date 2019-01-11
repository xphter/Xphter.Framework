using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework;

namespace Xphter.Framework.Test {
    /// <summary>
    ///This is a test class for DateTimeUtilityTest and is intended
    ///to contain all DateTimeUtilityTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DateTimeUtilityTest {
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
        ///A test for GetMonthsCount
        ///</summary>
        [TestMethod()]
        public void GetMonthsCountTest_SameYear() {
            Assert.AreEqual<int>(2, DateTimeUtility.GetCrossedMonthsCount(new DateTime(2013, 7, 13), new DateTime(2013, 8, 13)));
            Assert.AreEqual<int>(2, DateTimeUtility.GetCrossedMonthsCount(new DateTime(2013, 8, 13), new DateTime(2013, 7, 13)));
        }

        /// <summary>
        ///A test for GetMonthsCount
        ///</summary>
        [TestMethod()]
        public void GetMonthsCountTest_DiffYear() {
            Assert.AreEqual<int>(19, DateTimeUtility.GetCrossedMonthsCount(new DateTime(2012, 2, 13), new DateTime(2013, 8, 13)));
            Assert.AreEqual<int>(19, DateTimeUtility.GetCrossedMonthsCount(new DateTime(2013, 8, 13), new DateTime(2012, 2, 13)));

            Assert.AreEqual<int>(31, DateTimeUtility.GetCrossedMonthsCount(new DateTime(2011, 2, 13), new DateTime(2013, 8, 13)));
            Assert.AreEqual<int>(31, DateTimeUtility.GetCrossedMonthsCount(new DateTime(2013, 8, 13), new DateTime(2011, 2, 13)));
        }

        /// <summary>
        ///A test for GetWeekOfYear
        ///</summary>
        [TestMethod()]
        public void GetWeekOfYearTest() {
            Assert.AreEqual(1, new DateTime(2017, 1, 1).GetWeekOfYear());
            Assert.AreEqual(2, new DateTime(2017, 1, 4).GetWeekOfYear());
            Assert.AreEqual(2, new DateTime(2017, 1, 8).GetWeekOfYear());
            Assert.AreEqual(3, new DateTime(2017, 1, 12).GetWeekOfYear());
            Assert.AreEqual(4, new DateTime(2017, 1, 22).GetWeekOfYear());
            Assert.AreEqual(5, new DateTime(2017, 1, 25).GetWeekOfYear());
            Assert.AreEqual(6, new DateTime(2017, 1, 31).GetWeekOfYear());

            Assert.AreEqual(1, new DateTime(2020, 1, 1).GetWeekOfYear());
            Assert.AreEqual(1, new DateTime(2020, 1, 5).GetWeekOfYear());
            Assert.AreEqual(2, new DateTime(2020, 1, 7).GetWeekOfYear());
            Assert.AreEqual(2, new DateTime(2020, 1, 12).GetWeekOfYear());
        }

        /// <summary>
        ///A test for IsInSameWeek
        ///</summary>
        [TestMethod()]
        public void IsInSameWeekTest() {
            Assert.IsTrue(DateTimeUtility.IsInSameWeek(new DateTime(2018, 12, 24), new DateTime(2018, 12, 30)));
            Assert.IsFalse(DateTimeUtility.IsInSameWeek(new DateTime(2018, 12, 7), new DateTime(2018, 12, 10)));

            Assert.IsTrue(DateTimeUtility.IsInSameWeek(new DateTime(2018, 12, 31), new DateTime(2019, 1, 6)));
            Assert.IsFalse(DateTimeUtility.IsInSameWeek(new DateTime(2018, 12, 31), new DateTime(2019, 1, 7)));
        }

        /// <summary>
        ///A test for GetTimestamp
        ///</summary>
        [TestMethod()]
        public void GetTimestampTest() {
            DateTime now = DateTime.Now;

            long timestamp = now.GetTimestamp();
            DateTime time = DateTimeUtility.FromTimestamp(timestamp);

            Assert.AreEqual(now.Kind, time.Kind);
            Assert.AreEqual(now.Year, time.Year);
            Assert.AreEqual(now.Month, time.Month);
            Assert.AreEqual(now.Day, time.Day);
            Assert.AreEqual(now.Hour, time.Hour);
            Assert.AreEqual(now.Minute, time.Minute);
            Assert.AreEqual(now.Second, time.Second);
            Assert.AreEqual(now.Millisecond, time.Millisecond);
        }
    }
}
