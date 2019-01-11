using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.SqlServer {
    /// <summary>
    /// Provide SQL string of a SQL Server object with SQL string and name of it's base expression.
    /// </summary>
    internal class SqlServerObjectNamedSqlStringProvider : ISqlObjectSqlStringProvider {
        /// <summary>
        /// Initialize a new instance of SqlServerObjectAliasSqlStringProvider class.
        /// </summary>
        public SqlServerObjectNamedSqlStringProvider()
            : this(false) {
        }

        /// <summary>
        /// Initialize a new instance of SqlServerObjectAliasSqlStringProvider class.
        /// </summary>
        /// <param name="hasParentheses">Whether bracket the SQL string of based expression of a SQL object.</param>
        public SqlServerObjectNamedSqlStringProvider(bool hasParentheses) {
            this.m_hasParentheses = hasParentheses;
        }

        /// <summary>
        /// Whether bracket the SQL string of based expression of a SQL object.
        /// </summary>
        private bool m_hasParentheses = false;

        #region ISqlObjectSqlStringProvider Members

        /// <inheritdoc />
        public string GetSqlString(ISqlObject obj) {
            return string.Format(this.m_hasParentheses ? "({0}) AS [{1}]" : "{0} AS [{1}]", obj.Expression.SqlString, obj.Name);
        }

        #endregion
    }
}
