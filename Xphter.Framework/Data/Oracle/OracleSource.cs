using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.Oracle {
    /// <summary>
    /// Represents a data source in Oracle.
    /// </summary>
    public class OracleSource : SqlAliasObject {
        /// <summary>
        /// Initialize a new instance of OracleSource class from a table name or view name.
        /// </summary>
        /// <param name="name">Source name.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public OracleSource(string name)
            : this(null, name, null) {
        }

        /// <summary>
        /// Initialize a new instance of OracleSource class from a table name or view name and a alias.
        /// </summary>
        /// <param name="name">Source name.</param>
        /// <param name="alias">Source alias.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public OracleSource(string name, string alias)
            : this(null, name, alias) {
        }

        /// <summary>
        /// Initialize a new instance of OracleSource class from a table name or view name.
        /// </summary>
        /// <param name="owner">Source owner.</param>
        /// <param name="name">Source name.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public OracleSource(ISqlObject owner, string name)
            : this(owner, name, null) {
        }

        /// <summary>
        /// Initialize a new instance of OracleSource class from a table name or view name and a alias.
        /// </summary>
        /// <param name="owner">Source owner.</param>
        /// <param name="name">Source name.</param>
        /// <param name="alias">Source alias.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public OracleSource(ISqlObject owner, string name, string alias)
            : base(new SqlObject(owner, new SqlStringExpression(name), name, new OracleObjectFullnameProvider(), new SqlObjectFullnameSqlStringProvider()), alias, new OracleAliasSoucreFullnameProvider(), new OracleAliasSourceSqlStringProvider()) {
        }

        /// <summary>
        /// Initialize a new instance of OracleSource class from a SQL subquery.
        /// </summary>
        /// <param name="statement">A SQL SELECT statement.</param>
        /// <param name="name">Subquery name.</param>
        /// <exception cref="System.ArgumentException"><paramref name="statement"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public OracleSource(ISqlSelectStatement statement, string name)
            : base(new SqlObject(statement, name, new OracleObjectFullnameProvider(), new OracleObjectNamedSqlStringProvider(true))) {
            if(statement == null) {
                throw new ArgumentException("SQL SELECT statement is null.", "statement");
            }
        }
        
        /// <summary>
        /// Initialize a new instance of OracleSource class from a function.
        /// </summary>
        /// <param name="function">A SQL function.</param>
        /// <param name="name">Source name.</param>
        /// <exception cref="System.ArgumentException"><paramref name="function"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public OracleSource(ISqlFunction function, string name)
            : base(new SqlObject(function, name, new OracleObjectFullnameProvider(), new OracleObjectNamedSqlStringProvider(false))) {
        }
    }
}
