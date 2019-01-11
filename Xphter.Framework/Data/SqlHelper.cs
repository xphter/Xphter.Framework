using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Xphter.Framework.Collections;

namespace Xphter.Framework.Data {
    /// <summary>
    /// Represents a utility for executing SQL statement.
    /// </summary>
    public static class SqlHelper {
        private static int g_timeout;
        /// <summary>
        /// Gets or sets the timeout of executing command in seconds.
        /// </summary>
        public static int Timeout {
            get {
                return g_timeout;
            }
            set {
                g_timeout = value;
            }
        }

        private static int g_maxNoQueryStatementsLimit;
        /// <summary>
        /// Gets the maximum number of statements allowed in ExecuteNoQuery methods.
        /// </summary>
        public static int MaxNoQueryStatementsLimit {
            get {
                return g_maxNoQueryStatementsLimit;
            }
            set {
                if(g_maxNoQueryStatementsLimit <= 0 && value > 0 ||
                    g_maxNoQueryStatementsLimit > 0 && value > 0 && value < g_maxNoQueryStatementsLimit) {
                    g_maxNoQueryStatementsLimit = value;
                }
            }
        }

        #region Private Methods

        /// <summary>
        /// Create a command for executing SQL statement.
        /// </summary>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>The command used to executing these SQL statement.</returns>
        private static IDbCommand PrepareCommand(IDbConnection connection, IEnumerable<ISqlStatement> statements, ISqlStatementCommandTextProvider commandTextProvider) {
            // open connection
            switch(connection.State) {
                case ConnectionState.Broken:
                    connection.Close();
                    connection.Open();
                    break;
                case ConnectionState.Closed:
                    connection.Open();
                    break;
            }

            // find parameters
            List<IDataParameter> allParameters = new List<IDataParameter>();
            foreach(ISqlStatement statement in statements) {
                if(statement == null) {
                    continue;
                }

                allParameters.AddRange(statement.Parameters);
            }

            // create command      
            IDbCommand command = new AutoClearParametersDbCommand(connection.CreateCommand());
            command.CommandTimeout = g_timeout;
            command.CommandType = CommandType.Text;
            command.CommandText = (commandTextProvider ?? DefaultSqlStatementCommandTextProvider.Instance).GetCommandText(statements);
            foreach(IDataParameter parameter in allParameters) {
                if(parameter == null || command.Parameters.Contains(parameter.ParameterName)) {
                    continue;
                }

                command.Parameters.Add(parameter);
            }

            return command;
        }

        /// <summary>
        /// Create a command for executing SQL statement with the specified transaction isolation level.
        /// </summary>
        /// <param name="transactionLevel">Transaction isolation level, null indicate to use default isolation level.</param>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>The command used to executing these SQL statement.</returns>
        private static IDbCommand PrepareCommand(IsolationLevel? transactionLevel, IDbConnection connection, IEnumerable<ISqlStatement> statements, ISqlStatementCommandTextProvider commandTextProvider) {
            // open connection
            switch(connection.State) {
                case ConnectionState.Broken:
                    connection.Close();
                    connection.Open();
                    break;
                case ConnectionState.Closed:
                    connection.Open();
                    break;
            }

            // find parameters
            List<IDataParameter> allParameters = new List<IDataParameter>();
            foreach(ISqlStatement statement in statements) {
                if(statement == null) {
                    continue;
                }

                allParameters.AddRange(statement.Parameters);
            }

            //create command      
            IDbCommand command = new AutoClearParametersDbCommand(connection.CreateCommand());
            command.Transaction = transactionLevel.HasValue ? connection.BeginTransaction(transactionLevel.Value) : connection.BeginTransaction();
            command.CommandTimeout = g_timeout;
            command.CommandType = CommandType.Text;
            command.CommandText = (commandTextProvider ?? DefaultSqlStatementCommandTextProvider.Instance).GetCommandText(statements);
            foreach(IDataParameter parameter in allParameters) {
                if(parameter == null || command.Parameters.Contains(parameter.ParameterName)) {
                    continue;
                }

                command.Parameters.Add(parameter);
            }

            return command;
        }

        /// <summary>
        /// Create a command for executing SQL statement with the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction associated with the command.</param>
        /// <param name="statements">SQL statement.</param>
        /// <returns>The command used to executing these SQL statement.</returns>
        private static IDbCommand PrepareCommand(IDbTransaction transaction, IEnumerable<ISqlStatement> statements, ISqlStatementCommandTextProvider commandTextProvider) {
            // find parameters
            List<IDataParameter> allParameters = new List<IDataParameter>();
            foreach(ISqlStatement statement in statements) {
                if(statement == null) {
                    continue;
                }

                allParameters.AddRange(statement.Parameters);
            }

            // create command      
            IDbCommand command = new AutoClearParametersDbCommand(transaction.Connection.CreateCommand());
            command.Transaction = transaction;
            command.CommandTimeout = g_timeout;
            command.CommandType = CommandType.Text;
            command.CommandText = (commandTextProvider ?? DefaultSqlStatementCommandTextProvider.Instance).GetCommandText(statements);
            foreach(IDataParameter parameter in allParameters) {
                if(parameter == null || command.Parameters.Contains(parameter.ParameterName)) {
                    continue;
                }

                command.Parameters.Add(parameter);
            }

            return command;
        }

        /// <summary>
        /// Create a command for executing SQL stored procedure.
        /// </summary>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="spName">Name of a stored procedure.</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>The command used to executing a SQL stored procedure.</returns>
        private static IDbCommand PrepareCommand(IDbConnection connection, string spName, params IDataParameter[] parameters) {
            // open connection
            switch(connection.State) {
                case ConnectionState.Broken:
                    connection.Close();
                    connection.Open();
                    break;
                case ConnectionState.Closed:
                    connection.Open();
                    break;
            }

            // create command      
            IDbCommand command = new AutoClearParametersDbCommand(connection.CreateCommand());
            command.CommandTimeout = g_timeout;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = spName;
            foreach(IDataParameter parameter in parameters) {
                if(parameter == null || command.Parameters.Contains(parameter.ParameterName)) {
                    continue;
                }

                command.Parameters.Add(parameter);
            }

            return command;
        }

        /// <summary>
        /// Create a command for executing SQL stored procedure with the specified transaction isolation level.
        /// </summary>
        /// <param name="transactionLevel">Transaction isolation level, null indicate to use default isolation level.</param>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="spName">Name of a stored procedure.</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>The command used to executing a SQL stored procedure.</returns>
        private static IDbCommand PrepareCommand(IsolationLevel? transactionLevel, IDbConnection connection, string spName, params IDataParameter[] parameters) {
            // open connection
            switch(connection.State) {
                case ConnectionState.Broken:
                    connection.Close();
                    connection.Open();
                    break;
                case ConnectionState.Closed:
                    connection.Open();
                    break;
            }

            // create command      
            IDbCommand command = new AutoClearParametersDbCommand(connection.CreateCommand());
            command.Transaction = transactionLevel.HasValue ? connection.BeginTransaction(transactionLevel.Value) : connection.BeginTransaction();
            command.CommandTimeout = g_timeout;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = spName;
            foreach(IDataParameter parameter in parameters) {
                if(parameter == null || command.Parameters.Contains(parameter.ParameterName)) {
                    continue;
                }

                command.Parameters.Add(parameter);
            }

            return command;
        }

