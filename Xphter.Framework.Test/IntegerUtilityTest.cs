using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for IntegerUtilityTest and is intended
    ///to contain all IntegerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class IntegerUtilityTest {

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
        ///A test for GetBitCount
        ///</summary>
        [TestMethod()]
        public void GetBitCountTest_byte() {
            Assert.AreEqual<int>(0, IntegerUtility.GetBitCount((byte) 0x00));
            Assert.AreEqual<int>(sizeof(byte) * 8, IntegerUtility.GetBitCount((byte) 0xFF));
            Assert.AreEqual<int>(sizeof(byte) * 8 - 3, IntegerUtility.GetBitCount((byte) 0x12));
        }

        /// <summary>
        ///A test for GetBitCount
        ///</summary>
        [TestMethod()]
        public void GetBitCountTest_ushort() {
            Assert.AreEqual<int>(0, IntegerUtility.GetBitCount((ushort) 0x00));
            Assert.AreEqual<int>(sizeof(ushort) * 8, IntegerUtility.GetBitCount((ushort) 0xFFFF));
            Assert.AreEqual<int>(sizeof(ushort) * 8 - 3, IntegerUtility.GetBitCount((ushort) 0x1200));
        }

        /// <summary>
        ///A test for GetBitCount
        ///</summary>
        [TestMethod()]
        public void GetBitCountTest_uint() {
            Assert.AreEqual<int>(0, IntegerUtility.GetBitCount((uint) 0x00));
            Assert.AreEqual<int>(sizeof(uint) * 8, IntegerUtility.GetBitCount((uint) 0xFFFFFFFF));
            Assert.AreEqual<int>(sizeof(uint) * 8 - 3, IntegerUtility.GetBitCount((uint) 0x12000000));
        }

        /// <summary>
        ///A test for GetBitCount
        ///</summary>
        [TestMethod()]
        public void GetBitCountTest_ulong() {
            Assert.AreEqual<int>(0, IntegerUtility.GetBitCount((ulong) 0x00));
            Assert.AreEqual<int>(sizeof(ulong) * 8, IntegerUtility.GetBitCount((ulong) 0xFFFFFFFFFFFFFFFF));
            Assert.AreEqual<int>(sizeof(ulong) * 8 - 3, IntegerUtility.GetBitCount((ulong) 0x1200000000000000));
        }

        /// <summary>
        ///A test for ToBytes
        ///</summary>
        [TestMethod()]
        public void ToBytesTest() {
            int offset = 0;
            long value = 0;
            byte[] buffer = new byte[100];

            value = 0x1234;
            IntegerUtility.ToBytes((short) value, buffer, offset);
            Assert.AreEqual<byte>(buffer[0], 0x34);
            Assert.AreEqual<byte>(buffer[1], 0x12);

            value = 0x12345678;
            IntegerUtility.ToBytes((int) value, buffer, offset);
            Assert.AreEqual<byte>(buffer[0], 0x78);
            Assert.AreEqual<byte>(buffer[1], 0x56);
            Assert.AreEqual<byte>(buffer[2], 0x34);
            Assert.AreEqual<byte>(buffer[3], 0x12);

            value = 0x123456789ABCDEF0;
            IntegerUtility.ToBytes((long) value, buffer, offset);
            Assert.AreEqual<byte>(buffer[0], 0xF0);
            Assert.AreEqual<byte>(buffer[1], 0xDE);
            Assert.AreEqual<byte>(buffer[2], 0xBC);
            Assert.AreEqual<byte>(buffer[3], 0x9A);
            Assert.AreEqual<byte>(buffer[4], 0x78);
            Assert.AreEqual<byte>(buffer[5], 0x56);
            Assert.AreEqual<byte>(buffer[6], 0x34);
            Assert.AreEqual<byte>(buffer[7], 0x12);
        }

        /// <summary>
        ///A test for FromBytes
        ///</summary>
        [TestMethod()]
        public void Int32FromBytesTest() {
            byte[] buffer = new byte[] {
                0x78, 0x56, 0x34, 0x12,
            };
            int value = IntegerUtility.Int32FromBytes(buffer, 0);
            Assert.AreEqual<int>(0x12345678, value);
        }

        [TestMethod()]
        public void ToUppercaseChineseNumberTest() {
            Assert.AreEqual("零", IntegerUtility.ToUppercaseChineseNumber(0));
            Assert.AreEqual("负叁仟肆佰伍拾陆万柒仟捌佰玖拾捌亿柒仟陆佰伍拾肆万叁仟贰佰壹拾",
                IntegerUtility.ToUppercaseChineseNumber(-3456789876543210));

            Assert.AreEqual("叁", IntegerUtility.ToUppercaseChineseNumber(3));
            Assert.AreEqual("叁拾", IntegerUtility.ToUppercaseChineseNumber(30));
            Assert.AreEqual("叁佰", IntegerUtility.ToUppercaseChineseNumber(300));
            Assert.AreEqual("叁万", IntegerUtility.ToUppercaseChineseNumber(30000));
            Assert.AreEqual("叁佰万", IntegerUtility.ToUppercaseChineseNumber(3000000));

            Assert.AreEqual("叁佰零贰", IntegerUtility.ToUppercaseChineseNumber(302));
            Assert.AreEqual("叁仟零贰", IntegerUtility.ToUppercaseChineseNumber(3002));
            Assert.AreEqual("叁拾贰万", IntegerUtility.ToUppercaseChineseNumber(320000));
            Assert.AreEqual("叁佰零贰万", IntegerUtility.ToUppercaseChineseNumber(3020000));
            Assert.AreEqual("叁仟零贰万", IntegerUtility.ToUppercaseChineseNumber(30020000));

            Assert.AreEqual("叁佰万零叁拾贰", IntegerUtility.ToUppercaseChineseNumber(3000032));
            Assert.AreEqual("叁佰亿零叁拾贰", IntegerUtility.ToUppercaseChineseNumber(30000000032));

            Assert.AreEqual("叁仟零贰拾", IntegerUtility.ToUppercaseChineseNumber(3020));
            Assert.AreEqual("叁佰万叁仟零贰拾", IntegerUtility.ToUppercaseChineseNumber(3003020));
            Assert.AreEqual("叁佰亿叁仟零贰拾", IntegerUtility.ToUppercaseChineseNumber(30000003020));
        }

        [TestMethod()]
        public void GetIntegerPlacesTest() {
            Assert.AreEqual(1, (0).GetIntegerPlaces());
            Assert.AreEqual(1, (1).GetIntegerPlaces());
            Assert.AreEqual(1, (9).GetIntegerPlaces());
            Assert.AreEqual(2, (10).GetIntegerPlaces());
            Assert.AreEqual(2, (99).GetIntegerPlaces());
            Assert.AreEqual(3, (100).GetIntegerPlaces());
            Assert.AreEqual(3, (999).GetIntegerPlaces());
            Assert.AreEqual(4, (1000).GetIntegerPlaces());
            Assert.AreEqual(4, (9999).GetIntegerPlaces());
            Assert.AreEqual(5, (10000).GetIntegerPlaces());
            Assert.AreEqual(5, (99999).GetIntegerPlaces());
            Assert.AreEqual(6, (100000).GetIntegerPlaces());
            Assert.AreEqual(6, (999999).GetIntegerPlaces());
            Assert.AreEqual(7, (1000000).GetIntegerPlaces());

            Assert.AreEqual(9, (420000000).GetIntegerPlaces());
            Assert.AreEqual(10, (4200000000L).GetIntegerPlaces());
            Assert.AreEqual(11, (42000000001L).GetIntegerPlaces());
        }
    }
}
