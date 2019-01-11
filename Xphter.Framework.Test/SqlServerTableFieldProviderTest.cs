using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Data;
using Xphter.Framework.Data.SqlServer;

namespace Xphter.Framework.Test {


    /// <summary>
    ///This is a test class for SqlServerTableFieldProviderTest and is intended
    ///to contain all SqlServerTableFieldProviderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SqlServerTableFieldProviderTest {


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

        /// <summary>
        ///A test for GetFields
        ///</summary>
        [TestMethod()]
        public void GetFieldsTest() {
            IDbSourceEntity source = new DbSourceEntity(this.m_dataSource, null, new SqlServerDatabaseEntityProvider());
            IEnumerable<IDbDatabaseEntity> databases = source.Databases;

            IEnumerable<IDbTableFieldEntity> fields = from database in databases
                                                      where database.Name.Equals("Xphter.Framework.Test")
                                                      from table in database.Tables
                                                      where table.Name.Equals("table0", StringComparison.OrdinalIgnoreCase)
                                                      from field in table.TableFields
                                                      select field;

            IDbTableFieldEntity primaryKeyField = (from field in fields
                                                   where field.Name.Equals("PrimaryKeyField", StringComparison.OrdinalIgnoreCase)
                                                   select field).FirstOrDefault();
            Assert.IsTrue(primaryKeyField != null);
            Assert.IsTrue(primaryKeyField.Constraints.Count() == 1);
            Assert.IsTrue(primaryKeyField.Constraints.First().Type == DbConstraintType.PrimaryKey);
            Assert.AreEqual<string>("int", primaryKeyField.DatabaseType);
            Assert.AreEqual<Type>(typeof(int), primaryKeyField.Type);

            IDbTableFieldEntity identityField = (from field in fields
                                                 where field.Name.Equals("IdentityField", StringComparison.OrdinalIgnoreCase)
                                                 select field).FirstOrDefault();
            Assert.IsTrue(identityField != null);
            Assert.IsTrue(identityField.IsIdentity);

            IDbTableFieldEntity uniqueKeyField0 = (from field in fields
                                                   where field.Name.Equals("UniqueField0", StringComparison.OrdinalIgnoreCase)
                                                   select field).FirstOrDefault();
            Assert.IsTrue(uniqueKeyField0 != null);
            Assert.IsTrue(uniqueKeyField0.Constraints.Count() == 1);
            Assert.IsTrue(uniqueKeyField0.Constraints.First().Type == DbConstraintType.UniqueKey);

            IDbTableFieldEntity uniqueKeyField1 = (from field in fields
                                                   where field.Name.Equals("UniqueField1", StringComparison.OrdinalIgnoreCase)
                                                   select field).FirstOrDefault();
            Assert.IsTrue(uniqueKeyField1 != null);
            Assert.IsTrue(uniqueKeyField1.Constraints.Count() == 2);
            Assert.IsTrue(uniqueKeyField1.Constraints.ElementAt(0).Type == DbConstraintType.UniqueKey);
            Assert.IsTrue(uniqueKeyField1.Constraints.ElementAt(1).Type == DbConstraintType.UniqueKey);
            //Assert.IsTrue(uniqueKeyField1.Constraints[2].Type == DbConstraintType.UniqueKey);
            Assert.IsTrue(uniqueKeyField1.HasDefaultValue);

            IDbTableFieldEntity uniqueKeyField2 = (from field in fields
                                                   where field.Name.Equals("UniqueField2", StringComparison.OrdinalIgnoreCase)
                                                   select field).FirstOrDefault();
            Assert.IsTrue(uniqueKeyField2 != null);
            Assert.IsTrue(uniqueKeyField2.Constraints.Count() == 1);
            Assert.IsTrue(uniqueKeyField2.Constraints.First().Type == DbConstraintType.UniqueKey);

            IDbTableFieldEntity foreignKeyField0 = (from field in fields
                                                    where field.Name.Equals("ForeignKey0", StringComparison.OrdinalIgnoreCase)
                                                    select field).FirstOrDefault();
            Assert.IsTrue(foreignKeyField0 != null);
            Assert.IsTrue(foreignKeyField0.Constraints.Count() == 1);
            Assert.IsTrue(foreignKeyField0.Constraints.First().ReferenceTable != null);
            Assert.AreEqual<string>("table1", foreignKeyField0.Constraints.First().ReferenceTable.Name);
            Assert.IsTrue(foreignKeyField0.ReferencedField != null);
            Assert.AreEqual<string>("PrimaryKey1", foreignKeyField0.ReferencedField.Name);

            IDbTableFieldEntity foreignKeyField1 = (from field in fields
                                                    where field.Name.Equals("ForeignKey1", StringComparison.OrdinalIgnoreCase)
                                                    select field).FirstOrDefault();
            Assert.IsTrue(foreignKeyField1 != null);
            Assert.IsTrue(foreignKeyField1.Constraints.Count() == 1);
            Assert.IsTrue(foreignKeyField1.Constraints.First().ReferenceTable != null);
            Assert.AreEqual<string>("table1", foreignKeyField1.Constraints.First().ReferenceTable.Name);
            Assert.IsTrue(foreignKeyField1.ReferencedField != null);
            Assert.AreEqual<string>("PrimaryKey0", foreignKeyField1.ReferencedField.Name);

            IDbTableFieldEntity uniqueIndexField0 = (from field in fields
                                                     where field.Name.Equals("UniqueIndexField0", StringComparison.OrdinalIgnoreCase)
                                                     select field).FirstOrDefault();
            Assert.IsTrue(uniqueIndexField0 != null);
            Assert.IsTrue(uniqueIndexField0.Constraints.Count() == 1);
            Assert.IsTrue(uniqueIndexField0.Constraints.First().Type == DbConstraintType.UniqueKey);

            IDbTableFieldEntity uniqueIndexField1 = (from field in fields
                                                     where field.Name.Equals("UniqueIndexField1", StringComparison.OrdinalIgnoreCase)
                                                     select field).FirstOrDefault();
            Assert.IsTrue(uniqueIndexField1 != null);
            Assert.IsTrue(uniqueIndexField1.Constraints.Count() == 1);
            Assert.IsTrue(uniqueIndexField1.Constraints.First().Type == DbConstraintType.UniqueKey);

            IDbTableFieldEntity descriptionField = (from field in fields
                                                    where field.Name.Equals("DescriptionField", StringComparison.OrdinalIgnoreCase)
                                                    select field).FirstOrDefault();
            Assert.IsTrue(descriptionField != null);
            Assert.AreEqual<string>("Field Description", descriptionField.Description);
            Assert.AreEqual<string>("varchar", descriptionField.DatabaseType);
            Assert.AreEqual<Type>(typeof(string), descriptionField.Type);
        }
    }
}
