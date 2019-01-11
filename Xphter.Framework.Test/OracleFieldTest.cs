using Xphter.Framework.Data.Oracle;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Xphter.Framework.Data;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for OracleFieldTest and is intended
    ///to contain all OracleFieldTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OracleFieldTest {


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
        ///A test for OracleField Constructor
        ///</summary>
        [TestMethod()]
        public void OracleFieldConstructorTest_FieldName() {
            ISqlObject source = new OracleSource("Table");
            ISqlObject field = new OracleField(source, "Field");
            Assert.AreEqual<string>(string.Format("{0}.\"{1}\"", source.Fullname, field.Name.ToUpper()), field.Fullname);
            Assert.AreEqual<string>(string.Format("{0}.\"{1}\"", source.Fullname, field.Name.ToUpper()), field.SqlString);
        }

        /// <summary>
        ///A test for OracleField Constructor
        ///</summary>
        [TestMethod()]
        public void OracleFieldConstructorTest_FieldAlias() {
            ISqlObject source = new OracleSource("Table");
            SqlAliasObject field = new OracleField(source, "Field", "ID");
            Assert.AreEqual<string>(field.Object.Fullname, field.Fullname);
            Assert.AreEqual<string>(string.Format("{0} AS \"{1}\"", field.Object.SqlString, field.Alias.ToUpper()), field.SqlString);
        }

        /// <summary>
        ///A test for OracleField Constructor
        ///</summary>
        [TestMethod()]
        public void OracleFieldConstructorTest_Variable() {
            string variable = "@@error";
            ISqlObject field = new OracleField(variable, "Error");
            Assert.AreEqual<string>(string.Format("\"{0}\"", field.Name.ToUpper()), field.Fullname);
            Assert.AreEqual<string>(string.Format("{0} \"{1}\"", variable, field.Name.ToUpper()), field.SqlString);
        }

        /// <summary>
        ///A test for OracleField Constructor
        ///</summary>
        [TestMethod()]
        public void OracleFieldConstructorTest_Function() {
            ISqlFunction function = new SqlFunction("SUM");
            function.AddArgument(new SqlStringExpression("1"));
            function.AddArgument(new SqlStringExpression("2"));
            ISqlObject field = new OracleField(function, "Total");
            Assert.AreEqual<string>(string.Format("\"{0}\"", field.Name.ToUpper()), field.Fullname);
            Assert.AreEqual<string>(string.Format("{0} \"{1}\"", function.SqlString, field.Name.ToUpper()), field.SqlString);
        }
    }
}
