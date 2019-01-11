using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.SqlServer {
    /// <summary>
    /// Provide table and view data for a SQL Server database.
    /// </summary>
    public class SqlServerDataEntityProvider : IDbDataEntityProvider {
        #region IDbDataEntityProvider Members

        /// <inheritdoc />
        public IEnumerable<IDbTableEntity> GetTables(IDbDatabaseEntity database) {
            ISqlObject table = new SqlServerSource(new SqlServerSource("INFORMATION_SCHEMA"), "TABLES");
            ISqlObject nameField = new SqlServerField(table, "TABLE_NAME", null);
            ISqlObject schemaField = new SqlServerField(table, "TABLE_SCHEMA", null);
            ISqlObject typeField = new SqlServerField(table, "TABLE_TYPE", null);
            ISqlSelectStatement statement = new SqlServerSelectStatement();
            statement.FromClause.Source = table;
            statement.WhereClause.Condition = typeField.Equal(new SqlServerConstantExpressionFactory().Create("BASE TABLE"));
            statement.SelectClause.AddExpressions(nameField);
            statement.SelectClause.AddExpressions(schemaField);

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = database.Source.Name;
            builder.InitialCatalog = database.Name;
            if(!(builder.IntegratedSecurity = database.Source.DbCredential.IntegratedSecurity)) {
                builder.UserID = database.Source.DbCredential.UserName;
                builder.Password = database.Source.DbCredential.Password;
            }

            ICollection<IDbTableEntity> tables = new List<IDbTableEntity>();
            using(IDataReader reader = SqlHelper.ExecuteSelect(new SqlConnection(builder.ConnectionString), statement)) {
                while(reader.Read()) {
                    tables.Add(new SqlServerTableEntity(database, reader.GetString(0), reader.GetString(1), new SqlServerTableFieldProvider()));
                }
            }
            return tables;
        }

        /// <inheritdoc />
        public IEnumerable<IDbViewEntity> GetViews(IDbDatabaseEntity database) {
            ISqlObject table = new SqlServerSource(new SqlServerSource("INFORMATION_SCHEMA"), "VIEWS");
            ISqlObject nameField = new SqlServerField(table, "TABLE_NAME", null);
            ISqlObject schemaField = new SqlServerField(table, "TABLE_SCHEMA", null);
            ISqlSelectStatement statement = new SqlServerSelectStatement();
            statement.FromClause.Source = table;
            statement.SelectClause.AddExpressions(nameField);
            statement.SelectClause.AddExpressions(schemaField);

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = database.Source.Name;
            builder.InitialCatalog = database.Name;
            if(!(builder.IntegratedSecurity = database.Source.DbCredential.IntegratedSecurity)) {
                builder.UserID = database.Source.DbCredential.UserName;
                builder.Password = database.Source.DbCredential.Password;
            }

            ICollection<IDbViewEntity> views = new List<IDbViewEntity>();
            using(IDataReader reader = SqlHelper.ExecuteSelect(new SqlConnection(builder.ConnectionString), statement)) {
                while(reader.Read()) {
                    views.Add(new SqlServerViewEntity(database, reader.GetString(0), reader.GetString(1), new SqlServerViewFieldProvider()));
                }
            }
            return views;
        }

        #endregion
    }
}
