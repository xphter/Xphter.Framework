using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.SqlServer {
    /// <summary>
    /// Provider fullname of a SQL server alias source.
    /// </summary>
    internal class SqlServerAliasSourceFullnameProvider : ISqlAliasObjectFullnameProvider {
        #region ISqlAliasObjectFullnameProvider Members

        /// <inheritdoc />
        public string GetFullname(ISqlAliasObject obj) {
            return string.Format("[{0}]", obj.Alias);
        }

        #endregion
    }
}
