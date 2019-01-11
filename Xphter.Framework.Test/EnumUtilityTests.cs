using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for EnumUtilityTest and is intended
    ///to contain all EnumUtilityTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EnumUtilityTests {


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

        private enum TestEnum {
            [System.ComponentModel.Description("TestEnumValue")]
            Value = 0x01,
        }

        [Flags]
        private enum TestEnum2 {
            [System.ComponentModel.Description("Value0")]
            Value0 = 0x08,

            [System.ComponentModel.Description("Value1")]
            Value1 = 0x10,

            [System.ComponentModel.Description("Value2")]
            Value2 = 0x20,

            [System.ComponentModel.Description("Value3")]
            Value3 = 0x40,

            [System.ComponentModel.Description("Value4")]
            Value4 = 0x80,
        }

        /// <summary>
        ///A test for GetDescription
        ///</summary>
        [TestMethod()]
        public void GetDescriptionTest() {
            Assert.AreEqual<string>("TestEnumValue", EnumUtility.GetDescription(TestEnum.Value));

            TestEnum none = (TestEnum) 12345;
            Assert.AreEqual<string>(EnumUtility.GetDescription(none), none.ToString());
        }

        [TestMethod()]
        public void GetValueTest() {
            Assert.AreEqual(TestEnum2.Value2, EnumUtility.GetValue<TestEnum2>("Value2"));
            Assert.AreEqual(TestEnum2.Value1 | TestEnum2.Value4, EnumUtility.GetValue<TestEnum2>(new string[] { "Value1", "Value4" }));
        }
    }
}
