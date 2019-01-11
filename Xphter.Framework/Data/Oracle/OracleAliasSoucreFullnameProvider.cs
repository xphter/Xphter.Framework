using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.Oracle {
    /// <summary>
    /// Provider fullname of a oracle alias source.
    /// </summary>
    internal class OracleAliasSoucreFullnameProvider : ISqlAliasObjectFullnameProvider {
        #region ISqlAliasObjectFullnameProvider Members

        /// <inheritdoc />
        public string GetFullname(ISqlAliasObject obj) {
            return string.Format("\"{0}\"", obj.Alias.ToUpper());
        }

        #endregion
    }
}
