using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Xphter.Framework.Collections;

namespace Xphter.Framework.Data.SqlServer {
    /// <summary>
    /// Provide fields for a SQL Server table.
    /// </summary>
    internal class SqlServerTableFieldProvider : IDbTableFieldProvider {
        /// <summary>
        /// The fields cache which is a dictionary, the key is the table entity, the value is the field diectionary.
        /// </summary>
        private static IDictionary<IDbTableEntity, IDictionary<string, DbTableFieldEntity>> g_fieldsCache = new Dictionary<IDbTableEntity, IDictionary<string, DbTableFieldEntity>>();

        /// <summary>
        /// The fields cache which is a dictionary, the key is the field name, the value is whether this field is read-only.
        /// </summary>
        private IDictionary<string, bool> m_isReadOnlyCache = new Dictionary<string, bool>();

        /// <summary>
        /// Creates fields from the specified field items.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="items"></param>
        private void CreateFileds(IDbTableEntity table, IDictionary<string, DbConstraintEntity> constraints, IEnumerable<FieldItem> items) {
            DbTableFieldEntity field = null;
            DbConstraintEntity constraint = null;
            IDbTableEntity referencedTable = null;
            IDbTableFieldEntity referencedField = null;
            IDictionary<string, DbTableFieldEntity> fields = g_fieldsCache.ContainsKey(table) ?
                g_fieldsCache[table] :
                (g_fieldsCache[table] = new Dictionary<string, DbTableFieldEntity>());
            foreach(FieldItem item in items) {
                //determine referenced table and field
                referencedTable = string.IsNullOrWhiteSpace(item.ReferenceTable) ?
                    null :
                    (from t in table.Database.Tables
                     where t.Name.Equals(item.ReferenceTable, StringComparison.OrdinalIgnoreCase)
                     select t).FirstOrDefault();

                referencedField = string.IsNullOrWhiteSpace(item.ReferenceField) ?
                    null :
                    g_fieldsCache.ContainsKey(referencedTable) ?
                        g_fieldsCache[referencedTable][item.ReferenceField] :
                        (from f in referencedTable.TableFields
                         where f.Name.Equals(item.ReferenceField, StringComparison.OrdinalIgnoreCase)
                         select f).FirstOrDefault();

                //determine field constraint
                if(item.ConstraintType == DbConstraintType.None) {
                    constraint = null;
                } else {
                    if(constraints.ContainsKey(item.ConstraintName)) {
                        constraint = constraints[item.ConstraintName];
                    } else {
                        constraints[item.ConstraintName] = constraint = new DbConstraintEntity(item.ConstraintName, table, item.ConstraintType, referencedTable);
                    }
                }

                //check field name
                if(fields.ContainsKey(item.Name)) {
                    field = fields[item.Name];
                    if(field.ReferencedField == null) {
                        field.ReferencedField = referencedField;
                    }
                } else {
                    fields[item.Name] = field = new DbTableFieldEntity(item.Name,
                      item.Index,
                      SqlServerUtility.GetRuntimeType(item.Type),
                      SqlServerUtility.GetDbType(item.Type),
                      (int) SqlServerUtility.GetSqlDbType(item.Type),
                      item.Type,
                      item.MaxLength,
                      item.IsNullable,
                      table,
                      item.Description,
                      item.HasDefault,
                      item.IsIdentity,
                      this.m_isReadOnlyCache.ContainsKey(item.Name.ToLower()) ?
                        this.m_isReadOnlyCache[item.Name.ToLower()] :
                        false,
                      referencedField);
                }
                if(constraint != null) {
                    field.AddConstraints(constraint);
                    constraint.AddFields(field);
                }
            }
        }

