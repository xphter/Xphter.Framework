using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Xphter.Framework.IO;

namespace Xphter.Framework.Data.SqlServer {
    /// <summary>
    /// Provide a utility for SQL Server.
    /// </summary>
    public static class SqlServerUtility {
        /// <summary>
        /// Initialize SqlServerUtility class.
        /// </summary>
        static SqlServerUtility() {
            //intialize type mapping
            g_dataTypeMapping = new Dictionary<string, Type> {
                {"bit", typeof(bool)},
                {"tinyint", typeof(byte)},
                {"smallint", typeof(short)},
                {"int", typeof(int)},
                {"bigint", typeof(long)},
                {"numeric", typeof(decimal)},
                {"decimal", typeof(decimal)},
                {"smallmoney", typeof(decimal)},
                {"money", typeof(decimal)},
                {"float", typeof(double)},
                {"real", typeof(float)},
                {"smalldatetime", typeof(DateTime)},
                {"datetime", typeof(DateTime)},
                {"datetime2", typeof(DateTime)},
                {"date", typeof(DateTime)},
                {"time", typeof(TimeSpan)},
                {"datetimeoffset", typeof(DateTimeOffset)},
                {"char", typeof(string)},
                {"varchar", typeof(string)},
                {"text", typeof(string)},
                {"nchar", typeof(string)},
                {"nvarchar", typeof(string)},
                {"ntext", typeof(string)},
                {"binary", typeof(byte[])},
                {"varbinary", typeof(byte[])},
                {"image", typeof(byte[])},
                {"uniqueidentifier", typeof(Guid)},
                {"timestamp", typeof(byte[])},
                {"rowversion", typeof(byte[])},
            };
            g_dbTypeMapping = new Dictionary<string, DbType> {
                {"bit", DbType.Boolean},
                {"tinyint", DbType.Byte},
                {"smallint", DbType.Int16},
                {"int", DbType.Int32},
                {"bigint", DbType.Int64},
                {"numeric", DbType.Decimal},
                {"decimal", DbType.Decimal},
                {"smallmoney", DbType.Decimal},
                {"money", DbType.Decimal},
                {"float", DbType.Double},
                {"real", DbType.Single},
                {"smalldatetime", DbType.DateTime},
                {"datetime", DbType.DateTime},
                {"datetime2", DbType.DateTime2},
                {"date", DbType.Date},
                {"time", DbType.Time},
                {"datetimeoffset", DbType.DateTimeOffset},
                {"char", DbType.AnsiStringFixedLength},
                {"varchar", DbType.AnsiString},
                {"text", DbType.AnsiString},
                {"nchar", DbType.StringFixedLength},
                {"nvarchar", DbType.String},
                {"ntext", DbType.String},
                {"binary", DbType.Binary},
                {"varbinary", DbType.Binary},
                {"image", DbType.Binary},
                {"uniqueidentifier", DbType.Guid},
                {"timestamp", DbType.Binary},
                {"rowversion", DbType.Binary},
            };
            g_sqlDbTypeMapping = new Dictionary<string, SqlDbType> {
                {"bit", SqlDbType.Bit},
                {"tinyint", SqlDbType.TinyInt},
                {"smallint", SqlDbType.SmallInt},
                {"int", SqlDbType.Int},
                {"bigint", SqlDbType.BigInt},
                {"numeric", SqlDbType.Decimal},
                {"decimal", SqlDbType.Decimal},
                {"smallmoney", SqlDbType.SmallMoney},
                {"money", SqlDbType.Money},
                {"float", SqlDbType.Float},
                {"real", SqlDbType.Real},
                {"smalldatetime", SqlDbType.DateTime},
                {"datetime", SqlDbType.DateTime},
                {"datetime2", SqlDbType.DateTime2},
                {"date", SqlDbType.Date},
                {"time", SqlDbType.Time},
                {"datetimeoffset", SqlDbType.DateTimeOffset},
                {"char", SqlDbType.Char},
                {"varchar", SqlDbType.VarChar},
                {"text", SqlDbType.Text},
                {"nchar", SqlDbType.NChar},
                {"nvarchar", SqlDbType.NVarChar},
                {"ntext", SqlDbType.NText},
                {"binary", SqlDbType.Binary},
                {"varbinary", SqlDbType.VarBinary},
                {"image", SqlDbType.Binary},
                {"uniqueidentifier", SqlDbType.UniqueIdentifier},
                {"timestamp", SqlDbType.Timestamp},
                {"rowversion", SqlDbType.Timestamp},
            };
        }

