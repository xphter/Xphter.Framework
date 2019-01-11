using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.MySql {
    /// <summary>
    /// Provider fullname of a MySql alias source.
    /// </summary>
    internal class MySqlAliasSourceFullnameProvider : ISqlAliasObjectFullnameProvider {
        #region ISqlAliasObjectFullnameProvider Members

        /// <inheritdoc />
        public string GetFullname(ISqlAliasObject obj) {
            return string.Format("`{0}`", obj.Alias);
        }

        #endregion
    }
}
