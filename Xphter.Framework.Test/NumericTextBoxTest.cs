using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.WinForm;
using System.Windows.Forms;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for NumericTextBoxTest and is intended
    ///to contain all NumericTextBoxTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NumericTextBoxTest {


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
        ///A test for Value
        ///</summary>
        [TestMethod()]
        public void Test() {
            Form form = new Form();
            form.Text = "Test NumericTextBox";
            form.StartPosition = FormStartPosition.CenterScreen;
            Label result = new Label {
                Left = 10,
                Top = 10,
            };
            form.Controls.Add(result);
            NumericTextBox target = new NumericTextBox {
                Left = 10,
                Top = 100,
                Minimum = -100000000,
                Maximum = 100000000,
                DecimalPlaces = 3,
            };
            target.ValueChanged += delegate {
                result.Text = target.Value.ToString();
            };
            target.Value = 1234;
            form.Controls.Add(target);
            form.Load += delegate {
                result.Text = target.Value.ToString();
            };
            Application.Run(form);
        }
    }
}
