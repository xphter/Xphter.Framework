using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.SqlServer {
    /// <summary>
    /// Represents a data source in SQL Server.
    /// </summary>
    public class SqlServerSource : SqlAliasObject {
        /// <summary>
        /// Initialize a new instance of SqlServerSource class from a table name or view name.
        /// </summary>
        /// <param name="name">Source name.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public SqlServerSource(string name)
            : this(null, name, null) {
        }

        /// <summary>
        /// Initialize a new instance of SqlServerSource class from a table name or view name and a alias.
        /// </summary>
        /// <param name="name">Source name.</param>
        /// <param name="alias">Source alias.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public SqlServerSource(string name, string alias)
            : this(null, name, alias) {
        }

        /// <summary>
        /// Initialize a new instance of SqlServerSource class from a table name or view name.
        /// </summary>
        /// <param name="owner">Source owner.</param>
        /// <param name="name">Source name.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public SqlServerSource(ISqlObject owner, string name)
            : this(owner, name, null) {
        }

        /// <summary>
        /// Initialize a new instance of SqlServerSource class from a table name or view name and a alias.
        /// </summary>
        /// <param name="owner">Source owner.</param>
        /// <param name="name">Source name.</param>
        /// <param name="alias">Source alias.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public SqlServerSource(ISqlObject owner, string name, string alias)
            : base(new SqlObject(owner, new SqlStringExpression(name), name, new SqlServerObjectFullnameProvider(), new SqlObjectFullnameSqlStringProvider()), alias, new SqlServerAliasSourceFullnameProvider(), new SqlServerAliasObjectSqlStringProvider()) {
            this.m_tableHints = new SqlFunction(TABLE_HINTS_FUNCTION_NAME);
        }

        /// <summary>
        /// Initialize a new instance of SqlServerSource class from a SQL subquery.
        /// </summary>
        /// <param name="statement">A SQL SELECT statement.</param>
        /// <param name="name">Subquery name.</param>
        /// <exception cref="System.ArgumentException"><paramref name="statement"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public SqlServerSource(ISqlSelectStatement statement, string name)
            : base(new SqlObject(statement, name, new SqlServerObjectFullnameProvider(), new SqlServerObjectNamedSqlStringProvider(true))) {
            if(statement == null) {
                throw new ArgumentException("SQL SELECT statement is null.", "statement");
            }
        }

        /// <summary>
        /// Initialize a new instance of SqlServerSource class from a function.
        /// </summary>
        /// <param name="function">A SQL function.</param>
        /// <param name="name">Source name.</param>
        /// <exception cref="System.ArgumentException"><paramref name="function"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public SqlServerSource(ISqlFunction function, string name)
            : base(new SqlObject(function, name, new SqlServerObjectFullnameProvider(), new SqlServerObjectNamedSqlStringProvider(false))) {
        }

        public const string TABLE_HINTS_FUNCTION_NAME = "WITH";

        protected ISqlFunction m_tableHints;
        /// <summary>
        /// Get the table hints of this sql server source.
        /// </summary>
        public virtual ISqlFunction TableHints {
            get {
                return m_tableHints;
            }
        }

        public override string SqlString {
            get {
                if(this.m_tableHints != null && this.m_tableHints.Arguments.Any()) {
                    return string.Format("{0} {1}", base.SqlString, this.m_tableHints.SqlString);
                } else {
                    return base.SqlString;
                }
            }
        }

        /// <summary>
        /// Creates a SqlServerSource by the specified source with a alias.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public static SqlServerSource SetAlias(ISqlObject source, string alias) {
            if(source == null) {
                throw new ArgumentNullException("source");
            }

            if(string.IsNullOrWhiteSpace(alias)) {
                throw new ArgumentException("alias is null or empty", "alias");
            }

            return new SqlServerSource(source.Owner, source.Name, alias);
        }
    }
}
