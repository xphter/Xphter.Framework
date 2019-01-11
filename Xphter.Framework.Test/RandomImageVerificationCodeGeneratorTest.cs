using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Collections;
using Xphter.Framework.Web;

namespace Xphter.Framework.Test {
    /// <summary>
    ///This is a test class for RandomImageVerificationCodeGeneratorTest and is intended
    ///to contain all RandomImageVerificationCodeGeneratorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RandomImageVerificationCodeGeneratorTest {


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
        ///A test for Generate
        ///</summary>
        [TestMethod()]
        public void GenerateTest_Empty() {
            RandomImageVerificationCodeOption option = new RandomImageVerificationCodeOption {
            };
            RandomImageVerificationCodeGenerator target = new RandomImageVerificationCodeGenerator(new RandomCharacterImageVerificationCodeValueProvider(new RandomCharacterVerificationCodeValueOption {
            }), option);
            Assert.IsNull(target.Generate());
        }

        /// <summary>
        ///A test for Generate
        ///</summary>
        [TestMethod()]
        public void GenerateTest_One() {
            RandomImageVerificationCodeOption option = new RandomImageVerificationCodeOption {
                BackgroundColors = new Color[] { Color.Black },
                ForegroundColors = new Color[] { Color.White },
                CanvasWidth = new Range(100, 1),
                CanvasHeight = new Range(40, 1),
                FontSize = new Range(20, 1),
                FontFamilies = new FontFamily[] { FontFamily.GenericSansSerif },
                FontStyles = new FontStyle[] { FontStyle.Underline },
                MaxRotationDegree = 45,
            };
            RandomImageVerificationCodeGenerator target = new RandomImageVerificationCodeGenerator(new RandomCharacterImageVerificationCodeValueProvider(new RandomCharacterVerificationCodeValueOption {
                CharactersCount = new Range(5, 1),
                Characters = new char[] { 'X' },
            }), option);
            target.Generate().Save(Guid.NewGuid() + ".png");
        }

        /// <summary>
        ///A test for Generate
        ///</summary>
        [TestMethod()]
        public void GenerateTest() {
            List<char> characters = new List<char>();
            for(char c = '0'; c <= '9'; c++) {
                characters.Add(c);
            }
            for(char c = 'a'; c <= 'z'; c++) {
                characters.Add(c);
            }
            for(char c = 'A'; c <= 'Z'; c++) {
                characters.Add(c);
            }
            characters.AddRange(new char[] {
                '#',
                '$',
                '%',
                '&',
                '*',
            });

            RandomImageVerificationCodeOption option = new RandomImageVerificationCodeOption {
                BackgroundColors = new Color[] { Color.White, Color.Gray, Color.LightGray },
                ForegroundColors = new Color[] { Color.Black, Color.Red, Color.Blue, Color.Yellow, Color.Orange },
                CanvasWidth = new Range(100, 50),
                CanvasHeight = new Range(40, 20),
                FontSize = new Range(15, 10),
                //FontFamilies = new InstalledFontCollection().Families,
                FontFamilies = new FontFamily[] { new FontFamily("Arial") },
                //FontStyles = new FontStyle[] { FontStyle.Regular, FontStyle.Bold, FontStyle.Italic, FontStyle.Underline },
                FontStyles = new FontStyle[] { FontStyle.Regular, FontStyle.Bold, FontStyle.Italic },
                MaxRotationDegree = 45,
            };
            RandomImageVerificationCodeGenerator target = new RandomImageVerificationCodeGenerator(new RandomCharacterImageVerificationCodeValueProvider(new RandomCharacterVerificationCodeValueOption {
                CharactersCount = new Range(4, 4),
                Characters = characters,
            }), option);
            string name = null;
            ImageVerificationCode code = null;
            for(int i = 0; i < 100; i++) {
                code = target.Generate();
                name = Guid.NewGuid().ToString();
                code.Save(name + ".png");
                File.WriteAllText(name + ".txt", code.Value, Encoding.UTF8);
            }
        }
    }
}
