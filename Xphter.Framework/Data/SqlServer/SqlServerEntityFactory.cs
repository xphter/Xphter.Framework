using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Data.SqlServer {
    /// <summary>
    /// Create database entities for SQL Server.
    /// </summary>
    internal class SqlServerEntityFactory : IDbEntityFactory {
        #region IDbEntityFactory Members

        /// <inheritdoc />
        public IDbSourceEntity CreateSource(string name, DbCredential credential) {
            return new DbSourceEntity(name, credential, new SqlServerDatabaseEntityProvider());
        }

        #endregion
    }
}
