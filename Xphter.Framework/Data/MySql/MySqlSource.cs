using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.MySql {
    /// <summary>
    /// Represents a data source in MySql.
    /// </summary>
    public class MySqlSource : SqlAliasObject {
        /// <summary>
        /// Initialize a new instance of MySqlSource class from a table name or view name.
        /// </summary>
        /// <param name="name">Source name.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public MySqlSource(string name)
            : this(null, name, null) {
        }

        /// <summary>
        /// Initialize a new instance of MySqlSource class from a table name or view name and a alias.
        /// </summary>
        /// <param name="name">Source name.</param>
        /// <param name="alias">Source alias.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public MySqlSource(string name, string alias)
            : this(null, name, alias) {
        }

        /// <summary>
        /// Initialize a new instance of MySqlSource class from a table name or view name.
        /// </summary>
        /// <param name="owner">Source owner.</param>
        /// <param name="name">Source name.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public MySqlSource(ISqlObject owner, string name)
            : this(owner, name, null) {
        }

        /// <summary>
        /// Initialize a new instance of MySqlSource class from a table name or view name and a alias.
        /// </summary>
        /// <param name="owner">Source owner.</param>
        /// <param name="name">Source name.</param>
        /// <param name="alias">Source alias.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public MySqlSource(ISqlObject owner, string name, string alias)
            : base(new SqlObject(owner, new SqlStringExpression(name), name, new MySqlObjectFullnameProvider(), new SqlObjectFullnameSqlStringProvider()), alias, new MySqlAliasSourceFullnameProvider(), new MySqlAliasObjectSqlStringProvider()) {
        }

        /// <summary>
        /// Initialize a new instance of SqlServerSource class from a SQL subquery.
        /// </summary>
        /// <param name="statement">A SQL SELECT statement.</param>
        /// <param name="name">Subquery name.</param>
        /// <exception cref="System.ArgumentException"><paramref name="statement"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public MySqlSource(ISqlSelectStatement statement, string name)
            : base(new SqlObject(statement, name, new MySqlObjectFullnameProvider(), new MySqlObjectNamedSqlStringProvider(true))) {
            if(statement == null) {
                throw new ArgumentException("SQL SELECT statement is null.", "statement");
            }
        }
    }
}
