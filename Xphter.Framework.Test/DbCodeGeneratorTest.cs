using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;
using Xphter.Framework.Data.SqlServer;

namespace Xphter.Framework.Test {

    /// <summary>
    ///This is a test class for DbCodeGeneratorTest and is intended
    ///to contain all DbCodeGeneratorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DbCodeGeneratorTest {


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

        private string m_dataSource = new SqlConnectionStringBuilder(Properties.Settings.Default.SqlConnectionString).DataSource;

        private string m_entityNamespaceName = "Xphter.Framework.Test.Model";

        private string m_accessorNamespaceName = "Xphter.Framework.Test.DAL";

        /// <summary>
        ///A test for GenerateEntityCode
        ///</summary>
        [TestMethod()]
        public void GenerateEntityCodeTest_View() {
            IDbViewEntity view = (from v in new DbDatabaseEntity(new DbSourceEntity(this.m_dataSource, null, new SqlServerDatabaseEntityProvider()), "Xphter.Framework.Test", new SqlServerDataEntityProvider()).Views
                                  where v.Name.Equals("viewGeneration0", StringComparison.OrdinalIgnoreCase)
                                  select v).FirstOrDefault();

            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            DbCodeGenerator target = new DbCodeGenerator(new SqlServerCodeProvider(), null, provider);
            CodeCompileUnit unit = target.GenerateCompileUnit();
            target.GenerateEntityCode(target.GenerateNamespace(unit, this.m_entityNamespaceName), view);

            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BlankLinesBetweenMembers = true;
            options.BracingStyle = "Block";
            options.ElseOnClosing = true;
            options.IndentString = "  ";
            options.VerbatimOrder = true;
            using(TextWriter writer = new StreamWriter(string.Format("{0}.{1}", target.GetEntityCodeFilename(view), provider.FileExtension), false, Encoding.UTF8)) {
                provider.GenerateCodeFromCompileUnit(unit, writer, options);
            }
        }

        /// <summary>
        ///A test for GenerateAccessorCode
        ///</summary>
        [TestMethod()]
        public void GenerateAccessorCodeTest_View() {
            IDbViewEntity view = (from v in new DbDatabaseEntity(new DbSourceEntity(this.m_dataSource, null, new SqlServerDatabaseEntityProvider()), "Xphter.Framework.Test", new SqlServerDataEntityProvider()).Views
                                  where v.Name.Equals("viewGeneration0", StringComparison.OrdinalIgnoreCase)
                                  select v).FirstOrDefault();

            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            DbCodeGenerator target = new DbCodeGenerator(new SqlServerCodeProvider(), null, provider);
            CodeCompileUnit unit = target.GenerateCompileUnit();
            target.GenerateAccessorCode(target.GenerateNamespace(unit, this.m_accessorNamespaceName), view, this.m_entityNamespaceName);

            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BlankLinesBetweenMembers = true;
            options.BracingStyle = "Block";
            options.ElseOnClosing = true;
            options.IndentString = "  ";
            options.VerbatimOrder = true;
            using(TextWriter writer = new StreamWriter(string.Format("{0}.{1}", target.GetAccessorCodeFilename(view), provider.FileExtension), false, Encoding.UTF8)) {
                provider.GenerateCodeFromCompileUnit(unit, writer, options);
            }
        }

        /// <summary>
        ///A test for GenerateEntityCode
        ///</summary>
        [TestMethod()]
        public void GenerateEntityCodeTest_Table() {
            IDbTableEntity table = (from t in new DbDatabaseEntity(new DbSourceEntity(this.m_dataSource, null, new SqlServerDatabaseEntityProvider()), "Xphter.Framework.Test", new SqlServerDataEntityProvider()).Tables
                                    where t.Name.Equals("tableGeneration0", StringComparison.OrdinalIgnoreCase)
                                    select t).FirstOrDefault();

            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            DbCodeGenerator target = new DbCodeGenerator(new SqlServerCodeProvider(), null, provider);
            CodeCompileUnit unit = target.GenerateCompileUnit();
            target.GenerateEntityCode(target.GenerateNamespace(unit, this.m_entityNamespaceName), table);

            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BlankLinesBetweenMembers = true;
            options.BracingStyle = "Block";
            options.ElseOnClosing = true;
            options.IndentString = "  ";
            options.VerbatimOrder = true;
            using(TextWriter writer = new StreamWriter(string.Format("{0}.{1}", target.GetEntityCodeFilename(table), provider.FileExtension), false, Encoding.UTF8)) {
                provider.GenerateCodeFromCompileUnit(unit, writer, options);
            }
        }

        /// <summary>
        ///A test for GenerateAccessorCode
        ///</summary>
        [TestMethod()]
        public void GenerateAccessorCodeTest_Table() {
            IDbTableEntity table = (from t in new DbDatabaseEntity(new DbSourceEntity(this.m_dataSource, null, new SqlServerDatabaseEntityProvider()), "Xphter.Framework.Test", new SqlServerDataEntityProvider()).Tables
                                    where t.Name.Equals("tableGeneration0", StringComparison.OrdinalIgnoreCase)
                                    select t).FirstOrDefault();

            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            DbCodeGenerator target = new DbCodeGenerator(new SqlServerCodeProvider(), null, provider);
            CodeCompileUnit unit = target.GenerateCompileUnit();
            target.GenerateAccessorCode(target.GenerateNamespace(unit, this.m_accessorNamespaceName), table, true, this.m_entityNamespaceName);

            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BlankLinesBetweenMembers = true;
            options.BracingStyle = "Block";
            options.ElseOnClosing = true;
            options.IndentString = "  ";
            options.VerbatimOrder = true;
            using(TextWriter writer = new StreamWriter(string.Format("{0}.{1}", target.GetAccessorCodeFilename(table), provider.FileExtension), false, Encoding.UTF8)) {
                provider.GenerateCodeFromCompileUnit(unit, writer, options);
            }
        }

        /// <summary>
        ///A test for GenerateCode
        ///</summary>
        [TestMethod()]
        public void GenerateCodeTest() {
            new DbCodeGenerator(new SqlServerCodeProvider(), null, CodeDomProvider.CreateProvider("CSharp")).GenerateCode(new DbDatabaseEntity(new DbSourceEntity(this.m_dataSource, null, new SqlServerDatabaseEntityProvider()), "Xphter.Framework.Test", new SqlServerDataEntityProvider()), true, this.m_entityNamespaceName, this.m_accessorNamespaceName, null);
            //new DbCodeGenerator(new SqlServerCodeSqlObjectProvider(), CodeDomProvider.CreateProvider("CSharp")).GenerateCode(new DbDatabaseEntity(new DbSourceEntity("www.0377auto.com,759", new DbCredential(false, "sa", "liupeng$168"), new SqlServerDatabaseEntityProvider()), "nyauto_DB#168_Data", new SqlServerDataEntityProvider()), true, "Auto.Model", "Auto.DAL", null);
        }
    }
}