        /// <summary>
        /// A mapping from database type name to .Net runtime type.
        /// </summary>
        private static readonly IDictionary<string, Type> g_dataTypeMapping;

        /// <summary>
        /// A mapping from database type name to System.Data.DbType.
        /// </summary>
        private static readonly IDictionary<string, DbType> g_dbTypeMapping;

        /// <summary>
        /// A mapping from database type name to System.Data.SqlDbType.
        /// </summary>
        private static readonly IDictionary<string, SqlDbType> g_sqlDbTypeMapping;

        /// <summary>
        /// Create a SqlParameter.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="type">Parameter SqlDbType.</param>
        /// <param name="direction">Parameter direction.</param>
        /// <param name="value">Parameter value.</param>
        /// <returns>The created parameter.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        private static SqlParameter CreateParameterInternal(string name, SqlDbType? type, ParameterDirection? direction, object value) {
            if(string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("Parameter name is null or empty.", "name");
            }

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            if(type.HasValue) {
                parameter.SqlDbType = type.Value;
            }
            if(direction.HasValue) {
                parameter.Direction = direction.Value;
            }
            return parameter;
        }

        /// <summary>
        /// Creates a SqlCommand object.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="isolationLevel"></param>
        /// <param name="sqlStatement"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static SqlCommand CreateCommand(string connectionString, IsolationLevel? isolationLevel, string sqlStatement, IEnumerable<SqlParameter> parameters) {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlTransaction transaction = null;
            if(isolationLevel.HasValue) {
                transaction = connection.BeginTransaction(isolationLevel.Value);
            }

            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sqlStatement;
            if(parameters != null) {
                foreach(SqlParameter item in parameters) {
                    command.Parameters.Add(item);
                }
            }
            if(transaction != null) {
                command.Transaction = transaction;
            }