        private IEnumerable<UniqueIndexColumnItem> GetUniqueIndexColumns(IDbTableEntity table) {
            //create SQL statement
            IConstantSqlExpressionsFactory constantFactory = new SqlServerConstantExpressionFactory();
            ISqlObject owner = new SqlServerSource("sys");
            ISqlFunction tableID = new SqlFunction("OBJECT_ID").AddArgument(constantFactory.Create(table.SchemaQualifiedName));
            ISqlObject indexexView = new SqlServerSource(owner, "indexes");
            ISqlObject indexColumnsView = new SqlServerSource(owner, "index_columns");
            ISqlObject columnsView = new SqlServerSource(owner, "columns");

            ISqlObject indexNameField = new SqlServerField(indexexView, "name", "IndexName");
            ISqlObject columnNameField = new SqlServerField(columnsView, "name", "ColumnName");

            ISqlSelectStatement statement = new SqlServerSelectStatement();
            statement.SelectClause.AddExpressions(indexNameField, columnNameField);
            statement.FromClause.SetSource(indexexView);
            statement.FromClause.InnerJoin(indexColumnsView, new SqlServerField(indexexView, "index_id", null).Equal(new SqlServerField(indexColumnsView, "index_id", null)).And(new SqlServerField(indexColumnsView, "object_id", null).Equal(tableID)).And(new SqlServerField(indexColumnsView, "is_included_column", null).Equal(constantFactory.Create(0))));
            statement.FromClause.InnerJoin(columnsView, new SqlServerField(indexColumnsView, "column_id", null).Equal(new SqlServerField(columnsView, "column_id", null)).And(new SqlServerField(columnsView, "object_id", null).Equal(tableID)));
            statement.WhereClause.Condition = new SqlServerField(indexexView, "object_id", null).Equal(tableID).And(new SqlServerField(indexexView, "is_unique", null).Equal(constantFactory.Create(1)).And(new SqlServerField(indexexView, "is_primary_key", null).Equal(constantFactory.Create(0)).And(new SqlServerField(indexexView, "is_unique_constraint", null).Equal(constantFactory.Create(0)).And(new SqlServerField(indexexView, "is_disabled", null).Equal(constantFactory.Create(0))))));

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = table.Database.Source.Name;
            builder.InitialCatalog = table.Database.Name;
            if(!(builder.IntegratedSecurity = table.Database.Source.DbCredential.IntegratedSecurity)) {
                builder.UserID = table.Database.Source.DbCredential.UserName;
                builder.Password = table.Database.Source.DbCredential.Password;
            }

            //read all fields data
            ICollection<UniqueIndexColumnItem> items = new List<UniqueIndexColumnItem>();
            using(IDbConnection connection = new SqlConnection(builder.ConnectionString)) {
                using(IDataReader reader = SqlHelper.ExecuteSelect(connection, statement)) {
                    ReadOnlyCollection<ISqlExpression> selectFields = statement.SelectClause.Expressions;
                    int indexNameIndex = selectFields.IndexOf(indexNameField);
                    int columnNameIndex = selectFields.IndexOf(columnNameField);

                    while(reader.Read()) {
                        items.Add(new UniqueIndexColumnItem {
                            IndexName = reader.GetString(indexNameIndex),
                            ColumnName = reader.GetString(columnNameIndex),
                        });
                    }
                }
            }

            return items;
        }

        #region IDbTableFieldProvider Members

