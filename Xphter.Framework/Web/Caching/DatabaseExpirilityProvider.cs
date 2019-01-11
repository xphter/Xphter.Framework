using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Xphter.Framework.Data;

namespace Xphter.Framework.Web.Caching {
    /// <summary>
    /// Checks expirility of database item.
    /// </summary>
    /// <typeparam name="T">The data item type.</typeparam>
    /// <exception cref="System.ArgumentException"><paramref name="statement"/> or <paramref name="connection"/> is null.</exception>
    public class DatabaseExpirilityProvider<T> : IExpirilityProvider<T> {
        /// <summary>
        /// Initialize a instance of DatabaseExpirilityProvider class.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="connectionSelector"></param>
        public DatabaseExpirilityProvider(ISqlSelectStatement statement, Func<IDbConnection> connectionSelector)
            : this(statement != null ? (obj) => statement : (Func<T, ISqlSelectStatement>) null, connectionSelector) {
        }

        /// <summary>
        /// Initialize a instance of DatabaseExpirilityProvider class.
        /// </summary>
        /// <param name="statementSelector"></param>
        /// <param name="connectionSelector"></param>
        public DatabaseExpirilityProvider(Func<T, ISqlSelectStatement> statementSelector, Func<IDbConnection> connectionSelector) {
            if(statementSelector == null) {
                throw new ArgumentException("The delegate to get SQL SELECT statement is null.", "statementSelector");
            }
            if(connectionSelector == null) {
                throw new ArgumentException("The delegate to get connection is null.", "connectionSelector");
            }

            this.m_statementSelector = statementSelector;
            this.m_connectionSelector = connectionSelector;
        }

        /// <summary>
        /// The delegate to get a SQL SELECT statement.
        /// </summary>
        protected Func<T, ISqlSelectStatement> m_statementSelector;

        /// <summary>
        /// The delegate to get a database connection.
        /// </summary>
        protected Func<IDbConnection> m_connectionSelector;

        #region IExpirilityProvider<T> Members

        /// <inheritdoc />
        public virtual bool GetIsExpired(T dataItem) {
            using(IDbConnection connection = this.m_connectionSelector()) {
                return SqlHelper.ExecuteSelectScalar<object>(connection, this.m_statementSelector(dataItem)) == null;
            }
        }

        #endregion
    }
}
