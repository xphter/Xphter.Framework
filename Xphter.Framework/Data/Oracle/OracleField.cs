using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.Oracle {
    /// <summary>
    /// Represents a filed in Oracle.
    /// </summary>
    public class OracleField : SqlAliasObject {
        /// <summary>
        /// Initialize a new instance of OracleField class from a field name.
        /// </summary>
        /// <param name="source">A SQL Server source.</param>
        /// <param name="name">Field name.</param>
        /// <exception cref="System.ArgumentException"><paramref name="source"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public OracleField(ISqlObject source, string name)
            : this(source, name, null) {
        }

        /// <summary>
        /// Initialize a new instance of OracleField class from a field name and alias.
        /// </summary>
        /// <param name="source">A SQL Server source.</param>
        /// <param name="name">Field name.</param>
        /// <param name="alias">Field alias.</param>
        /// <exception cref="System.ArgumentException"><paramref name="source"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public OracleField(ISqlObject source, string name, string alias)
            : base(new SqlObject(source, new SqlStringExpression(name), name, new OracleObjectFullnameProvider(), new SqlObjectFullnameSqlStringProvider()), alias, new SqlAliasObjectFullnameProvider(), new OracleAliasObjectSqlStringProvider()) {
        }

        /// <summary>
        /// Initialize a new instance of OracleField class from a server variable.
        /// </summary>
        /// <param name="variable">Server variable.</param>
        /// <param name="name">Field name.</param>
        /// <exception cref="System.ArgumentException"><paramref name="variable"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public OracleField(string variable, string name)
            : base(new SqlObject(new SqlStringExpression(variable), name, new OracleObjectFullnameProvider(), new OracleObjectNamedSqlStringProvider())) {
        }

        /// <summary>
        /// Initialize a new instance of OracleField class from a function.
        /// </summary>
        /// <param name="function">A SQL function.</param>
        /// <param name="name">Field name.</param>
        /// <exception cref="System.ArgumentException"><paramref name="function"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public OracleField(ISqlFunction function, string name)
            : base(new SqlObject(function, name, new OracleObjectFullnameProvider(), new OracleObjectNamedSqlStringProvider())) {
        }
    }
}