        /// <inheritdoc />
        public IEnumerable<IDbTableFieldEntity> GetFields(IDbTableEntity table) {
            //create SQL statement
            IConstantSqlExpressionsFactory constantFactory = new SqlServerConstantExpressionFactory();
            ISqlObject owner = new SqlServerSource("INFORMATION_SCHEMA");
            ISqlExpression tableName = constantFactory.Create(table.Name);
            ISqlExpression tableSchema = constantFactory.Create(table.Schema);
            ISqlFunction tableID = new SqlFunction("OBJECT_ID").AddArgument(constantFactory.Create(table.SchemaQualifiedName));
            ISqlObject columnsView = new SqlServerSource(owner, "COLUMNS");
            ISqlObject extendedPropertiesView = new SqlServerSource(new SqlServerSource("sys"), "extended_properties");
            ISqlObject keyColumnUsageView = new SqlServerSource(owner, "KEY_COLUMN_USAGE");
            ISqlObject tableConstraintsView = new SqlServerSource(owner, "TABLE_CONSTRAINTS");
            ISqlObject referentialConstraintsView = new SqlServerSource(owner, "REFERENTIAL_CONSTRAINTS");
            ISqlObject keyColumnUsageView2 = new SqlServerSource(owner, "KEY_COLUMN_USAGE", "kcu");

            ISqlObject nameField = new SqlServerField(columnsView, "COLUMN_NAME", "Name");
            ISqlObject indexField = new SqlServerField(columnsView, "ORDINAL_POSITION", "Index");
            ISqlObject typeField = new SqlServerField(columnsView, "DATA_TYPE", "TypeName");
            ISqlObject descriptionField = new SqlServerField(extendedPropertiesView, "value", "Description");
            ISqlObject maxLengthField = new SqlServerField(columnsView, "CHARACTER_MAXIMUM_LENGTH", "MaxLength");
            ISqlObject defaultField = new SqlServerField(columnsView, "COLUMN_DEFAULT", "DefaultExpression");
            ISqlObject isNullableField = new SqlServerField(columnsView, "IS_NULLABLE", "IsNullable");
            ISqlObject isIdentityField = new SqlServerField(new SqlFunction("COLUMNPROPERTY").AddArgument(tableID).AddArgument(nameField, false).AddArgument(constantFactory.Create("IsIdentity")), "IsIdentity");
            ISqlObject constraintNameField = new SqlServerField(tableConstraintsView, "CONSTRAINT_NAME", "ConstraintName");
            ISqlObject constraintTypeField = new SqlServerField(tableConstraintsView, "CONSTRAINT_TYPE", "ConstraintType");
            ISqlObject referenceTableField = new SqlServerField(keyColumnUsageView2, "TABLE_NAME", "ReferenceTable");
            ISqlObject referenceFieldField = new SqlServerField(keyColumnUsageView2, "COLUMN_NAME", "ReferenceField");

            ISqlSelectStatement statement = new SqlServerSelectStatement();
            statement.SelectClause.AddExpressions(nameField, indexField, typeField, descriptionField, maxLengthField, defaultField, isNullableField, isIdentityField, constraintNameField, constraintTypeField, referenceTableField, referenceFieldField);
            statement.FromClause.SetSource(columnsView);
            statement.FromClause.LeftOuterJoin(extendedPropertiesView, new SqlServerField(extendedPropertiesView, "class", null).Equal(constantFactory.Create(1)).And(new SqlServerField(extendedPropertiesView, "major_id", null).Equal(tableID)).And(new SqlServerField(extendedPropertiesView, "minor_id", null).Equal(new SqlFunction("COLUMNPROPERTY").AddArgument(tableID).AddArgument(nameField, false).AddArgument(constantFactory.Create("ColumnId")))));
            statement.FromClause.LeftOuterJoin(keyColumnUsageView, new SqlServerField(columnsView, "COLUMN_NAME", null).Equal(new SqlServerField(keyColumnUsageView, "COLUMN_NAME", null)).And(new SqlServerField(keyColumnUsageView, "TABLE_NAME", null).Equal(tableName)).And(new SqlServerField(keyColumnUsageView, "TABLE_SCHEMA", null).Equal(tableSchema)));
            statement.FromClause.LeftOuterJoin(tableConstraintsView, new SqlServerField(keyColumnUsageView, "CONSTRAINT_NAME", null).Equal(new SqlServerField(tableConstraintsView, "CONSTRAINT_NAME", null)).And(new SqlServerField(tableConstraintsView, "TABLE_NAME", null).Equal(tableName)).And(new SqlServerField(tableConstraintsView, "TABLE_SCHEMA", null).Equal(tableSchema)));
            statement.FromClause.LeftOuterJoin(referentialConstraintsView, new SqlServerField(tableConstraintsView, "CONSTRAINT_NAME", null).Equal(new SqlServerField(referentialConstraintsView, "CONSTRAINT_NAME", null)));
            statement.FromClause.LeftOuterJoin(keyColumnUsageView2, new SqlServerField(referentialConstraintsView, "UNIQUE_CONSTRAINT_NAME", null).Equal(new SqlServerField(keyColumnUsageView2, "CONSTRAINT_NAME", null)).And(new SqlServerField(keyColumnUsageView, "ORDINAL_POSITION", null).Equal(new SqlServerField(keyColumnUsageView2, "ORDINAL_POSITION", null))));
            statement.WhereClause.Condition = new SqlServerField(columnsView, "TABLE_NAME", null).Equal(tableName).And(new SqlServerField(columnsView, "TABLE_SCHEMA", null).Equal(tableSchema));
            statement.OrderClause.AddExpression(indexField, SqlOrder.Asc);

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = table.Database.Source.Name;
            builder.InitialCatalog = table.Database.Name;
            if(!(builder.IntegratedSecurity = table.Database.Source.DbCredential.IntegratedSecurity)) {
                builder.UserID = table.Database.Source.DbCredential.UserName;
                builder.Password = table.Database.Source.DbCredential.Password;
            }

            //read all fields data            
            ICollection<FieldItem> items = new List<FieldItem>();
            IList<ISqlExpression> selectFields = new List<ISqlExpression>(statement.SelectClause.Expressions);

            using(IDbConnection connection = new SqlConnection(builder.ConnectionString)) {
                using(IDataReader reader = SqlHelper.ExecuteSelect(connection, statement)) {
                    int nameIndex = selectFields.IndexOf(nameField);
                    int indexIndex = selectFields.IndexOf(indexField);
                    int typeIndex = selectFields.IndexOf(typeField);
                    int descriptionIndex = selectFields.IndexOf(descriptionField);
                    int maxLengthIndex = selectFields.IndexOf(maxLengthField);
                    int defaultIndex = selectFields.IndexOf(defaultField);
                    int isNullableIndex = selectFields.IndexOf(isNullableField);
                    int isIdentityIndex = selectFields.IndexOf(isIdentityField);
                    int constraintNameIndex = selectFields.IndexOf(constraintNameField);
                    int constraintTypeIndex = selectFields.IndexOf(constraintTypeField);
                    int referenceTableIndex = selectFields.IndexOf(referenceTableField);
                    int referenceFieldIndex = selectFields.IndexOf(referenceFieldField);

                    while(reader.Read()) {
                        items.Add(new FieldItem {
                            Name = reader.GetString(nameIndex),
                            Index = reader.GetInt32(indexIndex),
                            Type = reader.GetString(typeIndex),
                            Description = reader.IsDBNull(descriptionIndex) ? null : reader.GetString(descriptionIndex),
                            MaxLength = reader.IsDBNull(maxLengthIndex) ? 0 : reader.GetInt32(maxLengthIndex),
                            HasDefault = !reader.IsDBNull(defaultIndex),
                            IsNullable = reader.GetString(isNullableIndex).Equals("YES", StringComparison.OrdinalIgnoreCase),
                            IsIdentity = reader.GetInt32(isIdentityIndex) == 1,
                            ConstraintName = reader.IsDBNull(constraintNameIndex) ? null : reader.GetString(constraintNameIndex),
                            ConstraintTypeName = reader.IsDBNull(constraintTypeIndex) ? null : reader.GetString(constraintTypeIndex),
                            ReferenceTable = reader.IsDBNull(referenceTableIndex) ? null : reader.GetString(referenceTableIndex),
                            ReferenceField = reader.IsDBNull(referenceFieldIndex) ? null : reader.GetString(referenceFieldIndex),
                        });
                    }
                }
            }

            //inject unique index data
            FieldItem field = null;
            foreach(UniqueIndexColumnItem item in this.GetUniqueIndexColumns(table)) {
                field = items.Where((obj) => string.Equals(obj.Name, item.ColumnName, StringComparison.OrdinalIgnoreCase)).First();
                if(field.ConstraintName == null) {
                    field.ConstraintName = item.IndexName;
                    field.ConstraintTypeName = "UNIQUE";
                } else {
                    field = items.AddItem((FieldItem) field.Clone());
                    field.ConstraintName = item.IndexName;
                    field.ConstraintTypeName = "UNIQUE";
                }

            }

            //get read-only data
            ISqlObject tableSource = new SqlServerSource(new SqlServerSource(table.Schema), table.Name);
            statement = new SqlServerSelectStatement();
            statement.SelectClause.AddExpressions(new SqlAllField(tableSource));
            statement.FromClause.Source = tableSource;
            using(IDbConnection connection = new SqlConnection(builder.ConnectionString)) {
                using(DataTable schema = SqlHelper.ExecuteSchema(connection, statement)) {
                    using(DataColumn columnNameColumn = schema.Columns["ColumnName"]) {
                        using(DataColumn isReadOnlyColumn = schema.Columns["IsReadOnly"]) {
                            foreach(DataRow row in schema.Rows) {
                                if(row.IsNull(columnNameColumn)) {
                                    continue;
                                }

                                this.m_isReadOnlyCache[((string) row[columnNameColumn]).ToLower()] = (bool) row[isReadOnlyColumn];
                            }
                        }
                    }
                }
            }

            //analyze fields
            IDictionary<string, DbConstraintEntity> constraints = new Dictionary<string, DbConstraintEntity>();
            this.CreateFileds(table, constraints, items.Where((obj) => string.IsNullOrWhiteSpace(obj.ReferenceField)));
            this.CreateFileds(table, constraints, items.Where((obj) => !string.IsNullOrWhiteSpace(obj.ReferenceField)));

            return new List<IDbTableFieldEntity>(g_fieldsCache[table].Values);
        }

