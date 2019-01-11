using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Xphter.Framework.Data;

namespace Xphter.Framework.Data.SqlServer {
    /// <summary>
    /// Provide databases for a SQL Server.
    /// </summary>
    public class SqlServerDatabaseEntityProvider : IDbDatabaseEntityProvider {
        #region IDbDatabaseEntityProvider Members

        /// <inheritdoc />
        public IEnumerable<IDbDatabaseEntity> GetDatabases(IDbSourceEntity source) {
            ISqlObject table = new SqlServerSource(new SqlServerSource("sys"), "databases");
            ISqlObject nameField = new SqlServerField(table, "name", null);
            ISqlObject ownerID = new SqlServerField(table, "owner_sid", null);
            ISqlSelectStatement statement = new SqlServerSelectStatement();
            statement.FromClause.Source = table;
            statement.WhereClause.Condition = ownerID.Equal(new SqlServerConstantExpressionFactory().Create(new byte[] { 0x01 })).Not();
            statement.SelectClause.AddExpressions(nameField);

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = source.Name;
            if(!(builder.IntegratedSecurity = source.DbCredential.IntegratedSecurity)) {
                builder.UserID = source.DbCredential.UserName;
                builder.Password = source.DbCredential.Password;
            }

            ICollection<IDbDatabaseEntity> databases = new List<IDbDatabaseEntity>();
            using(IDataReader reader = SqlHelper.ExecuteSelect(new SqlConnection(builder.ConnectionString), statement)) {
                while(reader.Read()) {
                    databases.Add(new DbDatabaseEntity(source, reader.GetString(statement.SelectClause.Expressions.IndexOf(nameField)), new SqlServerDataEntityProvider()));
                }
            }
            return databases;
        }

        #endregion
    }
}
