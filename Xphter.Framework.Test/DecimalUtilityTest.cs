using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for DecimalUtilityTest and is intended
    ///to contain all DecimalUtilityTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DecimalUtilityTest {


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
        ///A test for HasDecimalPlaces
        ///</summary>
        [TestMethod()]
        public void HasDecimalPlacesTest() {
            Assert.IsFalse((0M).HasDecimalPlaces());
            Assert.IsFalse((1M).HasDecimalPlaces());
            Assert.IsFalse((-1M).HasDecimalPlaces());

            Assert.IsTrue((1.1M).HasDecimalPlaces());
            Assert.IsTrue((-1.1M).HasDecimalPlaces());
        }

        /// <summary>
        ///A test for GetDecimalPlaces
        ///</summary>
        [TestMethod()]
        public void GetDecimalPlacesTest() {
            Assert.AreEqual<int>(0, (0M).GetDecimalPlaces());
            Assert.AreEqual<int>(0, (1M).GetDecimalPlaces());
            Assert.AreEqual<int>(0, (-1M).GetDecimalPlaces());

            Assert.AreEqual<int>(3, (1.000M).GetDecimalPlaces());
            Assert.AreEqual<int>(3, (-1.000M).GetDecimalPlaces());

            Assert.AreEqual<int>(4, (1.2345M).GetDecimalPlaces());
            Assert.AreEqual<int>(4, (-1.2345M).GetDecimalPlaces());

            Assert.AreEqual<int>(5, (1.23045M).GetDecimalPlaces());
            Assert.AreEqual<int>(5, (-1.23045M).GetDecimalPlaces());

            Assert.AreEqual<int>(3, (12.345M).GetDecimalPlaces());
            Assert.AreEqual<int>(3, (-12.345M).GetDecimalPlaces());

            Assert.AreEqual<int>(4, (12.0000M).GetDecimalPlaces());
            Assert.AreEqual<int>(4, (-12.0000M).GetDecimalPlaces());

            Assert.AreEqual<int>(5, (12.30405M).GetDecimalPlaces());
            Assert.AreEqual<int>(5, (-12.30405M).GetDecimalPlaces());
        }

        /// <summary>
        ///A test for GetValidDecimalPlaces
        ///</summary>
        [TestMethod()]
        public void GetValidDecimalPlacesTest() {
            Assert.AreEqual<int>(0, (0M).GetValidDecimalPlaces());
            Assert.AreEqual<int>(0, (1M).GetValidDecimalPlaces());
            Assert.AreEqual<int>(0, (-1M).GetValidDecimalPlaces());

            Assert.AreEqual<int>(0, (1.0000M).GetValidDecimalPlaces());
            Assert.AreEqual<int>(0, (-1.0000M).GetValidDecimalPlaces());

            Assert.AreEqual<int>(3, (1.23400000M).GetValidDecimalPlaces());
            Assert.AreEqual<int>(3, (-1.2340000M).GetValidDecimalPlaces());

            Assert.AreEqual<int>(6, (1.020304000M).GetValidDecimalPlaces());
            Assert.AreEqual<int>(6, (-1.020304000M).GetValidDecimalPlaces());

            Assert.AreEqual<int>(0, (12.0000M).GetValidDecimalPlaces());
            Assert.AreEqual<int>(0, (-12.0000M).GetValidDecimalPlaces());

            Assert.AreEqual<int>(3, (12.34500000M).GetValidDecimalPlaces());
            Assert.AreEqual<int>(3, (-12.3450000M).GetValidDecimalPlaces());

            Assert.AreEqual<int>(6, (12.030405000M).GetValidDecimalPlaces());
            Assert.AreEqual<int>(6, (-12.030405000M).GetValidDecimalPlaces());
        }

        [TestMethod()]
        public void ToUppercaseRMBTest() {
            Assert.AreEqual("叁拾贰元整", DecimalUtility.ToUppercaseRMB(32M));
            Assert.AreEqual("叁拾贰元伍角肆分", DecimalUtility.ToUppercaseRMB(32.54M));
            Assert.AreEqual("叁拾贰元伍角", DecimalUtility.ToUppercaseRMB(32.5M));
            Assert.AreEqual("叁拾贰元肆分", DecimalUtility.ToUppercaseRMB(32.04678M));
        }
    }
}
