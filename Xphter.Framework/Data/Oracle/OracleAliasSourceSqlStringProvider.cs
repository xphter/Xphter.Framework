using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.Oracle {
    /// <summary>
    /// Provide SQL string of a Oracle field.
    /// </summary>
    internal class OracleAliasSourceSqlStringProvider : ISqlAliasObjectSqlStringProvider {
        #region ISqlAliasObjectSqlStringProvider Members

        /// <inheritdoc />
        public string GetSqlString(ISqlAliasObject field) {
            return string.IsNullOrWhiteSpace(field.Alias) ? field.Object.SqlString : string.Format("{0} \"{1}\"", field.Object.SqlString, field.Alias.ToUpper());
        }

        #endregion
    }
}
