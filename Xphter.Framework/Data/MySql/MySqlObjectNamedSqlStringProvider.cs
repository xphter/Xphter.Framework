using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.MySql {
    /// <summary>
    /// Provide SQL string of a MySql object with SQL string and name of it's base expression.
    /// </summary>
    internal class MySqlObjectNamedSqlStringProvider : ISqlObjectSqlStringProvider {
        /// <summary>
        /// Initialize a new instance of MySqlObjectNamedSqlStringProvider class.
        /// </summary>
        public MySqlObjectNamedSqlStringProvider()
            : this(false) {
        }

        /// <summary>
        /// Initialize a new instance of MySqlObjectNamedSqlStringProvider class.
        /// </summary>
        /// <param name="hasParentheses">Whether bracket the SQL string of based expression of a SQL object.</param>
        public MySqlObjectNamedSqlStringProvider(bool hasParentheses) {
            this.m_hasParentheses = hasParentheses;
        }

        /// <summary>
        /// Whether bracket the SQL string of based expression of a SQL object.
        /// </summary>
        private bool m_hasParentheses = false;

        #region ISqlObjectSqlStringProvider Members

        /// <inheritdoc />
        public string GetSqlString(ISqlObject obj) {
            return string.Format(this.m_hasParentheses ? "({0}) AS `{1}`" : "{0} AS `{1}`", obj.Expression.SqlString, obj.Name);
        }

        #endregion
    }
}
