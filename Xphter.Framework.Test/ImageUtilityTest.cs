using System;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Drawing;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for ImageUtilityTest and is intended
    ///to contain all ImageUtilityTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ImageUtilityTest {


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
        ///A test for Scale
        ///</summary>
        [TestMethod()]
        public void ScaleTest() {
            int width = 800, height = 600;
            using(Image image = Properties.Resources.Deramon.Scale(width, height)) {
                Assert.AreEqual<int>(width, image.Width);
                Assert.AreEqual<int>(height, image.Height);
            }
        }

        /// <summary>
        ///A test for Transparency
        ///</summary>
        [TestMethod()]
        public void TransparencyTest() {
            using(Image image = Properties.Resources.Car.Transparency(0.5F)) {
                image.Save("Car.png", ImageFormat.Png);
            }
        }

        /// <summary>
        ///A test for MakeTransparent
        ///</summary>
        [TestMethod()]
        public void MakeTransparentTest() {
            using(Image image = Properties.Resources.CityWeekly.MakeTransparent(Color.White, 40)) {
                image.Save("CityWeekly.png", ImageFormat.Png);
            }
        }

        /// <summary>
        ///A test for MakeBlankAndWhite
        ///</summary>
        [TestMethod()]
        public void MakeBlankAndWhiteTest() {
            using(Image image = Properties.Resources.Arrow.MakeBlankAndWhite()) {
                image.Save("Arrow.jpg", ImageFormat.Jpeg);
            }
        }
    }
}
