using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;
using Xphter.Framework.Data.SqlServer;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for SqlServerFieldTest and is intended
    ///to contain all SqlServerFieldTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SqlServerFieldTest {


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
        ///A test for SqlServerField Constructor
        ///</summary>
        [TestMethod()]
        public void SqlServerFieldConstructorTest_FieldName() {
            ISqlObject source = new SqlServerSource("Table");
            ISqlObject field = new SqlServerField(source, "Field", null);
            Assert.AreEqual<string>(string.Format("{0}.[{1}]", source.Fullname, field.Name), field.Fullname);
            Assert.AreEqual<string>(string.Format("{0}.[{1}]", source.Fullname, field.Name), field.SqlString);
        }

        /// <summary>
        ///A test for SqlServerField Constructor
        ///</summary>
        [TestMethod()]
        public void SqlServerFieldConstructorTest_FieldAlias() {
            ISqlObject source = new SqlServerSource("Table");
            SqlAliasObject field = new SqlServerField(source, "Field", "ID");
            Assert.AreEqual<string>(field.Object.Fullname, field.Fullname);
            Assert.AreEqual<string>(string.Format("{0} AS [{1}]", field.Object.SqlString, field.Alias), field.SqlString);
        }

        /// <summary>
        ///A test for SqlServerField Constructor
        ///</summary>
        [TestMethod()]
        public void SqlServerFieldConstructorTest_Variable() {
            string variable = "@@error";
            ISqlObject field = new SqlServerField(variable, "Error");
            Assert.AreEqual<string>(string.Format("[{0}]", field.Name), field.Fullname);
            Assert.AreEqual<string>(string.Format("{0} AS [{1}]", variable, field.Name), field.SqlString);
        }

        /// <summary>
        ///A test for SqlServerField Constructor
        ///</summary>
        [TestMethod()]
        public void SqlServerFieldConstructorTest_Function() {
            ISqlFunction function = new SqlFunction("SUM");
            function.AddArgument(new SqlStringExpression("1"));
            function.AddArgument(new SqlStringExpression("2"));
            ISqlObject field = new SqlServerField(function, "Total");
            Assert.AreEqual<string>(string.Format("[{0}]", field.Name), field.Fullname);
            Assert.AreEqual<string>(string.Format("{0} AS [{1}]", function.SqlString, field.Name), field.SqlString);
        }
    }
}