            return command;
        }

        /// <summary>
        /// Dispose resources of the specified SqlCommand object.
        /// </summary>
        /// <param name="command"></param>
        private static void CleanCommand(SqlCommand command) {
            if(command == null) {
                return;
            }

            command.Dispose();

            if(command.Transaction != null) {
                command.Transaction.Dispose();
            }

            if(command.Connection != null) {
                command.Connection.Dispose();
            }
        }

        /// <summary>
        /// Get corresponding .net type of the speicifed database type name.
        /// </summary>
        /// <param name="typeName">Database type name.</param>
        /// <returns>Corresponding .net type of <paramref name="typeName"/>.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="typeName"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="typeName"/> is a invalid database type name.</exception>
        public static Type GetRuntimeType(string typeName) {
            if(string.IsNullOrWhiteSpace(typeName)) {
                throw new ArgumentException("Database type name is null or empty.", "dbType");
            }
            if(!g_dataTypeMapping.ContainsKey(typeName = typeName.ToLower().Trim())) {
                throw new ArgumentException(string.Format("Database type name \"{0}\" is not supprted.", typeName), "dbType");
            }

            return g_dataTypeMapping[typeName];
        }

        /// <summary>
        /// Get corresponding System.Data.DbType of the speicifed database type name.
        /// </summary>
        /// <param name="typeName">Database type name.</param>
        /// <returns>Corresponding System.Data.DbType of <paramref name="typeName"/>.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="typeName"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="typeName"/> is a invalid database type name.</exception>
        public static DbType GetDbType(string typeName) {
            if(string.IsNullOrWhiteSpace(typeName)) {
                throw new ArgumentException("Database type name is null or empty.", "dbType");
            }
            if(!g_dbTypeMapping.ContainsKey(typeName = typeName.ToLower().Trim())) {
                throw new ArgumentException(string.Format("Database type name \"{0}\" is not supprted.", typeName), "dbType");
            }

            return g_dbTypeMapping[typeName];
        }

        /// <summary>
        /// Gets corresponding System.Data.SqlDbType of the speicifed database type name.
        /// </summary>
        /// <param name="typeName">Database type name.</param>
        /// <returns>Corresponding System.Data.SqlDbType of <paramref name="typeName"/>.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="typeName"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="typeName"/> is a invalid database type name.</exception>
        public static SqlDbType GetSqlDbType(string typeName) {
            if(string.IsNullOrWhiteSpace(typeName)) {
                throw new ArgumentException("Database type name is null or empty.", "dbType");
            }
            if(!g_dbTypeMapping.ContainsKey(typeName = typeName.ToLower().Trim())) {
                throw new ArgumentException(string.Format("Database type name \"{0}\" is not supprted.", typeName), "dbType");
            }

            return g_sqlDbTypeMapping[typeName];
        }

        /// <summary>
        /// Creates a SqlParameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SqlParameter CreateParameter(string name, object value) {
            return CreateParameterInternal(name, null, null, value);
        }

        /// <summary>
        /// Creates a SqlParameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SqlParameter CreateParameter(string name, SqlDbType type, object value) {
            return CreateParameterInternal(name, type, null, value);
        }

        /// <summary>
        /// Creates a SqlParameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SqlParameter CreateParameter(string name, SqlDbType type, ParameterDirection direction, object value) {
            return CreateParameterInternal(name, type, direction, value);
        }

        /// <summary>
        /// Creates the SQL expression of a the SqlParameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ISqlExpression CreateParameterExpression(string name, object value) {
            return SqlStringExpression.FromParameter(CreateParameterInternal(name, null, null, value));
        }

        /// <summary>
        /// Creates the SQL expression of a the SqlParameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ISqlExpression CreateParameterExpression(string name, SqlDbType type, object value) {
            return SqlStringExpression.FromParameter(CreateParameterInternal(name, type, null, value));
        }

        /// <summary>
        /// Creates the SQL expression of a the SqlParameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ISqlExpression CreateParameterExpression(string name, SqlDbType type, ParameterDirection direction, object value) {
            return SqlStringExpression.FromParameter(CreateParameterInternal(name, type, direction, value));
        }

        /// <summary>
        /// Encoding the specified string used in SQL Server.
        /// </summary>
        /// <param name="s">A string.</param>
        /// <returns>The string after encoding.</returns>
        public static string EncodeString(string s) {
            if(string.IsNullOrWhiteSpace(s)) {
                return s;
            }

            return s.Replace("'", "''");
        }

        /// <summary>
        /// Encoding the specified string used in SQL LIKE operator.
        /// </summary>
        /// <param name="pattern">The query pattern.</param>
        /// <returns>The string after encoding.</returns>
        public static string EncodeLikeString(string pattern) {
            if(string.IsNullOrWhiteSpace(pattern)) {
                return pattern;
            }

            StringBuilder result = new StringBuilder(pattern.Length);
            foreach(char c in pattern) {
                switch(c) {
                    case '%':
                    case '_':
                    case '[':
                    case '^':
                        result.AppendFormat("[{0}]", c);
                        break;
                    default:
                        result.Append(c);
                        break;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Encoding the specified string used in SQL Fulltext search operation.
        /// </summary>
        /// <param name="pattern">The query pattern.</param>
        /// <returns>The string after encoding.</returns>
        public static string EncodeFulltextString(string pattern) {
            if(string.IsNullOrWhiteSpace(pattern)) {
                return pattern;
            }

            return pattern.Replace("\"", "\"\"");
        }

        /// <summary>
        /// Gets the specified string as a SQL LIKE begin string.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string GetBeginLikeString(string pattern) {
            if(string.IsNullOrEmpty(pattern)) {
                return pattern;
            }

            return string.Format("{0}%", EncodeLikeString(pattern));
        }

        /// <summary>
        /// Gets the specified string as a SQL LIKE end string.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string GetEndLikeString(string pattern) {
            if(string.IsNullOrEmpty(pattern)) {
                return pattern;
            }

            return string.Format("%{0}", EncodeLikeString(pattern));
        }

        /// <summary>
        /// Gets the specified string as a SQL LIKE contain string.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string GetContainLikeString(string pattern) {
            if(string.IsNullOrEmpty(pattern)) {
                return pattern;
            }

            return string.Format("%{0}%", EncodeLikeString(pattern));
        }

        /// <summary>
        /// Gets the specified string to match the phrase in SQL FULLTEXT search.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string GetFulltextPhraseString(string pattern) {
            return string.Format("\"{0}\"", EncodeFulltextString(pattern));
        }

        /// <summary>
        /// Get a SELECT statement used to get the lasted generated identity value of the specified table in any session and any range.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <returns>A SELECT statement used to get the lasted generated identity value</returns>
        public static ISqlSelectStatement GetIdentityStatement(string tableName) {
            ISqlSelectStatement statement = new SqlServerSelectStatement();
            statement.SelectClause.AddExpressions(new SqlServerField(new SqlFunction("IDENT_CURRENT").AddArgument(new SqlServerExpressionFactory().ConstantFactory.Create(tableName)), "Identity"));
            return statement;
        }

        /// <summary>
        /// Gets a SELECT statement used to get the lasted generated identity value of any table in current session.
        /// </summary>
        /// <param name="inCurrentRange">True: in current range; Flase: in any range.</param>
        /// <returns>A SELECT statement used to get the lasted generated identity value</returns>
        public static ISqlSelectStatement GetIdentityStatement(bool inCurrentRange) {
            ISqlSelectStatement statement = new SqlServerSelectStatement();
            if(inCurrentRange) {
                statement.SelectClause.AddExpressions(new SqlServerField(new SqlFunction("SCOPE_IDENTITY"), "Identity"));
            } else {
                statement.SelectClause.AddExpressions(new SqlServerField("@@IDENTITY", "Identity"));
            }
            return statement;
        }

        /// <summary>
        /// Gets the query condition to determine whether the field value equals <paramref name="value"/>. This field may has multiple values separated by <paramref name="separator"/>.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <param name="parameterSelector"></param>
        /// <returns></returns>
        public static ISqlExpression EqualWhenMultiple(ISqlExpression field, string value, string separator, Func<string, ISqlExpression> parameterSelector) {
            if(field == null) {
                throw new ArgumentNullException("field");
            }
            if(string.IsNullOrWhiteSpace(value)) {
                throw new ArgumentException("value is null or empty.", "value");
            }
            if(string.IsNullOrWhiteSpace(separator)) {
                throw new ArgumentException("separator is null or empty.", "separator");
            }

            return field.Equal(parameterSelector(value)).Or(field.Like(parameterSelector(GetBeginLikeString(value + separator)))).Or(field.Like(parameterSelector(GetContainLikeString(separator + value + separator)))).Or(field.Like(parameterSelector(GetEndLikeString(separator + value))));
        }

        /// <summary>
        /// Begins ExecuteNonQuery operation.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="isolationLevel"></param>
        /// <param name="sqlStatement"></param>
        /// <param name="parameters"></param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        public static IAsyncResult BeginExecuteNonQuery(string connectionString, IsolationLevel? isolationLevel, string sqlStatement, IEnumerable<SqlParameter> parameters, AsyncCallback callback, object userState) {
            if(string.IsNullOrWhiteSpace(connectionString)) {
                throw new ArgumentException("Connection string is null or empty.", "connectionString");
            }
            if(string.IsNullOrWhiteSpace(sqlStatement)) {
                throw new ArgumentException("SQL statement is null or empty.", "sqlStatement");
            }

            AsyncResult<int> result = new AsyncResult<int>(callback, userState);
            try {
                SqlCommand sqlCommand = CreateCommand(connectionString, isolationLevel, sqlStatement, parameters);
                sqlCommand.BeginExecuteNonQuery((ar) => {
                    int count = 0;
                    Exception error = null;
                    SqlCommand command = (SqlCommand) ar.AsyncState;

                    try {
                        count = command.EndExecuteNonQuery(ar);
                        if(command.Transaction != null) {
                            command.Transaction.Commit();
                        }
                    } catch(Exception ex) {
                        if(command.Transaction != null) {
                            command.Transaction.Rollback();
                        }

                        error = ex;
                    } finally {
                        CleanCommand(command);
                    }

                    result.MarkCompleted(error, ar.CompletedSynchronously, count);
                }, sqlCommand);
            } catch(Exception ex) {
                result.MarkCompleted(ex, true);
            }
            return result;
        }

        /// <summary>
        /// Ends ExecuteNonQuery operation.
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        public static int EndExecuteNonQuery(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<int>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<int>) asyncResult).End();
        }

        /// <summary>
        /// Begins a operation to backup the specified database.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        /// <param name="filePath"></param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        public static IAsyncResult BeginBackup(string connectionString, string databaseName, string filePath, AsyncCallback callback, object userState) {
            if(string.IsNullOrWhiteSpace(connectionString)) {
                throw new ArgumentException("Connection string is null or empty.", "connectionString");
            }
            if(string.IsNullOrWhiteSpace(databaseName)) {
                throw new ArgumentException("Database name is null or empty.", "databaseName");
            }
            FileUtility.CreateFolder(filePath);

            return BeginExecuteNonQuery(new SqlConnectionStringBuilder(connectionString) {
                AsynchronousProcessing = true,
                InitialCatalog = "master",
            }.ConnectionString, null, "BACKUP DATABASE @Name TO  DISK = @File WITH NOFORMAT, INIT, SKIP", new SqlParameter[] {
                SqlHelper.CreateParameter<SqlParameter>("@Name", DbType.String, databaseName),
                SqlHelper.CreateParameter<SqlParameter>("@File", DbType.String, filePath),
            }, callback, userState);
        }

        /// <summary>
        /// Ends a operation to backup the specified database.
        /// </summary>
        /// <param name="asyncResult"></param>
        public static void EndBackup(IAsyncResult asyncResult) {
            EndExecuteNonQuery(asyncResult);
        }

        /// <summary>
        /// Begins a operation to restore the specified database.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        /// <param name="filePath"></param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        public static IAsyncResult BeginRestore(string connectionString, string databaseName, string filePath, AsyncCallback callback, object userState) {
            if(string.IsNullOrWhiteSpace(connectionString)) {
                throw new ArgumentException("Connection string is null or empty.", "connectionString");
            }
            if(string.IsNullOrWhiteSpace(databaseName)) {
                throw new ArgumentException("Database name is null or empty.", "databaseName");
            }
            if(!File.Exists(filePath)) {
                throw new FileNotFoundException();
            }

            return BeginExecuteNonQuery(new SqlConnectionStringBuilder(connectionString) {
                AsynchronousProcessing = true,
                InitialCatalog = "master",
            }.ConnectionString, null, "RESTORE DATABASE @Name FROM  DISK = @File WITH  FILE = 1,  REPLACE", new SqlParameter[] {
                SqlHelper.CreateParameter<SqlParameter>("@Name", DbType.String, databaseName),
                SqlHelper.CreateParameter<SqlParameter>("@File", DbType.String, filePath),
            }, callback, userState);
        }

        /// <summary>
        /// Ends a operation to restore the specified database.
        /// </summary>
        /// <param name="asyncResult"></param>
        public static void EndRestore(IAsyncResult asyncResult) {
            EndExecuteNonQuery(asyncResult);
        }

        /// <summary>
        /// Begins a operation to clear data of the specified database.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        public static IAsyncResult BeginClearData(string connectionString, string databaseName, AsyncCallback callback, object userState) {
            if(string.IsNullOrWhiteSpace(connectionString)) {
                throw new ArgumentException("Connection string is null or empty.", "connectionString");
            }
            if(string.IsNullOrWhiteSpace(databaseName)) {
                throw new ArgumentException("Database name is null or empty.", "databaseName");
            }

            return BeginExecuteNonQuery(new SqlConnectionStringBuilder(connectionString) {
                AsynchronousProcessing = true,
                InitialCatalog = databaseName,
                MultipleActiveResultSets = true,
            }.ConnectionString, IsolationLevel.Serializable, string.Format("USE [{0}]; EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'; EXEC sp_MSForEachTable 'ALTER TABLE ? DISABLE TRIGGER ALL'; EXEC sp_MSForEachTable 'DELETE FROM ?'; EXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL'; EXEC sp_MSForEachTable 'ALTER TABLE ? ENABLE TRIGGER ALL';", databaseName), null, callback, userState);
        }

        /// <summary>
        /// Ends a operation to clear data of the specified database.
        /// </summary>
        /// <param name="asyncResult"></param>
        public static void EndClearData(IAsyncResult asyncResult) {
            EndExecuteNonQuery(asyncResult);
        }
    }
}
