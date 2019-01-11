using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.SqlServer {
    /// <summary>
    /// Provide SQL string of a SQL server alias object.
    /// </summary>
    internal class SqlServerAliasObjectSqlStringProvider : ISqlAliasObjectSqlStringProvider {
        #region ISqlAliasObjectSqlStringProvider Members

        /// <inheritdoc />
        public string GetSqlString(ISqlAliasObject field) {
            return string.IsNullOrWhiteSpace(field.Alias) ? field.Object.SqlString : string.Format("{0} AS [{1}]", field.Object.SqlString, field.Alias);
        }

        #endregion
    }
}
