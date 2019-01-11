using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Drawing;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for WatermarkTest and is intended
    ///to contain all WatermarkTest Unit Tests
    ///</summary>
    [TestClass()]
    public class WatermarkTest {


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
        ///A test for Mark
        ///</summary>
        [TestMethod()]
        public void MarkTest_FilePath() {
            Properties.Resources.Logo.Save("Logo.png", System.Drawing.Imaging.ImageFormat.Png);
            using(Watermark watermark = new Watermark("Logo.png", 1, 150, 60)) {
                watermark.Margin = new Point(5, 5);
                watermark.Position = WatermarkPosition.Center;
                using(Image image = watermark.Mark(Properties.Resources.Car)) {
                    image.Save("MarkTest_FilePath.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }

        /// <summary>
        ///A test for Mark
        ///</summary>
        [TestMethod()]
        public void MarkTest_Image() {
            using(Watermark watermark = new Watermark(Properties.Resources.Logo, 0.5F, 150, 60)) {
                watermark.Margin = new Point(5, 5);
                watermark.Position = WatermarkPosition.RightBottom;
                using(Image image = watermark.Mark(Properties.Resources.Car)) {
                    image.Save("MarkTest_Image.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }

        /// <summary>
        ///A test for Mark
        ///</summary>
        [TestMethod()]
        public void MarkTest_Text() {
            using(Watermark watermark = new Watermark("杜彭 www.xphter.com", SystemFonts.DefaultFont, Color.White, 1)) {
                watermark.Margin = new Point(5, 5);
                watermark.Position = WatermarkPosition.LeftBottom;
                using(Image image = watermark.Mark(Properties.Resources.Car)) {
                    image.Save("MarkTest_Text.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }
    }
}