        /// <summary>
        /// Create a command for executing SQL stored procedure with the specified transaction.
        /// </summary>
        /// <param name="spName">The transaction associated with the command.</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>The command used to executing a SQL stored procedure.</returns>
        private static IDbCommand PrepareCommand(IDbTransaction transaction, string spName, params IDataParameter[] parameters) {
            // create command      
            IDbCommand command = new AutoClearParametersDbCommand(transaction.Connection.CreateCommand());
            command.Transaction = transaction;
            command.CommandTimeout = g_timeout;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = spName;
            foreach(IDataParameter parameter in parameters) {
                if(parameter == null || command.Parameters.Contains(parameter.ParameterName)) {
                    continue;
                }

                command.Parameters.Add(parameter);
            }

            return command;
        }

        /// <summary>
        /// Create a SQL parameter.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="name">Parameter name.</param>
        /// <param name="type">Parameter DbType.</param>
        /// <param name="direction">Parameter direction.</param>
        /// <param name="value">Parameter value.</param>
        /// <returns>The created parameter.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        private static T CreateParameterInternal<T>(string name, DbType? type, ParameterDirection? direction, object value) where T : IDataParameter, new() {
            if(string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("Parameter name is null or empty.", "name");
            }

            T parameter = new T();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            if(type.HasValue) {
                parameter.DbType = type.Value;
            }
            parameter.Direction = direction.HasValue ? direction.Value : ParameterDirection.Input;
            return parameter;
        }

        /// <summary>
        /// Execute the specified statements and return the count of rows affected.
        /// </summary>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>The count of rows affected.</returns>
        private static int ExecuteNoQueryInternal(IDbConnection connection, IEnumerable<ISqlStatement> statements, ISqlStatementCommandTextProvider commandTextProvider) {
            using(IDbCommand command = PrepareCommand(connection, statements, commandTextProvider)) {
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Execute the specified statements under the specified transaction isolation level and return the count of rows affected.
        /// </summary>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>The count of rows affected.</returns>
        private static int ExecuteNoQueryInternal(IsolationLevel? transactionLevel, IDbConnection connection, IEnumerable<ISqlStatement> statements, ISqlStatementCommandTextProvider commandTextProvider) {
            using(IDbCommand command = PrepareCommand(transactionLevel, connection, statements, commandTextProvider)) {
                using(IDbTransaction transaction = command.Transaction) {
                    try {
                        int value = command.ExecuteNonQuery();
                        transaction.Commit();
                        return value;
                    } catch(DbException ex) {
                        try {
                            transaction.Rollback();
                        } catch {
                        }
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Execute the specified statements with the specified transaction and return the count of rows affected.
        /// </summary>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>The count of rows affected.</returns>
        private static int ExecuteNoQueryInternal(IDbTransaction transaction, IEnumerable<ISqlStatement> statements, ISqlStatementCommandTextProvider commandTextProvider) {
            using(IDbCommand command = PrepareCommand(transaction, statements, commandTextProvider)) {
                return command.ExecuteNonQuery();
            }
        }

        #endregion

        #region Create Parameter Methods

        /// <summary>
        /// Create SQL expression of a parameter.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="name">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        /// <returns>The created parameter expression.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public static ISqlExpression CreateParameterExpression<T>(string name, object value) where T : IDataParameter, new() {
            return SqlStringExpression.FromParameter(CreateParameterInternal<T>(name, null, null, value));
        }

        /// <summary>
        /// Create SQL expression of a parameter.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="name">Parameter name.</param>
        /// <param name="type">Parameter DbType.</param>
        /// <param name="value">Parameter value.</param>
        /// <returns>The created parameter expression.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public static ISqlExpression CreateParameterExpression<T>(string name, DbType type, object value) where T : IDataParameter, new() {
            return SqlStringExpression.FromParameter(CreateParameterInternal<T>(name, type, null, value));
        }

        /// <summary>
        /// Create SQL expression of a parameter.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="name">Parameter name.</param>
        /// <param name="type">Parameter DbType.</param>
        /// <param name="direction">Parameter direction.</param>
        /// <param name="value">Parameter value.</param>
        /// <returns>The created parameter expression.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public static ISqlExpression CreateParameterExpression<T>(string name, DbType type, ParameterDirection direction, object value) where T : IDataParameter, new() {
            return SqlStringExpression.FromParameter(CreateParameterInternal<T>(name, type, direction, value));
        }

        /// <summary>
        /// Create a SQL parameter.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="name">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        /// <returns>The created parameter.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public static T CreateParameter<T>(string name, object value) where T : IDataParameter, new() {
            return CreateParameterInternal<T>(name, null, null, value);
        }

        /// <summary>
        /// Create a SQL parameter.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="name">Parameter name.</param>
        /// <param name="type">Parameter DbType.</param>
        /// <param name="value">Parameter value.</param>
        /// <returns>The created parameter.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public static T CreateParameter<T>(string name, DbType type, object value) where T : IDataParameter, new() {
            return CreateParameterInternal<T>(name, type, null, value);
        }

        /// <summary>
        /// Create a SQL parameter.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="name">Parameter name.</param>
        /// <param name="type">Parameter DbType.</param>
        /// <param name="value">Parameter value.</param>
        /// <returns>The created parameter.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public static T CreateParameter<T>(string name, DbType type, ParameterDirection direction, object value) where T : IDataParameter, new() {
            return CreateParameterInternal<T>(name, type, direction, value);
        }

        #endregion

        #region Common Query Methods

        /// <summary>
        /// Splits <paramref name="statements"/> to some groups.
        /// </summary>
        /// <param name="statements"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<ISqlStatement>> SplitStatements(params ISqlStatement[] statements) {
            if(g_maxNoQueryStatementsLimit <= 0 || statements.Length <= g_maxNoQueryStatementsLimit) {
                throw new NotSupportedException("Not need to split statements.");
            }