        #endregion

        /// <summary>
        /// Internal class: a field item.
        /// </summary>
        private class FieldItem : ICloneable {
            public string Name = null;
            public int Index = 0;
            public string Type = null;
            public string Description = null;
            public int MaxLength = 0;
            public bool HasDefault = false;
            public bool IsNullable = false;
            public bool IsIdentity = false;
            public string ConstraintName = null;
            public string ConstraintTypeName = null;
            public string ReferenceTable = null;
            public string ReferenceField = null;

            private DbConstraintType? m_constraintType;
            public DbConstraintType ConstraintType {
                get {
                    if(!this.m_constraintType.HasValue) {
                        if(string.IsNullOrWhiteSpace(this.ConstraintName) || string.IsNullOrWhiteSpace(this.ConstraintTypeName)) {
                            this.m_constraintType = DbConstraintType.None;
                        } else {
                            switch(this.ConstraintTypeName.ToUpper()) {
                                case "PRIMARY KEY":
                                    this.m_constraintType = DbConstraintType.PrimaryKey;
                                    break;
                                case "UNIQUE":
                                    this.m_constraintType = DbConstraintType.UniqueKey;
                                    break;
                                case "FOREIGN KEY":
                                    this.m_constraintType = DbConstraintType.ForeignKey;
                                    break;
                                default:
                                    this.m_constraintType = DbConstraintType.None;
                                    break;
                            }
                        }
                    }

                    return this.m_constraintType.Value;
                }
            }

            public override string ToString() {
                return this.Name;
            }

            #region ICloneable Members

            public object Clone() {
                FieldItem obj = (FieldItem) this.MemberwiseClone();
                obj.m_constraintType = null;
                return obj;
            }

            #endregion
        }

        /// <summary>
        /// Internal class: a unique index column item.
        /// </summary>
        private class UniqueIndexColumnItem {
            public string IndexName;
            public string ColumnName;
        }
    }
}
