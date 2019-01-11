using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.SqlServer {
    /// <summary>
    /// Provide fields for a SQL Server table.
    /// </summary>
    internal class SqlServerViewFieldProvider : IDbViewFieldProvider {
        #region IDbViewFieldProvider Members

        /// <inheritdoc />
        public IEnumerable<IDbViewFieldEntity> GetFields(IDbViewEntity view) {
            ISqlObject owner = new SqlServerSource("INFORMATION_SCHEMA");
            ISqlExpression tableName = new SqlServerConstantExpressionFactory().Create(view.Name);
            ISqlExpression tableSchema = new SqlServerConstantExpressionFactory().Create(view.Schema);
            ISqlObject columnsView = new SqlServerSource(owner, "COLUMNS");

            ISqlObject nameField = new SqlServerField(columnsView, "COLUMN_NAME", "Name");
            ISqlObject indexField = new SqlServerField(columnsView, "ORDINAL_POSITION", "Index");
            ISqlObject typeField = new SqlServerField(columnsView, "DATA_TYPE", "TypeName");
            ISqlObject maxLengthField = new SqlServerField(columnsView, "CHARACTER_MAXIMUM_LENGTH", "MaxLength");
            ISqlObject isNullableField = new SqlServerField(columnsView, "IS_NULLABLE", "IsNullable");

            ISqlSelectStatement statement = new SqlServerSelectStatement();
            statement.SelectClause.AddExpressions(nameField, indexField, typeField, maxLengthField, isNullableField);
            statement.FromClause.SetSource(columnsView);
            statement.WhereClause.Condition = new SqlServerField(columnsView, "TABLE_NAME", null).Equal(tableName).And(new SqlServerField(columnsView, "TABLE_SCHEMA", null).Equal(tableSchema));
            statement.OrderClause.AddExpression(indexField, SqlOrder.Asc);

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = view.Database.Source.Name;
            builder.InitialCatalog = view.Database.Name;
            if(!(builder.IntegratedSecurity = view.Database.Source.DbCredential.IntegratedSecurity)) {
                builder.UserID = view.Database.Source.DbCredential.UserName;
                builder.Password = view.Database.Source.DbCredential.Password;
            }

            ICollection<IDbViewFieldEntity> fields = new List<IDbViewFieldEntity>();
            IList<ISqlExpression> selectFields = new List<ISqlExpression>(statement.SelectClause.Expressions);
            using(IDataReader reader = SqlHelper.ExecuteSelect(new SqlConnection(builder.ConnectionString), statement)) {
                while(reader.Read()) {
                    fields.Add(new DbViewFieldEntity(reader.GetString(selectFields.IndexOf(nameField)),
                      reader.GetInt32(selectFields.IndexOf(indexField)),
                      SqlServerUtility.GetRuntimeType(reader.GetString(selectFields.IndexOf(typeField))),
                      SqlServerUtility.GetDbType(reader.GetString(selectFields.IndexOf(typeField))),
                      (int) SqlServerUtility.GetSqlDbType(reader.GetString(selectFields.IndexOf(typeField))),
                      reader.GetString(selectFields.IndexOf(typeField)),
                      reader.IsDBNull(selectFields.IndexOf(maxLengthField)) ? 0 : reader.GetInt32(selectFields.IndexOf(maxLengthField)),
                      reader.GetString(selectFields.IndexOf(isNullableField)).Equals("YES", StringComparison.OrdinalIgnoreCase),
                      view,
                      null,
                      null));
                }
            }

            return fields;
        }

        #endregion
    }
}