            return statements.Split(g_maxNoQueryStatementsLimit);
        }

        /// <summary>
        /// Execute the specified SQL statements and return a IDataReader object.
        /// </summary>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>A IDataReader used to read result.</returns>
        public static IDataReader ExecuteReader(IDbConnection connection, params ISqlStatement[] statements) {
            return ExecuteReader(connection, (ISqlStatementCommandTextProvider) null, statements);
        }

        /// <summary>
        /// Execute the specified SQL statements and return a IDataReader object.
        /// </summary>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="commandTextProvider">A ISqlStatementCommandTextProvider object.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>A IDataReader used to read result.</returns>
        public static IDataReader ExecuteReader(IDbConnection connection, ISqlStatementCommandTextProvider commandTextProvider, params ISqlStatement[] statements) {
            using(IDbCommand command = PrepareCommand(connection, statements, commandTextProvider)) {
                return command.ExecuteReader();
            }
        }

        /// <summary>
        /// Execute the specified SQL statements under the specified transaction isolation level and return a IDataReader object.
        /// </summary>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>A IDataReader used to read result.</returns>
        public static IDataReader ExecuteReader(IsolationLevel? transactionLevel, IDbConnection connection, params ISqlStatement[] statements) {
            return ExecuteReader(transactionLevel, connection, (ISqlStatementCommandTextProvider) null, statements);
        }

        /// <summary>
        /// Execute the specified SQL statements under the specified transaction isolation level and return a IDataReader object.
        /// </summary>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>A IDataReader used to read result.</returns>
        public static IDataReader ExecuteReader(IsolationLevel? transactionLevel, IDbConnection connection, ISqlStatementCommandTextProvider commandTextProvider, params ISqlStatement[] statements) {
            using(IDbCommand command = PrepareCommand(transactionLevel, connection, statements, commandTextProvider)) {
                IDbTransaction transaction = command.Transaction;
                try {
                    return new AutoCommitDataReader(command.ExecuteReader(), transaction);
                } catch(DbException ex) {
                    try {
                        transaction.Rollback();
                        transaction.Dispose();
                    } catch {
                    }
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Execute the specified SQL statements with the specified transaction and return a IDataReader object.
        /// </summary>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>A IDataReader used to read result.</returns>
        public static IDataReader ExecuteReader(IDbTransaction transaction, params ISqlStatement[] statements) {
            return ExecuteReader(transaction, (ISqlStatementCommandTextProvider) null, statements);
        }

        /// <summary>
        /// Execute the specified SQL statements with the specified transaction and return a IDataReader object.
        /// </summary>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>A IDataReader used to read result.</returns>
        public static IDataReader ExecuteReader(IDbTransaction transaction, ISqlStatementCommandTextProvider commandTextProvider, params ISqlStatement[] statements) {
            using(IDbCommand command = PrepareCommand(transaction, statements, commandTextProvider)) {
                return command.ExecuteReader();
            }
        }

        /// <summary>
        /// Execute the specified SQL stored procedure and return a IDataReader object.
        /// </summary>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="spName">Name of a SQL stored procedure.</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>A IDataReader used to read result.</returns>
        public static IDataReader ExecuteReader(IDbConnection connection, string spName, params IDataParameter[] parameters) {
            using(IDbCommand command = PrepareCommand(connection, spName, parameters)) {
                return command.ExecuteReader();
            }
        }

        /// <summary>
        /// Execute the specified SQL stored procedure under the specified transaction isolation level and return a IDataReader object.
        /// </summary>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="spName">Name of a SQL stored procedure.</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>A IDataReader used to read result.</returns>
        public static IDataReader ExecuteReader(IsolationLevel? transactionLevel, IDbConnection connection, string spName, params IDataParameter[] parameters) {
            using(IDbCommand command = PrepareCommand(transactionLevel, connection, spName, parameters)) {
                IDbTransaction transaction = command.Transaction;
                try {
                    return new AutoCommitDataReader(command.ExecuteReader(), transaction);
                } catch(DbException ex) {
                    try {
                        transaction.Rollback();
                        transaction.Dispose();
                    } catch {
                    }
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Execute the specified SQL stored procedure with the specified transaction and return a IDataReader object.
        /// </summary>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="spName">Name of a SQL stored procedure.</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>A IDataReader used to read result.</returns>
        public static IDataReader ExecuteReader(IDbTransaction transaction, string spName, params IDataParameter[] parameters) {
            using(IDbCommand command = PrepareCommand(transaction, spName, parameters)) {
                return command.ExecuteReader();
            }
        }

        /// <summary>
        /// Get schema of query result.
        /// </summary>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="statement">A SQL SELECT statement used to query schema.</param>
        /// <returns>Schema of query result.</returns>
        public static DataTable ExecuteSchema(IDbConnection connection, ISqlSelectStatement statement) {
            return ExecuteSchema(connection, (ISqlStatementCommandTextProvider) null, statement);
        }

        /// <summary>
        /// Get schema of query result.
        /// </summary>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="statement">A SQL SELECT statement used to query schema.</param>
        /// <returns>Schema of query result.</returns>
        public static DataTable ExecuteSchema(IDbConnection connection, ISqlStatementCommandTextProvider commandTextProvider, ISqlSelectStatement statement) {
            using(IDbCommand command = PrepareCommand(connection, new ISqlStatement[] { statement }, commandTextProvider)) {
                using(IDataReader reader = command.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo)) {
                    return reader.GetSchemaTable();
                }
            }
        }

        /// <summary>
        /// Execute the specified SQL statements and return the first column of this first row in result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>The first column of this first row in result.</returns>
        public static T ExecuteScalar<T>(IDbConnection connection, params ISqlStatement[] statements) {
            return ExecuteScalar<T>(connection, (ISqlStatementCommandTextProvider) null, statements);
        }

        /// <summary>
        /// Execute the specified SQL statements and return the first column of this first row in result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>The first column of this first row in result.</returns>
        public static T ExecuteScalar<T>(IDbConnection connection, ISqlStatementCommandTextProvider commandTextProvider, params ISqlStatement[] statements) {
            using(IDbCommand command = PrepareCommand(connection, statements, commandTextProvider)) {
                object value = command.ExecuteScalar();
                return !Convert.IsDBNull(value) && value != null ? (T) value : default(T);
            }
        }

        /// <summary>
        /// Execute the specified SQL statements under the specified transaction isolation level and return the first column of this first row in result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>The first column of this first row in result.</returns>
        public static T ExecuteScalar<T>(IsolationLevel? transactionLevel, IDbConnection connection, params ISqlStatement[] statements) {
            return ExecuteScalar<T>(transactionLevel, connection, (ISqlStatementCommandTextProvider) null, statements);
        }

        /// <summary>
        /// Execute the specified SQL statements under the specified transaction isolation level and return the first column of this first row in result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>The first column of this first row in result.</returns>
        public static T ExecuteScalar<T>(IsolationLevel? transactionLevel, IDbConnection connection, ISqlStatementCommandTextProvider commandTextProvider, params ISqlStatement[] statements) {
            using(IDbCommand command = PrepareCommand(transactionLevel, connection, statements, commandTextProvider)) {
                using(IDbTransaction transaction = command.Transaction) {
                    try {
                        object value = command.ExecuteScalar();
                        transaction.Commit();
                        return !Convert.IsDBNull(value) && value != null ? (T) value : default(T);
                    } catch(DbException ex) {
                        try {
                            transaction.Rollback();
                        } catch {
                        }
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Execute the specified SQL statements with the specified transaction with the specified transaction and return the first column of this first row in result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>The first column of this first row in result.</returns>
        public static T ExecuteScalar<T>(IDbTransaction transaction, params ISqlStatement[] statements) {
            return ExecuteScalar<T>(transaction, (ISqlStatementCommandTextProvider) null, statements);
        }

        /// <summary>
        /// Execute the specified SQL statements with the specified transaction with the specified transaction and return the first column of this first row in result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>The first column of this first row in result.</returns>
        public static T ExecuteScalar<T>(IDbTransaction transaction, ISqlStatementCommandTextProvider commandTextProvider, params ISqlStatement[] statements) {
            using(IDbCommand command = PrepareCommand(transaction, statements, commandTextProvider)) {
                object value = command.ExecuteScalar();
                return !Convert.IsDBNull(value) && value != null ? (T) value : default(T);
            }
        }

        /// <summary>
        /// Execute the specified SQL stored procedure and return the first column of this first row in result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="spName">Name of a SQL stored procedure.</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>The first column of this first row in result.</returns>
        public static T ExecuteScalar<T>(IDbConnection connection, string spName, params IDataParameter[] parameters) {
            using(IDbCommand command = PrepareCommand(connection, spName, parameters)) {
                object value = command.ExecuteScalar();
                return !Convert.IsDBNull(value) && value != null ? (T) value : default(T);
            }
        }

        /// <summary>
        /// Execute the specified SQL stored procedure under the specified transaction isolation level and return the first column of this first row in result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="spName">Name of a SQL stored procedure.</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>The first column of this first row in result.</returns>
        public static T ExecuteScalar<T>(IsolationLevel? transactionLevel, IDbConnection connection, string spName, params IDataParameter[] parameters) {
            using(IDbCommand command = PrepareCommand(transactionLevel, connection, spName, parameters)) {
                using(IDbTransaction transaction = command.Transaction) {
                    try {
                        object value = command.ExecuteScalar();
                        transaction.Commit();
                        return !Convert.IsDBNull(value) && value != null ? (T) value : default(T);
                    } catch(DbException ex) {
                        try {
                            transaction.Rollback();
                        } catch {
                        }
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Execute the specified SQL stored procedure with the specified transaction and return the first column of this first row in result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="spName">Name of a SQL stored procedure.</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>The first column of this first row in result.</returns>
        public static T ExecuteScalar<T>(IDbTransaction transaction, string spName, params IDataParameter[] parameters) {
            using(IDbCommand command = PrepareCommand(transaction, spName, parameters)) {
                object value = command.ExecuteScalar();
                return !Convert.IsDBNull(value) && value != null ? (T) value : default(T);
            }
        }

        /// <summary>
        /// Execute the specified statements and return the count of rows affected.
        /// </summary>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>The count of rows affected.</returns>
        public static int ExecuteNoQuery(IDbConnection connection, params ISqlStatement[] statements) {
            return ExecuteNoQuery(connection, (ISqlStatementCommandTextProvider) null, statements);
        }

        /// <summary>
        /// Execute the specified statements and return the count of rows affected.
        /// </summary>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>The count of rows affected.</returns>
        public static int ExecuteNoQuery(IDbConnection connection, ISqlStatementCommandTextProvider commandTextProvider, params ISqlStatement[] statements) {
            if(statements.Length == 0) {
                return 0;
            }

            int count = 0;
            if(g_maxNoQueryStatementsLimit <= 0 || statements.Length <= g_maxNoQueryStatementsLimit) {
                count = ExecuteNoQueryInternal(connection, statements, commandTextProvider);
            } else {
                foreach(IEnumerable<ISqlStatement> item in SplitStatements(statements)) {
                    count += ExecuteNoQueryInternal(connection, item, commandTextProvider);
                }
            }

            return count;
        }

        /// <summary>
        /// Execute the specified statements under the specified transaction isolation level and return the count of rows affected.
        /// </summary>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>The count of rows affected.</returns>
        public static int ExecuteNoQuery(IsolationLevel? transactionLevel, IDbConnection connection, params ISqlStatement[] statements) {
            return ExecuteNoQuery(transactionLevel, connection, (ISqlStatementCommandTextProvider) null, statements);
        }

        /// <summary>
        /// Execute the specified statements under the specified transaction isolation level and return the count of rows affected.
        /// </summary>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>The count of rows affected.</returns>
        public static int ExecuteNoQuery(IsolationLevel? transactionLevel, IDbConnection connection, ISqlStatementCommandTextProvider commandTextProvider, params ISqlStatement[] statements) {
            if(statements.Length == 0) {
                return 0;
            }

            int count = 0;
            if(g_maxNoQueryStatementsLimit <= 0 || statements.Length <= g_maxNoQueryStatementsLimit) {
                count = ExecuteNoQueryInternal(transactionLevel, connection, statements, commandTextProvider);
            } else {
                foreach(IEnumerable<ISqlStatement> item in SplitStatements(statements)) {
                    count += ExecuteNoQueryInternal(transactionLevel, connection, item, commandTextProvider);
                }
            }

            return count;
        }

        /// <summary>
        /// Execute the specified statements with the specified transaction and return the count of rows affected.
        /// </summary>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>The count of rows affected.</returns>
        public static int ExecuteNoQuery(IDbTransaction transaction, params ISqlStatement[] statements) {
            return ExecuteNoQuery(transaction, (ISqlStatementCommandTextProvider) null, statements);
        }

        /// <summary>
        /// Execute the specified statements with the specified transaction and return the count of rows affected.
        /// </summary>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="statements">SQL statements.</param>
        /// <returns>The count of rows affected.</returns>
        public static int ExecuteNoQuery(IDbTransaction transaction, ISqlStatementCommandTextProvider commandTextProvider, params ISqlStatement[] statements) {
            if(statements.Length == 0) {
                return 0;
            }

            int count = 0;
            if(g_maxNoQueryStatementsLimit <= 0 || statements.Length <= g_maxNoQueryStatementsLimit) {
                count = ExecuteNoQueryInternal(transaction, statements, commandTextProvider);
            } else {
                foreach(IEnumerable<ISqlStatement> item in SplitStatements(statements)) {
                    count += ExecuteNoQueryInternal(transaction, item, commandTextProvider);
                }
            }

            return count;
        }

        /// <summary>
        /// Execute the specified statements and return the count of rows affected.
        /// </summary>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="spName">Name of a SQL stored procedure.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns>The count of rows affected.</returns>
        public static int ExecuteNoQuery(IDbConnection connection, string spName, params IDataParameter[] parameters) {
            using(IDbCommand command = PrepareCommand(connection, spName, parameters)) {
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Execute the specified statements under the specified transaction isolation level and return the count of rows affected.
        /// </summary>
        /// <param name="connection">A SQL connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="spName">Name of a SQL stored procedure.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns>The count of rows affected.</returns>
        public static int ExecuteNoQuery(IsolationLevel? transactionLevel, IDbConnection connection, string spName, params IDataParameter[] parameters) {
            using(IDbCommand command = PrepareCommand(transactionLevel, connection, spName, parameters)) {
                using(IDbTransaction transaction = command.Transaction) {
                    try {
                        int value = command.ExecuteNonQuery();
                        transaction.Commit();
                        return value;
                    } catch(DbException ex) {
                        try {
                            transaction.Rollback();
                        } catch {
                        }
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Execute the specified statements with the specified transaction and return the count of rows affected.
        /// </summary>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="spName">Name of a SQL stored procedure.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns>The count of rows affected.</returns>
        public static int ExecuteNoQuery(IDbTransaction transaction, string spName, params IDataParameter[] parameters) {
            using(IDbCommand command = PrepareCommand(transaction, spName, parameters)) {
                return command.ExecuteNonQuery();
            }
        }

        #endregion

        #region Common Execute SQL Statement Methods

        /// <summary>
        /// Execute SQL SELECT statements under default transaction isolation level and returns a IDataReader.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="statements">SQL SELECT statements.</param>
        /// <returns>A IDataReader object.</returns>
        public static IDataReader ExecuteSelect(IDbConnection connection, params ISqlSelectStatement[] statements) {
            return ExecuteReader(connection, (ISqlStatementCommandTextProvider) null, statements);
        }

        /// <summary>
        /// Execute SQL SELECT statements under default transaction isolation level and returns a IDataReader.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="statements">SQL SELECT statements.</param>
        /// <returns>A IDataReader object.</returns>
        public static IDataReader ExecuteSelect(IDbConnection connection, ISqlStatementCommandTextProvider commandTextProvider, params ISqlSelectStatement[] statements) {
            return ExecuteReader(connection, commandTextProvider, statements);
        }

        /// <summary>
        /// Execute SQL SELECT statements and returns a IDataReader.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="statements">SQL SELECT statements.</param>
        /// <returns>A IDataReader object.</returns>
        public static IDataReader ExecuteSelect(IsolationLevel? transactionLevel, IDbConnection connection, params ISqlSelectStatement[] statements) {
            return ExecuteReader(transactionLevel, connection, (ISqlStatementCommandTextProvider) null, statements);
        }

        /// <summary>
        /// Execute SQL SELECT statements and returns a IDataReader.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="statements">SQL SELECT statements.</param>
        /// <returns>A IDataReader object.</returns>
        public static IDataReader ExecuteSelect(IsolationLevel? transactionLevel, IDbConnection connection, ISqlStatementCommandTextProvider commandTextProvider, params ISqlSelectStatement[] statements) {
            return ExecuteReader(transactionLevel, connection, commandTextProvider, statements);
        }

        /// <summary>
        /// Execute SQL SELECT statements with the specified transaction and returns a IDataReader.
        /// </summary>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="statements">SQL SELECT statements.</param>
        /// <returns>A IDataReader object.</returns>
        public static IDataReader ExecuteSelect(IDbTransaction transaction, params ISqlSelectStatement[] statements) {
            return ExecuteReader(transaction, (ISqlStatementCommandTextProvider) null, statements);
        }

        /// <summary>
        /// Execute SQL SELECT statements with the specified transaction and returns a IDataReader.
        /// </summary>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="statements">SQL SELECT statements.</param>
        /// <returns>A IDataReader object.</returns>
        public static IDataReader ExecuteSelect(IDbTransaction transaction, ISqlStatementCommandTextProvider commandTextProvider, params ISqlSelectStatement[] statements) {
            return ExecuteReader(transaction, commandTextProvider, statements);
        }

        /// <summary>
        /// Execute a SQL SELECT statement under default transaction isolation level and returns the first column of the first row in the result set returned by this query.
        /// </summary>
        /// <typeparam name="T">Type of return value.</typeparam>
        /// <param name="connection">Database connection.</param>
        /// <param name="statement">A SQL SELECT statement.</param>
        /// <returns>The first column of the first row in the result set returned by this query.</returns>
        public static T ExecuteSelectScalar<T>(IDbConnection connection, ISqlSelectStatement statement) {
            return ExecuteScalar<T>(connection, (ISqlStatementCommandTextProvider) null, statement);
        }

        /// <summary>
        /// Execute a SQL SELECT statement under default transaction isolation level and returns the first column of the first row in the result set returned by this query.
        /// </summary>
        /// <typeparam name="T">Type of return value.</typeparam>
        /// <param name="connection">Database connection.</param>
        /// <param name="statement">A SQL SELECT statement.</param>
        /// <returns>The first column of the first row in the result set returned by this query.</returns>
        public static T ExecuteSelectScalar<T>(IDbConnection connection, ISqlStatementCommandTextProvider commandTextProvider, ISqlSelectStatement statement) {
            return ExecuteScalar<T>(connection, commandTextProvider, statement);
        }

        /// <summary>
        /// Execute a SQL SELECT statement and returns the first column of the first row in the result set returned by this query.
        /// </summary>
        /// <typeparam name="T">Type of return value.</typeparam>
        /// <param name="connection">Database connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="statement">A SQL SELECT statement.</param>
        /// <returns>The first column of the first row in the result set returned by this query.</returns>
        public static T ExecuteSelectScalar<T>(IsolationLevel? transactionLevel, IDbConnection connection, SqlSelectStatement statement) {
            return ExecuteScalar<T>(transactionLevel, connection, (ISqlStatementCommandTextProvider) null, statement);
        }

        /// <summary>
        /// Execute a SQL SELECT statement and returns the first column of the first row in the result set returned by this query.
        /// </summary>
        /// <typeparam name="T">Type of return value.</typeparam>
        /// <param name="connection">Database connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="statement">A SQL SELECT statement.</param>
        /// <returns>The first column of the first row in the result set returned by this query.</returns>
        public static T ExecuteSelectScalar<T>(IsolationLevel? transactionLevel, IDbConnection connection, ISqlStatementCommandTextProvider commandTextProvider, ISqlSelectStatement statement) {
            return ExecuteScalar<T>(transactionLevel, connection, commandTextProvider, statement);
        }

        /// <summary>
        /// Execute a SQL SELECT statement with the specified transaction and returns the first column of the first row in the result set returned by this query.
        /// </summary>
        /// <typeparam name="T">Type of return value.</typeparam>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="statement">A SQL SELECT statement.</param>
        /// <returns>The first column of the first row in the result set returned by this query.</returns>
        public static T ExecuteSelectScalar<T>(IDbTransaction transaction, ISqlSelectStatement statement) {
            return ExecuteScalar<T>(transaction, (ISqlStatementCommandTextProvider) null, statement);
        }

        /// <summary>
        /// Execute a SQL SELECT statement with the specified transaction and returns the first column of the first row in the result set returned by this query.
        /// </summary>
        /// <typeparam name="T">Type of return value.</typeparam>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="statement">A SQL SELECT statement.</param>
        /// <returns>The first column of the first row in the result set returned by this query.</returns>
        public static T ExecuteSelectScalar<T>(IDbTransaction transaction, ISqlStatementCommandTextProvider commandTextProvider, ISqlSelectStatement statement) {
            return ExecuteScalar<T>(transaction, commandTextProvider, statement);
        }

        /// <summary>
        /// Execute SQL INSERT statements under default transaction isolation level.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="statement">SQL INSERT statements</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteInsert(IDbConnection connection, params ISqlInsertStatement[] statements) {
            return ExecuteNoQuery(connection, (ISqlStatementCommandTextProvider) null, statements);
        }

        /// <summary>
        /// Execute SQL INSERT statements under default transaction isolation level.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="statement">SQL INSERT statements</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteInsert(IDbConnection connection, ISqlStatementCommandTextProvider commandTextProvider, params ISqlInsertStatement[] statements) {
            return ExecuteNoQuery(connection, commandTextProvider, statements);
        }

        /// <summary>
        /// Execute SQL INSERT statements.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="statement">SQL INSERT statements</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteInsert(IsolationLevel? transactionLevel, IDbConnection connection, params ISqlInsertStatement[] statements) {
            return ExecuteNoQuery(transactionLevel, connection, (ISqlStatementCommandTextProvider) null, statements);
        }

        /// <summary>
        /// Execute SQL INSERT statements.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="transactionLevel">Transaction isolation level.</param>
        /// <param name="statement">SQL INSERT statements</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteInsert(IsolationLevel? transactionLevel, IDbConnection connection, ISqlStatementCommandTextProvider commandTextProvider, params ISqlInsertStatement[] statements) {
            return ExecuteNoQuery(transactionLevel, connection, commandTextProvider, statements);
        }

        /// <summary>
        /// Execute SQL INSERT statements with the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="statement">SQL INSERT statements</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteInsert(IDbTransaction transaction, params ISqlInsertStatement[] statements) {
            return ExecuteNoQuery(transaction, (ISqlStatementCommandTextProvider) null, statements);
        }

        /// <summary>
        /// Execute SQL INSERT statements with the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="statement">SQL INSERT statements</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteInsert(IDbTransaction transaction, ISqlStatementCommandTextProvider commandTextProvider, params ISqlInsertStatement[] statements) {
            return ExecuteNoQuery(transaction, commandTextProvider, statements);
        }

        /// <summary>
        /// Execute a SQL INSERT statement and fetch the generated identity value.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="statement">A SQL INSERT statement</param>
        /// <param name="identityFetcher">A SQL SELECT statement to query identity value of the target source.</param>
        /// <returns>The lasted generated identity value of the target source.</returns>
        public static int ExecuteInsert(IDbConnection connection, ISqlInsertStatement statement, ISqlSelectStatement identityFetcher) {
            return ExecuteInsert(connection, (ISqlStatementCommandTextProvider) null, statement, identityFetcher);
        }

        /// <summary>
        /// Execute a SQL INSERT statement and fetch the generated identity value.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="statement">A SQL INSERT statement</param>
        /// <param name="identityFetcher">A SQL SELECT statement to query identity value of the target source.</param>
        /// <returns>The lasted generated identity value of the target source.</returns>
        public static int ExecuteInsert(IDbConnection connection, ISqlStatementCommandTextProvider commandTextProvider, ISqlInsertStatement statement, ISqlSelectStatement identityFetcher) {
            ExecuteInsert(connection, commandTextProvider, statement);
            return Convert.ToInt32(ExecuteScalar<object>(connection, commandTextProvider, identityFetcher));
        }

        /// <summary>
        /// Execute a SQL INSERT statement and fetch the generated identity value with the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="statement">A SQL INSERT statement</param>
        /// <param name="identityFetcher">A SQL SELECT statement to query identity value of the target source.</param>
        /// <returns>The lasted generated identity value of the target source.</returns>
        public static int ExecuteInsert(IDbTransaction transaction, ISqlInsertStatement statement, ISqlSelectStatement identityFetcher) {
            return ExecuteInsert(transaction, (ISqlStatementCommandTextProvider) null, statement, identityFetcher);
        }

        /// <summary>
        /// Execute a SQL INSERT statement and fetch the generated identity value with the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="statement">A SQL INSERT statement</param>
        /// <param name="identityFetcher">A SQL SELECT statement to query identity value of the target source.</param>
        /// <returns>The lasted generated identity value of the target source.</returns>
        public static int ExecuteInsert(IDbTransaction transaction, ISqlStatementCommandTextProvider commandTextProvider, ISqlInsertStatement statement, ISqlSelectStatement identityFetcher) {
            ExecuteInsert(transaction, commandTextProvider, statement);
            return Convert.ToInt32(ExecuteScalar<object>(transaction, commandTextProvider, identityFetcher));
        }

        /// <summary>
        /// Execute SQL UPDATE statements under default transaction isolation level.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="statements">SQL UPDATE statements.</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteUpdate(IDbConnection connection, params ISqlUpdateStatement[] statements) {
            return ExecuteNoQuery(connection, (ISqlStatementCommandTextProvider) null, statements);
        }

        /// <summary>
        /// Execute SQL UPDATE statements under default transaction isolation level.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="statements">SQL UPDATE statements.</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteUpdate(IDbConnection connection, ISqlStatementCommandTextProvider commandTextProvider, params ISqlUpdateStatement[] statements) {
            return ExecuteNoQuery(connection, commandTextProvider, statements);
        }

        /// <summary>
        /// Execute SQL UPDATE statements.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="transactionLevel">Transaction level.</param>
        /// <param name="statements">SQL UPDATE statements.</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteUpdate(IsolationLevel? transactionLevel, IDbConnection connection, params ISqlUpdateStatement[] statements) {
            return ExecuteNoQuery(transactionLevel, connection, (ISqlStatementCommandTextProvider) null, statements);
        }

        /// <summary>
        /// Execute SQL UPDATE statements.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="transactionLevel">Transaction level.</param>
        /// <param name="statements">SQL UPDATE statements.</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteUpdate(IsolationLevel? transactionLevel, IDbConnection connection, ISqlStatementCommandTextProvider commandTextProvider, params ISqlUpdateStatement[] statements) {
            return ExecuteNoQuery(transactionLevel, connection, commandTextProvider, statements);
        }

        /// <summary>
        /// Execute SQL UPDATE statements with the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="statements">SQL UPDATE statements.</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteUpdate(IDbTransaction transaction, params ISqlUpdateStatement[] statements) {
            return ExecuteNoQuery(transaction, (ISqlStatementCommandTextProvider) null, statements);
        }

        /// <summary>
        /// Execute SQL UPDATE statements with the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="statements">SQL UPDATE statements.</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteUpdate(IDbTransaction transaction, ISqlStatementCommandTextProvider commandTextProvider, params ISqlUpdateStatement[] statements) {
            return ExecuteNoQuery(transaction, commandTextProvider, statements);
        }

        /// <summary>
        /// Execute SQL DELETE statements under default transaction isolation level.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="statements">SQL DELETE statements.</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteDelete(IDbConnection connection, params ISqlDeleteStatement[] statements) {
            return ExecuteNoQuery(connection, (ISqlStatementCommandTextProvider) null, statements);
        }

        /// <summary>
        /// Execute SQL DELETE statements under default transaction isolation level.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="statements">SQL DELETE statements.</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteDelete(IDbConnection connection, ISqlStatementCommandTextProvider commandTextProvider, params ISqlDeleteStatement[] statements) {
            return ExecuteNoQuery(connection, commandTextProvider, statements);
        }

        /// <summary>
        /// Execute SQL DELETE statements.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="transactionLevel">Transaction level.</param>
        /// <param name="statements">SQL DELETE statements.</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteDelete(IsolationLevel? transactionLevel, IDbConnection connection, params ISqlDeleteStatement[] statements) {
            return ExecuteNoQuery(transactionLevel, connection, (ISqlStatementCommandTextProvider) null, statements);
        }

        /// <summary>
        /// Execute SQL DELETE statements.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="transactionLevel">Transaction level.</param>
        /// <param name="statements">SQL DELETE statements.</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteDelete(IsolationLevel? transactionLevel, IDbConnection connection, ISqlStatementCommandTextProvider commandTextProvider, params ISqlDeleteStatement[] statements) {
            return ExecuteNoQuery(transactionLevel, connection, commandTextProvider, statements);
        }

        /// <summary>
        /// Execute SQL DELETE statements with the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="statements">SQL DELETE statements.</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteDelete(IDbTransaction transaction, params ISqlDeleteStatement[] statements) {
            return ExecuteNoQuery(transaction, (ISqlStatementCommandTextProvider) null, statements);
        }

        /// <summary>
        /// Execute SQL DELETE statements with the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction used to execute SQL statement.</param>
        /// <param name="statements">SQL DELETE statements.</param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteDelete(IDbTransaction transaction, ISqlStatementCommandTextProvider commandTextProvider, params ISqlDeleteStatement[] statements) {
            return ExecuteNoQuery(transaction, commandTextProvider, statements);
        }

        #endregion

        #region AutoClearParametersDbCommand

        /// <summary>
        /// Represents a IDbCommand which can automatic clear parameters after closed.
        /// </summary>
        private class AutoClearParametersDbCommand : IDbCommand {
            /// <summary>
            /// Initialize a new instance of AutoClearParametersDbCommand class.
            /// </summary>
            /// <param name="command">A IDbCommand object.</param>
            public AutoClearParametersDbCommand(IDbCommand command) {
                this.m_command = command;
            }

            /// <summary>
            /// Based IDbCommand object.
            /// </summary>
            private IDbCommand m_command;

            #region IDbCommand Members

            public void Cancel() {
                this.m_command.Cancel();
            }

            public string CommandText {
                get {
                    return this.m_command.CommandText;
                }
                set {
                    this.m_command.CommandText = value;
                }
            }

            public int CommandTimeout {
                get {
                    return this.m_command.CommandTimeout;
                }
                set {
                    this.m_command.CommandTimeout = value;
                }
            }

            public CommandType CommandType {
                get {
                    return this.m_command.CommandType;
                }
                set {
                    this.m_command.CommandType = value;
                }
            }

            public IDbConnection Connection {
                get {
                    return this.m_command.Connection;
                }
                set {
                    this.m_command.Connection = value;
                }
            }

            public IDbDataParameter CreateParameter() {
                return this.m_command.CreateParameter();
            }

            public int ExecuteNonQuery() {
                return this.m_command.ExecuteNonQuery();
            }

            public IDataReader ExecuteReader(CommandBehavior behavior) {
                return this.m_command.ExecuteReader(behavior);
            }

            public IDataReader ExecuteReader() {
                return this.m_command.ExecuteReader();
            }

            public object ExecuteScalar() {
                return this.m_command.ExecuteScalar();
            }

            public IDataParameterCollection Parameters {
                get {
                    return this.m_command.Parameters;
                }
            }

            public void Prepare() {
                this.m_command.Prepare();
            }

            public IDbTransaction Transaction {
                get {
                    return this.m_command.Transaction;
                }
                set {
                    this.m_command.Transaction = value;
                }
            }

            public UpdateRowSource UpdatedRowSource {
                get {
                    return this.m_command.UpdatedRowSource;
                }
                set {
                    this.m_command.UpdatedRowSource = value;
                }
            }

            #endregion

            #region Dispose Pattern

            /// <summary>
            /// Indicate whether this object has disposed.
            /// </summary>
            private bool m_disposed = false;

            /// <summary>
            /// Finalizer.
            /// </summary>
            ~AutoClearParametersDbCommand() {
                this.Disposing(false);
            }

            /// <inheritdoc />
            public void Close() {
                this.Dispose();
            }

            /// <inheritdoc />
            public void Dispose() {
                this.Disposing(true);
                GC.SuppressFinalize(this);
            }

            /// <inheritdoc />
            protected virtual void Disposing(bool disposing) {
                if(this.m_disposed) {
                    return;
                }

                if(disposing) {
                }

                this.m_command.Parameters.Clear();
                this.m_command.Dispose();
                this.m_disposed = true;
            }

            #endregion
        }

        #endregion

        #region AutoCommitDataReader Class

        /// <summary>
        /// Represents a IDataReader which can automatic commit transaction after closed.
        /// </summary>
        private class AutoCommitDataReader : ISqlDataReader {
            /// <summary>
            /// Initialize a new instance of AutoCommitDataReader class.
            /// </summary>
            /// <param name="reader">A IDataReader object.</param>
            /// <param name="transaction">Then transaction used by this reader.</param>
            public AutoCommitDataReader(IDataReader reader, IDbTransaction transaction) {
                this.m_reader = reader;
                this.m_transaction = transaction;
            }

            /// <summary>
            /// Based IDataReader object.
            /// </summary>
            private IDataReader m_reader;

            /// <summary>
            /// Transaction.
            /// </summary>
            private IDbTransaction m_transaction;

            #region IDataReader Members

            /// <inheritdoc />
            public int Depth {
                get {
                    return this.m_reader.Depth;
                }
            }

            /// <inheritdoc />
            public DataTable GetSchemaTable() {
                return this.m_reader.GetSchemaTable();
            }

            /// <inheritdoc />
            public bool IsClosed {
                get {
                    return this.m_reader.IsClosed;
                }
            }

            /// <inheritdoc />
            public bool NextResult() {
                return this.m_reader.NextResult();
            }

            /// <inheritdoc />
            public bool Read() {
                return this.m_reader.Read();
            }

            /// <inheritdoc />
            public int RecordsAffected {
                get {
                    return this.m_reader.RecordsAffected;
                }
            }

            /// <inheritdoc />
            public int FieldCount {
                get {
                    return this.m_reader.FieldCount;
                }
            }

            /// <inheritdoc />
            public bool GetBoolean(int i) {
                return this.m_reader.GetBoolean(i);
            }

            /// <inheritdoc />
            public byte GetByte(int i) {
                return this.m_reader.GetByte(i);
            }

            /// <inheritdoc />
            public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) {
                return this.m_reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
            }

            /// <inheritdoc />
            public char GetChar(int i) {
                return this.m_reader.GetChar(i);
            }

            /// <inheritdoc />
            public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) {
                return this.m_reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
            }

            /// <inheritdoc />
            public IDataReader GetData(int i) {
                return this.m_reader.GetData(i);
            }

            /// <inheritdoc />
            public string GetDataTypeName(int i) {
                return this.m_reader.GetDataTypeName(i);
            }

            /// <inheritdoc />
            public DateTime GetDateTime(int i) {
                return this.m_reader.GetDateTime(i);
            }

            /// <inheritdoc />
            public decimal GetDecimal(int i) {
                return this.m_reader.GetDecimal(i);
            }

            /// <inheritdoc />
            public double GetDouble(int i) {
                return this.m_reader.GetDouble(i);
            }

            /// <inheritdoc />
            public Type GetFieldType(int i) {
                return this.m_reader.GetFieldType(i);
            }

            /// <inheritdoc />
            public float GetFloat(int i) {
                return this.m_reader.GetFloat(i);
            }

            /// <inheritdoc />
            public Guid GetGuid(int i) {
                return this.m_reader.GetGuid(i);
            }

            /// <inheritdoc />
            public short GetInt16(int i) {
                return this.m_reader.GetInt16(i);
            }

            /// <inheritdoc />
            public int GetInt32(int i) {
                return this.m_reader.GetInt32(i);
            }

            /// <inheritdoc />
            public long GetInt64(int i) {
                return this.m_reader.GetInt64(i);
            }

            /// <inheritdoc />
            public string GetName(int i) {
                return this.m_reader.GetName(i);
            }

            /// <inheritdoc />
            public int GetOrdinal(string name) {
                return this.m_reader.GetOrdinal(name);
            }

            /// <inheritdoc />
            public string GetString(int i) {
                return this.m_reader.GetString(i);
            }

            /// <inheritdoc />
            public object GetValue(int i) {
                return this.m_reader.GetValue(i);
            }

            /// <inheritdoc />
            public int GetValues(object[] values) {
                return this.m_reader.GetValues(values);
            }

            /// <inheritdoc />
            public bool IsDBNull(int i) {
                return this.m_reader.IsDBNull(i);
            }

            /// <inheritdoc />
            public object this[string name] {
                get {
                    return this.m_reader[name];
                }
            }

            /// <inheritdoc />
            public object this[int i] {
                get {
                    return this.m_reader[i];
                }
            }

            #endregion

            #region ISqlDataReader Members

            /// <inheritdoc />
            public object this[ISqlObject obj] {
                get {
                    if(obj == null) {
                        throw new ArgumentException("SQL object is null.", "obj");
                    }
                    return this.m_reader[this.m_reader.GetOrdinal(obj.Name)];
                }
            }

            /// <inheritdoc />
            public bool GetBoolean(ISqlObject obj) {
                if(obj == null) {
                    throw new ArgumentException("SQL object is null.", "obj");
                }
                return this.m_reader.GetBoolean(this.m_reader.GetOrdinal(obj.Name));
            }

            /// <inheritdoc />
            public byte GetByte(ISqlObject obj) {
                if(obj == null) {
                    throw new ArgumentException("SQL object is null.", "obj");
                }
                return this.m_reader.GetByte(this.m_reader.GetOrdinal(obj.Name));
            }

            /// <inheritdoc />
            public long GetBytes(ISqlObject obj, long fieldOffset, byte[] buffer, int bufferoffset, int length) {
                if(obj == null) {
                    throw new ArgumentException("SQL object is null.", "obj");
                }
                return this.m_reader.GetBytes(this.m_reader.GetOrdinal(obj.Name), fieldOffset, buffer, bufferoffset, length);
            }

            /// <inheritdoc />
            public char GetChar(ISqlObject obj) {
                if(obj == null) {
                    throw new ArgumentException("SQL object is null.", "obj");
                }
                return this.m_reader.GetChar(this.m_reader.GetOrdinal(obj.Name));
            }

            /// <inheritdoc />
            public long GetChars(ISqlObject obj, long fieldoffset, char[] buffer, int bufferoffset, int length) {
                if(obj == null) {
                    throw new ArgumentException("SQL object is null.", "obj");
                }
                return this.m_reader.GetChars(this.m_reader.GetOrdinal(obj.Name), fieldoffset, buffer, bufferoffset, length);
            }

            /// <inheritdoc />
            public DateTime GetDateTime(ISqlObject obj) {
                if(obj == null) {
                    throw new ArgumentException("SQL object is null.", "obj");
                }
                return this.m_reader.GetDateTime(this.m_reader.GetOrdinal(obj.Name));
            }

            /// <inheritdoc />
            public decimal GetDecimal(ISqlObject obj) {
                if(obj == null) {
                    throw new ArgumentException("SQL object is null.", "obj");
                }
                return this.m_reader.GetDecimal(this.m_reader.GetOrdinal(obj.Name));
            }

            /// <inheritdoc />
            public double GetDouble(ISqlObject obj) {
                if(obj == null) {
                    throw new ArgumentException("SQL object is null.", "obj");
                }
                return this.m_reader.GetDouble(this.m_reader.GetOrdinal(obj.Name));
            }

            /// <inheritdoc />
            public float GetFloat(ISqlObject obj) {
                if(obj == null) {
                    throw new ArgumentException("SQL object is null.", "obj");
                }
                return this.m_reader.GetFloat(this.m_reader.GetOrdinal(obj.Name));
            }

            /// <inheritdoc />
            public Guid GetGuid(ISqlObject obj) {
                if(obj == null) {
                    throw new ArgumentException("SQL object is null.", "obj");
                }
                return this.m_reader.GetGuid(this.m_reader.GetOrdinal(obj.Name));
            }

            /// <inheritdoc />
            public short GetInt16(ISqlObject obj) {
                if(obj == null) {
                    throw new ArgumentException("SQL object is null.", "obj");
                }
                return this.m_reader.GetInt16(this.m_reader.GetOrdinal(obj.Name));
            }

            /// <inheritdoc />
            public int GetInt32(ISqlObject obj) {
                if(obj == null) {
                    throw new ArgumentException("SQL object is null.", "obj");
                }
                return this.m_reader.GetInt32(this.m_reader.GetOrdinal(obj.Name));
            }

            /// <inheritdoc />
            public long GetInt64(ISqlObject obj) {
                if(obj == null) {
                    throw new ArgumentException("SQL object is null.", "obj");
                }
                return this.m_reader.GetInt64(this.m_reader.GetOrdinal(obj.Name));
            }

            /// <inheritdoc />
            public int GetOrdinal(ISqlObject obj) {
                if(obj == null) {
                    throw new ArgumentException("SQL object is null.", "obj");
                }
                return this.m_reader.GetOrdinal(obj.Name);
            }

            /// <inheritdoc />
            public string GetString(ISqlObject obj) {
                if(obj == null) {
                    throw new ArgumentException("SQL object is null.", "obj");
                }
                return this.m_reader.GetString(this.m_reader.GetOrdinal(obj.Name));
            }

            /// <inheritdoc />
            public object GetValue(ISqlObject obj) {
                if(obj == null) {
                    throw new ArgumentException("SQL object is null.", "obj");
                }
                return this.m_reader.GetValue(this.m_reader.GetOrdinal(obj.Name));
            }

            /// <inheritdoc />
            public bool IsDBNull(ISqlObject obj) {
                if(obj == null) {
                    throw new ArgumentException("SQL object is null.", "obj");
                }
                return this.m_reader.IsDBNull(this.m_reader.GetOrdinal(obj.Name));
            }

            #endregion

            #region Dispose Pattern

            /// <summary>
            /// Indicate whether this object has disposed.
            /// </summary>
            private bool m_disposed = false;

            /// <summary>
            /// Finalizer.
            /// </summary>
            ~AutoCommitDataReader() {
                this.Disposing(false);
            }

            /// <inheritdoc />
            public void Close() {
                this.Dispose();
            }

            /// <inheritdoc />
            public void Dispose() {
                this.Disposing(true);
                
            }

            /// <inheritdoc />
            protected virtual void Disposing(bool disposing) {
                if(this.m_disposed) {
                    return;
                }
                this.m_disposed = true;

                this.m_reader.Close();
                this.m_transaction.Commit();
                this.m_transaction.Dispose();

                if(disposing) {
                    GC.SuppressFinalize(this);
                }
            }

            #endregion
        }

        #endregion
    }

    public interface ISqlDataReader : IDataReader {
        object this[ISqlObject obj] {
            get;
        }

        bool GetBoolean(ISqlObject obj);

        byte GetByte(ISqlObject obj);

        long GetBytes(ISqlObject obj, long fieldOffset, byte[] buffer, int bufferoffset, int length);

        char GetChar(ISqlObject obj);

        long GetChars(ISqlObject obj, long fieldoffset, char[] buffer, int bufferoffset, int length);

        DateTime GetDateTime(ISqlObject obj);

        decimal GetDecimal(ISqlObject obj);

        double GetDouble(ISqlObject obj);

        float GetFloat(ISqlObject obj);

        Guid GetGuid(ISqlObject obj);

        short GetInt16(ISqlObject obj);

        int GetInt32(ISqlObject obj);

        long GetInt64(ISqlObject obj);

        int GetOrdinal(ISqlObject obj);

        string GetString(ISqlObject obj);

        object GetValue(ISqlObject obj);

        bool IsDBNull(ISqlObject obj);
    }
}
