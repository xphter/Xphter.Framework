using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.Oracle {
    /// <summary>
    /// Provide SQL string of a Oracle object with SQL string and name of it's base expression.
    /// </summary>
    internal class OracleObjectNamedSqlStringProvider : ISqlObjectSqlStringProvider {
        /// <summary>
        /// Initialize a new instance of OracleObjectNamedSqlStringProvider class.
        /// </summary>
        public OracleObjectNamedSqlStringProvider()
            : this(false) {
        }

        /// <summary>
        /// Initialize a new instance of OracleObjectNamedSqlStringProvider class.
        /// </summary>
        /// <param name="hasParentheses">Whether bracket the SQL string of based expression of a SQL object.</param>
        public OracleObjectNamedSqlStringProvider(bool hasParentheses) {
            this.m_hasParentheses = hasParentheses;
        }

        /// <summary>
        /// Whether bracket the SQL string of based expression of a SQL object.
        /// </summary>
        private bool m_hasParentheses = false;

        #region ISqlObjectSqlStringProvider Members

        /// <inheritdoc />
        public string GetSqlString(ISqlObject obj) {
            return string.Format(this.m_hasParentheses ? "({0}) \"{1}\"" : "{0} \"{1}\"", obj.Expression.SqlString, obj.Name.ToUpper());
        }

        #endregion
    }
}
